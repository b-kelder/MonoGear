using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear.Network
{
    /// <summary>
    /// Stores data about a player/server
    /// </summary>
    public class NetworkPlayer
    {
        public byte Id { get; set; }
        public bool IsServer { get { return Id == 0; } }
        public string Name { get; set; }
    }
}
