using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MonoGear.Engine
{
    /// <summary>
    /// Represents a list of level names
    /// </summary>
    public class LevelListData
    {
        // Index of current level
        private int index;

        [XmlArray]
        [XmlArrayItem(ElementName = "Level")]
        public List<string> Levels { get; set; }

        /// <summary>
        /// Resets the index and returns the start level
        /// </summary>
        /// <returns>First level in the list</returns>
        public string Start()
        {
            index = 0;
            return Levels[0];
        }

        /// <summary>
        /// Updates index and returns the next level. Unchecked.
        /// </summary>
        /// <returns>Next level</returns>
        public string NextLevel()
        {
            index++;
            return Levels[index];
        }

        /// <summary>
        /// Returns the first level.
        /// </summary>
        /// <returns></returns>
        public string FirstLevel()
        {
            return Levels[0];
        }

        /// <summary>
        /// Returns the last level.
        /// </summary>
        /// <returns></returns>
        public string LastLevel()
        {
            return Levels[Levels.Count - 1];
        }
    }
}
