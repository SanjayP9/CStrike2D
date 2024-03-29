﻿// Author: Mark Voong
// File Name: ServerClientInterface.cs
// Project Name: Global Offensive
// Creation Date: Jan 15th, 2016
// Modified Date: Jan 21st, 2016
// Description: Global reference of common data between the client and server.
//              Sharing this class between both projects ensures both ends follow a
//              common protocol
using System;
using Microsoft.Xna.Framework;

namespace CStrike2DServer
{
    public static class ServerClientInterface
    {
        // Colours of each team
        public static readonly Color CT_Colour = new Color(0, 81, 200);

        public static readonly Color T_Colour = new Color(255, 0, 25);

        public static readonly Color S_Colour = new Color(40, 40, 40);

        // Movement speed
        public static float MOVEMENT_SPEED = 2.5f;

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

        public const byte PLAYER_DISCONNECTED = 18;

        // Teams
        public const byte CHANGE_TEAM = 2;
        public const byte TEAM_COUNTER_TERRORIST = 3;
        public const byte TEAM_TERRORIST = 4;
        public const byte TEAM_SPECTATOR = 5;

        // Start/End round
        public const byte BEGIN_ROUND = 6;
        public const byte END_ROUND = 7;
                
        // Who won the round
        public const byte CT_WIN = 26;
        public const byte T_WIN = 27;
        public const byte DRAW = 28;

        // Movement
        public const byte MOVE_UP = 10;
        public const byte MOVE_DOWN = 11;
        public const byte MOVE_LEFT = 12;
        public const byte MOVE_RIGHT = 13;
        public const byte MOVE_UPRIGHT = 14;
        public const byte MOVE_DOWNRIGHT = 15;
        public const byte MOVE_DOWNLEFT = 16;
        public const byte MOVE_UPLEFT = 17;
        public const byte ROTATE_PLAYER = 25;
        public const byte SYNC_MOVEMENT = 29;

        // Sync 
        public const byte REQUEST_SYNC = 19;
        public const byte SYNC_BEGIN = 20;
        public const byte SYNC_CHUNK = 21;
        public const byte SYNC_COMPLETE = 22;
        public const byte SYNC_NEW_PLAYER = 23;

        // Denied team change
        public const byte CHANGE_TEAM_DENIED = 24;

        // Player spawning
        public const byte SPAWN_PLAYER = 30;
        public const byte RESPAWN_PLAYER = 31;

        // Player buying
        public const byte BUY_WEAPON = 32;

        // Player state
        public const byte ALIVE = 33;
        public const byte DEAD = 34;

        // In-Gmae logic
        public const byte FIRE_WEAPON = 35;
        public const byte EXPLODE_FLASHBANG = 36;
        public const byte DAMAGE = 37;
        public const byte REQUEST_RESPAWN = 38;
        public const byte EXPLODE_GRENADE = 39;
        public const byte PLANT_BOMB = 40;

        /// <summary>
        /// Possible teams a player could be on
        /// </summary>
        public enum Team
        {
            CounterTerrorist,
            Terrorist,
            Spectator
        }

        /// <summary>
        /// Possible states a player could be in
        /// </summary>
        public enum PlayerState
        {
            Alive,
            Dead
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

        /// <summary>
        /// Returns the state of a player in the form of a byte
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static byte StateToByte(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Alive:
                    return ALIVE;
                case PlayerState.Dead:
                    return DEAD;
                default:
                    throw new Exception("Somehow this happened");
            }
        }

        /// <summary>
        /// Returns the state of a player in the form of an enum
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static PlayerState ByteToState(byte state)
        {
            switch (state)
            {
                case ALIVE:
                    return PlayerState.Alive;
                case DEAD:
                    return PlayerState.Dead;
                default:
                    throw new Exception("Somehow this happened");
            }
        }
    }
}