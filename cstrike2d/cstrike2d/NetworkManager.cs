using System.Diagnostics;
using System.Threading;
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
        private int counter;
        private int byteCount;

        GameEngine engine;

        public NetState CurState { get; private set; }

        public short PlayerID { get; private set; }

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
                                Thread.Sleep(100);
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
                                            if (!engine.Exists(playerID))
                                            {
                                                engine.AddPlayer(name, new Vector2(playerX, playerY), playerID, rotation, team);



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
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
            
            client.Recycle(msg);
        }

        public void SendInputData(byte code)
        {
            NetOutgoingMessage outMsg = client.CreateMessage();
            outMsg.Write(code);
            byteCount += outMsg.LengthBytes;
            client.SendMessage(outMsg, NetDeliveryMethod.UnreliableSequenced);
        }

        public void SendRotData(float rotation)
        {
            NetOutgoingMessage outMsg = client.CreateMessage();
            outMsg.Write(NetInterface.ROTATE);
            outMsg.Write(rotation);
            byteCount += outMsg.LengthBytes;
            client.SendMessage(outMsg, NetDeliveryMethod.UnreliableSequenced);
        }

        public void SendChangeTeam(byte team)
        {
            NetOutgoingMessage outMsg = client.CreateMessage();
            outMsg.Write(NetInterface.PLY_CHANGE_TEAM);
            outMsg.Write(team);
            byteCount += outMsg.LengthBytes;
            client.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered);
        }

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