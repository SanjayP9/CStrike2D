// Author: Sanjay Paraboo
// File Name: AI.cs
// Project Name: Global Offensive
// Creation Date: Dec 20th, 2015
// Modified Date: Jan 20th, 2016
// Description: This class has all of the update and draw logic for an AI player.
//              An Ai has the same functinality as a player except they are controlled by the server and they
//              Get to objectives by using the A star pathfinding algorithm
using CStrike2DServer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CStrike2D
{
    class AI
    {
        // Constant integers used to record the straight g movement cost
        private const int STRAIGHT_G_COST = 10;
        private const int DIAG_G_COST = 14;

        // Stores the 2d array of tiles on the map
        Tile[,] map;

        // Stores the path for the pathfinding
        List<Point> pointsOnPath = new List<Point>();


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

        // Stores an instance of player in order to use player functionality
        private ClientPlayer player;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="team"></param>
        public AI(ServerClientInterface.Team team)
        {
            this.currentTeam = team;
        }


        /// <summary>
        /// Retrieves the 2D tile map and stores
        /// </summary>
        /// <param name="map"></param>
        public void Initialize(Tile[,] map)
        {
            this.map = map;
        }

        public void Update()
        {
            switch (currentState)
            {
                case AIStates.Defuse:

                    //if (currentTeam == ServerClientInterface.Team.CT)
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

        public void GotoNextTile(Point destinationTile)
        {

        }


        /// <summary>
        /// Main Pathfinding method that uses the A Star Algorithm in order to find the most efficient route to the destination
        /// </summary>
        /// <param name="startingPoint"> Specifies the starting node </param>
        /// <param name="endingPoint"> Passes through the end node </param>
        /// <returns> Return a list of ordered points that the bot will follow </returns>
        public List<Point> PathFinder(Point startingPoint, Point endingPoint)
        {
            // Creates path list and intializes it
            List<Point> path = new List<Point>();

            // Stores the current row and column of the current node
            int curCol = startingPoint.X;
            int curRow = startingPoint.Y;

            // Records the movement cost of the surrounding tiles
            int[] tileScores;

            // While loop that only exits when both the current x and y of the node are equal to the 
            // x and y of the ending node
            while (curCol != endingPoint.X ||
                   curRow != endingPoint.Y)
            {
                // Stores the g costs of each of surrounding tiles
                tileScores = new int[8];

                // Stores g cost and h cost
                int g;
                int h;

                // 0 1 2
                // 3 X 4
                // 5 6 7

                // Cycles through all surrounding tiles
                for (int i = 0; i < tileScores.Length; i++)
                {
                    switch (i)
                    {
                        // If the surroundiung tile is diagonal to the current node
                        // the g cost is 14 and 10 if straight
                        case 0:
                        case 2:
                        case 5:
                        case 7:
                            g = 14;
                            break;
                        default:
                            g = 10;
                            break;
                    }

                    // Runs the DirectionToPoint method in order to store the new point
                    Point checkPoint = DirectionToPoint(i);

                    // Gets the direction and adds it to the current node position in order to 
                    // get the right placement
                    checkPoint.X += curCol;
                    checkPoint.Y += curRow;

                    //
                    if (!path.Exists(p => p == checkPoint))
                    {
                        if (checkPoint.X > 0 && checkPoint.Y > 0 &&
                            checkPoint.X < map.GetLength(0) &&
                            checkPoint.Y < map.GetLength(1))
                        {
                            if (map[checkPoint.X, checkPoint.Y].TileType != Tile.SOLID)
                            {
                                h = FindHCost(checkPoint, endingPoint);
                            }
                            else
                            {
                                h = Int16.MaxValue;
                            }
                        }
                        else
                        {
                            h = Int16.MaxValue;
                        }

                    }
                    else
                    {
                        h = Int16.MaxValue;
                    }

                    tileScores[i] = g + h;
                }

                int direction = Array.IndexOf(tileScores, tileScores.Min());

                Point nextNode = DirectionToPoint(direction);
                nextNode.X += curCol;
                nextNode.Y += curRow;


                path.Add(nextNode);
                curCol = nextNode.X;
                curRow = nextNode.Y;
            }

            return path;
        }


        public Point DirectionToPoint(int direction)
        {
            switch (direction)
            {
                case 0:
                    return new Point(-1, -1);
                case 1:
                    return new Point(0, -1);
                case 2:
                    return new Point(1, -1);
                case 3:
                    return new Point(-1, 0);
                case 4:
                    return new Point(1, 0);
                case 5:
                    return new Point(-1, 1);
                case 6:
                    return new Point(0, 1);
                case 7:
                    return new Point(1, 1);
                default:
                    throw new Exception("Direction is not a valid direction");
            }
        }

        public int FindHCost(Point current, Point end)
        {
            return ((Math.Abs(end.X - current.X)) + (Math.Abs(end.Y - current.Y)));
        }

        public int FindGCost(Point parent, Point current)
        {
            if (parent.X == current.X || parent.Y == current.Y)
            {
                return STRAIGHT_G_COST;
            }

            return DIAG_G_COST;
        }

        public int GetFCost(Point current, Point end, Point parent)
        {
            return (FindGCost(parent, current) + FindHCost(current, end));
        }

    }

    class NodeInfo
    {
        public Point Location { get; set; }
        public int FCost { get; set; }

        public NodeInfo(Point location, int fCost)
        {
            this.Location = location;
            this.FCost = fCost;
        }
    }
}
