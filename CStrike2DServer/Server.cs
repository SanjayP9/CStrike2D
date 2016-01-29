// Author: Mark Voong
// File Name: Server.cs
// Project Name: Global Offensive
// Creation Date: Jan 3rd, 2016
// Modified Date: Jan 21st, 2016
// Description: Handles all logic and drawing of the in-game components
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CStrike2D;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace CStrike2DServer
{
    class Server
    {
        private static string serverVersion = "0.7.0a";                                     // Server Version
        private static string serverName = "Global Offensive Server - " + serverVersion;    // The name of the server

        private static NetServer server;                // Lidgren server API
        private static NetPeerConfiguration config;     // Network configuration properties
        private static NetIncomingMessage msg;          // Container for reading messages sent by clients
        private static NetOutgoingMessage outMsg;       // Container for writing messages that are to be sent to clients

        private static List<ServerPlayer> players = new List<ServerPlayer>();       // Stores all clients in the server
        private static List<ServerGrenade> grenades = new List<ServerGrenade>();    // Stores all grenades active in the server

        private static short playerIdentifier;              // Used to give clients a different identifier from eachother
        private static int maxPlayers = 32;                 // Maximum number of players allowed to connect
        private static int port = 27015;                    // Port the server recieves traffic on
        private static string buffer = "";                  // Used for processing user commands entered in the server
        private static bool forceConfigRewrite = true;
        private static int maxCTs = 16;                     // Maximum number of Counter-Terrorist players
        private static int maxTs = 16;                      // Maximum number of Terrorist players
        private static int numTs;                           // Number of currently connected terrorists
        private static int numCts;                          // Number of currently connected counter terrorists
        private static float numPlayers;                    // Counts number of currently connected players
        private static bool enableCollision = true;         // Should players be allowed to collide with eachoter
        private static bool friendlyFire = false;           // Whether shooting teammates should cause damage
        private static short entityCounter;                 // Entity counter used to assign unique identifiers to grenades
        
        public const double UPDATE_RATE = 7d;               // Update rate of the server
        public const double NET_UPDATE_RATE = 16.66d;       // Rate at which the server processes the network
        public const double SYNC_RATE = 20d;

        private const float AFTER_ROUND_TIME = 5f;          // Time between switching the round state
        private const float FREEZE_TIME = 10f;              // Time at the start of a round to allow users to buy
        private static Stopwatch timer = new Stopwatch();   // Used to time the after round timer and freeze time

        private static int tsAlive;                         // Number of Terrorists alive
        private static int ctsAlive;                        // Number of Counter-Terrorists alive
        private static string mapName = "de_cache";         // Current map

        private static RoundState state = RoundState.Empty; // Default the current round state to an empty server
        private static DemoRecorder recorder;               // Used to record the game state

        private static Random rand = new Random();          // Random number generator

        private static bool teamCollision = false;

        /// <summary>
        /// Stores data about the map including tile types such as
        /// spawn points and AI nodes
        /// </summary>
        public static ServerMap MapData { get; private set; }

        private static RayCast raycaster = new RayCast();   // Used for bullet to player collision detection

        /// <summary>
        /// Different round states the server is in
        /// </summary>
        enum RoundState
        {
            Empty,
            Buytime,
            Play,
            AfterRound
        }

        /// <summary>
        /// Returns the list of players for data saving
        /// </summary>
        /// <returns></returns>
        public static List<ServerPlayer> RetrievePlayers()
        {
            return players;
        }

        /// <summary>
        /// Entry point of the server
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            // Start recorder and map
            recorder = new DemoRecorder();
            MapData = new ServerMap();

            // Start up all timers
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

            // Read config file
            ReadConfig();

            Console.WriteLine("Loading map de_cache...");

            // If the map failed to load, do not allow the server to run
            if (!LoadMap("de_cache.txt"))
            {
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Booting up server...");

            // Start server API
            config = new NetPeerConfiguration("cstrike") {Port = port, EnableUPnP = true};
            server = new NetServer(config);
            server.Start();
            Thread.Sleep(1000);
            Console.WriteLine("Server is live.");

            // Start recording
            recorder.StartRecording(mapName);

            // Start timers
            updateTimer.Start();
            netUpdateTimer.Start();

            // Server loop until the user presses escape
            while (server.Status == NetPeerStatus.Running)
            {
                // If the user presses escape, the server gracefully shuts down
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();

                    if (key.Key == ConsoleKey.Escape)
                    {
                        recorder.EndRecording();
                        server.Shutdown("bye");
                        return;
                    }
                }

                // Simulate world logic
                if (updateTimer.Elapsed.TotalMilliseconds > UPDATE_RATE)
                {
                    Simulate();
                    recorder.Update();
                    updateTimer.Restart();
                }

                // Process network activity
                if (netUpdateTimer.Elapsed.TotalMilliseconds > NET_UPDATE_RATE)
                {
                    UpdateNetwork();
                    netUpdateTimer.Restart();
                }

                // Synchronizes everyone at specific intervals
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

        /// <summary>
        /// Simulates world logic
        /// </summary>
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
                            Console.WriteLine("A game has begun");
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
                    if (!timer.IsRunning)
                    {
                        timer.Start();
                        Console.WriteLine("Buytime...");

                    }

                    // If someone joins a team during buytime and they're dead, respawn them
                    foreach (ServerPlayer player in players)
                    {
                        if (player.State == ServerClientInterface.PlayerState.Dead &&
                            player.CurrentTeam != ServerClientInterface.Team.Spectator)
                        {
                            RespawnPlayer(player);
                        }
                    }

                    // If buytime is over, start the round
                    if (timer.Elapsed.TotalSeconds >= FREEZE_TIME)
                    {
                        // Let everyone know the round started
                        outMsg = server.CreateMessage();
                        outMsg.Write(ServerClientInterface.BEGIN_ROUND);
                        server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
                        state = RoundState.Play;
                    }
                    break;
                case RoundState.Play:
                    
                    break;
                case RoundState.AfterRound:
                    break;
            }
        }

        /// <summary>
        /// Updates and processes requests by users
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
                                // If someone has successfully connected to the server, check
                                // if there are too many connected clients and prevent entry from
                                // the client if the number of users exceed the max number of players
                                if (numPlayers > maxPlayers)
                                {
                                    msg.SenderConnection.Deny("Server Is Full");
                                }
                                
                                // initialize handshake with the client. Give them a unique identifier
                                // which allows the server to differ between multiple clients
                                outMsg.Write(ServerClientInterface.HANDSHAKE);

                                // Send the message
                                server.SendMessage(outMsg, msg.SenderConnection, NetDeliveryMethod.ReliableSequenced);
                                break;
                            case NetConnectionStatus.Disconnected:
                                // Get the player that just disconnected
                                player =
                                    players.Find(
                                        ply => ply.ConnectionIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);

                                // Tell everyone that the user disconnected
                                outMsg.Write(ServerClientInterface.PLAYER_DISCONNECTED);
                                outMsg.Write(player.Identifier);
                                server.SendToAll(outMsg, NetDeliveryMethod.ReliableSequenced);

                                // Subtract number of players of the associated team
                                switch (player.CurrentTeam)
                                {
                                    case ServerClientInterface.Team.CounterTerrorist:
                                        numCts--;
                                        break;
                                    case ServerClientInterface.Team.Terrorist:
                                        numTs--;
                                        break;
                                }
                                // Subtract one less player on the server
                                numPlayers--;

                                Console.WriteLine("\"" + player.UserName + "\" has left the server");
                                
                                // Remove the player from the server
                                players.Remove(player);
                                break;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        // Get identifier byte used to determine message type
                        code = msg.ReadByte();
                        switch (code)
                        {
                                // A user requested to retrieve information of all players on server
                            case ServerClientInterface.REQUEST_SYNC:
                                string username = msg.ReadString();
                                // Set up a new player in the server
                                player = new ServerPlayer(username, playerIdentifier, msg.SenderConnection.RemoteUniqueIdentifier);
                                players.Add(player);
                                playerIdentifier++;
                                numPlayers++;

                                // Let the client know their information was recieved and processed
                                outMsg.Write(ServerClientInterface.HANDSHAKE_COMPLETE);
                                outMsg.Write(player.Identifier);
                                server.SendMessage(outMsg, msg.SenderConnection, NetDeliveryMethod.ReliableSequenced);

                                // Resend data about players to everyone in order to stay in sync
                                SyncCurrentPlayers();
                                Console.WriteLine("\"" + username + "\" has joined the server");
                                break;
                            case ServerClientInterface.CHANGE_TEAM:
                                // Find the player with the matching unique identifier
                                player = players.Find(
                                    ply => ply.ConnectionIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);
                                    
                                // Change their team
                                player.SetTeam(msg.ReadByte());

                                // Increase the number of players currently on the
                                // associated team
                                switch (player.CurrentTeam)
                                {
                                    case ServerClientInterface.Team.CounterTerrorist:
                                        numCts++;
                                        break;
                                    case ServerClientInterface.Team.Terrorist:
                                        numTs++;
                                        break;
                                }

                                // Tell everyone else that the player had switched teams and
                                // what team they switched to
                                outMsg.Write(ServerClientInterface.CHANGE_TEAM);
                                outMsg.Write(player.Identifier);
                                outMsg.Write(ServerClientInterface.TeamToByte(player.CurrentTeam));
                                server.SendToAll(outMsg,NetDeliveryMethod.ReliableSequenced);

                                Console.WriteLine("\"" + player.UserName + "\" joined the " + player.CurrentTeam);
                                break;
                                // Movement Processing for 8 directions
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
                                // Rotation processing
                            case ServerClientInterface.ROTATE_PLAYER:
                                Rotate(msg.ReadFloat(), msg.SenderConnection.RemoteUniqueIdentifier);
                                break;
                                // Weapon buying processing
                            case ServerClientInterface.BUY_WEAPON:
                                byte wep = msg.ReadByte();
                                SpawnWeapon(msg.SenderConnection.RemoteUniqueIdentifier, wep);
                                break;
                                // Weapon firing processing
                            case ServerClientInterface.FIRE_WEAPON:
                                FireWeapon(msg.SenderConnection.RemoteUniqueIdentifier);
                                break;
                                // Flashbang exploding processing
                            case ServerClientInterface.EXPLODE_FLASHBANG:
                                outMsg.Write(ServerClientInterface.EXPLODE_FLASHBANG);
                                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
                                break;
                                // User requested respawn
                            case ServerClientInterface.REQUEST_RESPAWN:
                                // Client requested respawn
                                player =
                                    players.Find(
                                        ply => ply.ConnectionIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);

                                // If they are dead, respawn the player
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

            // Let everyone know the sync is complete
            outMsg = server.CreateMessage();
            outMsg.Write(ServerClientInterface.SYNC_COMPLETE);
            server.SendToAll(outMsg, NetDeliveryMethod.ReliableSequenced);
        }

        /// <summary>
        /// Processes firing request from a user
        /// </summary>
        /// <param name="identifier"></param>
        static void FireWeapon(long identifier)
        {
            if (state == RoundState.Play)
            {
                ServerPlayer player = players.Find(ply => ply.ConnectionIdentifier == identifier);

                // Tell everyone that the user fired their weapon
                outMsg.Write(ServerClientInterface.FIRE_WEAPON);
                outMsg.Write(player.Identifier);
                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);

                // Do collision detection for bulet
                BulletToPlayer(player);
            }
        }

        /// <summary>
        /// Moves a player
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="identifier"></param>
        private static void Move(byte direction, long identifier)
        {
            if (state != RoundState.Buytime)
            {
                ServerPlayer player = players.Find(ply => ply.ConnectionIdentifier == identifier);

                // If the player is allowed to move there, let everyone know
                // the player requested to move in that direction
                if (CheckPlayerCollision(player, direction))
                {
                    player.Move(direction);
                    outMsg.Write(direction);
                    outMsg.Write(player.Identifier);
                    server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
                }
            }
        }

        /// <summary>
        /// Rotates a player
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="identifier"></param>
        static void Rotate(float rotation, long identifier)
        {
            // Sends data about the player's current rotation to everyone
            ServerPlayer player = players.Find(ply => ply.ConnectionIdentifier == identifier);
            player.Rotate(rotation);

            outMsg.Write(ServerClientInterface.ROTATE_PLAYER);
            outMsg.Write(player.Identifier);
            outMsg.Write(player.Rotation);
            server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
        }

        /// <summary>
        /// Synchronizes everyone on the server with the correct rotation and position
        /// </summary>
        static void SyncWorld()
        {
            // Send data such as position and rotation to everyone
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

        /// <summary>
        /// Commences the round
        /// </summary>
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

        /// <summary>
        /// Ends the round
        /// </summary>
        static void EndRound()
        {
            // TODO: Processes kills, calculates money, etc
            // If the server was empty before this method was called
            // Set up everyones start money
            

        }

        /// <summary>
        /// Gives a weapon to a player
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="weapon"></param>
        private static void SpawnWeapon(long identifier, byte weapon)
        {
            // Find the associated player and give them the specified weapon
            ServerPlayer player = players.Find(ply => ply.ConnectionIdentifier == identifier);
            player.SetWeapon(WeaponData.ByteToWeapon(weapon));

            // Let everyone else know what weapon they purchased
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

        /// <summary>
        /// Respwans a player at their spawn point
        /// </summary>
        /// <param name="player"></param>
        static void RespawnPlayer(ServerPlayer player)
        {
            // Default location
            Vector2 location = Vector2.Zero;

            // Randomly determine a spawn point depending on their team and
            // respawns them there
            int spawnPoint;
            switch (player.CurrentTeam)
            {
                case ServerClientInterface.Team.CounterTerrorist:
                    while (true)
                    {
                        // Get a random index in the CT spawn points
                        spawnPoint = rand.Next(0, MapData.CTTile.Count);

                        bool empty = true;
                        // Check if nobody is currently on that spawn point
                        foreach (ServerPlayer ply in players)
                        {
                            if (ply.Identifier != player.Identifier)
                            {
                                if (MapData.CTTile[spawnPoint].TileRect.Contains((int) ply.Position.X,
                                    (int)ply.Position.Y))
                                {
                                    empty = false;
                                    break;
                                }
                            }
                        }

                        if (empty)
                        {
                            location = new Vector2(MapData.CTTile[spawnPoint].TileRect.X + 16,
                                MapData.CTTile[spawnPoint].TileRect.Y + 16);
                            break;
                        }
                    }
                    break;
                case ServerClientInterface.Team.Terrorist:
                    while (true)
                    {
                        // Get a random index in the T spawn points
                        spawnPoint = rand.Next(0, MapData.TTile.Count);

                        bool empty = true;
                        // Check if nobody is currently on that spawn point
                        foreach (ServerPlayer ply in players)
                        {
                            if (ply.Identifier != player.Identifier)
                            {
                                if (MapData.CTTile[spawnPoint].TileRect.Contains((int)ply.Position.X,
                                    (int)ply.Position.Y))
                                {
                                    empty = false;
                                    break;
                                }
                            }
                        }

                        // If the spawn point is empty, allow the player to spawn there
                        if (empty)
                        {
                            location = new Vector2(MapData.TTile[spawnPoint].TileRect.X + 16,
                                MapData.TTile[spawnPoint].TileRect.Y + 16);
                            break;
                        }
                    }
                    break;
            }

            // Tell everyone the player respawned and their position
            outMsg = server.CreateMessage();
            player.Respawn(location);
            outMsg.Write(ServerClientInterface.RESPAWN_PLAYER);
            outMsg.Write(player.Identifier);
            outMsg.Write(player.Position.X);
            outMsg.Write(player.Position.Y);
            server.SendToAll(outMsg, NetDeliveryMethod.ReliableSequenced);
        }

        /// <summary>
        /// Plants the bomb and sets up the round for retake
        /// </summary>
        /// <param name="playerIdentifier"></param>
        /// <param name="location"></param>
        /// <param name="aSite"></param>
        static void PlantBomb(long playerIdentifier, Vector2 location, bool aSite)
        {
            // TODO: Spawns a bomb at a site

            
            Console.WriteLine("Player planted the bomb at " + (aSite? "A Site" : "B Site"));
        }

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

        #region Collision Detection

        /// <summary>
        /// Checks the collision for a player and their direction
        /// </summary>
        /// <param name="player"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        static bool CheckPlayerCollision(ServerPlayer player, byte direction)
        {
            // If the player isn't currently a spectator
            if (player.CurrentTeam != ServerClientInterface.Team.Spectator)
            {
                // If collision between player is enabled
                if (enableCollision)
                {
                    // Get the direection and adjust the movement speed
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

                    // Circle to Circle collision with the player wishing to move
                    // and every other player on the map. Returning false
                    // prevents the movement byte being sent to the player
                    foreach (ServerPlayer ply in players)
                    {
                        // If team collision is enabled, don't check players who are on the same team
                        if (!teamCollision && ply.CurrentTeam == player.CurrentTeam)
                        {
                            continue;
                        }

                        if (ply.Identifier != player.Identifier &&
                            ply.State != ServerClientInterface.PlayerState.Dead)
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
        }

        /// <summary>
        /// Does collision detection between the shooter and the player
        /// </summary>
        /// <param name="shooter"></param>
        static void BulletToPlayer(ServerPlayer shooter)
        {
            // Get the distance between the player and the shooter
            foreach (ServerPlayer player in players)
            {
                // The shooter can't shoot themself, obviously.
                if (player != shooter)
                {
                    // If friendly fire is disabled and the player is
                    // on the same team, don't do collision detection
                    if (!friendlyFire && player.CurrentTeam == shooter.CurrentTeam)
                    {
                        continue;
                    }

                    // Get distance between the player and the enemy
                    Vector2 delta = player.Position - shooter.Position;

                    // Get -2Pi - 2Pi version of the shooter's angle
                    float angle = shooter.Rotation < 0 ? (float)(shooter.Rotation + (2 * Math.PI)) : shooter.Rotation;

                    // Get angle between shooter and player
                    float shooterToPlayerAngle = (float)Math.Atan2(delta.Y, delta.X);

                    // If the angle between the shooter and player is less than 0 radians
                    // add 2 Pi to convert it from -Pi - Pi domain to -2Pi - 2Pi domain
                    shooterToPlayerAngle = shooterToPlayerAngle < 0
                        ? (float)(shooterToPlayerAngle + (2 * Math.PI))
                        : shooterToPlayerAngle;

                    // If the angle of the shooter is within 0.2 radians of the
                    // angle between the shooter and the player, it means they are
                    // not aiming in the opposite direction of the player which would
                    // result in the collision detection returning true
                    if (angle > shooterToPlayerAngle - 0.2f &&
                        angle < shooterToPlayerAngle + 0.2f)
                    {
                        // Get the direction of the shooter
                        Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                        // Get distance between the player and any possible obstacles in between the
                        // player and the enemy
                        RayCastResult result = raycaster.RayCastMethod(shooter.Position, direction, 1280,
                            MapData.TileMap, MapData.MapArea, angle);

                        // Get the delta between the collision point and the shooter
                        Vector2 raycastDistance = result.CollisionPos - shooter.Position;

                        // If the raycast had collided with an object in between two players
                        // the distance of the raycast would be shorter, therefore, the player
                        // has no direct line of sight with the other player
                        if (raycastDistance.Length() > delta.Length())
                        {
                            // If the shot passes through the player and they are alive
                            if (Collision.NonAACollision(shooter.Position,
                                shooter.Rotation, new Rectangle(
                                    (int)player.Position.X - 16, (int)player.Position.Y + 16,
                                    32, 32), player.Rotation) && player.State == ServerClientInterface.PlayerState.Alive)
                            {

                                // Deal the correct amount of damage depending on the weapon
                                switch (shooter.CurrentWeapon.Weapon)
                                {
                                    case WeaponData.Weapon.Knife:
                                        break;
                                    case WeaponData.Weapon.Ak47:
                                        player.Damage(12, 0);
                                        break;
                                    case WeaponData.Weapon.Glock:
                                        break;
                                    case WeaponData.Weapon.Awp:
                                        break;
                                    case WeaponData.Weapon.Usp:
                                        break;
                                    case WeaponData.Weapon.M4A1:
                                        player.Damage(12, 0);
                                        break;
                                }

                                Console.WriteLine("\"" + shooter.UserName + "\" shot \"" + player.UserName + " with " +
                                                  shooter.CurrentWeapon.Weapon);

                                // If the player's health is less than zero, they died. Let everyone know.
                                if (player.Health <= 0)
                                {
                                    player.SetHealth(0);
                                    player.SetArmor(0);
                                    player.SetState(ServerClientInterface.PlayerState.Dead);
                                    Console.WriteLine(shooter.UserName + " killed " + player.UserName +
                                                      " with " + shooter.CurrentWeapon.Weapon);
                                }

                                // Send data to all players
                                outMsg = server.CreateMessage();
                                outMsg.Write(ServerClientInterface.DAMAGE);
                                outMsg.Write(player.Identifier);
                                outMsg.Write(player.Health);
                                outMsg.Write(player.Armor);
                                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets a set of tiles relative to the player's position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static CStrike2D.Tile[] GetTiles(Vector2 position, byte direction)
        {
            List<CStrike2D.Tile> tiles = new List<CStrike2D.Tile>();

            // The point refers to the column, row of the player, which will be used to
            // offset in order to determine adjacent tiles
            Point location = new Point((int)(position.X) / ServerMap.TILE_SIZE, (int)(position.Y) / ServerMap.TILE_SIZE);

            // Gets the points that need to be checked
            switch (direction)
            {
                case ServerClientInterface.MOVE_UP:
                    tiles.Add(MapData.TileMap[location.X, location.Y - 1]);     // UP
                    break;
                case ServerClientInterface.MOVE_DOWN:
                    tiles.Add(MapData.TileMap[location.X, location.Y + 1]);     // DOWN
                    break;
                case ServerClientInterface.MOVE_LEFT:
                    tiles.Add(MapData.TileMap[location.X - 1, location.Y]);     // LEFT
                    break;
                case ServerClientInterface.MOVE_RIGHT:
                    tiles.Add(MapData.TileMap[location.X + 1, location.Y]);     // RIGHT
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

            // Return the tiles as an array
            return tiles.ToArray();
        }

        #endregion
    }
}