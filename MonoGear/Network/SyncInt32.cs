using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear.Network
{
    public class SyncInt32 : SyncValue<int>
    {
        public bool HasChanged
        {
            get; set;
        }

        private int value;

        public SyncInt32()
        {
            value = 0;
        }

        public SyncInt32(int value)
        {
            HasChanged = true;
            SetValue(value);
        }

        public byte[] GetBytes()
        {
            return BitConverter.GetBytes(value);
        }

        public int GetValue()
        {
            return value;
        }

        public void SetBytes(byte[] bytes)
        {
            //TODO: Actually check?
            HasChanged = true;
            value = BitConverter.ToInt32(bytes, 0);
        }

        public void SetValue(int value)
        {
            if(this.value != value)
            {
                HasChanged = true;
                this.value = value;
            }
        }
    }
}
