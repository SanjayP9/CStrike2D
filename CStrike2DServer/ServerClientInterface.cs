using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStrike2DServer
{
    public static class ServerClientInterface
    {
        public const byte HANDSHAKE = 0;
        public const byte HANDSHAKE_COMPLETE = 1;

        public const byte CHANGE_TEAM = 2;
        public const byte TEAM_COUNTER_TERRORIST = 3;
        public const byte TEAM_TERRORIST = 4;
        public const byte TEAM_SPECTATOR = 5;

        public const byte BEGIN_ROUND = 6;
        public const byte END_ROUND = 7;

        public const byte MOVE_UP = 10;
        public const byte MOVE_DOWN = 11;
        public const byte MOVE_LEFT = 12;
        public const byte MOVE_RIGHT = 13;
        public const byte MOVE_UPRIGHT = 14;
        public const byte MOVE_DOWNRIGHT = 15;
        public const byte MOVE_DOWNLEFT = 16;
        public const byte MOVE_UPLEFT = 17;
    }
}