﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    class ResourceManager
    {
        private static Dictionary<string, ResourceManager> managers = new Dictionary<string, ResourceManager>();

        //Hardcode for now
        private readonly Dictionary<Type, string[]> resources = new Dictionary<Type, string[]>
        {
            {
                typeof(Texture2D),
                new string[]
                {
                    "Sprites/s_generator",
                }
            },
        };
        
        private Dictionary<string, object> loadedResources;

        public ResourceManager(string name)
        {
            if(managers.ContainsKey(name))
            {
                throw new ArgumentException("Dupliacte manager name " + name);
            }

            managers.Add(name, this);
            loadedResources = new Dictionary<string, object>();
        }

        public void LoadResources(ContentManager content)
        {
            foreach(var type in resources)
            {
                if(type.Key == typeof(Texture2D))
                {
                    foreach(var asset in type.Value)
                    {
                        RegisterLoadedResource(asset, content.Load<Texture2D>(asset));
                    }
                }
            }
        }

        private void RegisterLoadedResource(string name, object resource)
        {
            if(loadedResources.ContainsKey(name))
            {
                throw new ArgumentException("Duplicate resource name " + name);
            }

            loadedResources.Add(name, resource);
        }

        public T GetResource<T>(string name)
        {
            object result = default(T);
            if(loadedResources.TryGetValue(name, out result))
            {
                return (T)result;
            }
            throw new KeyNotFoundException("Unknow resource " + name);
        }

        public static ResourceManager GetManager(string name)
        {
            ResourceManager manager;
            managers.TryGetValue(name, out manager);
            return manager;
        }
    }
}