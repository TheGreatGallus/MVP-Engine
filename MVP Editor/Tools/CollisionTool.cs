using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MVP_Core.Entities;
using MVP_Core.Global;

namespace MVP_Editor.Tools
{
    public class CollisionTool : Tool
    {
        public CollisionTool(GraphicsDevice graphicsDevice, Room room, OrthographicCamera cam) : base(room, cam)
        {
            canLeftBeHeld = true;
            canRightBeHeld = true;
        }

        // TODO: Fix implementation for colliisions
        public override void PressLeftClick(int x, int y, int currentLayer)
        {
            Vector2 tPos = getTilePosition(x, y);
            int tileArrayPos = room.width * (int)tPos.Y + (int)tPos.X;
            if (room.collisionLayer.tileArray[tileArrayPos] != primaryTileNum)
            {
                RoomChanged = true;
                room.collisionLayer.tileArray[tileArrayPos] = primaryTileNum;
                string[] tileString = primaryTileNum.Split('-');
                Tile refTile = Tile.tileDictionary[tileString[1]];
                Tile newTile = new Tile((int)tPos.X * GameValues.tileDim, (int)tPos.Y * GameValues.tileDim, refTile.c, refTile.t, refTile.sequence, tileString[1]);
                room.collisionLayer.tiles[(int)tPos.X, (int)tPos.Y] = newTile;
            }
        }

        public override void PressRightClick(int x, int y, int currentLayer)
        {
            Vector2 tPos = getTilePosition(x, y);
            int tileArrayPos = room.width * (int)tPos.Y + (int)tPos.X;
            if (room.collisionLayer.tileArray[tileArrayPos] != secondaryTileNum)
            {
                RoomChanged = true;
                room.collisionLayer.tileArray[tileArrayPos] = secondaryTileNum;
                string[] tileString = secondaryTileNum.Split('-');
                Tile refTile = Tile.tileDictionary[tileString[1]];
                Tile newTile = new Tile((int)tPos.X * GameValues.tileDim, (int)tPos.Y * GameValues.tileDim, refTile.c, refTile.t, refTile.sequence, tileString[1]);
                room.collisionLayer.tiles[(int)tPos.X, (int)tPos.Y] = newTile;
            }
        }

        public override void PressMiddleClick(int x, int y, int currentLayer)
        {
            Vector2 tPos = getTilePosition(x, y);
            primaryTileNum = room.layers[currentLayer].tileArray[(int)(tPos.Y * room.layers[currentLayer].width + tPos.X)];
        }
    }
}
