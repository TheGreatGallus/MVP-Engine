using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MVP_Core.Entities;

namespace MVP_Editor.Tools
{
    public class BucketTool : Tool
    {
        public BucketTool(GraphicsDevice graphicsDevice, Room room, OrthographicCamera cam) : base(room, cam)
        {

        }

        public override void PressLeftClick(int x, int y, int currentLayer)
        {
            Vector2 tPos = getTilePosition(x, y);
            string initialTileNum = room.layers[currentLayer].tileArray[(int)((int)tPos.Y * room.layers[currentLayer].width + (int)tPos.X)];
            if (initialTileNum != primaryTileNum)
                FloodFill(tPos, currentLayer, primaryTileNum, initialTileNum);

        }

        private void FloodFill(Vector2 tPos, int currentLayer, string tileToFill, string initialTileNum)
        {
            if (tPos.X < 0 || tPos.Y < 0 || tPos.X >= room.width || tPos.Y >= room.height)
                return;
            if (room.layers[currentLayer].tileArray[(int)((int)tPos.Y * room.layers[currentLayer].width + (int)tPos.X)] == initialTileNum &&
                room.layers[currentLayer].tileArray[(int)((int)tPos.Y * room.layers[currentLayer].width + (int)tPos.X)] != tileToFill)
            {
                RoomChanged = true;
                room.layers[currentLayer].tileArray[(int)((int)tPos.Y * room.layers[currentLayer].width + (int)tPos.X)] = tileToFill;

                FloodFill(new Vector2(tPos.X - 1, tPos.Y), currentLayer, tileToFill, initialTileNum);
                FloodFill(new Vector2(tPos.X, tPos.Y - 1), currentLayer, tileToFill, initialTileNum);
                FloodFill(new Vector2(tPos.X + 1, tPos.Y), currentLayer, tileToFill, initialTileNum);
                FloodFill(new Vector2(tPos.X, tPos.Y + 1), currentLayer, tileToFill, initialTileNum);
            }
        }

        public override void ReleaseLeftClick(int x, int y, int currentLayer)
        {
            base.ReleaseLeftClick(x, y, currentLayer);
        }

        public override void PressRightClick(int x, int y, int currentLayer)
        {
            Vector2 tPos = getTilePosition(x, y);
            string initialTileNum = room.layers[currentLayer].tileArray[(int)((int)tPos.Y * room.layers[currentLayer].width + (int)tPos.X)];
            if (initialTileNum != secondaryTileNum)
                FloodFill(tPos, currentLayer, secondaryTileNum, initialTileNum);
        }

        public override void ReleaseRightClick(int x, int y, int currentLayer)
        {
            base.ReleaseRightClick(x, y, currentLayer);
        }

        public override void PressMiddleClick(int x, int y, int currentLayer)
        {
            Vector2 tPos = getTilePosition(x, y);
            primaryTileNum = room.layers[currentLayer].tileArray[(int)((int)tPos.Y * room.layers[currentLayer].width + (int)tPos.X)];
        }
    }
}
