using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MVP_Core.Entities;
using MVP_Core.Global;

namespace MVP_Editor.Tools
{
    public class Tool
    {
        public Room room;
        public string primaryTileNum;
        public string secondaryTileNum;
        public OrthographicCamera cam;
        public Texture2D icon;
        public Vector2 clickPosition;
        public Vector2 releasePosition;
        public bool canLeftBeHeld = false;
        public bool canRightBeHeld = false;
        private bool roomChanged;
        public bool RoomChanged
        {
            get { return roomChanged; }
            set
            {
                if (!roomChanged)
                    room.ChangeRoomState();
                roomChanged = value;
            }
        }

        public Tool(Room room, OrthographicCamera cam)
        {
            this.room = room;
            this.cam = cam;
        }

        public virtual void PressLeftClick(int x, int y, int currentLayer)
        {

        }

        public virtual void ReleaseLeftClick(int x, int y, int currentLayer)
        {
            if (RoomChanged)
                RoomChanged = false;
        }

        public virtual void PressRightClick(int x, int y, int currentLayer)
        {

        }

        public virtual void ReleaseRightClick(int x, int y, int currentLayer)
        {
            if (RoomChanged)
                RoomChanged = false;
        }

        public virtual void PressMiddleClick(int x, int y, int currentLayer)
        {

        }

        public virtual void ReleaseMiddleClick(int x, int y, int currentLayer)
        {

        }

        public virtual void TypeX()
        {
            string temp = primaryTileNum;
            primaryTileNum = secondaryTileNum;
            secondaryTileNum = temp;
        }

        public Vector2 getTilePosition(int x, int y)
        {
            return new Vector2((x + cam.Position.X) / GameValues.tileDim, (y + cam.Position.Y) / GameValues.tileDim);
        }

        public Tool switchActiveToolTo(Tool newTool)
        {
            newTool.primaryTileNum = primaryTileNum;
            newTool.secondaryTileNum = secondaryTileNum;
            return newTool;
        }

        public bool switchRooms(Room newRoom)
        {
            room = newRoom;
            return true;
        }
    }
}
