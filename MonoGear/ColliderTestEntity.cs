using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    class ColliderTestEntity : WorldEntity
    {
        public ColliderTestEntity()
        {
            TextureAssetName = "Sprites/s_generator";
            Tag = "ColliderTest";

            LoadContent();

            Collider = new BoxCollider(this, Size);

            // Fountain bottem left collision
            var fountain1 = new CircleCollider(new WorldEntity(), 3 * 16);
            fountain1.Entity.Position = new Microsoft.Xna.Framework.Vector2(1823, 2448);
            fountain1.Entity.Tag = "Fountain";

            // Fountain bottem right collision
            var fountain2 = new CircleCollider(new WorldEntity(), 3 * 16);
            fountain2.Entity.Position = new Microsoft.Xna.Framework.Vector2(2879, 2448);
            fountain2.Entity.Tag = "Fountain";

            // Fountain middel collision
            var fountain3 = new CircleCollider(new WorldEntity(), 3 * 16);
            fountain3.Entity.Position = new Microsoft.Xna.Framework.Vector2(2351, 1920);
            fountain3.Entity.Tag = "Fountain";

            // Fountain top collision
            var fountain4 = new CircleCollider(new WorldEntity(), 3 * 16);
            fountain4.Entity.Position = new Microsoft.Xna.Framework.Vector2(2351, 1215);
            fountain4.Entity.Tag = "Fountain";

        }
    }
}
