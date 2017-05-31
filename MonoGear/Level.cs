using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;

namespace MonoGear
{
    public class Level
    {
        /// <summary>
        /// All layers as loaded from the level file
        /// </summary>
        List<LevelLayer> tileLayers;
        /// <summary>
        /// Contains the 'top most' tiles, used for collisions
        /// </summary>
        LevelLayer combinedLayer;

        public Tile[] Tiles { get { return combinedLayer.tiles; } }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        List<LevelOverlay> backgroundLayers;
        List<LevelOverlay> foregroundLayers;
        HashSet<WorldEntity> levelEntities;

        private Level()
        {
            tileLayers = new List<LevelLayer>();

            backgroundLayers = new List<LevelOverlay>();
            foregroundLayers = new List<LevelOverlay>();
            levelEntities = new HashSet<WorldEntity>();
        }

        public void AddBackgroundLayer(LevelOverlay layer)
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

        public void AddForegroundLayer(LevelOverlay layer)
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

        public void DrawTiles(SpriteBatch batch, Camera camera)
        {
            var clip = camera.GetClippingRect();

            int ys = Math.Max(0, clip.Y / TileHeight - 1);
            int ye = Math.Min(Height, clip.Bottom / TileHeight + 1);
            int xs = Math.Max(0, clip.X / TileWidth - 1);
            int xe = Math.Min(Width, clip.Right / TileWidth + 1);

            foreach(var layer in tileLayers)
            {
                for(int y = ys; y < ye; y++)
                {
                    for(int x = xs; x < xe; x++)
                    {
                        layer.tiles[x + y * Width]?.Draw(x, y, batch);
                    }
                }
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

        public Tile GetTile(Vector2 position)
        {
            var xy = new Vector2(position.X / TileWidth, position.Y / TileHeight);
            if(xy.X < 0 || xy.Y < 0 || xy.X > Width || xy.Y > Height)
            {
                return null;
            }
            return combinedLayer.tiles[(int)xy.X + Width * (int)xy.Y];
        }

        public static Level LoadLevel(string resource)
        {
            var level = new Level();

            Task.Run(() =>
            {
                var map = new TmxMap(Path.Combine("Content/Levels", resource + ".tmx"));

                level.Width = map.Width;
                level.Height = map.Height;

                // Get tileset
                var tileset = map.Tilesets[0];

                level.TileHeight = tileset.TileHeight;
                level.TileWidth = tileset.TileWidth;

                var tilesetDict = new Dictionary<int, Tile>();
                tilesetDict.Add(0, null);                       // 0 means no tile

                var tilesetTexture = ResourceManager.GetManager().GetResource<Texture2D>(Path.Combine("Levels\\Tilesets", Path.GetFileNameWithoutExtension(tileset.Image.Source)));

                int rows = (int)tileset.TileCount / (int)tileset.Columns;
                bool done = false;
                for(int x = 0; x < tileset.Columns && !done; x++)
                {
                    for(int y = 0; y < rows; y++)
                    {
                        if(x + y * tileset.Columns > tileset.TileCount)
                        {
                            done = true;
                            break;
                        }

                        // Load tile
                        Tile tile = new Tile(tilesetTexture);
                        tile.textureRect.X = x * tileset.TileWidth;
                        tile.textureRect.Y = y * tileset.TileHeight;
                        tile.textureRect.Width = tileset.TileWidth;
                        tile.textureRect.Height = tileset.TileHeight;

                        
                        int index = x + y * (int)tileset.Columns;
                        tile.tilesetId = index;


                        // Load custom properties
                        TmxTilesetTile tileData;
                        if(tileset.Tiles.TryGetValue(index, out tileData))
                        {
                            Debug.WriteLine("Loading custom properties for tile " + index);
                            string solid;
                            if(tileData.Properties.TryGetValue("solid", out solid))
                            {
                                if(solid == "true")
                                {
                                    Debug.WriteLine("Tile " + index + " is SOLID");
                                    tile.Walkable = false;
                                }
                            }
                        }


                        tilesetDict.Add(tileset.FirstGid + index, tile);
                    }
                }

                // Get layers
                level.combinedLayer.tiles = new Tile[map.Width * map.Height];

                var reversedLayers = map.Layers.Reverse();          // Layers get stored bottom to top, we need top to bottom
                foreach(var layer in reversedLayers)
                {
                    Debug.WriteLine("Loading layer: " + layer.Name);

                    
                    // Add layer
                    var levelLayer = new LevelLayer();
                    levelLayer.tiles = new Tile[map.Width * map.Height];

                    for(int tileIndex = 0; tileIndex < map.Width * map.Height; tileIndex++)
                    {
                        levelLayer.tiles[tileIndex] = tilesetDict[layer.Tiles[tileIndex].Gid];

                        // Update top layer
                        if(level.combinedLayer.tiles[tileIndex] == null && levelLayer.tiles[tileIndex] != null)
                        {
                            level.combinedLayer.tiles[tileIndex] = levelLayer.tiles[tileIndex];
                        }
                    }

                    level.tileLayers.Add(levelLayer);
                }

                level.tileLayers.Reverse();             // Sort them bottom to top again


                // Load objects
                var groups = map.ObjectGroups;

                var guardPaths = new Dictionary<Guard, string>();
                var paths = new Dictionary<string, List<Vector2>>();

                foreach(var objectGroup in groups)
                {
                    foreach(var obj in objectGroup.Objects)
                    {
                        var halfTileOffset = new Vector2(level.TileWidth, level.TileHeight) / 2;



                        Debug.WriteLine("Found object of type " + obj.Type);
                        WorldEntity entity = null;
                        if(obj.Type == "spawnpoint")
                        {
                            entity = new SpawnPoint(new Vector2((float)obj.X, (float)obj.Y) + halfTileOffset);
                        }
                        else if(obj.Type == "guard")
                        {
                            entity = new Guard();
                            entity.Position = new Vector2((float)obj.X, (float)obj.Y) + halfTileOffset;

                            string path;
                            if(obj.Properties.TryGetValue("patrolpath", out path))
                            {
                                guardPaths.Add(entity as Guard, path);
                            }
                        }
                        else if(obj.Type == "path")
                        {
                            // A patrol path

                            if(!paths.ContainsKey(obj.Name))
                            {
                                paths[obj.Name] = new List<Vector2>();
                                foreach(var point in obj.Points)
                                {
                                    Debug.WriteLine(point.X + ", " + point.Y);
                                    paths[obj.Name].Add(new Vector2((float)point.X + (float)obj.X, (float)point.Y + (float)obj.Y));
                                }
                            }
                            else
                            {
                                Debug.WriteLine("Duplicate path name " + obj.Name);
                            }
                        }
                        //TODO: Add other objects we want to load here

                        if(entity != null)
                        {
                            // Set tag
                            string tag;
                            if(obj.Properties.TryGetValue("tag", out tag))
                            {
                                entity.Tag = tag;
                            }

                            level.AddEntity(entity);
                            Debug.WriteLine("Added entity");
                        }
                    }
                }


                // Assing guard patrol paths
                foreach(var guardPath in guardPaths)
                {
                    List<Vector2> path;
                    if(paths.TryGetValue(guardPath.Value, out path))
                    {
                        guardPath.Key.PatrolPath = path;
                    }
                    else
                    {
                        Debug.WriteLine("Guard requested unknown path " + guardPath.Value);
                    }
                }
                
            }).Wait();
            return level;

        }
    }

    public struct LevelLayer
    {
        public Tile[] tiles;
    }

    public struct LevelOverlay
    {
        public int layer;
        public Vector2 offset;
        public string textureName;
        public Texture2D texture;
    }
}
