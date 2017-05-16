using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
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
