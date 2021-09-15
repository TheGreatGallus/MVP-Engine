using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MVP_Core.Entities;

namespace MVP_Editor.Tools
{
    // TODO: BUILD AFTER TILESETS ARE IMPLEMENTED
    public class EraserTool : Tool
    {
        public EraserTool(GraphicsDevice graphicsDevice, Room room, OrthographicCamera cam) : base(room, cam)
        {
            canLeftBeHeld = true;
        }

        public override void PressLeftClick(int x, int y, int currentLayer)
        {
            Vector2 tPos = getTilePosition(x, y);
            if (room.layers[currentLayer].tileArray[(int)((int)tPos.Y * room.layers[currentLayer].width + (int)tPos.X)] != primaryTileNum)
            {
                RoomChanged = true;
                room.layers[currentLayer].tileArray[(int)((int)tPos.Y * room.layers[currentLayer].width + (int)tPos.X)] = primaryTileNum;
            }

        }

        public override void ReleaseLeftClick(int x, int y, int currentLayer)
        {
            base.ReleaseLeftClick(x, y, currentLayer);
        }

        public override void PressRightClick(int x, int y, int currentLayer)
        {
            Vector2 tPos = getTilePosition(x, y);
            if (room.layers[currentLayer].tileArray[(int)((int)tPos.Y * room.layers[currentLayer].width + (int)tPos.X)] != secondaryTileNum)
            {
                RoomChanged = true;
                room.layers[currentLayer].tileArray[(int)((int)tPos.Y * room.layers[currentLayer].width + (int)tPos.X)] = secondaryTileNum;
            }
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
