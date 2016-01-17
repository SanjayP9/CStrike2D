using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStrike2DServer
{
    public static class ServerClientInterface
    {
        /// <summary>
        /// Request sent by the server for the client to 
        /// exchange primary information such as their username
        /// </summary>
        public const byte HANDSHAKE = 0;

        /// <summary>
        /// Confirmation sent by the server when it is done
        /// setting up the client and any additional data that is
        /// required by the client before they interact with the server
        /// </summary>
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

        public const byte PLAYER_DISCONNECTED = 18;

        /// <summary>
        /// Used after HANDSHAKE is recieved by the server by the client
        /// to request the server to send all data including players and
        /// entities
        /// </summary>
        public const byte REQUEST_SYNC = 19;

        public const byte SYNC_BEGIN = 20;

        public const byte SYNC_COMPLETE = 22;

        public const byte SYNC_NEW_PLAYER = 23;

        public const byte CHANGE_TEAM_DENIED = 24;

        public const byte ROTATE_PLAYER = 25;

        public enum Team
        {
            CounterTerrorist,
            Terrorist,
            Spectator
        }
        
        /// <summary>
        /// Returns the byte equivalent of a team for use in sending
        /// information to/from the server
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public static byte TeamToByte(Team team)
        {
            switch (team)
            {
                case Team.CounterTerrorist:
                    return TEAM_COUNTER_TERRORIST;
                case Team.Terrorist:
                    return TEAM_TERRORIST;
                case Team.Spectator:
                    return TEAM_SPECTATOR;
                default:
                    throw new Exception("Somehow this happened.");
            }
        }

        /// <summary>
        /// Returns the Team Enum equivalent of a team for use in converting
        /// a message recieved from/to the server
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public static Team ByteToTeam(byte team)
        {
            switch (team)
            {
                case TEAM_COUNTER_TERRORIST:
                    return Team.CounterTerrorist;
                case TEAM_TERRORIST:
                    return Team.Terrorist;
                case TEAM_SPECTATOR:
                    return Team.Spectator;
                default:
                    throw new Exception("Somehow this happened");
            }
        }
    }
}