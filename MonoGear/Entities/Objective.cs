using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGear.Engine;

namespace MonoGear.Entities
{
    class Objective : WorldEntity
    {
        string Description;

        public Objective(string description)
        {
            Description = description;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
