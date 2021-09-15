using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MVP_Core.Entities;
using MVP_Core.Managers;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

namespace MVP_Core.Global
{
    public class MVPScripts
    {
        public static NameValueCollection section = (NameValueCollection)ConfigurationManager.GetSection("customSettings/scriptDefinitions");
        public static SoundEffect sound;
        private static MVPGame game;

        private static Room room
        {
            get
            {
                return RoomManager.Instance.room;
            }
        }

        //public static bool SetGame(MyGameEditor inGame)
        public static bool SetGame(MVPGame inGame)
        {
            game = inGame;
            return true;
        }

        public static bool ReloadPlayerScripts()
        {
            //game.LoadScripts();
            return true;
        }

        // Missing other reload scripts

        public static bool StartSound(string name, float volume = 0.1f, float pitch = 0, float pan = 0, bool isLooping = false)
        {
            SoundEffectManager.Instance.PlaySound(name, volume, pitch, pan, isLooping);
            return true;
        }

        public static bool StopSound(string name)
        {
            SoundEffectManager.Instance.StopSound(name);
            return true;
        }

        public static bool MoveCamera(Vector2 newPosition)
        {
            //game.cam.CenterAt(newPosition);
            game.camera.Position = new Vector2(newPosition.X - GameValues.ScaledWidth / 2, newPosition.Y - GameValues.ScaledHeight / 2);
            return true;
        }

        public static bool MoveEntity(int owner, float xMove, float yMove)
        {
            ComponentManager.Instance.entities.Where(e => e.id == owner).FirstOrDefault().SetPosition(new Vector2(xMove, yMove));
            return true;
        }

        public static Vector2 GetEntityPosition(int owner)
        {
            Vector2 pos = ComponentManager.Instance.entities.Where(e => e.id == owner).FirstOrDefault().position;
            return pos;
        }

        public static Vector2 GetEntityDimensions(int owner)
        {
            Vector2 dim = ComponentManager.Instance.entities.Where(e => e.id == owner).FirstOrDefault().dimensions;
            return dim;
        }

        public static bool SetEntityAnimationState(int owner, string state)
        {
            ComponentManager.Instance.RenderComponents().Where(rc => rc.entityId == owner).FirstOrDefault().SetAnimationState(state);
            return true;
        }

        public static bool CreateEntity(string entityType, Vector2 position)
        {
            return true;
        }

        public static bool SetEntityProperty(int owner, string propertyName, object value)
        {
            Entity entity = ComponentManager.Instance.entities.Where(e => e.id == owner).FirstOrDefault();
            if (entity.properties.ContainsKey(propertyName))
                ComponentManager.Instance.entities.Where(e => e.id == owner).FirstOrDefault().properties[propertyName] = value;
            else
                ComponentManager.Instance.entities.Where(e => e.id == owner).FirstOrDefault().properties.Add(propertyName, value);
            return true;
        }

        public static object GetEntityProperty(int owner, string propertyName)
        {
            return ComponentManager.Instance.entities.Where(e => e.id == owner).FirstOrDefault().properties[propertyName];
        }

        public static bool HasEntityProperty(int owner, string propertyName)
        {
            return ComponentManager.Instance.entities.Where(e => e.id == owner).FirstOrDefault().properties.ContainsKey(propertyName);
        }

        public static Tile[,] GetRows(int top, int bottom)
        {
            return room.GetTileRows(top, bottom);
        }

        public static Tile[,] GetColumns(int left, int right)
        {
            return room.GetTileColumns(left, right);
        }

        public static Tile[,] GetTiles(int xLeft, int xRight, int yTop, int yBottom)
        {
            return room.GetTileCluster(xLeft / GameValues.tileDim - 2, xRight / GameValues.tileDim + 3,
                yTop / GameValues.tileDim - 3, yBottom / GameValues.tileDim + 1);
        }

        public static Tile GetCollisionTiles(int line, Tile.Type[] types, Tile[,] tiles, int dir, Vector2 position, int width)
        {
            Tile collision = null;

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                if (line < 0 || line >= tiles.GetLength(1))
                {
                    break;
                }
                Tile tile = tiles[i, line];
                if (types.Contains(tile.t))
                {
                    if (tile.isSlope && (position.X < tile.x || position.X >= tile.x + GameValues.tileDim))
                    {
                        break;
                    }
                    collision = tile;
                }
            }

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                if (line - 1 < 0 || line - 1 >= tiles.GetLength(1))
                {
                    break;
                }
                Tile tile = tiles[i, line - 1];
                if (types.Contains(tile.t))
                {
                    if (tile.isSlope && (position.X + width / 2 > tile.x && position.X + width / 2 <= tile.x + GameValues.tileDim))
                    {
                        collision = tile;
                    }
                }
            }

            if (collision != null)
                collision.isColliding = true;
            return collision;
        }

        public static int GetTopTile(Vector2 position)
        {
            int top = (int)(position.Y / GameValues.tileDim);
            return top;
        }

        public static int GetBottomTile(Vector2 position, Vector2 dimensions)
        {
            int bottom = (int)((position.Y + dimensions.Y) / GameValues.tileDim);
            if ((position.Y + dimensions.Y) % GameValues.tileDim != 0)
            {
                bottom++;
            }
            return bottom;
        }

        public static int GetLeftTile(Vector2 position)
        {
            int left = (int)(position.X / GameValues.tileDim);
            return left;
        }

        public static int GetRightTile(Vector2 position, Vector2 dimensions)
        {
            int right = (int)((position.X + dimensions.X) / GameValues.tileDim);
            if ((position.X + dimensions.X) % GameValues.tileDim != 0)
            {
                right++;
            }
            return right;
        }

        public static int GetCenterTile(Vector2 position, Vector2 dimensions)
        {
            int center = (int)((position.X + dimensions.X / 2) / GameValues.tileDim);
            return center;
        }


        public static bool Print(string text)
        {
            Console.WriteLine(text);
            return true;
        }

        public static Zone GetZone(int number)
        {
            return room.zones[number - 1];
        }

        public static bool SwitchRoom(int number)
        {
            RoomManager.Instance.ChangeRoom(number);
            return true;
        }

        public static bool SwitchRoom(string zoneName, string roomName)
        {
            //RoomManager.Instance.ChangeRoom(zoneName, roomName);
            return true;
        }

        public static bool QueueTexture(string spriteName, Vector2 position, Rectangle? sourceRectangle, Color color, bool frameStart)
        {
            TextureManager.Instance.Queue(spriteName, position, sourceRectangle, color, frameStart);
            return true;
        }
    }
}
