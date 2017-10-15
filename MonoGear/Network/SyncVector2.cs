using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear.Network
{
    public class SyncVector2 : SyncValue<Vector2>
    {
        private Vector2 value;
        private List<byte> bytes = new List<byte>();

        public bool HasChanged { get; set; }

        public SyncVector2()
        {
            value = Vector2.Zero;
        }

        public SyncVector2(Vector2 value)
        {
            HasChanged = true;
            SetValue(value);
        }

        public byte[] GetBytes()
        {
            bytes.Clear();
            bytes.AddRange(BitConverter.GetBytes(value.X));
            bytes.AddRange(BitConverter.GetBytes(value.Y));
            return bytes.ToArray();
        }

        public Vector2 GetValue()
        {
            return this.value;
        }

        public void SetBytes(byte[] bytes)
        {
            if(bytes.Length >= 8)
            {
                HasChanged = true;
                value = new Vector2(BitConverter.ToSingle(bytes, 0), BitConverter.ToSingle(bytes, 4));
            }
        }

        public void SetValue(Vector2 value)
        {
            if(this.value != value)
            {
                HasChanged = true;
                this.value = value;
            }
        }
    }
}
