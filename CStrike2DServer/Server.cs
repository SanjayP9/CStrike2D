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
        private static List<ServerWeapon> weapons = new List<ServerWeapon>();
        private static List<ServerGrenade> grenades = new List<ServerGrenade>();

        private static short playerIdentifier;
        private static string serverVersion = "0.4.0a";            // Server Version
        private static int maxPlayers = 32;
        private static int port = 27015;
        private static string buffer = "";
        private static string serverName = "Global Offensive Server - " + serverVersion;
        private static bool forceConfigRewrite = true;
        private static NetOutgoingMessage outMsg;
        static int curRow = 3;
        private static int tickCount;
        private static int bytesIn;
        private static int bytesOut;
        private static int maxCTs = 16;
        private static int maxTs = 16;
        private static int numTs = 0;
        private static int numCts = 0;
        private static float roundTimer = 0;
        private static float numPlayers = 0;
        private static bool enableCollision = true;
        private static short entityCounter;
        
        private static double updateTimer;
        public const double UPDATE_RATE = 7d;
        public const double NET_UPDATE_RATE = 15d;
        public const double SYNC_RATE = 20d;

        private const float FREEZE_TIME = 5f;
        private float freezeTime;

        private static int tsAlive;
        private static int ctsAlive;
        private static string mapName = "de_cache";

        private static RoundState state = RoundState.Empty;
        private static int numCols;
        private static int numRows;
        private static Rectangle mapArea;
        private static Tile[,] tiles;
        private static DemoRecorder recorder;

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

            Console.WriteLine("Booting up server...");

            config = new NetPeerConfiguration("cstrike") {Port = port, EnableUPnP = true};
            server = new NetServer(config);
            server.Start();
            Console.WriteLine("Server is live.");
            Thread.Sleep(1000);

            recorder.StartRecording(mapName);
            updateTimer.Start();
            netUpdateTimer.Start();
            while (server.Status == NetPeerStatus.Running)
            {
                Console.Clear();   
                Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
                Console.WriteLine("Players " + numPlayers + "/" + maxPlayers);

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

            /*
            int tick = 0;
            while (server.Status == NetPeerStatus.Running)
            {
                if (Console.KeyAvailable)
                {
                    buffer += Console.ReadKey();
                }

                if (sw.Elapsed.TotalMilliseconds >= 15.625)
                {
                    tick++;
                    Update();
                    bytesIn += server.Statistics.ReceivedBytes;
                    bytesOut += server.Statistics.SentBytes;

                    sw.Restart();

                    if (tick == 60)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("==================================");
                        Console.WriteLine("Global Offensive - Version " + serverVersion);
                        Console.WriteLine("==================================");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Players: " + players.Count + "/" + maxPlayers);

                        foreach (Player ply in players)
                        {
                            Console.WriteLine("Client: " + ply.PlayerName + " Ping: " + Math.Round(ply.Client.AverageRoundtripTime, 2) + "ms");
                        }
                        Console.WriteLine("KB In: " + bytesIn / 1024d + " KB Out: " + bytesOut / 1024d);
                        bytesIn = 0;
                        bytesOut = 0;
                        Console.WriteLine(buffer);
                        SyncServer();
                        tick = 0;
                    }
                }
            }
            */
        }

        static void LoadMap(string mapName)
        {
            if (!File.Exists(mapName))
            {
                Console.WriteLine("Missing map: "+ mapName);
                return;
            }

            StreamReader inFile = File.OpenText(mapName);

            // Stores the data for a single line as a time

            // Checks the first and second line of the text to set the number of columns and the number of rows
            numCols = Convert.ToInt32(inFile.ReadLine());
            numRows = Convert.ToInt32(inFile.ReadLine());

            // Changes the placement area according to the number of columns and rows
            mapArea = new Rectangle(-200, -250, 32 * numCols, 32 * numRows);

            // Initialize the number of tiles to be according the the number of columns and rows
            tiles = new Tile[numCols, numRows];

            // Goes through every line in the text past the first two
            for (int rows = 0; rows < numRows; rows++)
            {
                // Sets the row data to be split by commas to siginify a new column
                string[] rowData = inFile.ReadLine().Split(',');

                // Goes through every column in the row
                for (int cols = 0; cols < rowData.Length; cols++)
                {
                    // If the data in the column is not blank
                    if (rowData[cols] != "")
                    {
                        // According to each character in the text check to see for the 0/1
                        // for each property and set the property to be true or false accordingly
                        bool isPlantSpot = rowData[cols][rowData[cols].Length - 6] == '1';

                        bool isSaveSpot = rowData[cols][rowData[cols].Length - 5] == '1';

                        bool isSolid = rowData[cols][rowData[cols].Length - 4] == '1';

                        bool isCTSpawnPoint = rowData[cols][rowData[cols].Length - 3] == '1';

                        bool isTSpawnPoint = rowData[cols][rowData[cols].Length - 2] == '1';

                        bool isSiteDefencePoint = rowData[cols][rowData[cols].Length - 1] == '1';

                        // Initialize each property of the tile
                        tiles[cols, rows] = new Tile(Convert.ToInt32(rowData[cols].Substring(0, rowData[cols].Length - 6)), isPlantSpot, isSaveSpot, isSolid, isCTSpawnPoint, isTSpawnPoint, isSiteDefencePoint);
                    }
                }
            }

            // Close the file
            inFile.Close();
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
                    break;
                case RoundState.Buytime:
                    break;
                case RoundState.Play:
                    break;
                case RoundState.AfterRound:
                    break;
            }
        }

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
                                SyncNewPlayer(player);
                                SyncCurrentPlayers(msg.SenderConnection);

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

                                RespawnPlayer(player, new Vector2(numCts));
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
        /// Sends data of all connected players to a newly connected client
        /// </summary>
        /// <param name="client"></param>
        public static void SyncCurrentPlayers(NetConnection client)
        {
            outMsg = server.CreateMessage();
            outMsg.Write(ServerClientInterface.SYNC_BEGIN);
            server.SendMessage(outMsg, client, NetDeliveryMethod.ReliableSequenced);
            foreach (ServerPlayer ply in players)
            {
                // Send data about every player except their own player to the client
                if (ply.ConnectionIdentifier != client.RemoteUniqueIdentifier)
                {
                    outMsg = server.CreateMessage();
                    outMsg.Write(ServerClientInterface.SYNC_CHUNK);
                    outMsg.Write(ply.Identifier);
                    outMsg.Write(ply.UserName);
                    outMsg.Write(ServerClientInterface.TeamToByte(ply.CurrentTeam));
                    outMsg.Write(ply.Position.X);
                    outMsg.Write(ply.Position.Y);
                    outMsg.Write(ply.Rotation);
                    outMsg.Write(WeaponData.WeaponToByte(ply.CurrentWeapon.Weapon));
                    server.SendMessage(outMsg, client, NetDeliveryMethod.ReliableSequenced);
                }
            }
            outMsg = server.CreateMessage();
            outMsg.Write(ServerClientInterface.SYNC_COMPLETE);
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
            foreach (ServerPlayer ply in players)
            {
                ply.SetHealth(100);
                ply.SetArmor(0);

                if (ply.State == ServerPlayer.PlayerState.Dead)
                {
                    ply.SetState(ServerPlayer.PlayerState.Alive);
                }
            }
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
                    SpawnPlayer(player, 
                        new Vector2(0 + (100*player.Identifier), 0 + (100*player.Identifier)));
                }
            }
        }

        static void SpawnWeapon(long playerIdentifier, ServerWeapon weapon)
        {
            // TODO: Gives a weapon to a player
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
        static void SpawnPlayer(ServerPlayer player, Vector2 location)
        {
            // TODO: Spawns a player onto the map
            player.SetPosition(location);
        }

        static void RespawnPlayer(ServerPlayer player, Vector2 location)
        {
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

        static bool CheckPlayerCollision(ServerPlayer player, byte direction)
        {
            if (player.CurrentTeam != ServerClientInterface.Team.Spectator && enableCollision)
            {
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
    }
}