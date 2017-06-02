using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGear
{
    public class Camera
    {
        public static Camera main;

        private Viewport viewport;
        /// <summary>
        /// Camera's position
        /// </summary>
        public Vector2 Position { get; set; }
        /// <summary>
        /// Camera's rotation
        /// </summary>
        public float Rotation { get; set; }
        /// <summary>
        /// Camera's zoom value
        /// </summary>
        public float Zoom { get; set; }
        /// <summary>
        /// Camera's origin location
        /// </summary>
        public Vector2 Origin { get; set; }

        public Camera(Viewport viewport)
        {
            main = this;

            this.viewport = viewport;
            Rotation = 0;
            Zoom = 1;
            Position = Vector2.Zero;
            RecalculateOrigin();
        }

        public void RecalculateOrigin()
        {
            Origin = new Vector2(viewport.Width / 2f, viewport.Height / 2f);
        }

        public void UpdateViewport(Viewport viewport)
        {
            this.viewport = viewport;
            RecalculateOrigin();
        }

        /// <summary>
        /// Method that returns a view matrix
        /// </summary>
        /// <returns></returns>
        public Matrix GetViewMatrix()
        {
            return
                Matrix.CreateTranslation(new Vector3(-Position, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        public Rectangle GetClippingRect()
        {
            var matrix = Matrix.Invert(GetViewMatrix());
            var topLeft = -new Vector2(viewport.Width / 2, viewport.Height / 2) + Origin;
            var bottomRight = new Vector2(viewport.Width / 2, viewport.Height / 2) + Origin;

            topLeft = Vector2.Transform(topLeft, matrix);
            bottomRight = Vector2.Transform(bottomRight, matrix);

            return new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)bottomRight.X - (int)topLeft.X, (int)bottomRight.Y - (int)topLeft.Y);
        }
    }
}