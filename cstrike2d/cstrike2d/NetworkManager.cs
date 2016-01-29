// Author: Mark Voong
// Class Name: NetworkManager.cs
// Project Name: Global Offensive
// Creation Date: Dec 31st, 2015
// Modified Date: Jan 21st, 2016
// Description: Handles all network logic with the servers
using CStrike2DServer;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace CStrike2D
{
    public class NetworkManager
    {
        // Address and version
        private string address;
        private string clientVersion = "0.0.1a";

        /// <summary>
        /// The user's name
        /// </summary>
        public string ClientName { get; private set; }

        // Lidgren configuration for client
        private NetPeerConfiguration config;
        private NetClient client;

        // Messaging
        private NetIncomingMessage msg;
        private NetOutgoingMessage outMsg;

        // Data statistics
        private int counter;
        private int byteCount;

        // The game engine instance
        GameEngine engine;

        /// <summary>
        /// The current state of the network manager
        /// </summary>
        public NetState CurState { get; private set; }

        /// <summary>
        /// The player's unique identifier assigned by the server
        /// </summary>
        public short UniqueIdentifier { get; private set; }

        /// <summary>
        /// States the network manager could be in
        /// </summary>
        public enum NetState
        {
            Disconnected,   // Not connected
            Handshake,      // Syncing with a potential server
            Connected       // Connected
        }

        /// <summary>
        /// Initializes the network processor class
        /// </summary>
        /// <param name="engine"></param>
        public NetworkManager(GameEngine engine)
        {
            // Initialize config for network and set local copy
            // of engine instance
            config = new NetPeerConfiguration("cstrike");
            client = new NetClient(config);
            CurState = NetState.Disconnected;
            ClientName = "";
            this.engine = engine;
        }


        /// <summary>
        /// Initiates a connection to a server
        /// </summary>
        /// <param name="address"></param>
        public void Connect(string address, string username)
        {
            if (CurState != NetState.Connected)
            {
                ClientName = username;
                client.Start();
                client.Connect(address, 27015);
                CurState = NetState.Handshake;
            }
        }

        /// <summary>
        /// Update loop for network logic
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(float gameTime)
        {
            // If there is a message to be processed
            while ((msg = client.ReadMessage()) != null)
            {
                byte code;
                ClientPlayer player;
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.Data:

                        switch (CurState)
                        {
                            case NetState.Handshake:
                                // If the client was able to connect successfully
                                if (client.ConnectionStatus == NetConnectionStatus.Connected)
                                {
                                    code = msg.ReadByte();
                                    // If a handshake was initialized by the server
                                    if (code == ServerClientInterface.HANDSHAKE)
                                    {
                                        // Send the server the client name and a 
                                        // request for synchronization
                                        outMsg = client.CreateMessage();

                                        outMsg.Write(ServerClientInterface.REQUEST_SYNC);
                                        outMsg.Write(ClientName);

                                        // Send the message
                                        client.SendMessage(outMsg, NetDeliveryMethod.ReliableSequenced);
                                        CurState = NetState.Connected;
                                    }
                                }
                                break;
                            case NetState.Connected:
                                code = msg.ReadByte();
                                if (engine.CurState == GameEngine.GameEngineState.Loaded)
                                {
                                    if (code == ServerClientInterface.HANDSHAKE_COMPLETE)
                                    {
                                        // Sync the client's own player instance
                                        UniqueIdentifier = msg.ReadInt16();
                                        engine.SyncClient(ClientName, UniqueIdentifier);
                                    }

                                    if (code == ServerClientInterface.SYNC_CHUNK)
                                    {
                                        // Get data of all currently connected players
                                        engine.SyncPlayer(msg.ReadInt16(), msg.ReadString(),
                                            msg.ReadByte(), msg.ReadFloat(), msg.ReadFloat(),
                                            msg.ReadFloat(), msg.ReadByte(), msg.ReadByte());
                                    }

                                    if (code == ServerClientInterface.SYNC_COMPLETE)
                                    {
                                        // The client has all data of the server, enter the game
                                        engine.CurState = GameEngine.GameEngineState.Active;
                                    }
                                }
                                else
                                {
                                    short id;
                                    switch (code)
                                    {
                                            // Synchronizing players given from the server
                                        case ServerClientInterface.SYNC_CHUNK:
                                            engine.SyncPlayer(msg.ReadInt16(), msg.ReadString(),
                                             msg.ReadByte(), msg.ReadFloat(), msg.ReadFloat(),
                                             msg.ReadFloat(), msg.ReadByte(), msg.ReadByte());
                                            break;
                                            // Movement
                                        case ServerClientInterface.MOVE_UP:
                                        case ServerClientInterface.MOVE_DOWN:
                                        case ServerClientInterface.MOVE_LEFT:
                                        case ServerClientInterface.MOVE_RIGHT:
                                        case ServerClientInterface.MOVE_UPLEFT:
                                        case ServerClientInterface.MOVE_UPRIGHT:
                                        case ServerClientInterface.MOVE_DOWNRIGHT:
                                        case ServerClientInterface.MOVE_DOWNLEFT:
                                            engine.MovePlayer(msg.ReadInt16(), code);
                                            break;
                                            // Change teams
                                        case ServerClientInterface.CHANGE_TEAM:
                                            engine.ChangeTeam(msg.ReadInt16(), msg.ReadByte());
                                            break;
                                            // Synchronize movement
                                        case ServerClientInterface.SYNC_MOVEMENT:
                                            id = msg.ReadInt16();
                                            player = engine.Players.Find(ply => ply.Identifier == id);
                                            player.SetPosition(new Vector2(msg.ReadFloat(), msg.ReadFloat()));
                                            player.SetRotation(msg.ReadFloat());
                                            break;
                                            // Respawns a player
                                        case ServerClientInterface.RESPAWN_PLAYER:
                                            id = msg.ReadInt16();
                                            player = engine.Players.Find(ply => ply.Identifier == id);
                                            player.Respawn(new Vector2(msg.ReadFloat(), msg.ReadFloat()));
                                            engine.PlaySound(player, "pickup");
                                            break;
                                            // Removes a player that disconnected
                                        case ServerClientInterface.PLAYER_DISCONNECTED:
                                            id = msg.ReadInt16();
                                            engine.Players.Remove(engine.Players.Find(ply => ply.Identifier == id));
                                            break;
                                            // Rotates a player
                                        case ServerClientInterface.ROTATE_PLAYER:
                                            id = msg.ReadInt16();
                                            engine.Players.Find(ply => ply.Identifier == id)
                                                .SetRotation(msg.ReadFloat());
                                            break;
                                            // Buys a weapon
                                        case ServerClientInterface.BUY_WEAPON:
                                            engine.SetWeapon(msg.ReadInt16(), msg.ReadByte());
                                            break;
                                            // Fires a weapon
                                        case ServerClientInterface.FIRE_WEAPON:
                                            engine.FireWeapon(msg.ReadInt16());
                                            break;
                                            // Explodes a flashbang
                                        case ServerClientInterface.EXPLODE_FLASHBANG:
                                            engine.FlashPlayer();
                                            break;
                                            // Damages a player
                                        case ServerClientInterface.DAMAGE:
                                            engine.Damage(msg.ReadInt16(), msg.ReadInt32(), msg.ReadInt32());
                                            break;
                                            // Spawns a player
                                        case ServerClientInterface.SPAWN_PLAYER:
                                            engine.SpawnPlayer(msg.ReadInt16());
                                            break;
                                        case ServerClientInterface.BEGIN_ROUND:
                                            // Play round begin sound
                                            engine.PlaySoundNonPos(engine.Client, "roundStart");
                                            break;
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Tells the server that the client wishes to change to the specified team
        /// </summary>
        /// <param name="team"></param>
        public void RequestTeamChange(ServerClientInterface.Team team)
        {
            outMsg = client.CreateMessage();
            outMsg.Write(ServerClientInterface.CHANGE_TEAM);
            outMsg.Write(ServerClientInterface.TeamToByte(team));
            client.SendMessage(outMsg, NetDeliveryMethod.ReliableSequenced);
        }

        /// <summary>
        /// Tells the server that the client wishes to rotate their player
        /// </summary>
        /// <param name="rotation"></param>
        public void Rotate(float rotation)
        {
            outMsg = client.CreateMessage();
            outMsg.Write(ServerClientInterface.ROTATE_PLAYER);
            outMsg.Write(rotation);
            client.SendMessage(outMsg, NetDeliveryMethod.UnreliableSequenced);
        }

        /// <summary>
        /// Tells the server that the client wishes to move in a certain direction
        /// </summary>
        /// <param name="direction"></param>
        public void Move(byte direction)
        {
            outMsg = client.CreateMessage();
            outMsg.Write(direction);
            client.SendMessage(outMsg, NetDeliveryMethod.UnreliableSequenced);
        }

        /// <summary>
        /// Tells the server that the client wishes to fire their weapon
        /// </summary>
        public void FireWeapon()
        {
            outMsg = client.CreateMessage();
            outMsg.Write(ServerClientInterface.FIRE_WEAPON);
            client.SendMessage(outMsg, NetDeliveryMethod.UnreliableSequenced);
        }

        /// <summary>
        /// Tells the server that the client wishes to buy a weapon
        /// </summary>
        /// <param name="weapon"></param>
        public void BuyWeapon(WeaponData.Weapon weapon)
        {
            outMsg = client.CreateMessage();
            outMsg.Write(ServerClientInterface.BUY_WEAPON);
            outMsg.Write(WeaponData.WeaponToByte(weapon));
            client.SendMessage(outMsg, NetDeliveryMethod.ReliableSequenced);
        }

        /// <summary>
        /// Asks the server if the player can respawn
        /// </summary>
        public void RequestRespawn()
        {
            outMsg = client.CreateMessage();
            outMsg.Write(ServerClientInterface.REQUEST_RESPAWN);
            client.SendMessage(outMsg, NetDeliveryMethod.UnreliableSequenced);
        }

        /* UNFINISHED

        public void SyncWorld()
        {
            // TODO: Sends a snapshot of the world to all players to ensure
            // TODO: everyone is viewing the same thing
        }

        public void StartRound()
        {
            // TODO: Spawns all players, sets up all timers, etc
        }

        public void EndRound()
        {
            // TODO: Processes kills, calculates money, etc
        }

        public void SpawnWeapon(long playerIdentifier, ClientWeapon weapon)
        {
            // TODO: Gives a weapon to a player
        }

        public void SpawnPlayer(long playerIdentifier, Vector2 location)
        {
            // TODO: Spawns a player onto the map
        }

        public void PlantBomb(long playerIdentifier, Vector2 location, bool aSite)
        {
            // TODO: Spawns a bomb at a site

        }

        */

        /// <summary>
        /// Flashes the player
        /// </summary>
        public void Flash()
        {
            outMsg = client.CreateMessage();
            outMsg.Write(ServerClientInterface.EXPLODE_FLASHBANG);
            client.SendMessage(outMsg, NetDeliveryMethod.UnreliableSequenced);
        }

        /// <summary>
        /// Shuts down the client
        /// </summary>
        public void ShutDown()
        {
            client.Shutdown("bye");
        }
    }
}