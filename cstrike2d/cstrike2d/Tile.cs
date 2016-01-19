using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class Tile
    {
        const int BORDER_THICKNESS = 1;
        public Rectangle TileRect { get; private set; }
        public int TileType { get; private set; }
        public Rectangle Bounds { get; private set; }
        public bool IsSolid { get; private set; }
        public bool IsAPlantSpot { get; private set; }
        public bool IsBPlantSpot { get; private set; }
        public bool IsSaveSpot { get; private set; }
        public bool IsCTSpawnPoint { get; private set; }
        public bool IsTSpawnPoint { get; private set; }
        public bool IsASiteDefencePoint { get; private set; }
        public bool IsBSiteBDefencePoint { get; private set; }

        public Tile(int tileType, bool iSolid,bool isAPlantSpot, bool isBPlantSpot, bool isSaveSpot, bool isCTSpawnPoint, bool isTSpawnPoint, bool isASiteDefencePoint, bool isBSiteDefence)
        {
            //TileType = tileType;
            //IsAPlantSpot = isAPlantSpot;
            //IsBPlantSpot = isBPlantSpot;
            //IsSaveSpot = isSaveSpot;
            //IsSolid = isSolid;
            //IsCTSpawnPoint = isCTSpawnPoint;
            //IsTSpawnPoint = isTSpawnPoint;
            //IsSiteDefencePoint = isSiteDefencePoint;
            TileType = tileType;
            IsSolid = isSolidbool isAPlantSpot, bool isBPlantSpot, bool isSaveSpot, bool isCTSpawnPoint, bool isTSpawnPoint, bool isASiteDefencePoint, bool isBSiteDefence
        }
        public void SetTileType(int tileType)
        {
            TileType = tileType;
        }
        public void SetIsSolid(bool isSolid)
        {
            IsSolid = isSolid;
        }
        public void SetIsPlantSpot(bool isPlantSpot)
        {
            IsPlantSpot = isPlantSpot;
        }
        public void SetIsSaveSpot(bool isSaveSpot)
        {
            IsSaveSpot = isSaveSpot;
        }
        public void SetIsCTSpawnPoint(bool isCTSpawnPoint)
        {
            IsCTSpawnPoint = isCTSpawnPoint;
        }
        public void SetIsTSpawnPoint(bool isTSpawnPoint)
        {
            IsTSpawnPoint = isTSpawnPoint;
        }
        public void SetIsSiteDefencePoint(bool isSiteDefencePoint)
        {
            IsSiteDefencePoint = isSiteDefencePoint;
        }

        public void Draw(SpriteBatch sb, Texture2D tileTexture)
        {
            sb.Draw(tileTexture, new Rectangle(TileRect.X, TileRect.Y, TileRect.Width, TileRect.Height), Color.White);

            sb.Draw(tileTexture, new Rectangle(TileRect.X, TileRect.Y, BORDER_THICKNESS, TileRect.Height), Color.Black);
            sb.Draw(tileTexture, new Rectangle(TileRect.X, TileRect.Y, TileRect.Width, BORDER_THICKNESS), Color.Black);
            sb.Draw(tileTexture, new Rectangle(TileRect.X, TileRect.Bottom - BORDER_THICKNESS, TileRect.Width, BORDER_THICKNESS), Color.Black);
            sb.Draw(tileTexture, new Rectangle(TileRect.Right - BORDER_THICKNESS, TileRect.Y, BORDER_THICKNESS, TileRect.Height), Color.Black);
        }
    }
}
