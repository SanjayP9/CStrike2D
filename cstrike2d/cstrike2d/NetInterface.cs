namespace CStrike2D
{
    static class NetInterface
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
        public const byte FIRE = 20;            // Used for requesting player left fire button



        // Serverside Send Messages
        public const byte PLAYER_MOVE = 31;     // Used for notifying the client that a movement
                                                // call was initiated. This will be followed
                                                // by an identifer byte letting the client know
                                                // which player to move, as well as their intended
                                                // direction.

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

        // Bytes 100 to 131 are reserved for identifying player indexes
        // This implies that the maximum number of players in a server is 32.


        public const byte PLAY_SOUND = 32;      // Used for notifying the client that a sound
                                                // was initiated. This will be followed by an
                                                // identifer byte letting the client know
                                                // which player to play the sound from

        // Sounds currently implemented in the game
        public const short AK47_SHOT = 0;
        public const short AK47_SHOT_DISTANT = 1;
        public const short AWP_SHOT = 2;

    }
}
