using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    class Level
    {
        List<LevelLayer> backgroundLayers;
        List<LevelLayer> foregroundLayers;

        public Level()
        {
            backgroundLayers = new List<LevelLayer>();
            foregroundLayers = new List<LevelLayer>();
        }

        public void AddBackgroundLayer(LevelLayer layer)
        {
            if(layer.texture == null)
            {
                layer.texture = ResourceManager.GetManager("Global").GetResource<Texture2D>(layer.textureName);
            }
            
            backgroundLayers.Add(layer);
            backgroundLayers.Sort(
                (a, b) =>
                {
                    return a.layer.CompareTo(b.layer);
                });
        }

        public void DrawForeground(SpriteBatch batch)
        {
            foreach(var layer in foregroundLayers)
            {
                batch.Draw(layer.texture, layer.offset, Color.White);
            }
        }

        public void DrawBackground(SpriteBatch batch)
        {
            foreach(var layer in backgroundLayers)
            {
                batch.Draw(layer.texture, layer.offset, Color.White);
            }
        }
    }

    struct LevelLayer
    {
        public int layer;
        public Vector2 offset;
        public string textureName;
        public Texture2D texture;
    }
}
