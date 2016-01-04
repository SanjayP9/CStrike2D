using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace CStrike2D
{
    public class NetworkManager
    {
        private string address;
        private string clientVersion = "0.0.1a";
        private int port = 27015;
        private string clientName = "DevHalo";
        private NetPeerConfiguration config;
        private NetClient client;
        private NetBuffer buffer;

        private NetState curState;

        public enum NetState
        {
            Disconnected,
            Handshake,
            Connected
        }

        public NetworkManager()
        {
            config = new NetPeerConfiguration("cstrike");
            config.Port = port;
            client = new NetClient(config);
            curState = NetState.Disconnected;
            buffer = new NetBuffer();
        }


        public void Connect(string address)
        {
            if (curState != NetState.Connected)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                client.Connect(address, port);

                if (sw.Elapsed.Seconds < 5)
                {
                    if (client.ConnectionStatus == NetConnectionStatus.Connected)
                    {
                        NetOutgoingMessage msg = client.CreateMessage();
                        msg.Write("HNDSHAKE" + clientName);
                        client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                        curState = NetState.Connected;
                        sw.Stop();
                    }
                }
            }
        }

        public void Update()
        {
            if (curState != NetState.Disconnected)
            {
                NetIncomingMessage msg = client.ReadMessage();
                switch (curState)
                {
                    case NetState.Handshake:
                        if (msg != null)
                        {
                            if (msg.MessageType == NetIncomingMessageType.Data)
                            {
                                if (msg.ReadString() == "accepted")
                                {
                                    curState = NetState.Connected;
                                }
                            }
                        }
                        break;
                    case NetState.Connected:
                        if (msg.MessageType != null)
                        {

                        }
                        break;
                }
            }
        }


    }
}
