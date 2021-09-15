using Microsoft.Xna.Framework;

namespace MVP_Core.Entities
{
    public class Frame
    {
        public string spriteName;
        public int frameCount;
        public Rectangle sourceRectangle;

        public Frame(string spriteName, int frameCount, Rectangle sourceRectangle)
        {
            this.spriteName = spriteName;
            this.frameCount = frameCount;
            this.sourceRectangle = sourceRectangle;
        }

        public void SetValues(string spriteName, int frameCount, Rectangle sourceRectangle)
        {
            this.spriteName = spriteName;
            this.frameCount = frameCount;
            this.sourceRectangle = sourceRectangle;
        }
    }
}
