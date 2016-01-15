using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace CStrike2D
{
    static class ClientHandler
    {


        public static void Serialize(NetOutgoingMessage msg, NetClient client)
        {
            client.SendMessage(msg, NetDeliveryMethod.UnreliableSequenced);

           
        }


        public static void Deserialize(NetIncomingMessage msg)
        {
            
        }
    }
}
