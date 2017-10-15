using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear.Network
{
    public interface SyncValue
    {
        bool HasChanged { get; set; }
        byte[] GetBytes();
        void SetBytes(byte[] bytes);
    }

    public interface SyncValue<T> : SyncValue
    {
        T GetValue();
        void SetValue(T value);
    }
}
