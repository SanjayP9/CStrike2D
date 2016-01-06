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

                                switch (message)
                                {
                                    case NetInterface.SYNC_NEW_PLAYER:
                                        string name = msg.ReadString();
                                        long identifier = msg.SenderConnection.RemoteUniqueIdentifier;
                                        int playerID = msg.ReadInt32();
                                        if (!engine.Exists(playerID))
                                        {
                                            engine.AddPlayer(new Player(name, Vector2.Zero, playerID));
                                        }
                                        break;
                                    case NetInterface.PLAYER_MOVE:
                                        playerNum = msg.ReadByte();
                                        engine.MovePlayer(playerNum, msg.ReadByte());
                                        break;
                                    case NetInterface.PLAY_SOUND:
                                        playerNum = msg.ReadByte();
                                        engine.PlaySound(playerNum, msg.ReadInt16());
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
            client.SendMessage(outMsg, NetDeliveryMethod.Unreliable);
        }
    }
}