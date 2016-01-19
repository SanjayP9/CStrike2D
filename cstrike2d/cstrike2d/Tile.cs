using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class Tile
    {
        const int BORDER_THICKNESS = 1;
        public Rectangle TileRect { get; private set; }
        public Rectangle Bounds { get; private set; }
        public byte Property { get; private set; }
        public const byte NO_PROPERTY = 0;
        public const byte SOLID = 1;
        public const byte A_PLANT_SPOT = 2;
        public const byte B_PLANT_SPOT = 3;
        public const byte SAVE_SPOT = 4;
        public const byte CT_SPAWN_POINT = 5;
        public const byte T_SPAWN_POINT = 6;
        public const byte A_SITE_DEFENCE_POINT = 7;
        public const byte B_SITE_DEFENCE_POINT = 8;
        public int FCost { get; set; }
        public int GCost { get; set; }
        public int HCost { get; set; }
        public byte TileType { get; private set; }
        //public bool IsSolid { get; private set; }
        //public bool IsAPlantSpot { get; private set; }
        //public bool IsBPlantSpot { get; private set; }
        //public bool IsSaveSpot { get; private set; }
        //public bool IsCTSpawnPoint { get; private set; }
        //public bool IsTSpawnPoint { get; private set; }
        //public bool IsASiteDefencePoint { get; private set; }
        //public bool IsBSiteDefencePoint { get; private set; }

        //public Tile(int tileType, bool isSolid,bool isAPlantSpot, bool isBPlantSpot, bool isSaveSpot, bool isCTSpawnPoint, bool isTSpawnPoint, bool isASiteDefencePoint, bool isBSiteDefencePoint)
        //{
        //    TileType = tileType;
        //    IsSolid = isSolid;
        //    IsAPlantSpot = isAPlantSpot;
        //    IsBPlantSpot = isBPlantSpot;
        //    IsSaveSpot = isSaveSpot;
        //    IsCTSpawnPoint = isCTSpawnPoint;
        //    IsTSpawnPoint = isTSpawnPoint;
        //    IsASiteDefencePoint = isASiteDefencePoint;
        //    IsBSiteDefencePoint = isBSiteDefencePoint;
        //}
        public Tile(byte tileType, byte property)
        {
            TileType = tileType;
            Property = property;
        }
        public void SetProperty(byte property)
        {
            Property = property;
        }
        //public void ResetProperties()
        //{
        //    IsSolid = false;
        //    IsAPlantSpot = false;
        //    IsBPlantSpot = false;
        //    IsSaveSpot = false;
        //    IsCTSpawnPoint = false;
        //    IsTSpawnPoint = false;
        //    IsASiteDefencePoint = false;
        //    IsBSiteDefencePoint = false;
        //}
        //public void ChangeProperty(byte property)
        //{
        //    ResetProperties();
        //    switch (property)
        //    {
        //        case 1:
        //            IsSolid = true;
        //            break;
        //        case 2:
        //            IsAPlantSpot = true;
        //            break;
        //        case 3:
        //            IsBPlantSpot = true;
        //            break;
        //        case 4:
        //            IsSaveSpot = true;
        //            break;
        //        case 5:
        //            IsCTSpawnPoint = true;
        //            break;
        //        case 6:
        //            IsTSpawnPoint = true;
        //            break;
        //        case 7:
        //            IsASiteDefencePoint = true;
        //            break;
        //        case 8:
        //            IsBSiteDefencePoint = true;
        //            break;
        //    }
        //}
        public void SetTileType(byte tileType)
        {
            TileType = tileType;
        }
        //public void SetIsSolid(bool isSolid)
        //{
        //    IsSolid = isSolid;
        //}
        //public void SetIsAPlantSpot(bool isAPlantSpot)
        //{
        //    IsAPlantSpot = isAPlantSpot;
        //}
        //public void SetIsBPlantSpot(bool isBPlantSpot)
        //{
        //    IsBPlantSpot = isBPlantSpot;
        //}
        //public void SetIsSaveSpot(bool isSaveSpot)
        //{
        //    IsSaveSpot = isSaveSpot;
        //}
        //public void SetIsCTSpawnPoint(bool isCTSpawnPoint)
        //{
        //    IsCTSpawnPoint = isCTSpawnPoint;
        //}
        //public void SetIsTSpawnPoint(bool isTSpawnPoint)
        //{
        //    IsTSpawnPoint = isTSpawnPoint;
        //}
        //public void SetIsASiteDefencePoint(bool isASiteDefencePoint)
        //{
        //    IsASiteDefencePoint = isASiteDefencePoint;
        //}
        //public void SetIsBSiteDefencePoint(bool isBSiteDefencePoint)
        //{
        //    IsBSiteDefencePoint = isBSiteDefencePoint;
        //}

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
