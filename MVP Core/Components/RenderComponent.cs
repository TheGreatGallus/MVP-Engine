using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MVP_Core.Global;
using MVP_Core.Managers;

namespace MVP_Core.Components
{
    public class RenderComponent : Component
    {
        private Texture2D texture;
        private string spriteName;
        private string currentAnimationState;
        private int frameStart;
        private float rotation = 0.0f;
        private float scale = 1.0f;
        //private string Animation;

        public RenderComponent() : base()
        {
            texture = null;
            currentAnimationState = "";
        }

        public RenderComponent(int eId) : base(eId)
        {
            texture = null;
        }

        public void SetTexture(string spriteName)
        {
            this.spriteName = spriteName;
        }

        public void SetTexture(Texture2D inTexture)
        {
            texture = inTexture;
        }

        public bool SetAnimationState(string animationState)
        {
            currentAnimationState = animationState;
            frameStart = GameValues.frame;
            return true;
        }

        //public void Draw(SpriteBatch spriteBatch, Vector2 camPosition, Vector2 position)
        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            //spriteBatch.Draw(texture, position, Color.White);
            //TextureManager.Instance.Draw(spriteBatch, spriteName, position, null, Color.White);
            if (currentAnimationState != null && currentAnimationState != "")
                AnimationManager.Instance.Draw(spriteBatch, currentAnimationState, position, null, Color.White, GameValues.frame - frameStart);
            else
            //rotation += 0.1f;
                TextureManager.Instance.Draw(spriteBatch, spriteName, position, null, Color.White, rotation, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        }

        public void Draw(double fractionalFrames)
        {

        }
    }
}
