using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MVP_Core.Entities;
using MVP_Core.Global;

namespace MVP_Editor.Tools
{
    public class ZoneTool : Tool
    {
        bool isClicked = false;
        string zoneType;

        public ZoneTool(GraphicsDevice graphicsDevice, Room room, OrthographicCamera cam) : base(room, cam)
        {
            canLeftBeHeld = true;
            zoneType = "None";
        }

        public void ChangeZoneType(string type)
        {
            zoneType = type;
        }

        public override void PressLeftClick(int x, int y, int currentLayer)
        {
            if (!isClicked)
            {
                clickPosition = getTilePosition(x, y);
            }
            isClicked = true;
            RoomChanged = true;
        }

        public override void ReleaseLeftClick(int x, int y, int currentLayer)
        {
            releasePosition = getTilePosition(x, y);
            if (isClicked)
            {
                int x1 = (int)((clickPosition.X <= releasePosition.X) ? clickPosition.X : releasePosition.X);
                int x2 = (int)((clickPosition.X > releasePosition.X) ? clickPosition.X : releasePosition.X);
                int y1 = (int)((clickPosition.Y <= releasePosition.Y) ? clickPosition.Y : releasePosition.Y);
                int y2 = (int)((clickPosition.Y > releasePosition.Y) ? clickPosition.Y : releasePosition.Y);
                // TODO: Add type correctly
                Zone newZone = new Zone(x1 * GameValues.tileDim, (x2 + 1) * GameValues.tileDim - 1, y1 * GameValues.tileDim, (y2 + 1) * GameValues.tileDim - 1, zoneType, null);
                room.zones.Add(newZone);
            }
            isClicked = false;
            base.ReleaseLeftClick(x, y, currentLayer);
        }

        public override void PressRightClick(int x, int y, int currentLayer)
        {
            bool hasSelected = false;
            foreach (Zone zone in room.zones)
            {
                if (zone.boundary.Contains(new Vector2(x + cam.Position.X, y + cam.Position.Y)) && !hasSelected)
                {
                    zone.selected = true;
                    hasSelected = true;
                }
                else
                {
                    zone.selected = false;
                }
            }
        }

        public override void PressMiddleClick(int x, int y, int currentLayer)
        {
            bool hasSelected = false;
            foreach (Zone zone in room.zones)
            {
                if (zone.boundary.Contains(new Vector2(x + cam.Position.X, y + cam.Position.Y)) && !hasSelected)
                {
                    zone.selected = true;
                    hasSelected = true;
                }
                else
                {
                    zone.selected = false;
                }
            }
        }
    }
}
