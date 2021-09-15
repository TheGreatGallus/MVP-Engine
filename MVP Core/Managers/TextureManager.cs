using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Managers
{
    public class TextureManager : Manager<Texture2D>
    {
        private static readonly Lazy<TextureManager> lazy =
            new Lazy<TextureManager>(() => new TextureManager());

        public static TextureManager Instance { get { return lazy.Value; } }
        public static Dictionary<int, string> Tilesets;
        private List<QueuedTexture> textureQueue;

        private TextureManager()
        {
            Tilesets = new Dictionary<int, string>();
            Initialize();
            textureQueue = new List<QueuedTexture>();
        }

        public void RegisterTileset(int key, string name)
        {
            Tilesets.Add(key, name);
        }

        public string GetTilesetName(int key)
        {
            return Tilesets[key];
        }

        public List<string> GetTilesetNames()
        {
            List<string> names = Tilesets.Select(ts => ts.Value).ToList();
            return names;
        }

        public void Draw(SpriteBatch spriteBatch, string spriteName, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            spriteBatch.Draw(GetItem(spriteName), destinationRectangle, sourceRectangle, color);
        }

        public void Draw(SpriteBatch spriteBatch, string spriteName, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            spriteBatch.Draw(GetItem(spriteName), position, sourceRectangle, color);
        }

        public void Draw(SpriteBatch spriteBatch, string spriteName, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerdepth)
        {
            Texture2D texture = GetItem(spriteName);
            if(sourceRectangle.HasValue)
                spriteBatch.Draw(texture, new Vector2(position.X + sourceRectangle.Value.Width/2, position.Y+ sourceRectangle.Value.Height/2), sourceRectangle, color, rotation, new Vector2(texture.Width / 2, texture.Height / 2), scale, effects, layerdepth);
            else
                spriteBatch.Draw(texture, new Vector2(position.X + texture.Width / 2, position.Y + texture.Height / 2), sourceRectangle, color, rotation, new Vector2(texture.Width / 2, texture.Height / 2), scale, effects, layerdepth);
        }

        public void DrawQueue(SpriteBatch spriteBatch, bool frameStart)
        {
            foreach (QueuedTexture queuedTexture in textureQueue.Where(qt => qt.frameStart == frameStart).ToList())
            {
                if (queuedTexture.destinationRectangle != null)
                    Draw(spriteBatch, queuedTexture.spriteName, (Rectangle)queuedTexture.destinationRectangle, queuedTexture.sourceRectangle, queuedTexture.color);
                else
                    Draw(spriteBatch, queuedTexture.spriteName, queuedTexture.position, queuedTexture.sourceRectangle, queuedTexture.color);
                textureQueue.Remove(queuedTexture);
            }
        }

        public void Queue(string spriteName, Rectangle? destinationRectangle, Rectangle? sourceRectangle, Color color, bool frameStart)
        {
            textureQueue.Add(new QueuedTexture(spriteName, destinationRectangle, sourceRectangle, color, frameStart));
        }

        public void Queue(string spriteName, Vector2 position, Rectangle? sourceRectangle, Color color, bool frameStart)
        {
            textureQueue.Add(new QueuedTexture(spriteName, position, sourceRectangle, color, frameStart));
        }
    }

    public class QueuedTexture
    {
        public string spriteName;
        public Rectangle? destinationRectangle = null;
        public Rectangle? sourceRectangle;
        public Vector2 position;
        public Color color;
        public bool frameStart = false;

        public QueuedTexture(string spriteName, Rectangle? destinationRectangle, Rectangle? sourceRectangle, Color color, bool frameStart)
        {
            this.spriteName = spriteName;
            this.destinationRectangle = destinationRectangle;
            this.sourceRectangle = sourceRectangle;
            this.color = color;
            this.frameStart = frameStart;
        }

        public QueuedTexture(string spriteName, Vector2 position, Rectangle? sourceRectangle, Color color, bool frameStart)
        {
            this.spriteName = spriteName;
            this.position = position;
            this.sourceRectangle = sourceRectangle;
            this.color = color;
            this.frameStart = frameStart;
        }
    }
}
