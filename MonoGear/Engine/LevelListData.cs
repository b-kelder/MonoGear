using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MonoGear.Engine
{
    public class LevelListData
    {
        private int index;

        [XmlArray]
        [XmlArrayItem(ElementName = "Level")]
        public List<string> Levels { get; set; }

        public string Start()
        {
            index = 0;
            return Levels[0];
        }

        public string NextLevel()
        {
            index++;
            return Levels[index];
        }

        public string FirstLevel()
        {
            return Levels[0];
        }

        public string LastLevel()
        {
            return Levels[Levels.Count - 1];
        }

        public void Reset()
        {
            index = 0;
        }
    }
}
