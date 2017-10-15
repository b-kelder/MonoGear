using MonoGear.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear.Network
{
    /// <summary>
    /// Forgiv me for I have sinned
    /// </summary>
    public static class EntityTypeDatabase
    {
        private static Dictionary<string, Type> ohgod = new Dictionary<string, Type>();

        public static void Build()
        {
            var types = typeof(WorldEntity).GetTypeInfo().Assembly.GetTypes().Where(x => x.IsAssignableFrom(typeof(WorldEntity)));
            foreach(var t in types)
            {
                ohgod.Add(t.AssemblyQualifiedName, t);
            }
        }

        public static WorldEntity Instantiate(string name)
        {
            Type t;
            if(ohgod.TryGetValue(name, out t))
            {
                return Activator.CreateInstance(t) as WorldEntity;
            }
            return null;
        }
    }
}
