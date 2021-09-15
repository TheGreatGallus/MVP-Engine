using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MVP_Core.Managers;
using System.Collections.Generic;

namespace MVP_Core.Entities
{
    public class Animation
    {
        public string name;
        public Dictionary<int, Frame> Frames;
        public int endFrame;

        public Animation()
        {

        }

        public void DrawFrame(SpriteBatch spriteBatch, string animationName, Vector2 position, Rectangle? sourceRectangle, Color color, int frameCount)
        {
            int currentFrameCount = -1;
            foreach (int key in Frames.Keys)
            {
                if (key >= currentFrameCount && key <= frameCount % endFrame)
                    currentFrameCount = key;
            }
            Frame currentFrame = Frames[currentFrameCount];
            Texture2D frameTexture = TextureManager.Instance.GetItem(currentFrame.spriteName);

            //if (currentFrame.sourceRectangle.HasValue)
                spriteBatch.Draw(frameTexture, new Vector2(position.X + currentFrame.sourceRectangle.Width / 2, position.Y + currentFrame.sourceRectangle.Height / 2), currentFrame.sourceRectangle, Color.White);
            //else
            //    spriteBatch.Draw(frameTexture, new Vector2(position.X + frameTexture.Width / 2, position.Y + frameTexture.Height / 2), currentFrame.sourceRectangle, Color.White);

        }
    }
}
