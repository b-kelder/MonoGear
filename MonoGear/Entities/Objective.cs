using MonoGear.Engine;

namespace MonoGear.Entities
{
    public class Objective : WorldEntity
    {
        public int Index { get; set; }
        // Objective's description
        string description;

        /// <summary>
        /// Constructor of the objective class.
        /// </summary>
        /// <param name="description">Objective's description</param>
        /// <param name="index">Objective's index number</param>
        public Objective(string description, int index)
        {
            this.description = description;
            Index = index;
        }
        
        /// <summary>
        /// Method that gets the objective's description
        /// </summary>
        /// <returns>The objective's description</returns>
        public override string ToString()
        {
            return description;
        }
    }
}
