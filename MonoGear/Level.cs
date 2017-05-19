using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    public class Level
    {
        List<LevelLayer> backgroundLayers;
        List<LevelLayer> foregroundLayers;
        HashSet<WorldEntity> levelEntities;

        public Level()
        {
            backgroundLayers = new List<LevelLayer>();
            foregroundLayers = new List<LevelLayer>();
            levelEntities = new HashSet<WorldEntity>();
        }

        public void AddBackgroundLayer(LevelLayer layer)
        {
            if(layer.texture == null)
            {
                layer.texture = ResourceManager.GetManager().GetResource<Texture2D>(layer.textureName);
            }
            
            backgroundLayers.Add(layer);
            backgroundLayers.Sort(
                (a, b) =>
                {
                    return a.layer.CompareTo(b.layer);
                });
        }

        public void AddForegroundLayer(LevelLayer layer)
        {
            if (layer.texture == null)
            {
                layer.texture = ResourceManager.GetManager().GetResource<Texture2D>(layer.textureName);
            }

            foregroundLayers.Add(layer);
            foregroundLayers.Sort(
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

        public void AddEntity(WorldEntity entity)
        {
            entity.OnLevelUnloaded();
            levelEntities.Add(entity);
        }

        public WorldEntity[] GetEntities()
        {
            return levelEntities.ToArray();
        }
    }

    public struct LevelLayer
    {
        public int layer;
        public Vector2 offset;
        public string textureName;
        public Texture2D texture;
    }
}
