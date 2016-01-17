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

        private AIStates currentState = AIStates.Idle;

        private  NetInterface.Team currentTeam = NetInterface.Team.Spectator;

        private ClientPlayer player;

        public AI( NetInterface.Team team)
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
