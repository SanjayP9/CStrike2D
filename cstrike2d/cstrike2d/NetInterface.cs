using System;
using Microsoft.Xna.Framework;

namespace CStrike2D
{
    [Obsolete("NetCode Rewritten")]
    public static class NetInterface
    {
        // Server Update Rate / Tick Rate Summary
        /// The server updates the game logic at a certain rate, Tick Rate.
        /// The default server Tick Rate is 64 tick. This is to ensure smooth performance
        /// on most desktop computers. If the server has capable hardware, the tick rate can be
        /// increased to a maximum of 128 ticks per second. This increases accuracy of physics,
        /// hit detection, and responsiveness by the server. However, this effectively doubles
        /// CPU requirements, thus a more capable desktop is required to run at this server
        /// configuration.
        /// The server sends data to each player connected at a certain rate, Update Rate.
        /// The default server Update Rate is 32 tick. This is to ensure the network does not
        /// get too saturated as the server uses UDP unsequenced packets to send data quickly
        /// to clients. If the network is capable of handling higher bandwidth, the Update Rate
        /// can be inceased to a maximum of 64 ticks per second. This can act positively in
        /// addition to an increased Tick Rate, as more logic updates are conducted per second,
        /// and more states are sent out in a second. 

        public static readonly Color CT_Color = new Color(0, 81, 200);

        public static readonly Color T_Color = new Color(255, 0, 25);

        public static readonly Color S_Color = new Color(40, 40, 40);

        // Byte Headers for Message Type

        // Clientside Send Messages
        public const byte HANDSHAKE = 0;        // Used for username exchange and sync 
                                                // when connecting for the first time

        public const byte PING = 1;             // Used to test ping

        public const byte MOVE_UP = 10;         // Used for requesting player movement Up
        public const byte MOVE_DOWN = 11;       // Used for requesting player movement Down
        public const byte MOVE_LEFT = 12;       // Used for requesting player movement Left
        public const byte MOVE_RIGHT = 13;      // Used for requesting player movement Right
        public const byte ROTATE = 14;
        public const byte MOVE_UPRIGHT = 16;
        public const byte MOVE_DOWNRIGHT = 17;
        public const byte MOVE_DOWNLEFT = 18;
        public const byte MOVE_UPLEFT = 19;
        public const byte FIRE = 20;            // Used for requesting player left fire button

        // Serverside Send Messages

        public const byte SYNC_MOVEMENT = 21;   // Used for notifying the client that a movement
                                                // synchronization call was initiated. This will
                                                // be followed by the player ID, and their position
                                                // stored in two shorts, X and Y.
                                                // This would be a network intensive task, and by design
                                                // the call should only be initated every couple ticks
                                                // to prevent network saturation but ensure accurate movement
                                                // between characters. In a 32 update rate server, this should be called
                                                // somewhere between 4-16 ticks. Any lower would increase accuracy, but
                                                // heavily increase network activity.

        public const byte SYNC_NEW_PLAYER = 22; // Used for notifying the client that a new player
                                                // has joined the server and is about to send information
                                                // such as their name

        public const byte PLAYER_DC = 23;       // Used for notifying the client that a player has disconnected
                                                // from the server. The server also sends their playerID so that
                                                // it can be deleted from the client's list

        public const byte PLY_CHANGE_TEAM = 24;
        public const byte PLY_CT = 25;
        public const byte PLY_T = 26;
        public const byte PLY_SP = 27;
        
        public const byte SPAWN_WEAPON = 28;    // Used for notifying the client that a weapon was bought
                                                // and must be spawned into the game. This will be followed by
                                                // an identifier byte letting the client know what weapon was
                                                // spawned.


        public const byte PLAYER_MOVE = 31;     // Used for notifying the client that a movement
                                                // call was initiated. This will be followed
                                                // by an identifer byte letting the client know
                                                // which player to move, as well as their intended
                                                // direction.

            

        // Bytes 100 to 131 are reserved for identifying player indexes
        // This implies that the maximum number of players in a server is 32.


        public const byte PLAY_SOUND = 32;      // Used for notifying the client that a sound
                                                // was initiated. This will be followed by an
                                                // identifer byte letting the client know
                                                // which player to play the sound from
        public const byte SWITCH_WEAPON = 33;
        public const byte SWITCH_PRIMARY = 34;
        public const byte SWITCH_SECONDARY = 35;
        public const byte SWITCH_FRAG = 36;
        public const byte SWITCH_FLASHBANG = 37;
        public const byte SWITCH_SMOKE = 38;
        public const byte SWITCH_INCEND = 39;
        public const byte SWITCH_BOMB = 40;
        public const byte SWITCH_KNIFE = 41;

        // Sounds currently implemented in the game
        public const short AK47_SHOT = 0;
        public const short AK47_SHOT_DISTANT = 1;
        public const short AWP_SHOT = 2;

        // Weapons
        public const short WEAPON_AK47 = 100;
        public const short WEAPON_AWP = 101;
        public const short WEAPON_KNIFE = 102;
        public const short WEAPON_M4A1_SILENCER = -1;
        public const short WEAPON_GLOCK = -1;
        public const short WEAPON_USP_SILENCER = -1;
        public const short WEAPON_BOMB = -1;

        /// <summary>
        /// Possible teams a player could be in
        /// </summary>
        public enum Team
        {
            CT,
            T,
            Spectator
        }

        /// <summary>
        /// Returns the enum value of a team given the
        /// network byte identifier
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public static Team GetTeam(byte team)
        {
            switch (team)
            {
                case PLY_CT:
                    return Team.CT;
                case PLY_T:
                    return Team.T;
                case PLY_SP:
                    return Team.Spectator;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns the network byte identifier for a team
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public static byte GetTeamByte(Team team)
        {
            switch (team)
            {
                case Team.CT:
                    return PLY_CT;
                case Team.T:
                    return PLY_T;
                case Team.Spectator:
                    return PLY_SP;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}