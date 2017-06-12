using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGear.Engine;

namespace MonoGear.Entities
{
    public class Objective : WorldEntity
    {
        public int index { get; set; }

        string description;

        public Objective(string description, int index)
        {
            this.description = description;
            this.index = index;
        }

        public override string ToString()
        {
            return description;
        }
    }
}
