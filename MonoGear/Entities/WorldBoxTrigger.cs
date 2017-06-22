using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using MonoGear.Engine;
using MonoGear.Engine.Collisions;

namespace MonoGear.Entities
{
    public class WorldBoxTrigger : WorldEntity
    {
        HashSet<Collider> previousColliders;
        Action<Collider, IEnumerable<Collider>, IEnumerable<Collider>> onTrigger;

        /// <summary>
        /// Constructor of the world box trigger class.
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="size">The size</param>
        /// <param name="onTrigger">The action on trigger</param>
        public WorldBoxTrigger(Vector2 position, Vector2 size, Action<Collider, IEnumerable<Collider>, IEnumerable<Collider>> onTrigger)
        {
            if(onTrigger == null)
            {
                throw new ArgumentNullException("onTrigger");
            }

            previousColliders = new HashSet<Collider>();
            Position = position;
            Size = size;
            Collider = new BoxCollider(this, size);
            Collider.Trigger = true;
            this.onTrigger = onTrigger;
        }

        /// <summary>
        /// Method that updates the game
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            // Check for colliders
            var colliders = Collider.BoxOverlapAny(Collider);
            if(colliders.Count() > 0)
            {
                onTrigger(Collider, previousColliders, colliders);
            }
            // Update prev colliders set
            previousColliders.Clear();
            foreach(var collider in colliders)
            {
                previousColliders.Add(collider);
            }
        }
    }
}
