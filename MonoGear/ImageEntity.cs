using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    class ImageEntity : WorldEntity
    {
        public ImageEntity(string resource, Vector3 position) : base()
        {
            this.TextureAssetName = resource;
            this.Position = position;

            LoadContent();
        }
    }
}
