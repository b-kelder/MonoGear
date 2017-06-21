using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear.Engine
{
    /// <summary>
    /// Input handling class
    /// </summary>
    public class Input
    {
        /// <summary>
        /// Buttons used in the game
        /// </summary>
        public enum Button
        {
            Up,
            Down,
            Left,
            Right,
            Throw,
            Shoot,
            Restart,
            Interact,
            Sneak,
        }

        // Current and previous frame's keyboard and gamepad states
        KeyboardState currentState;
        KeyboardState previousState;
        GamePadState currentPadState;
        GamePadState previousPadState;

        // Key mappings
        Dictionary<Button, Tuple<Keys, Keys>> buttonsToKeys;
        Dictionary<Button, Buttons> buttonsToPad;

        public Input()
        {
            // Create key mappings
            buttonsToKeys = new Dictionary<Button, Tuple<Keys, Keys>> {
                { Button.Left,      new Tuple<Keys, Keys>(Keys.Left,        Keys.A) },
                { Button.Right,     new Tuple<Keys, Keys>(Keys.Right,       Keys.D) },
                { Button.Up,        new Tuple<Keys, Keys>(Keys.Up,          Keys.W) },
                { Button.Down,      new Tuple<Keys, Keys>(Keys.Down,        Keys.S) },

                { Button.Throw,     new Tuple<Keys, Keys>(Keys.Z,           Keys.J) },
                { Button.Shoot,     new Tuple<Keys, Keys>(Keys.X,           Keys.K) },
                { Button.Restart,   new Tuple<Keys, Keys>(Keys.G,           Keys.G) },
                { Button.Interact,  new Tuple<Keys, Keys>(Keys.C,           Keys.L) },
                { Button.Sneak,     new Tuple<Keys, Keys>(Keys.LeftShift,   Keys.RightShift) },
            };

            buttonsToPad = new Dictionary<Button, Buttons> {
                { Button.Left,      Buttons.DPadLeft},
                { Button.Right,     Buttons.DPadRight},
                { Button.Up,        Buttons.RightTrigger},
                { Button.Down,      Buttons.LeftTrigger},

                { Button.Throw,     Buttons.Y},
                { Button.Shoot,     Buttons.X},
                { Button.Restart,   Buttons.Back},
                { Button.Interact,  Buttons.A},
                { Button.Sneak,     Buttons.B},
            };
        }

        public void Update()
        {
            // Get new states
            previousState = currentState;
            currentState = Keyboard.GetState();
            previousPadState = currentPadState;
            currentPadState = GamePad.GetState(PlayerIndex.One);
        }

        /// <summary>
        /// Returns the current gamepad state.
        /// </summary>
        /// <returns>GamePadState</returns>
        public GamePadState GetGamepadState()
        {
            return currentPadState;
        }

        /// <summary>
        /// Returns true if the button was pressed this frame.
        /// </summary>
        /// <param name="button">The button</param>
        /// <returns>True or false</returns>
        public bool IsButtonPressed(Button button)
        {
            if(currentPadState.IsConnected)
            {
                var buttons = buttonsToPad[button];
                return currentPadState.IsButtonDown(buttons) && previousPadState.IsButtonUp(buttons);
            }
            else
            {
                var keys = buttonsToKeys[button];
                return IsKeyPressed(keys.Item1) || IsKeyPressed(keys.Item2);
            }
        }

        /// <summary>
        /// Returns true if the button is down this frame.
        /// </summary>
        /// <param name="button">The button</param>
        /// <returns>True or false</returns>
        public bool IsButtonDown(Button button)
        {
            if(currentPadState.IsConnected)
            {
                var buttons = buttonsToPad[button];
                return currentPadState.IsButtonDown(buttons);
            }
            else
            {
                var keys = buttonsToKeys[button];
                return IsKeyDown(keys.Item1) || IsKeyDown(keys.Item2);
            }
        }

        /// <summary>
        /// Returns true if the button was released this frame.
        /// </summary>
        /// <param name="button">The button</param>
        /// <returns>True or false</returns>
        public bool IsButtonReleased(Button button)
        {
            if(currentPadState.IsConnected)
            {
                var buttons = buttonsToPad[button];
                return currentPadState.IsButtonUp(buttons) && previousPadState.IsButtonDown(buttons);
            }
            else
            {
                var keys = buttonsToKeys[button];
                return IsKeyReleased(keys.Item1) || IsKeyReleased(keys.Item2);
            }
        }

        /// <summary>
        /// Returns true if the button is up this frame.
        /// </summary>
        /// <param name="button">The button</param>
        /// <returns>True or false</returns>
        public bool IsButtonUp(Button button)
        {
            if(currentPadState.IsConnected)
            {
                var buttons = buttonsToPad[button];
                return currentPadState.IsButtonUp(buttons);
            }
            else
            {
                var keys = buttonsToKeys[button];
                return IsKeyUp(keys.Item1) && IsKeyUp(keys.Item2);
            }
        }

        /// <summary>
        /// Returns true if a gamepad is connected.
        /// </summary>
        /// <returns>True or false</returns>
        public bool PadConnected()
        {
            return currentPadState.IsConnected;
        }

        /// <summary>
        /// Returns true if the key is down this frame.
        /// </summary>
        /// <param name="key">XNA key</param>
        /// <returns>True or false</returns>
        public bool IsKeyDown(Keys key)
        {
            return currentState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns true if the key is up this frame.
        /// </summary>
        /// <param name="key">XNA key</param>
        /// <returns>True or false</returns>
        public bool IsKeyUp(Keys key)
        {
            return currentState.IsKeyUp(key);
        }

        /// <summary>
        /// Returns true if the key is pressed this frame.
        /// </summary>
        /// <param name="key">XNA key</param>
        /// <returns>True or false</returns>
        public bool IsKeyPressed(Keys key)
        {
            return previousState.IsKeyUp(key) && currentState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns true if the key is released this frame.
        /// </summary>
        /// <param name="key">XNA key</param>
        /// <returns>True or false</returns>
        public bool IsKeyReleased(Keys key)
        {
            return previousState.IsKeyDown(key) && currentState.IsKeyUp(key);
        }
    }
}
