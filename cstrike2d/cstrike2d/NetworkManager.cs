using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using Lidgren.Network;
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

        // Byte Constants
        public const byte HANDSHAKE = 0;
        public const byte MOVE_UP = 10;
        public const byte MOVE_DOWN = 11;
        public const byte MOVE_LEFT = 12;
        public const byte MOVE_RIGHT = 13;

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
                        string line = msg.ReadString();
                        switch (CurState)
                        {
                            case NetState.Disconnected:
                                break;
                            case NetState.Handshake:
                                if (line.Contains("welcome"))
                                {
                                    NetOutgoingMessage outgoing = client.CreateMessage();
                                    outgoing.Write("HNDSHAKE" + ClientName);
                                    client.SendMessage(outgoing, NetDeliveryMethod.ReliableOrdered);
                                    CurState = NetState.Connected;
                                }
                                break;
                            case NetState.Connected:
                                if (line.Contains("PLYMOVE"))
                                {
                                    int playerNum = line[6];
                                }
                                else if (line.Contains("AK47SHOT"))
                                {
                                    engine.PlaySound();
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
            outMsg.Write(code.ToString());
            client.SendMessage(outMsg, NetDeliveryMethod.Unreliable);
        }
    }
}