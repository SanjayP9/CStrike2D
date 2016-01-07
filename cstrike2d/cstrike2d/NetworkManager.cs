using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
                Stopwatch sw = new Stopwatch();
                sw.Start();
                
                sw.Stop();
            }
        }

        public void Update()
        {
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
                                        playerX = msg.ReadInt64();
                                        playerY = msg.ReadInt64();
                                        if (!engine.Exists(playerID))
                                        {
                                            engine.AddPlayer(name, new Vector2(playerX, playerY), playerID);

                                            if (PlayerID == playerID)
                                            {
                                                engine.SetClientPlayer(engine.Players.Find(ply => ply.PlayerID == PlayerID));
                                            }
                                        }
                                        break;
                                    case NetInterface.MOVE_UP:
                                        engine.MovePlayer(msg.ReadInt16(), NetInterface.MOVE_UP);
                                        break;
                                    case NetInterface.MOVE_DOWN:
                                        engine.MovePlayer(msg.ReadInt16(), NetInterface.MOVE_DOWN);
                                        break;
                                    case NetInterface.MOVE_LEFT:
                                        engine.MovePlayer(msg.ReadInt16(), NetInterface.MOVE_LEFT);
                                        break;
                                    case NetInterface.MOVE_RIGHT:
                                        engine.MovePlayer(msg.ReadInt16(), NetInterface.MOVE_RIGHT);
                                        break;
                                    case NetInterface.PLAY_SOUND:
                                        playerNum = msg.ReadByte();
                                        engine.PlaySound(playerNum, msg.ReadInt16());
                                        break;
                                    case NetInterface.SYNC_MOVEMENT:
                                        
                                        playerID = msg.ReadInt16();
                                        playerX = msg.ReadInt64();
                                        playerY = msg.ReadInt64();

                                        if (engine.Players.Count > 0)
                                        {
                                            engine.Players.Find(ply => ply.PlayerID == playerID)
                                                .SetPosition(new Vector2(playerX, playerY));
                                        }
                                        break;
                                    case NetInterface.ROTATE:
                                        playerID = msg.ReadInt16();
                                        engine.Players.Find(ply => ply.PlayerID == playerID).SetRot(msg.ReadFloat());
                                        break;
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
            client.SendMessage(outMsg, NetDeliveryMethod.UnreliableSequenced);
        }

        public void SendRotData(float rotation)
        {
            NetOutgoingMessage outMsg = client.CreateMessage();
            outMsg.Write(NetInterface.ROTATE);
            outMsg.Write((long)rotation);
            client.SendMessage(outMsg, NetDeliveryMethod.UnreliableSequenced);
        }

        public void ShutDown()
        {
            client.Shutdown("bye");
        }
    }
}