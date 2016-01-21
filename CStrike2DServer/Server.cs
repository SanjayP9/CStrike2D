using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using CStrike2D;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace CStrike2DServer
{
    class Server
    {
        private static NetServer server;
        private static NetPeerConfiguration config;
        private static NetIncomingMessage msg;

        private static List<ServerPlayer> players = new List<ServerPlayer>();
        private static List<ServerGrenade> grenades = new List<ServerGrenade>();

        private static short playerIdentifier;
        private static string serverVersion = "0.6.0a";            // Server Version
        private static int maxPlayers = 32;
        private static int port = 27015;
        private static string buffer = "";
        private static string serverName = "Global Offensive Server - " + serverVersion;
        private static bool forceConfigRewrite = true;
        private static NetOutgoingMessage outMsg;
        private static int maxCTs = 16;
        private static int maxTs = 16;
        private static int numTs = 0;
        private static int numCts = 0;
        private static float roundTimer = 0;
        private static float numPlayers = 0;
        private static bool enableCollision = true;
        private static short entityCounter;
        
        public const double UPDATE_RATE = 7d;
        public const double NET_UPDATE_RATE = 16.66d;
        public const double SYNC_RATE = 20d;

        private const float AFTER_ROUND_TIME = 5f;          // Time between switching the round state
        private const float FREEZE_TIME = 10f;              // Time at the start of a round to allow users to buy
        private static Stopwatch timer = new Stopwatch();   // Used to time the after round timer and freeze time

        private static int tsAlive;                 // Number of Terrorists alive
        private static int ctsAlive;                // Number of Counter-Terrorists alive
        private static string mapName = "de_cache"; // Current map

        private static RoundState state = RoundState.Empty; // Default the current round state to an empty server
        private static DemoRecorder recorder;               // Used to record the game state

        private static Random rand = new Random();          // Random number generator

        public static ServerMap MapData { get; private set; }

        enum RoundState
        {
            Empty,
            Buytime,
            Play,
            AfterRound
        }

        public static List<ServerPlayer> RetrievePlayers()
        {
            return players;
        }

        static void Main(string[] args)
        {
            /* Ballpark estimates for bandwidth usage
             * - Each command requires 1 byte for the identifier.
             * - Each player sends input requests of the following:
             * - Direction (2 Bytes)
             * - Rotation (3 Bytes)
             * - Player Updates at 60Hz
             * - 5 bytes * 60 times a second = 0.3kb/s Upload
             * - The server must send this to every player on the server on demand
             * - Direction (3 Bytes)
             * - Rotation (5 Bytes)
             * - Server Updates at 64Hz up to 128Hz
             * - At 64Hz
             * - 8 bytes * 64 times a second * 32 players = 16kb/s Upload
             * - At 128Hz
             * - 8 bytes * 128 times a second * 32 players = 32kb/s Upload
             * - The server syncs everyone's postion and rotation every 60 ticks or ~930ms
             * - Direction (4 Bytes)
             * - Rotation (5 Bytes)
             * - PlayerID (2 Bytes)
             * - At 64 Hz
             * - 11 bytes * 64 times a second * 32 players = 22kb/s Upload
             * - At 128 Hz
             * - 11 bytes * 128 times a second * 32 players = 44kb/s Upload
             */

            Vector2 defSpawnPosition = new Vector2(350, 350);
            recorder = new DemoRecorder();
            MapData = new ServerMap();

            Stopwatch updateTimer = new Stopwatch();
            Stopwatch netUpdateTimer = new Stopwatch();
            Stopwatch syncTimer = new Stopwatch();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("==================================");
            Console.WriteLine("Global Offensive - Version " + serverVersion);
            Console.WriteLine("==================================");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Loading config file...");

            if (forceConfigRewrite)
            {
                WriteFile();
            }

            ReadConfig();

            Console.WriteLine("Loading map de_cache...");

            if (!LoadMap("de_cache.txt"))
            {
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Booting up server...");

            config = new NetPeerConfiguration("cstrike") {Port = port, EnableUPnP = true};
            server = new NetServer(config);
            server.Start();
            Thread.Sleep(1000);
            Console.WriteLine("Server is live.");
            recorder.StartRecording(mapName);
            updateTimer.Start();
            netUpdateTimer.Start();
            while (server.Status == NetPeerStatus.Running)
            {
                //Console.Clear();   
                //Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
                //Console.WriteLine("Players " + numPlayers + "/" + maxPlayers);

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();

                    if (key.Key == ConsoleKey.Escape)
                    {
                        recorder.EndRecording();
                        return;
                    }
                }

                if (updateTimer.Elapsed.TotalMilliseconds > UPDATE_RATE)
                {
                    Simulate();
                    recorder.Update();
                    updateTimer.Restart();
                }

                if (netUpdateTimer.Elapsed.TotalMilliseconds > NET_UPDATE_RATE)
                {
                    UpdateNetwork();
                    netUpdateTimer.Restart();
                }

                if (syncTimer.Elapsed.TotalMilliseconds > SYNC_RATE)
                {
                    SyncWorld();
                    syncTimer.Restart();
                }
            }
        }

        /// <summary>
        /// Loads the map from file
        /// </summary>
        /// <param name="mapName"></param>
        /// <returns></returns>
        static bool LoadMap(string mapName)
        {
            return MapData.Load(mapName);
        }

        #region Old Code
        /*
        public static void SyncServer()
        {
            NetOutgoingMessage syncMsg;

            foreach (Player ply in players)
            {
                syncMsg = server.CreateMessage();
                syncMsg.Write(NetInterface.SYNC_MOVEMENT);
                syncMsg.Write(ply.PlayerID);
                syncMsg.Write(ply.GetPosition().X);
                syncMsg.Write(ply.GetPosition().Y);
                server.SendToAll(syncMsg, NetDeliveryMethod.UnreliableSequenced);
            }
        }

        public static void Update()
        {
            while ((msg = server.ReadMessage()) != null)
            {
                outMsg = server.CreateMessage();
                Player player;
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus) msg.ReadByte())
                        {
                            case NetConnectionStatus.Connected:
                                outMsg.Write(NetInterface.HANDSHAKE);
                                playerIdentifier++;
                                outMsg.Write(playerIdentifier);
                                Console.WriteLine("Client given identifier: " + playerIdentifier);
                                msg.SenderConnection.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered, 0);
                                break;
                            case NetConnectionStatus.Disconnected:
                                player = players.Find(ply => ply.Client.RemoteUniqueIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);

                                outMsg.Write(NetInterface.PLAYER_DC);
                                outMsg.Write(player.PlayerID);
                                server.SendToAll(outMsg, NetDeliveryMethod.ReliableSequenced);
                                Console.WriteLine("\"" + player.PlayerName + "\" has left the server");
                                players.Remove(player);
                                break;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        byte identifier = msg.ReadByte();
                        player = players.Find(ply => ply.Client.RemoteUniqueIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);
                        switch (identifier)
                        {
                            case NetInterface.HANDSHAKE:
                                player = new Player(msg.ReadString(), msg.SenderConnection, playerIdentifier);
                                player.SetPosition(new Vector2(players.Count * 50, players.Count * 50));
                                player.SetCurrentWeapon(NetInterface.SWITCH_KNIFE, entityCounter);
                                players.Add(player);
                                entityCounter++;
                                Console.WriteLine("Player: \"" + player.PlayerName + "\" Connected. Identifier: " + playerIdentifier);
                               
                                // Send data about the new player to all connected players
                                foreach (Player plyr in players)
                                {
                                    outMsg = server.CreateMessage();
                                    outMsg.Write(NetInterface.SYNC_NEW_PLAYER);
                                    outMsg.Write(plyr.PlayerName);
                                    outMsg.Write(plyr.PlayerID);
                                    outMsg.Write(plyr.GetPosition().X);
                                    outMsg.Write(plyr.GetPosition().Y);
                                    outMsg.Write(plyr.Rotation);
                                    outMsg.Write(NetInterface.GetTeamByte(plyr.Team));
                                    outMsg.Write(entityCounter);
                                    outMsg.Write(plyr.CurrentWeapon);
                                    server.SendToAll(outMsg, NetDeliveryMethod.ReliableSequenced);
                                    Console.WriteLine("Sent data about \"" + player.PlayerName + "\"" +
                                                      " to player \"" + plyr.PlayerName + "\"");
                                }
                                Console.WriteLine("Sync Complete.");
                                break;
                            case NetInterface.MOVE_UP:
                            case NetInterface.MOVE_DOWN:
                            case NetInterface.MOVE_LEFT:
                            case NetInterface.MOVE_RIGHT:
                            case NetInterface.MOVE_UPRIGHT:
                            case NetInterface.MOVE_DOWNRIGHT:
                            case NetInterface.MOVE_DOWNLEFT:
                            case NetInterface.MOVE_UPLEFT:
                                if (CheckPlayerCollision(player, identifier))
                                {
                                    Move(identifier, player);
                                }
                                break;
                            case NetInterface.FIRE:
                                foreach (Player ply in players)
                                {
                                    if (player.PlayerID != ply.PlayerID)
                                    {
                                        if (Collision.BulletToPerson(player.GetPosition(), ply.GetPosition(),
                                                player.Rotation, 128f))
                                        {
                                            Console.WriteLine("Collision");
                                        }
                                    }
                                }
                                outMsg.Write(NetInterface.PLAY_SOUND);
                                outMsg.Write(player.PlayerID);
                                outMsg.Write(NetInterface.AK47_SHOT);
                                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
                                break;
                            case NetInterface.ROTATE:
                                outMsg.Write(NetInterface.ROTATE);
                                player = players.Find(ply => ply.Client.RemoteUniqueIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);
                                player.SetRotation(msg.ReadFloat());
                                outMsg.Write(player.PlayerID);
                                outMsg.Write(player.Rotation);
                                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
                                break;
                            case NetInterface.PLY_CHANGE_TEAM:
                                player = players.Find(ply => ply.Client.RemoteUniqueIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);
                                player.ChangeTeam(NetInterface.GetTeam(msg.ReadByte()));
                                outMsg.Write(NetInterface.PLY_CHANGE_TEAM);
                                outMsg.Write(player.PlayerID);
                                outMsg.Write(NetInterface.GetTeamByte(player.Team));
                                server.SendToAll(outMsg, NetDeliveryMethod.ReliableSequenced);
                                break;
                            case NetInterface.SPAWN_WEAPON:
                                short weapon = msg.ReadInt16();
                                player = players.Find(ply => ply.Client.RemoteUniqueIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);
                                if (WeaponInfo.GetWeaponType(WeaponInfo.GetWeapon(weapon)) ==
                                    WeaponInfo.WeaponType.Primary)
                                {
                                    player.SetPrimaryWeapon(weapon, entityCounter);
                                    player.SetCurrentWeapon(weapon, player.PrimaryWepEntID);

                                }
                                else
                                {
                                    player.SetSecondaryWeapon(weapon, entityCounter);
                                    player.SetCurrentWeapon(weapon, player.PrimaryWepEntID);
                                }
                                outMsg.Write(NetInterface.SPAWN_WEAPON);
                                outMsg.Write(player.PlayerID);
                                outMsg.Write(entityCounter);
                                outMsg.Write(weapon);
                                server.SendToAll(outMsg, NetDeliveryMethod.ReliableSequenced);
                                entityCounter++;
                                break;
                            case NetInterface.SWITCH_WEAPON:
                                player = players.Find(ply => ply.Client.RemoteUniqueIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);
                                player.SetCurrentWeapon(msg.ReadInt16(), msg.ReadInt16());
                                outMsg.Write(NetInterface.SWITCH_WEAPON);
                                outMsg.Write(player.PlayerID);
                                outMsg.Write(player.CurrentWeapon);
                                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
                                break;
                        }
                        break;
                }
            }
            server.Recycle(msg);
        }
        */

        #endregion

        #region New Code

        public static void Simulate()
        {
            // TODO: Runs all server based logic
            switch (state)
            {
                case RoundState.Empty:
                    // If someone joined a team, begin a new game
                    if (numCts > 0 || numTs > 0)
                    {

                        // If the timer has not started yet
                        if (!timer.IsRunning)
                        {
                            // start the timer
                            timer.Start();
                            Console.WriteLine("GAME STARTING");
                        }

                        // Give users a few seconds to join teams
                        // before reseting the round
                        if (timer.Elapsed.TotalSeconds >= AFTER_ROUND_TIME)
                        {
                            StartRound();
                            timer.Reset();
                        }
                    }
                    break;
                case RoundState.Buytime:
                    break;
                case RoundState.Play:
                    break;
                case RoundState.AfterRound:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void UpdateNetwork()
        {
            // TODO : Updates the network, recieves input.
            while ((msg = server.ReadMessage()) != null)
            {
                outMsg = server.CreateMessage();
                ServerPlayer player;
                byte code;
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus) msg.ReadByte())
                        {
                            case NetConnectionStatus.Connected:
                                // If someone has successfully connected to the server, initialize
                                // handshake with the client. Give them a unique identifier
                                // which allows the server to differ between multiple clients
                                outMsg.Write(ServerClientInterface.HANDSHAKE);

                                // Send the message
                                server.SendMessage(outMsg, msg.SenderConnection, NetDeliveryMethod.ReliableSequenced);
                                break;
                            case NetConnectionStatus.Disconnected:
                                player =
                                    players.Find(
                                        ply => ply.ConnectionIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);
                                outMsg.Write(ServerClientInterface.PLAYER_DISCONNECTED);
                                outMsg.Write(player.Identifier);
                                server.SendToAll(outMsg, NetDeliveryMethod.ReliableSequenced);
                                Console.WriteLine("\"" + player.UserName + "\" has left the server");

                                switch (player.CurrentTeam)
                                {
                                    case ServerClientInterface.Team.CounterTerrorist:
                                        numCts--;
                                        break;
                                    case ServerClientInterface.Team.Terrorist:
                                        numTs--;
                                        break;
                                }
                                numPlayers--;
                                
                                players.Remove(player);
                                break;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        code = msg.ReadByte();
                        switch (code)
                        {
                            case ServerClientInterface.REQUEST_SYNC:
                                string username = msg.ReadString();

                                player = new ServerPlayer(username, playerIdentifier, msg.SenderConnection.RemoteUniqueIdentifier);
                                players.Add(player);
                                playerIdentifier++;
                                numPlayers++;

                                outMsg.Write(ServerClientInterface.HANDSHAKE_COMPLETE);
                                outMsg.Write(player.Identifier);
                                server.SendMessage(outMsg, msg.SenderConnection, NetDeliveryMethod.ReliableSequenced);

                                // Sync the new player with everyone else on the server
                                //SyncNewPlayer(player);

                                // Send data about everyone else to the new player
                                SyncCurrentPlayers();
                                Console.WriteLine("\"" + username + "\" has joined the server");
                                break;
                            case ServerClientInterface.CHANGE_TEAM:
                                player = players.Find(
                                    ply => ply.ConnectionIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);
                                    
                                player.SetTeam(msg.ReadByte());

                                Console.WriteLine("\"" + player.UserName + "\" joined the " + player.CurrentTeam);

                                switch (player.CurrentTeam)
                                {
                                    case ServerClientInterface.Team.CounterTerrorist:
                                        numCts++;
                                        break;
                                    case ServerClientInterface.Team.Terrorist:
                                        numTs++;
                                        break;
                                }

                                outMsg.Write(ServerClientInterface.CHANGE_TEAM);
                                outMsg.Write(player.Identifier);
                                outMsg.Write(ServerClientInterface.TeamToByte(player.CurrentTeam));
                                server.SendToAll(outMsg,NetDeliveryMethod.ReliableSequenced);

                                break;
                            case ServerClientInterface.MOVE_UP:
                            case ServerClientInterface.MOVE_DOWN:
                            case ServerClientInterface.MOVE_LEFT:
                            case ServerClientInterface.MOVE_RIGHT:
                            case ServerClientInterface.MOVE_UPLEFT:
                            case ServerClientInterface.MOVE_UPRIGHT:
                            case ServerClientInterface.MOVE_DOWNRIGHT:
                            case ServerClientInterface.MOVE_DOWNLEFT:
                                Move(code, msg.SenderConnection.RemoteUniqueIdentifier);
                                break;
                            case ServerClientInterface.ROTATE_PLAYER:
                                Rotate(msg.ReadFloat(), msg.SenderConnection.RemoteUniqueIdentifier);
                                break;
                            case ServerClientInterface.BUY_WEAPON:
                                byte wep = msg.ReadByte();
                                SpawnWeapon(msg.SenderConnection.RemoteUniqueIdentifier, wep);
                                break;
                            case ServerClientInterface.FIRE_WEAPON:
                                FireWeapon(msg.SenderConnection.RemoteUniqueIdentifier);
                                break;
                            case ServerClientInterface.EXPLODE_FLASHBANG:
                                outMsg.Write(ServerClientInterface.EXPLODE_FLASHBANG);
                                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
                                break;
                            case ServerClientInterface.REQUEST_RESPAWN:
                                // Client requested respawn
                                player =
                                    players.Find(
                                        ply => ply.ConnectionIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);

                                if (player.State == ServerClientInterface.PlayerState.Dead)
                                {
                                    RespawnPlayer(player);
                                }
                                break;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Sends data about a newly connected player to all players
        /// </summary>
        /// <param name="newPlayer"></param>
        public static void SyncNewPlayer(ServerPlayer newPlayer)
        {
            foreach (NetConnection client in server.Connections)
            {
                outMsg = server.CreateMessage();
                outMsg.Write(ServerClientInterface.SYNC_NEW_PLAYER);
                outMsg.Write(newPlayer.UserName);
                outMsg.Write(newPlayer.Identifier);

                // Don't resend the player's information to 
                if (client.RemoteUniqueIdentifier != newPlayer.ConnectionIdentifier)
                {
                    server.SendMessage(outMsg, client, NetDeliveryMethod.ReliableSequenced);
                }
            }
        }

        /// <summary>
        /// Sends data of all connected players
        /// </summary>
        public static void SyncCurrentPlayers()
        {
            foreach (ServerPlayer ply in players)
            {
                // Send data about every player except their own player to the client
                outMsg = server.CreateMessage();
                outMsg.Write(ServerClientInterface.SYNC_CHUNK);
                outMsg.Write(ply.Identifier);
                outMsg.Write(ply.UserName);
                outMsg.Write(ServerClientInterface.TeamToByte(ply.CurrentTeam));
                outMsg.Write(ply.Position.X);
                outMsg.Write(ply.Position.Y);
                outMsg.Write(ply.Rotation);
                outMsg.Write(WeaponData.WeaponToByte(ply.CurrentWeapon.Weapon));
                outMsg.Write(ServerClientInterface.StateToByte(ply.State));
                server.SendToAll(outMsg, NetDeliveryMethod.ReliableSequenced);
            }
            outMsg = server.CreateMessage();
            outMsg.Write(ServerClientInterface.SYNC_COMPLETE);
            server.SendToAll(outMsg, NetDeliveryMethod.ReliableSequenced);
        }

        static void FireWeapon(long identifier)
        {
            ServerPlayer player = players.Find(ply => ply.ConnectionIdentifier == identifier);
            
            outMsg.Write(ServerClientInterface.FIRE_WEAPON);
            outMsg.Write(player.Identifier);
            server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
            PlayerToPlayer(player);
        }

        static bool PlayerToPlayer(ServerPlayer shooter)
        {
            // Get the distance between the player and the shooter
            foreach (ServerPlayer player in players)
            {
                // The shooter can't shoot themself, obviously.
                if (player != shooter)
                {

                    // If the shot passes through the player 
                    if (Collision.BulletToPlayer(shooter.Position, player.Position,
                        shooter.Rotation, 24f, new Rectangle(
                            (int)player.Position.X -16, (int)player.Position.Y +16,
                            32, 32), player.Rotation) && player.State == ServerClientInterface.PlayerState.Alive)
                    {
                        player.Damage(15, 0);

                        if (player.Health <= 0)
                        {
                            player.SetHealth(0);
                            player.SetArmor(0);
                            player.SetState(ServerClientInterface.PlayerState.Dead);
                            Console.WriteLine(shooter.UserName + " killed " + player.UserName +
                                " with " + shooter.CurrentWeapon.Weapon);
                        }

                        outMsg = server.CreateMessage();
                        outMsg.Write(ServerClientInterface.DAMAGE);
                        outMsg.Write(player.Identifier);
                        outMsg.Write(player.Health);
                        outMsg.Write(player.Armor);
                        server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
                        // Check if there are any obstacles in between the player and
                        // the shooter
                        Console.WriteLine("lit for 99 with weapon_ak47. csgo logic");
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Moves a player
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="identifier"></param>
        static void Move(byte direction, long identifier)
        {
            ServerPlayer player = players.Find(ply => ply.ConnectionIdentifier == identifier);

            if (CheckPlayerCollision(player, direction))
            {
                player.Move(direction);
                outMsg.Write(direction);
                outMsg.Write(player.Identifier);
                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
            }
        }

        /// <summary>
        /// Rotates a player
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="identifier"></param>
        static void Rotate(float rotation, long identifier)
        {
            ServerPlayer player = players.Find(ply => ply.ConnectionIdentifier == identifier);
            player.Rotate(rotation);

            outMsg.Write(ServerClientInterface.ROTATE_PLAYER);
            outMsg.Write(player.Identifier);
            outMsg.Write(player.Rotation);
            server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
        }

        static void SyncWorld()
        {
            // TODO: Sends a snapshot of the world to all players to ensure
            // TODO: everyone is viewing the same thing
            foreach (ServerPlayer player in players)
            {
                outMsg = server.CreateMessage();
                outMsg.Write(ServerClientInterface.SYNC_MOVEMENT);
                outMsg.Write(player.Identifier);
                outMsg.Write(player.Position.X);
                outMsg.Write(player.Position.Y);
                outMsg.Write(player.Rotation);
                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
            }
        }

        static void StartRound()
        {
            // TODO: Spawns all players, sets up all timers, etc
            // Reset everyone's health
            Console.WriteLine("FREEZE TIME START");
            // Spawn all players

            foreach (ServerPlayer player in players)
            {
                if (player.CurrentTeam != ServerClientInterface.Team.Spectator)
                {
                    RespawnPlayer(player);
                    Console.WriteLine("Spawned \"" + player.UserName + "\"");
                }
            }

            state = RoundState.Buytime;
        }

        static void EndRound()
        {
            // TODO: Processes kills, calculates money, etc
            // If the server was empty before this method was called
            // Set up everyones start money
            if (state == RoundState.Empty)
            {
                foreach (ServerPlayer player in players)
                {
                    player.SetCash(800);
                    SpawnPlayer(player.Identifier, 
                        new Vector2(0 + (100*player.Identifier), 0 + (100*player.Identifier)));
                }
            }
        }

        static void SpawnWeapon(long identifier, byte weapon)
        {
            // TODO: Gives a weapon to a player
            ServerPlayer player = players.Find(ply => ply.ConnectionIdentifier == identifier);
            player.SetWeapon(WeaponData.ByteToWeapon(weapon));
            
            outMsg = server.CreateMessage();
            outMsg.Write(ServerClientInterface.BUY_WEAPON);
            outMsg.Write(player.Identifier);
            outMsg.Write(weapon);
            server.SendToAll(outMsg, NetDeliveryMethod.ReliableSequenced);
            Console.WriteLine("\"" + player.UserName + "\" purchased weapon " + WeaponData.ByteToWeapon(weapon));
        }

        static void SpawnPlayer(long playerIdentifier, Vector2 location)
        {
            // TODO: Spawns a player onto the map
        }

        /// <summary>
        /// Spawns a player who is currently alive
        /// </summary>
        /// <param name="player"></param>
        /// <param name="location"></param>
        static void SpawnPlayer(ServerPlayer player)
        {
            player.SetState(ServerClientInterface.PlayerState.Alive);
            player.SetHealth(100);
            player.SetArmor(0);

            outMsg = server.CreateMessage();
            outMsg.Write(ServerClientInterface.SPAWN_PLAYER);
            outMsg.Write(player.Identifier);
        }

        static void RespawnPlayer(ServerPlayer player)
        {
            Vector2 location = Vector2.Zero;
            int spawnPoint;
            switch (player.CurrentTeam)
            {
                case ServerClientInterface.Team.CounterTerrorist:
                    spawnPoint = rand.Next(0, MapData.CTTile.Count);
                    location = new Vector2(MapData.CTTile[spawnPoint].TileRect.X + 16,
                        MapData.CTTile[spawnPoint].TileRect.Y + 16);
                    break;
                case ServerClientInterface.Team.Terrorist:
                    spawnPoint = rand.Next(0, MapData.TTile.Count);
                    location = new Vector2(MapData.TTile[spawnPoint].TileRect.X + 16,
                        MapData.TTile[spawnPoint].TileRect.Y + 16);
                    break;
            }

            outMsg = server.CreateMessage();
            player.Respawn(location);
            outMsg.Write(ServerClientInterface.RESPAWN_PLAYER);
            outMsg.Write(player.Identifier);
            outMsg.Write(player.Position.X);
            outMsg.Write(player.Position.Y);
            server.SendToAll(outMsg, NetDeliveryMethod.ReliableSequenced);
        }

        static void PlantBomb(long playerIdentifier, Vector2 location, bool aSite)
        {
            // TODO: Spawns a bomb at a site

            
            Console.WriteLine("Player planted the bomb at " + (aSite? "A Site" : "B Site"));
        }

        #endregion

        /// <summary>
        /// Reads the configuration 
        /// </summary>
        static void ReadConfig()
        {
            // If the config file exists, open it and process it
            if (File.Exists("config.txt"))
            {
                string[] commands = File.ReadAllLines("config.txt");

                foreach (string command in commands)
                {
                    string[] cmd = command.Split('=');

                    switch (cmd[0])
                    {
                        case "servername=":
                            break;
                    }
                }

                Console.WriteLine("Configuration Successfully Loaded.");
            }
            else
            {
                WriteFile();
            }
        }

        /// <summary>
        /// Writes a fresh config to the directory if none exists
        /// </summary>
        static void WriteFile()
        {
            Console.WriteLine("Configuration not found, creating default configuration...");
            StreamWriter writer = File.CreateText("config.txt");

            // Default settings
            writer.WriteLine("cstrikeserverconfig=" + serverVersion);
            writer.WriteLine("servername=" + serverName);
            writer.WriteLine("port=" + port);
            writer.WriteLine("maxplayers=" + maxPlayers);

            writer.Close();

            Console.WriteLine("Configuration written to disk.");
        }

        /// <summary>
        /// Checks the collision for a player and their direction
        /// </summary>
        /// <param name="player"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        static bool CheckPlayerCollision(ServerPlayer player, byte direction)
        {
            if (player.CurrentTeam != ServerClientInterface.Team.Spectator)
            {
                if (enableCollision)
                {
                    // Get the direection
                    float vectorX = 0f;
                    float vectorY = 0f;
                    switch (direction)
                    {
                        case ServerClientInterface.MOVE_UP:
                            vectorY = -ServerClientInterface.MOVEMENT_SPEED;
                            break;
                        case ServerClientInterface.MOVE_DOWN:
                            vectorY = ServerClientInterface.MOVEMENT_SPEED;
                            break;
                        case ServerClientInterface.MOVE_LEFT:
                            vectorX = -ServerClientInterface.MOVEMENT_SPEED;
                            break;
                        case ServerClientInterface.MOVE_RIGHT:
                            vectorX = ServerClientInterface.MOVEMENT_SPEED;
                            break;
                        case ServerClientInterface.MOVE_UPRIGHT:
                            vectorX = ServerClientInterface.MOVEMENT_SPEED;
                            vectorY = -ServerClientInterface.MOVEMENT_SPEED;
                            break;
                        case ServerClientInterface.MOVE_DOWNRIGHT:
                            vectorX = ServerClientInterface.MOVEMENT_SPEED;
                            vectorY = ServerClientInterface.MOVEMENT_SPEED;
                            break;
                        case ServerClientInterface.MOVE_DOWNLEFT:
                            vectorX = -ServerClientInterface.MOVEMENT_SPEED;
                            vectorY = ServerClientInterface.MOVEMENT_SPEED;
                            break;
                        case ServerClientInterface.MOVE_UPLEFT:
                            vectorX = -ServerClientInterface.MOVEMENT_SPEED;
                            vectorY = -ServerClientInterface.MOVEMENT_SPEED;
                            break;
                    }

                    // Cirlce to Circle collision with the player wishing to move
                    // and every other player on the map. Returning false
                    // prevents the movement byte being sent to the player
                    foreach (ServerPlayer ply in players)
                    {
                        if (ply.Identifier != player.Identifier)
                        {
                            if (Collision.PlayerToPlayer(new Vector2(player.Position.X + vectorX,
                                player.Position.Y + vectorY), ply.Position, 23f))
                            {
                                return false;
                            }
                        }
                    }
                }

                // Player to wall collision
                // Gets the tiles that need to be checked
                Tile[] tiles = GetTiles(player.Position, direction);

                // Check each tile that is not null and is a solid
                // Circle to Rectangle collision. Returns false
                // if a collision is found
                foreach (Tile tile in tiles)
                {
                    if (tile != null && tile.Property == Tile.SOLID)
                    {
                        if (Collision.PlayerToRectangle(player.Position, tile.TileRect, 14f))
                        {
                            return false;
                        }
                    }
                }

                // If no collisions occur, allow the player to move
                return true;
            }
            return true;

            #region Old Code

            /*
            switch (direction)
            {
                case NetInterface.MOVE_UP:
                    foreach (Player ply in players)
                    {
                        if (ply.PlayerID != player.PlayerID)
                        {
                            if (Collision.PlayerToPlayer(new Vector2(player.GetPosition().X,
                                player.GetPosition().Y - 5f), ply.GetPosition(), 23f))
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case NetInterface.MOVE_DOWN:
                    foreach (Player ply in players)
                    {
                        if (ply.PlayerID != player.PlayerID)
                        {
                            if (Collision.PlayerToPlayer(new Vector2(player.GetPosition().X,
                                player.GetPosition().Y + 5f), ply.GetPosition(), 23f))
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case NetInterface.MOVE_LEFT:
                    foreach (Player ply in players)
                    {
                        if (ply.PlayerID != player.PlayerID)
                        {
                            if (Collision.PlayerToPlayer(new Vector2(player.GetPosition().X - 5f,
                                player.GetPosition().Y), ply.GetPosition(), 23f))
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case NetInterface.MOVE_RIGHT:
                    foreach (Player ply in players)
                    {
                        if (ply.PlayerID != player.PlayerID)
                        {
                            if (Collision.PlayerToPlayer(new Vector2(player.GetPosition().X + 5f,
                                player.GetPosition().Y), ply.GetPosition(), 23f))
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case NetInterface.MOVE_UPRIGHT:
                    foreach (Player ply in players)
                    {
                        if (ply.PlayerID != player.PlayerID)
                        {
                            if (Collision.PlayerToPlayer(new Vector2(player.GetPosition().X + 5f,
                                player.GetPosition().Y), ply.GetPosition(), 23f))
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case NetInterface.MOVE_DOWNRIGHT:
                    foreach (Player ply in players)
                    {
                        if (ply.PlayerID != player.PlayerID)
                        {
                            if (Collision.PlayerToPlayer(new Vector2(player.GetPosition().X + 5f,
                                player.GetPosition().Y), ply.GetPosition(), 23f))
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case NetInterface.MOVE_DOWNLEFT:
                    foreach (Player ply in players)
                    {
                        if (ply.PlayerID != player.PlayerID)
                        {
                            if (Collision.PlayerToPlayer(new Vector2(player.GetPosition().X + 5f,
                                player.GetPosition().Y), ply.GetPosition(), 23f))
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case NetInterface.MOVE_UPLEFT:
                    foreach (Player ply in players)
                    {
                        if (ply.PlayerID != player.PlayerID)
                        {
                            if (Collision.PlayerToPlayer(new Vector2(player.GetPosition().X - 5f,
                                player.GetPosition().Y - 5f), ply.GetPosition(), 23f))
                            {
                                return false;
                            }
                        }
                    }
                    break;
            }
            return true;
             */

            #endregion
        }

        public static Tile[] GetTiles(Vector2 position, byte direction)
        {
            List<Tile> tiles = new List<Tile>();
            Point location = new Point((int)(position.X) / ServerMap.TILE_SIZE, (int)(position.Y) / ServerMap.TILE_SIZE);

            // Gets the points that need to be checked
            switch (direction)
            {
                case ServerClientInterface.MOVE_UP:
                    tiles.Add(MapData.TileMap[location.X, location.Y - 1]);     // UP
                    //tiles.Add(MapData.TileMap[location.X - 1, location.Y - 1]); // UP-LEFT
                    //tiles.Add(MapData.TileMap[location.X + 1, location.Y - 1]); // UP-RIGHT
                    break;
                case ServerClientInterface.MOVE_DOWN:
                    tiles.Add(MapData.TileMap[location.X, location.Y + 1]);     // DOWN
                    //tiles.Add(MapData.TileMap[location.X - 1, location.Y + 1]); // DOWN-LEFT
                    //tiles.Add(MapData.TileMap[location.X + 1, location.Y + 1]); // DOWN-RIGHT
                    break;
                case ServerClientInterface.MOVE_LEFT:
                    tiles.Add(MapData.TileMap[location.X - 1, location.Y]);     // LEFT
                    //tiles.Add(MapData.TileMap[location.X - 1, location.Y - 1]); // LEFT-UP
                    //tiles.Add(MapData.TileMap[location.X - 1, location.Y + 1]); // LEFT-DOWN
                    break;
                case ServerClientInterface.MOVE_RIGHT:
                    tiles.Add(MapData.TileMap[location.X + 1, location.Y]);     // RIGHT
                    //tiles.Add(MapData.TileMap[location.X + 1, location.Y - 1]); // RIGHT-UP
                    //tiles.Add(MapData.TileMap[location.X + 1, location.Y + 1]); // RIGHT-DOWN
                    break;
                case ServerClientInterface.MOVE_UPLEFT:
                    tiles.Add(MapData.TileMap[location.X, location.Y - 1]);     // UP
                    tiles.Add(MapData.TileMap[location.X - 1, location.Y - 1]); // UP-LEFT
                    tiles.Add(MapData.TileMap[location.X - 1, location.Y]);     // LEFT
                    break;
                case ServerClientInterface.MOVE_UPRIGHT:
                    tiles.Add(MapData.TileMap[location.X, location.Y - 1]);     // UP
                    tiles.Add(MapData.TileMap[location.X + 1, location.Y - 1]); // UP-RIGHT
                    tiles.Add(MapData.TileMap[location.X + 1, location.Y]);     // RIGHT
                    break;
                case ServerClientInterface.MOVE_DOWNRIGHT:
                    tiles.Add(MapData.TileMap[location.X, location.Y + 1]);     // DOWN
                    tiles.Add(MapData.TileMap[location.X + 1, location.Y + 1]); // DOWN-RIGHT
                    tiles.Add(MapData.TileMap[location.X + 1, location.Y]);     // RIGHT
                    break;
                case ServerClientInterface.MOVE_DOWNLEFT:
                    tiles.Add(MapData.TileMap[location.X, location.Y + 1]);     // DOWN
                    tiles.Add(MapData.TileMap[location.X - 1, location.Y + 1]); // DOWN-LEFT
                    tiles.Add(MapData.TileMap[location.X - 1, location.Y]);     // LEFT
                    break;
            }

            return tiles.ToArray();
        }
    }
}