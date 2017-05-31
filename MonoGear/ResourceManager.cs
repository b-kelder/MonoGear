using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace MonoGear
{
    class ResourceManager
    {
        private static ResourceManager _instance;


        // Hardcode for now, might want to load this via some sort of configuration
        // e.g. when we want to use more ResourceManagers because they will all load the same stuff right now.
        private readonly Dictionary<Type, string[]> resources = new Dictionary<Type, string[]>
        {
            {
                typeof(Texture2D),
                new string[]
                {
                    "Sprites/s_generator",
                    "Sprites/WhiteHouse",
                    "Sprites/WhiteHouseShadows",
                    "Sprites/birdsheet",
                    "Sprites/Rock",
                    "Sprites/Guard",
                    "Sprites/Person",
                    "Sprites/Car",
                    "Sprites/Alert",
                    "Sprites/Searching",
                    "Sprites/Taxi",
                    "Sprites/Bullet",
                }
            },
            {
                typeof(SoundEffect),
                new string[]
                {
                    "Audio/AudioFX/Water_Fountain_cut",
                    "Audio/AudioFX/Running On Grass",
                    "Audio/AudioFX/Guard_Alert_Sound",
                    "Audio/AudioFX/Crickets_sound",
                    "Audio/AudioFX/Owl_sound",
                    "Audio/AudioFX/Water_Drop_Sound",
                    "Audio/AudioFX/Concrete",
                    "Audio/AudioFX/Car_sound",
                    "Audio/AudioFX/Deja Vu",
                    "Audio/AudioFX/StoneTrow_sound",
                    "Audio/AudioFX/Gunshot",
                    "Audio/AudioFX/Wasted_Sound",

                }
            },
            {
                typeof(Song),
                new string[]
                {
                    "Audio/Music/Epic music for an epic moment",
                    "Audio/Music/Main menu theme",
                }
            },
        };

        private Dictionary<string, object> loadedResources;

        public ResourceManager()
        {
            _instance = this;
            loadedResources = new Dictionary<string, object>();
        }

        public void LoadResources(ContentManager content)
        {
            foreach (var type in resources)
            {
                if (type.Key == typeof(Texture2D))
                {
                    foreach (var asset in type.Value)
                    {
                        RegisterLoadedResource(asset, content.Load<Texture2D>(asset));
                    }
                }
                else if (type.Key == typeof(Song))
                {
                    foreach (var asset in type.Value)
                    {
                        RegisterLoadedResource(asset, content.Load<Song>(asset));
                    }
                }
                else if (type.Key == typeof(SoundEffect))
                {
                    foreach (var asset in type.Value)
                    {
                        RegisterLoadedResource(asset, content.Load<SoundEffect>(asset));
                    }
                }
            }
        }

        private void RegisterLoadedResource(string name, object resource)
        {
            if (loadedResources.ContainsKey(name))
            {
                throw new ArgumentException("Duplicate resource name " + name);
            }

            loadedResources.Add(name, resource);
        }

        public T GetResource<T>(string name)
        {
            object result = default(T);
            if (loadedResources.TryGetValue(name, out result))
            {
                return (T)result;
            }
            throw new KeyNotFoundException("Unknow resource " + name);
        }

        public static ResourceManager GetManager()
        {
            return _instance;
        }
    }
}
