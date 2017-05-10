using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGear
{
    class Camera
    {
        public static Camera main;

        private Viewport viewport;

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Zoom { get; set; }
        public Vector2 Origin { get; set; }

        public Camera(Viewport viewport)
        {
            if(main == null)
                main = this;

            this.viewport = viewport;

            Rotation = 0;
            Zoom = 1;
            Position = Vector2.Zero;
            RecalculateOrigin(viewport);
        }

        public void RecalculateOrigin(Viewport viewport)
        {
            Origin = new Vector2(viewport.Width / 2f, viewport.Height / 2f);
        }

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