using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGear.Engine;


namespace MonoGear.Entities
{
    class SpawnPoint : WorldEntity
    {
        public SpawnPoint(Vector2 position)
        {
            Position = position;
            Tag = "SpawnPoint";
        }
    }
}
