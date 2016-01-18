using System;
using System.Diagnostics;
using System.Threading;
using CStrike2DServer;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace CStrike2D
{
    public class NetworkManager
    {
        private string address;
        private string clientVersion = "0.0.1a";
        private int port = 27014;
        public string ClientName { get; private set; }
        private NetPeerConfiguration config;
        private NetClient client;
        private NetBuffer buffer;
        private NetIncomingMessage msg;
        private NetOutgoingMessage outMsg;
        private int counter;
        private int byteCount;

        GameEngine engine;

        public NetState CurState { get; private set; }

        public short PlayerID { get; private set; }

        // Player
        public short UniqueIdentifier { get; private set; }

        public enum NetState
        {
            Disconnected,
            Handshake,
            Connected
        }

        public NetworkManager(GameEngine engine)
        {
            config = new NetPeerConfiguration("cstrike");
            client = new NetClient(config);
            CurState = NetState.Disconnected;
            buffer = new NetBuffer();
            ClientName = "DevHalo";
            this.engine = engine;
        }


        public void Connect(string address)
        {
            if (CurState != NetState.Connected)
            {
                client.Start();
                client.Connect(address, 27015);
                CurState = NetState.Handshake;
            }
        }

        public void Update(float gameTime)
        {
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
                                switch (code)
                                {
                                    case ServerClientInterface.HANDSHAKE_COMPLETE:
                                        // Sync the client's own player instance
                                        UniqueIdentifier = msg.ReadInt16();
                                        engine.SyncClient(ClientName, UniqueIdentifier);
                                        break;
                                    case ServerClientInterface.SYNC_NEW_PLAYER:
                                        engine.SyncNewPlayer(msg.ReadString(), msg.ReadInt16());
                                        break;
                                    case ServerClientInterface.SYNC_BEGIN:
                                        // Get data of all currently connected players
                                        while ((msg = client.ReadMessage()) != null)
                                        {
                                            if (msg.ReadByte() == ServerClientInterface.SYNC_COMPLETE)
                                            {
                                                break;
                                            }

                                            engine.SyncPlayer(msg.ReadInt16(), msg.ReadString(),
                                                msg.ReadByte(), msg.ReadFloat(), msg.ReadFloat(),
                                                msg.ReadFloat(), msg.ReadByte());
                                        }
                                        // The client has all data of the server, enter the game
                                        engine.CurState = GameEngine.GameEngineState.Active;
                                        break;
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
                                    case ServerClientInterface.CHANGE_TEAM:
                                        engine.ChangeTeam(msg.ReadInt16(), msg.ReadByte());
                                        break;
                                    case ServerClientInterface.SYNC_MOVEMENT:
                                        player = engine.Players.Find(ply => ply.Identifier == msg.ReadInt16());
                                        player.SetPosition(new Vector2(msg.ReadFloat(), msg.ReadFloat()));
                                        player.SetRotation(msg.ReadFloat());
                                        break;
                                    case ServerClientInterface.RESPAWN_PLAYER:
                                        player = engine.Players.Find(ply => ply.Identifier == msg.ReadInt16());
                                        player.Respawn(new Vector2(msg.ReadFloat(), msg.ReadFloat()));
                                        break;
                                    case ServerClientInterface.PLAYER_DISCONNECTED:
                                        short id = msg.ReadInt16();
                                        engine.Players.Remove(engine.Players.Find(ply => ply.Identifier == id));
                                        break;
                                    case ServerClientInterface.ROTATE_PLAYER:
                                        engine.Players.Find(ply => ply.Identifier == msg.ReadInt16())
                                            .SetRotation(msg.ReadFloat());
                                        break;
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
        /*
        public void Update()
        {
            counter++;

            if (counter == 60)
            {
                counter = 0;
                Debug.WriteLine("Kb/s: " + ((double)byteCount / 1024d));
                byteCount = 0;
            }
            while ((msg = client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        
                        switch (CurState)
                        {
                            case NetState.Disconnected:
                                break;
                            case NetState.Handshake:
                                Thread.Sleep(250);
                                byte acknowledge = msg.ReadByte();
                                if (acknowledge == NetInterface.HANDSHAKE)
                                {
                                    PlayerID = msg.ReadInt16();
                                    NetOutgoingMessage outgoing = client.CreateMessage();
                                    outgoing.Write(NetInterface.HANDSHAKE);
                                    outgoing.Write(ClientName);
                                    client.SendMessage(outgoing, NetDeliveryMethod.ReliableOrdered);
                                    CurState = NetState.Connected;
                                }
                                break;
                            case NetState.Connected:
                                if (engine.CurState == GameEngine.GameEngineState.Active)
                                {
                                    byte message = msg.ReadByte();
                                    byte playerNum;
                                    short playerID;
                                    float playerX;
                                    float playerY;
                                    switch (message)
                                    {
                                        case NetInterface.SYNC_NEW_PLAYER:
                                            string name = msg.ReadString();
                                            playerID = msg.ReadInt16();
                                            playerX = msg.ReadFloat();
                                            playerY = msg.ReadFloat();
                                            float rotation = msg.ReadFloat();
                                            byte team = msg.ReadByte();
                                            short entID = msg.ReadInt16();
                                            short wepID = msg.ReadInt16();
                                            if (!engine.Exists(playerID))
                                            {
                                                engine.AddPlayer(name, new Vector2(playerX, playerY), playerID, rotation, team, entID, wepID);
                                            }
                                            if (PlayerID == playerID)
                                            {
                                                engine.SetClientPlayer(
                                                    engine.Players.Find(ply => ply.PlayerID == PlayerID));
                                            }
                                            break;
                                        case NetInterface.MOVE_UP:
                                        case NetInterface.MOVE_DOWN:
                                        case NetInterface.MOVE_LEFT:
                                        case NetInterface.MOVE_RIGHT:
                                        case NetInterface.MOVE_UPRIGHT:
                                        case NetInterface.MOVE_DOWNRIGHT:
                                        case NetInterface.MOVE_DOWNLEFT:
                                        case NetInterface.MOVE_UPLEFT:
                                            engine.MovePlayer(msg.ReadInt16(), message);
                                            break;
                                        case NetInterface.PLAY_SOUND:
                                            playerNum = msg.ReadByte();
                                            engine.PlaySound(playerNum, msg.ReadInt16());
                                            break;
                                        case NetInterface.SYNC_MOVEMENT:

                                            playerID = msg.ReadInt16();
                                            playerX = msg.ReadFloat();
                                            playerY = msg.ReadFloat();

                                            if (engine.Players.Count > 0)
                                            {
                                                engine.Players.Find(ply => ply.PlayerID == playerID)
                                                    .SetPosition(new Vector2(playerX, playerY));
                                            }
                                            break;
                                        case NetInterface.ROTATE:
                                            playerID = msg.ReadInt16();

                                            if (engine.Players.Count > 0)
                                            {
                                                engine.Players.Find(ply => ply.PlayerID == playerID)
                                                    .SetRot(msg.ReadFloat());
                                            }
                                            break;
                                        case NetInterface.PLAYER_DC:
                                            playerID = msg.ReadInt16();
                                            if (engine.Players.Count > 0)
                                            {
                                                engine.Players.Remove(
                                                    engine.Players.Find(ply => ply.PlayerID == playerID));
                                            }
                                            break;
                                        case NetInterface.PLY_CHANGE_TEAM:
                                            playerID = msg.ReadInt16();

                                            if (engine.Players.Count > 0)
                                            {
                                                engine.Players.Find(ply => ply.PlayerID == playerID)
                                                    .SetTeam(NetInterface.GetTeam(msg.ReadByte()));
                                            }
                                            break;
                                        case NetInterface.SPAWN_WEAPON:
                                            playerID = msg.ReadInt16();
                                            short entityID = msg.ReadInt16();
                                            short weaponID = msg.ReadInt16();
                                            if (engine.Players.Count > 0)
                                            {
                                                Player player = engine.Players.Find(ply => ply.PlayerID == playerID);

                                                if (player.PlayerID == PlayerID)
                                                {
                                                    switch (WeaponInfo.GetWeaponType(WeaponInfo.GetWeapon(weaponID)))
                                                    {
                                                        case WeaponInfo.WeaponType.Primary:
                                                            player.SetPrimaryWeapon(entityID, weaponID);
                                                            break;
                                                        case WeaponInfo.WeaponType.Secondary:
                                                            player.SetSecondaryWeapon(entityID, weaponID);
                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                                    player.SetWeapon(entityID, weaponID);
                                                }
                                            }
                                            break;
                                        case NetInterface.SWITCH_WEAPON:
                                            playerID = msg.ReadInt16();
                                            weaponID = msg.ReadInt16();
                                            entityID = msg.ReadInt16();
                                            if (engine.Players.Count > 0)
                                            {
                                                Player player = engine.Players.Find(ply => ply.PlayerID == playerID);

                                                if (player.PlayerID == PlayerID)
                                                {
                                                    switch (WeaponInfo.GetWeaponType(WeaponInfo.GetWeapon(weaponID)))
                                                    {
                                                        case WeaponInfo.WeaponType.Primary:
                                                            player.SetPrimaryWeapon(entityID, weaponID);
                                                            break;
                                                        case WeaponInfo.WeaponType.Secondary:
                                                            player.SetSecondaryWeapon(entityID, weaponID);
                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                                    player.SetWeapon(entityID, weaponID);
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
            
            client.Recycle(msg);
        }
        */

        [Obsolete("Old Netcode")]
        public void SendInputData(byte code)
        {
            NetOutgoingMessage outMsg = client.CreateMessage();
            outMsg.Write(code);
            byteCount += outMsg.LengthBytes;
            client.SendMessage(outMsg, NetDeliveryMethod.UnreliableSequenced);
        }

        
        [Obsolete("Old Netcode")]
        public void SendRotData(float rotation)
        {
            NetOutgoingMessage outMsg = client.CreateMessage();
            outMsg.Write(NetInterface.ROTATE);
            outMsg.Write(rotation);
            byteCount += outMsg.LengthBytes;
            client.SendMessage(outMsg, NetDeliveryMethod.UnreliableSequenced);
        }


        [Obsolete("Old Netcode")]
        public void SendChangeTeam(byte team)
        {
            NetOutgoingMessage outMsg = client.CreateMessage();
            outMsg.Write(NetInterface.PLY_CHANGE_TEAM);
            outMsg.Write(team);
            byteCount += outMsg.LengthBytes;
            client.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered);
        }

        [Obsolete("Old Netcode")]
        public void SwitchWeapon(short entityID, byte weaponSwitch)
        {
            NetOutgoingMessage outMsg = client.CreateMessage();
            outMsg.Write(NetInterface.SWITCH_WEAPON);
            outMsg.Write(weaponSwitch);
            outMsg.Write(entityID);
            byteCount += outMsg.LengthBytes;
            client.SendMessage(outMsg, NetDeliveryMethod.UnreliableSequenced);
        }


        [Obsolete("Old Netcode")]
        public void RequestBuy(short weapon)
        {
            NetOutgoingMessage outMsg = client.CreateMessage();
            outMsg.Write(NetInterface.SPAWN_WEAPON);
            outMsg.Write(weapon);
            byteCount += outMsg.LengthBytes;
            client.SendMessage(outMsg, NetDeliveryMethod.UnreliableSequenced);
        }

        public void ShutDown()
        {
            client.Shutdown("bye");
        }
    }
}