using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;
using MonoGear.Engine.Collisions;
using MonoGear.Engine.Audio;
using MonoGear.Entities;
using MonoGear.Entities.Vehicles;

namespace MonoGear.Engine
{
    public class Level 
    {
        public string Name { get; private set; }
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
            if (layer.texture == null)
            {
                layer.texture = MonoGearGame.GetResource<Texture2D>(layer.textureName);
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
                layer.texture = MonoGearGame.GetResource<Texture2D>(layer.textureName);
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
            foreach (var layer in foregroundLayers)
            {
                batch.Draw(layer.texture, layer.offset, Color.White);
            }
        }

        public void DrawBackground(SpriteBatch batch)
        {
            foreach (var layer in backgroundLayers)
            {
                batch.Draw(layer.texture, layer.offset, Color.White);
            }
        }

        public void DrawTiles(SpriteBatch batch, Camera camera)
        {
            // TODO: Bake to image on level load and use that for rendering?
            var clip = camera.GetClippingRect();

            int ys = Math.Max(0, (int)clip.Y / TileHeight - 1);
            int ye = Math.Min(Height, (int)clip.Bottom / TileHeight + 1);
            int xs = Math.Max(0, (int)clip.X / TileWidth - 1);
            int xe = Math.Min(Width, (int)clip.Right / TileWidth + 1);

            foreach (var layer in tileLayers)
            {
                for (int y = ys; y < ye; y++)
                {
                    for (int x = xs; x < xe; x++)
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
            if (xy.X < 0 || xy.Y < 0 || xy.X > Width || xy.Y > Height)
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

                level.Name = resource;

                level.Width = map.Width;
                level.Height = map.Height;

                // Get tileset
                var tileset = map.Tilesets[0];

                level.TileHeight = tileset.TileHeight;
                level.TileWidth = tileset.TileWidth;

                var tilesetDict = new Dictionary<int, Tile>();
                tilesetDict.Add(0, null);                       // 0 means no tile

                var tilesetTexture = MonoGearGame.GetResource<Texture2D>(Path.Combine("Levels\\Tilesets", Path.GetFileNameWithoutExtension(tileset.Image.Source)));

                int rows = (int)tileset.TileCount / (int)tileset.Columns;
                bool done = false;
                for (int x = 0; x < tileset.Columns && !done; x++)
                {
                    for (int y = 0; y < rows; y++)
                    {
                        if (x + y * tileset.Columns > tileset.TileCount)
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
                        if (tileset.Tiles.TryGetValue(index, out tileData))
                        {
                            Debug.WriteLine("Loading custom properties for tile " + index);

                            string solid;
                            if(tileData.Properties.TryGetValue("solid", out solid))
                            {
                                if(solid == "true")
                                {
                                    tile.Walkable = false;
                                }
                                else
                                {
                                    Debug.WriteLine("Tile is not solid but has the property?");
                                }
                            }

                            string sound;
                            if(tileData.Properties.TryGetValue("sound", out sound))
                            {
                                Tile.TileSound soundEnum;
                                if(Enum.TryParse(sound, out soundEnum))
                                {
                                    tile.Sound = soundEnum;
                                }
                                else
                                {
                                    Debug.WriteLine("Unknown TileSound value " + sound);
                                }
                            }
                        }


                        tilesetDict.Add(tileset.FirstGid + index, tile);
                    }
                }

                // Get layers
                level.combinedLayer.tiles = new Tile[map.Width * map.Height];

                var reversedLayers = map.Layers.Reverse();          // Layers get stored bottom to top, we need top to bottom
                foreach (var layer in reversedLayers)
                {
                    Debug.WriteLine("Loading layer: " + layer.Name);


                    // Add layer
                    var levelLayer = new LevelLayer();
                    levelLayer.tiles = new Tile[map.Width * map.Height];

                    for (int tileIndex = 0; tileIndex < map.Width * map.Height; tileIndex++)
                    {
                        levelLayer.tiles[tileIndex] = tilesetDict[layer.Tiles[tileIndex].Gid];

                        // Update top layer if:
                        // level layer has a tile AND
                        // top layer is walkable but level is not OR
                        // top layer has no tile
                        if (levelLayer.tiles[tileIndex] != null &&
                        (level.combinedLayer.tiles[tileIndex] == null ||
                        (level.combinedLayer.tiles[tileIndex].Walkable && levelLayer.tiles[tileIndex].Walkable == false)))
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
                var carPaths = new Dictionary<Car, string>();
                var paths = new Dictionary<string, List<Vector2>>();
                var consoles = new Dictionary<string, PC>();
                var cameraConsole = new Dictionary<CCTV, string>();
                var objectives = new Dictionary<string, Objective>();       // Remains during gameplay due to some triggers needing it
                var pcWithObjective = new Dictionary<PC, string>();
                var driveObjective = new Dictionary<DrivableVehicle, string>();



                foreach (var objectGroup in groups)
                {
                    foreach (var obj in objectGroup.Objects)
                    {
                        var halfTileOffset = -new Vector2(-level.TileWidth, level.TileHeight) / 2;

                        //Debug.WriteLine("Found object of type " + obj.Type);
                        WorldEntity entity = null;
                        if (obj.Type == "spawnpoint")
                        {
                            entity = new SpawnPoint(new Vector2((float)obj.X, (float)obj.Y) + halfTileOffset);
                            entity.Rotation = MathHelper.ToRadians((float)obj.Rotation);
                        }
                        else if (obj.Type == "guard")
                        {
                            entity = new Guard();
                            entity.Position = new Vector2((float)obj.X, (float)obj.Y) + halfTileOffset;
                            entity.Rotation = MathHelper.ToRadians((float)obj.Rotation);

                            string path;
                            if (obj.Properties.TryGetValue("patrolpath", out path))
                            {
                                guardPaths.Add(entity as Guard, path);
                            }
                        }
                        else if (obj.Type == "car")
                        {
                            entity = new Car(new Vector2((float)obj.X, (float)obj.Y) + halfTileOffset, null, "Sprites/Car");
                            entity.Rotation = MathHelper.ToRadians((float)obj.Rotation);

                            string path;
                            if (obj.Properties.TryGetValue("path", out path))
                            {
                                carPaths.Add(entity as Car, path);
                            }
                        }
                        else if (obj.Type == "bird")
                        {
                            entity = new Bird() { YResetValue = level.Height * level.TileHeight + 200 };
                            entity.Position = new Vector2((float)obj.X, (float)obj.Y) + halfTileOffset;
                            entity.Rotation = MathHelper.ToRadians((float)obj.Rotation);
                        }
                        else if (obj.Type == "helicopter")
                        {
                            entity = new Helicopter();
                            entity.Position = new Vector2((float)obj.X, (float)obj.Y) + halfTileOffset;
                            entity.Rotation = MathHelper.ToRadians((float)obj.Rotation);

                            string objective;
                            if (obj.Properties.TryGetValue("objective", out objective))
                            {
                                driveObjective.Add(entity as DrivableVehicle, objective);
                            }
                        }
                        else if (obj.Type == "jeep")
                        {
                            entity = new Jeep();
                            entity.Position = new Vector2((float)obj.X, (float)obj.Y) + halfTileOffset;
                            entity.Rotation = MathHelper.ToRadians((float)obj.Rotation);

                            string objective;
                            if (obj.Properties.TryGetValue("objective", out objective))
                            {
                                driveObjective.Add(entity as DrivableVehicle, objective);
                            }
                            string autoEnter;
                            if (obj.Properties.TryGetValue("autoenter", out autoEnter))
                            {
                                (entity as Jeep).autoenter = true;
                            }
                            string creditmode;
                            if(obj.Properties.TryGetValue("creditmode", out creditmode))
                            {
                                (entity as Jeep).creditsMode = true;
                            }
                        }
                        else if(obj.Type == "tank")
                        {
                            entity = new Tank();
                            entity.Position = new Vector2((float)obj.X, (float)obj.Y) + halfTileOffset;
                            entity.Rotation = MathHelper.ToRadians((float)obj.Rotation);

                            string objective;
                            if (obj.Properties.TryGetValue("objective", out objective))
                            {
                                driveObjective.Add(entity as DrivableVehicle, objective);
                            }
                            string creditmode;
                            if(obj.Properties.TryGetValue("creditmode", out creditmode))
                            {
                                (entity as Tank).creditsMode = true;
                            }
                        }
                        else if (obj.Type == "objective")
                        {
                            string description;
                            string index;

                            if (obj.Properties.TryGetValue("description", out description))
                            {
                                if (obj.Properties.TryGetValue("index", out index))
                                {
                                    int ind = Int32.Parse(index);
                                    bool newIndex = true;

                                    foreach (var item in objectives)
                                    {
                                        if (item.Value.index == ind)
                                        {
                                            newIndex = false;
                                        }
                                    }
                                    if (newIndex)
                                    {
                                        entity = new Objective(description, ind);
                                        objectives.Add(obj.Name, entity as Objective);
                                    }
                                }
                                
                            }
                        }
                        else if (obj.Type == "cctv")
                        {
                            entity = new CCTV();
                            entity.Position = new Vector2((float)obj.X, (float)obj.Y) + halfTileOffset;
                            entity.Rotation = MathHelper.ToRadians((float)obj.Rotation);

                            string console;
                            if (obj.Properties.TryGetValue("pc", out console))
                            {
                                cameraConsole.Add(entity as CCTV, console);
                            }
                        }
                        else if (obj.Type == "pc")
                        {
                            entity = new PC();
                            entity.Position = new Vector2((float)obj.X, (float)obj.Y);
                            entity.Rotation = MathHelper.ToRadians((float)obj.Rotation);

                            string objective;
                            if (obj.Properties.TryGetValue("objective", out objective))
                            {
                                pcWithObjective.Add(entity as PC, objective);
                            }

                            if (!consoles.ContainsKey(obj.Name))
                            {
                                consoles[obj.Name] = entity as PC;
                            }
                            else
                            {
                                Debug.WriteLine("Duplicate PC name " + obj.Name);
                            }
                        }
                        else if (obj.Type == "audio")
                        {
                            // Global audio
                            string audio;
                            string loop;
                            string volume;
                            bool willWork = true;

                            if (!obj.Properties.TryGetValue("source", out audio))
                                willWork = false;
                            if (!obj.Properties.TryGetValue("loop", out loop))
                                loop = "true";
                            if (!obj.Properties.TryGetValue("volume", out volume))
                                volume = "1";

                            if (willWork)
                            {
                                var soundEffect = MonoGearGame.GetResource<SoundEffect>(audio).CreateInstance();
                                soundEffect.IsLooped = (loop == "true");
                                soundEffect.Volume = float.Parse(volume) * SettingsPage.Volume * SettingsPage.EffectVolume;
                                AudioManager.PlayGlobal(soundEffect);
                            }
                        }
                        else if (obj.Type == "audiosource")
                        {
                            // Positional audio
                            string audio;
                            string range;
                            string volume;

                            if(obj.Properties.TryGetValue("source", out audio))
                            {
                                if(!obj.Properties.TryGetValue("range", out range))
                                    range = "100";
                                if(!obj.Properties.TryGetValue("volume", out volume))
                                    volume = "1";
                                AudioManager.AddPositionalAudio(MonoGearGame.GetResource<SoundEffect>(audio), float.Parse(volume), float.Parse(range), new Vector2((float)obj.X, (float)obj.Y), true);
                            }
                        }
                        else if (obj.Type == "backgroundmusic")
                        {
                            // Global audio via media player
                            string audio;

                            if (obj.Properties.TryGetValue("source", out audio))
                            { 
                                MediaPlayer.Play(MonoGearGame.GetResource<Song>(audio));
                                MediaPlayer.Volume = 1 * SettingsPage.Volume * SettingsPage.EffectVolume;
                                Debug.WriteLine("Added music");
                            }
                        }
                        else if (obj.Type == "trigger")
                        {
                            string action;
                            if (obj.Properties.TryGetValue("action", out action))
                            {
                                Action<Collider, IEnumerable<Collider>, IEnumerable<Collider>> actionL = null;
                                if (action == "nextlevel")
                                {
                                    actionL = (self, previous, current) =>
                                    {
                                        foreach (var col in current)
                                        {
                                            var vehicle = col.Entity as DrivableVehicle;
                                            if (col.Entity.Tag == "Player" || (vehicle != null && vehicle.Entered))
                                            {
                                                if(vehicle != null && vehicle.Entered)
                                                {
                                                    vehicle.Exit();
                                                }
                                                MonoGearGame.NextLevel();
                                            }
                                            
                                        }
                                    };
                                }
                                else if (action == "alert")
                                {
                                    actionL = (self, previous, current) =>
                                    {
                                        foreach (var col in current)
                                        {
                                            var vehicle = col.Entity as DrivableVehicle;
                                            if((col.Entity.Tag == "Player" || (vehicle != null && vehicle.Entered)) && !previous.Contains(col))
                                            {
                                                var guards = MonoGearGame.FindEntitiesOfType<Guard>();
                                                foreach (var guard in guards)
                                                {
                                                    guard.Alert(col.Entity.Position);
                                                }
                                            }
                                        }
                                    };
                                }
                                else if (action == "objective")
                                {
                                    string objective;
                                    if (obj.Properties.TryGetValue("objective", out objective))
                                    {
                                        actionL = (self, previous, current) =>
                                        {
                                            foreach (var col in current)
                                            {
                                                var vehicle = col.Entity as DrivableVehicle;
                                                if(col.Entity.Tag == "Player" || (vehicle != null && vehicle.Entered))
                                                {
                                                    Objective ob;
                                                    if (objectives.TryGetValue(objective, out ob))
                                                    {
                                                        GameUI.CompleteObjective(ob);
                                                    }
                                                    else
                                                    {
                                                        Debug.WriteLine("Trgger could not find objective: " + objectives);
                                                    }
                                                }
                                            }
                                        };
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("Trigger " + obj.Name + " with unknown action " + action);
                                }

                                if (actionL != null)
                                {
                                    var size = new Vector2((float)obj.Width, (float)obj.Height);
                                    entity = new WorldBoxTrigger(new Vector2((float)obj.X, (float)obj.Y) + size / 2,
                                        size,
                                        actionL);
                                }
                            }
                            else
                            {
                                Debug.WriteLine("Trigger " + obj.Name + " with no action!");
                            }
                        }
                        else if (obj.Type == "path")
                        {
                            // A patrol path
                            if (!paths.ContainsKey(obj.Name))
                            {
                                paths[obj.Name] = new List<Vector2>();
                                foreach (var point in obj.Points)
                                {
                                    //Debug.WriteLine(point.X + ", " + point.Y);
                                    paths[obj.Name].Add(new Vector2((float)point.X + (float)obj.X, (float)point.Y + (float)obj.Y));
                                }
                            }
                            else
                            {
                                Debug.WriteLine("Duplicate path name " + obj.Name);
                            }
                        }

                        if (entity != null)
                        {
                            // Set tag
                            string tag;
                            if (obj.Properties.TryGetValue("tag", out tag))
                            {
                                entity.Tag = tag;
                            }

                            level.AddEntity(entity);
                            //Debug.WriteLine("Added entity" + entity.Tag);
                        }
                    }
                }

                // Assing guard patrol paths
                foreach (var guardPath in guardPaths)
                {
                    List<Vector2> path;
                    if (paths.TryGetValue(guardPath.Value, out path))
                    {
                        guardPath.Key.PatrolPath = path;
                    }
                    else
                    {
                        Debug.WriteLine("Guard requested unknown path " + guardPath.Value);
                    }
                }

                // Assing car paths
                foreach (var carPath in carPaths)
                {
                    List<Vector2> path;
                    if (paths.TryGetValue(carPath.Value, out path))
                    {
                        carPath.Key.SetPath(path);
                    }
                    else
                    {
                        Debug.WriteLine("Car requested unknown path " + carPath.Value);
                    }
                }

                // Assing PC/CCTV
                foreach (var kvPair in cameraConsole)
                {
                    PC pc;
                    if (consoles.TryGetValue(kvPair.Value, out pc))
                    {
                        pc.AddCCTV(kvPair.Key);
                    }
                    else
                    {
                        Debug.WriteLine("CCTV requested unknown PC " + kvPair.Value);
                    }
                }

                // Assing PC/Objective
                foreach (var pc in pcWithObjective)
                {
                    Objective ob;
                    if (objectives.TryGetValue(pc.Value, out ob))
                    {
                        pc.Key.objective = ob;
                    }
                    else
                    {
                        Debug.WriteLine("PC could not find objective: " + pc.Value);
                    }
                }

                // Assing drive/Objective
                foreach (var drive in driveObjective)
                {
                    Objective ob;
                    if (objectives.TryGetValue(drive.Value, out ob))
                    {
                        drive.Key.objective = ob;
                    }
                    else
                    {
                        Debug.WriteLine("PC could not find objective: " + drive.Value);
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
