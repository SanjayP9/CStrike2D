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

        // current AI position on map
        Point currentPos;

        /// <summary>
        /// Used to specify the current state of the AI
        /// </summary>
        public enum AIStates
        {
            Defuse,
            Plant,
            KillEnemy,
            Hide,
            Idle
        }

        // Stores an isntance of the AIStates enum and is used to specify the current state
        private AIStates currentState = AIStates.Idle;

        // Used to store the current team that teh bot is on
        private ServerClientInterface.Team currentTeam = ServerClientInterface.Team.Spectator;

        // Stores an instance of player in order to use player functionality
        private ClientPlayer player;

        // Records elasped time
        private float timeElasped = 0f;

        // List of nodes for paths
        List<Point> path = new List<Point>();
        int index = 0;


        /// <summary>
        /// Initializes AI and passes through team
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

        /// <summary>
        /// Runs update logic for the AI Bots
        /// </summary>
        public void Update(float gameTime, Point destinationLocal)
        {
            switch (currentState)
            {
                case AIStates.Defuse:

                    if (currentTeam == ServerClientInterface.Team.CounterTerrorist)
                    {
                        // Get path to active bombsite and defuse if bomb is not being defused
                        path = PathFinder(currentPos, destinationLocal);

                        GotoNextTile(path[index]);
                        index++;
                        {
                            if (index == path.Count - 1)
                            {
                                index = 0;
                            }
                        }

                        if (currentPos == destinationLocal)
                        {
                            player.Defuse();
                            currentState = AIStates.Idle;
                        }
                    }

                    break;

                case AIStates.Plant:
                    // Get path to the bombsite and plant if bomb is not being planted
                    if (currentTeam == ServerClientInterface.Team.Terrorist)
                    {
                        path = PathFinder(currentPos, destinationLocal);

                        GotoNextTile(path[index]);
                        index++;
                        {
                            if (index == path.Count - 1)
                            {
                                index = 0;
                            }
                        }

                        if (currentPos == destinationLocal)
                        {
                            player.Plant();
                            currentState = AIStates.Idle;
                        }
                    }
                    

                    break;

                case AIStates.KillEnemy:
                    break;

                case AIStates.Hide:
                    break;


                case AIStates.Idle:
                    break;
            }
        }


        /// <summary>
        /// Makes the bot travel to the destination tile
        /// </summary>
        /// <param name="destinationTile"></param>
        public void GotoNextTile(Point destinationTile)
        {
            if (currentPos.X != destinationTile.X)
            {
                currentPos.X += 1;
            }
            if (currentPos.Y != destinationTile.Y)
            {
                currentPos.Y += 1;
            }
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

                    // If the path exists
                    if (!path.Exists(p => p == checkPoint))
                    {

                        // Checks if the check point tile is on the map
                        if (checkPoint.X > 0 && checkPoint.Y > 0 &&
                            checkPoint.X < map.GetLength(0) &&
                            checkPoint.Y < map.GetLength(1))
                        {

                            // Checks if the next checkpoint tile is solid if not calculates H-Cost
                            if (map[checkPoint.X, checkPoint.Y].TileType != Tile.SOLID)
                            {
                                // Stores h cost in the int h
                                h = FindHCost(checkPoint, endingPoint);
                            }

                            // Sets the H-Cost to the max value of the tile is solid
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

                    // Calculates H-Cost by adding g and h cost
                    tileScores[i] = g + h;
                }

                // Stores direction to get to the next node
                int direction = Array.IndexOf(tileScores, tileScores.Min());

                // Gets the next point from the direction to point method
                Point nextNode = DirectionToPoint(direction);
                nextNode.X += curCol;
                nextNode.Y += curRow;

                // Adds the next node to the path list
                path.Add(nextNode);
                curCol = nextNode.X;
                curRow = nextNode.Y;
            }

            return path;
        }


        /// <summary>
        /// Retrives the next point based on the AI's direction
        /// </summary>
        /// <param name="direction"> Passes through the current direction </param>
        /// <returns> Returns new point based on direction </returns>
        public Point DirectionToPoint(int direction)
        {
            // Generates point based on 8 possible directions
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

                // If none are found an exception is thrown stating that the direction parameter was not valie
                default:
                    throw new Exception("Direction is not a valid direction");
            }
        }

        /// <summary>
        /// Calculates the Heuristic cost and returns it
        /// </summary>
        /// <param name="current"> Passes through current tile location </param>
        /// <param name="end"> Passes through destination tile location </param>
        /// <returns></returns>
        public int FindHCost(Point current, Point end)
        {
            // Calculates H Cost by getting the difference betweem the x and y and multiplying it by the movement cost of 10
            return (((Math.Abs(end.X - current.X)) + (Math.Abs(end.Y - current.Y))) * STRAIGHT_G_COST);
        }

        /// <summary>
        /// Calculates the G Cost of the current tile
        /// </summary>
        /// <param name="parent"> Passes through the parent tile </param>
        /// <param name="current"> Passes through the current tile to check </param>
        /// <returns></returns>
        public int FindGCost(Point parent, Point current)
        {
            // If the current tile is above, below to the left or right of the parent there is a straight g cost
            if (parent.X == current.X || parent.Y == current.Y)
            {
                return STRAIGHT_G_COST;
            }

            // Else it returns the diagonal cost
            return DIAG_G_COST;
        }

        /// <summary>
        /// Calculates the total cost of moving to another tile
        /// </summary>
        /// <param name="current"> Passes through the current tile that the player is on </param>
        /// <param name="end"> Passes through the destination tile </param>
        /// <param name="parent"> Specifies the parent tile of the current tile </param>
        /// <returns></returns>
        public int GetFCost(Point current, Point end, Point parent)
        {
            // Runs GCost and HCost methods and gets the sum of it
            return (FindGCost(parent, current) + FindHCost(current, end));
        }

    }

    class NodeInfo
    {
        // Stores the point location and the f cost integer for a node
        public Point Location { get; set; }
        public int FCost { get; set; }

        /// <summary>
        /// Creates an instance of NodeInfo
        /// </summary>
        /// <param name="location"> Passes through location of the tile </param>
        /// <param name="fCost"> Passes through fcost </param>
        public NodeInfo(Point location, int fCost)
        {
            this.Location = location;
            this.FCost = fCost;
        }
    }
}
