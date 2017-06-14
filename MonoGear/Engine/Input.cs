using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear.Engine
{
    public class Input
    {
        public enum Button
        {
            Up,
            Down,
            Left,
            Right,
            Throw,
            Shoot,
            Restart,
            Interact
        }

        KeyboardState currentState;
        KeyboardState previousState;

        Dictionary<Button, Tuple<Keys, Keys>> buttonsToKeys;

        public Input()
        {
            buttonsToKeys = new Dictionary<Button, Tuple<Keys, Keys>> {
                { Button.Left,      new Tuple<Keys, Keys>(Keys.Left,    Keys.A) },
                { Button.Right,     new Tuple<Keys, Keys>(Keys.Right,   Keys.D) },
                { Button.Up,        new Tuple<Keys, Keys>(Keys.Up,      Keys.W) },
                { Button.Down,      new Tuple<Keys, Keys>(Keys.Down,    Keys.S) },

                { Button.Throw,     new Tuple<Keys, Keys>(Keys.Z,       Keys.J) },
                { Button.Shoot,     new Tuple<Keys, Keys>(Keys.X,       Keys.K) },
                { Button.Restart,   new Tuple<Keys, Keys>(Keys.G,    Keys.G) },
                { Button.Interact,  new Tuple<Keys, Keys>(Keys.C,    Keys.L) },
            };
        }

        public void Update()
        {
            previousState = currentState;
            currentState = Keyboard.GetState();
        }

        public bool IsButtonPressed(Button button)
        {
            var keys = buttonsToKeys[button];
            return IsKeyPressed(keys.Item1) || IsKeyPressed(keys.Item2);
        }

        public bool IsButtonDown(Button button)
        {
            var keys = buttonsToKeys[button];
            return IsKeyDown(keys.Item1) || IsKeyDown(keys.Item2);
        }

        public bool IsButtonReleased(Button button)
        {
            var keys = buttonsToKeys[button];
            return IsKeyReleased(keys.Item1) || IsKeyReleased(keys.Item2);
        }

        public bool IsButtonUp(Button button)
        {
            var keys = buttonsToKeys[button];
            return IsKeyUp(keys.Item1) && IsKeyUp(keys.Item2);
        }

        public bool IsKeyDown(Keys key)
        {
            return currentState.IsKeyDown(key);
        }

        public bool IsKeyUp(Keys key)
        {
            return currentState.IsKeyUp(key);
        }

        public bool IsKeyPressed(Keys key)
        {
            return previousState.IsKeyUp(key) && currentState.IsKeyDown(key);
        }

        public bool IsKeyReleased(Keys key)
        {
            return previousState.IsKeyDown(key) && currentState.IsKeyUp(key);
        }
    }
}
