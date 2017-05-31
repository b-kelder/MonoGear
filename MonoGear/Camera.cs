using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGear
{
    class Camera
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
            if (main == null)
            {
                main = this;
            }

            this.viewport = viewport;

            Rotation = 0;
            Zoom = 1;
            Position = Vector2.Zero;
            //Recalculate the origin based on the given viewport
            RecalculateOrigin(viewport);
        }

        /// <summary>
        /// Method that recalculates the camera's origin
        /// </summary>
        /// <param name="viewport"></param>
        public void RecalculateOrigin(Viewport viewport)
        {
            Origin = new Vector2(viewport.Width / 2f, viewport.Height / 2f);
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
    }
}