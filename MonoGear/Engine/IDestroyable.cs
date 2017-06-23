using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear.Engine
{
    interface IDestroyable
    {
        void Damage(float damage);

        void Destroy();
    }
}
