using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MVP_Core.Global;

namespace MVP_Core.Entities
{
    public class Camera
    {
        public float Zoom = 1.0f;
        public Vector2 Location
        {
            get { return new Vector2(x, y); }
            set
            {
                x = value.X;
                y = value.Y;
            }
        }
        public Vector2 Origin;
        public float Rotation = 0.0f;
        private Rectangle Bounds;
        public float x;
        public float y;

        public Rectangle VisibleArea
        {
            get
            {
                var inverseViewMatrix = Matrix.Invert(TransformMatrix(Vector2.One));
                var topLeft = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
                var topRight = Vector2.Transform(new Vector2(GameValues.screenWidth, 0), inverseViewMatrix);
                var bottomLeft = Vector2.Transform(new Vector2(0, GameValues.screenHeight), inverseViewMatrix);
                var bottomRight = Vector2.Transform(new Vector2(GameValues.screenWidth, GameValues.screenHeight), inverseViewMatrix);

                var min = new Vector2(MathHelper.Min(topLeft.X, MathHelper.Min(topRight.X, MathHelper.Min(bottomLeft.X, bottomRight.X))),
                    MathHelper.Min(topLeft.Y, MathHelper.Min(topRight.Y, MathHelper.Min(bottomLeft.Y, bottomRight.Y))));

                var max = new Vector2(MathHelper.Max(topLeft.X, MathHelper.Max(topRight.X, MathHelper.Max(bottomLeft.X, bottomRight.X))),
                    MathHelper.Max(topLeft.Y, MathHelper.Max(topRight.Y, MathHelper.Max(bottomLeft.Y, bottomRight.Y))));

                return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
            }
        }

        public Camera()
        {

        }

        public Camera(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Camera(Viewport viewport)
        {
            Bounds = viewport.Bounds;
            //Location = Vector2.Zero;
            Origin = new Vector2(Location.X + Bounds.Width / 2f, Location.Y + Bounds.Height / 2f);
        }

        public void CenterAt(Vector2 center)
        {
            Location = center + new Vector2(-GameValues.ScaledWidth, GameValues.ScaledHeight);
        }

        public Matrix TransformMatrix(Vector2 parallax)
        {
            //return Matrix.CreateTranslation(new Vector3(-Location.X, -Location.Y, 0)) *
            //    Matrix.CreateScale(Zoom) *
            //    Matrix.CreateTranslation(new Vector3(Bounds.Width * 0.5f, Bounds.Height * 0.5f, 0));
            return Matrix.CreateTranslation(new Vector3(-Location * parallax, 0)) *
                Matrix.CreateTranslation(new Vector3(-Origin, 0)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateTranslation(new Vector3(Origin, 0));
        }
    }
}
