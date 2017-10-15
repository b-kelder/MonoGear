using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear.Network
{
    public class SyncFloat : SyncValue<float>
    {
        private float value;

        public bool HasChanged { get; set; }

        public SyncFloat()
        {
            value = 0;
        }

        public SyncFloat(float value)
        {
            HasChanged = true;
            SetValue(value);
        }

        public byte[] GetBytes()
        {
            return BitConverter.GetBytes(value);
        }

        public float GetValue()
        {
            return value;
        }

        public void SetBytes(byte[] bytes)
        {
            //TODO: Actually check?
            HasChanged = true;
            value = BitConverter.ToSingle(bytes, 0);
        }

        public void SetValue(float value)
        {
            if(this.value != value)
            {
                HasChanged = true;
                this.value = value;
            }
        }
    }
}
