using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CStrike2D
{
    class TileManager
    {
        public const int TILE_X = 18;
        public const int TILE_Y = 10;
        public const int TILE_SIDE_LENGTH = 85;

        public Tile[,] Tiles { get; private set; }


        public TileManager(GraphicsDevice graphics, Random randNum)
        {
            Tiles = new Tile[TILE_X, TILE_Y];


            for (int x = 0; x < TILE_X; x++)
            {
                for (int y = 0; y < TILE_Y; y++)
                {
                    bool temp;

                    if ((x==1||x==5||x==8)&&(y==2||y==5||y==7))//(randNum.Next(0, 50) < 1)
                    {
                        temp = true;
                    }
                    else
                    {
                        temp = false;
                    }


                    /*Tiles[x, y] = new Tile(new Rectangle(TILE_SIDE_LENGTH * x,
                                            TILE_SIDE_LENGTH * y,
                                            TILE_SIDE_LENGTH,
                                            TILE_SIDE_LENGTH),
                                            temp);*/
                    Tiles[x, y] = new Tile(1, false, false);
                }
            }
        }

        public void Draw(SpriteBatch sb, Texture2D pixelTexture, SpriteFont font)
        {
            for (int x = 0; x < TILE_X; x++)
            {
                for (int y = 0; y < TILE_Y; y++)
                {
                    Tiles[x, y].Draw(sb, pixelTexture);

                    sb.DrawString(font, x + ", " + y,
                                  new Vector2((Tiles[x, y].TileRect.Center.X) - (font.MeasureString(x + ", " + y).X) * 0.5f,
                                               (Tiles[x, y].TileRect.Center.Y) - (font.MeasureString(x + ", " + y).Y) * 0.5f),
                        Color.Black);

                }
            }
        }



        //public Tile[] ReturnCollidileTiles()
        //{
        //    List<Tile> tempTiles = new List<Tile>();

        //    for (int x = 0; x < TILE_X; x++)
        //    {
        //        for (int y = 0; y < TILE_Y; y++)
        //        {
        //            if (Tiles[x, y].IsCollidible == true)
        //            {
        //                tempTiles.Add(Tiles[x, y]);
        //            }
        //        }
        //    }

        //    return tempTiles.ToArray();

        //}
    }
}
