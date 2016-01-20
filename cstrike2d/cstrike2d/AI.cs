// Author: Sanjay Paraboo
// File Name: AI.cs
// Project Name: Global Offensive ISU
// Creation Date: Dec 20th, 2015
// Modified Date: Jan 18th, 2015
// Description: This class has all of the update and draw logic for an AI player.
//              An Ai has the same functinality as a player except they are controlled by the server and they
//              Get to objectives by using the A star pathfinding algorithm

using CStrike2DServer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStrike2D
{
    class AI
    {
        /// <summary>
        /// Used to specify the current state of the AI
        /// </summary>
        public enum AIStates
        {
            Defuse,
            Plant,
            KillEnemy,
            Hide,
            DefendSite,
            Buy,
            Idle
        }

        // Stores an isntance of the AIStates enum and is used to specify the current state
        private AIStates currentState = AIStates.Idle;

        // Used to store the current team that teh bot is on
        private ServerClientInterface.Team currentTeam = ServerClientInterface.Team.Spectator;

        // Stores an instance of player in oredr to use player functionality
        private ClientPlayer player;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="team"></param>
        public AI( ServerClientInterface.Team team)
        {
            this.currentTeam = team;
        }

        public void Update()
        {
            switch (currentState)
            {
                case AIStates.Defuse:

                    if (currentTeam == NetInterface.Team.CT)
                    {
                        // Get path to active bombsite and defuse if bomb is not being defused
                    }

                    break;

                case AIStates.Plant:
                    //

                    break;

                case AIStates.KillEnemy:
                    break;

                case AIStates.Hide:
                    break;

                case AIStates.DefendSite:
                    break;

                case AIStates.Buy:
                    break;

                case AIStates.Idle:
                    break;
            }
        }

        public void SpawnBot(Vector2 spawnVect)
        {
            // Spawn bot at spawn location
        }

    }
}
