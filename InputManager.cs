using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TileMap
{
    internal static class InputManager
    {   
        private static KeyboardState _keyboardState;
        private static KeyboardState _previousKeyboardState;
        
        private static MouseState _mouseState;
        private static MouseState _previousMouseState;

        public static Vector2 screenScaleFactor;

        public static readonly Keys LeftKey = Keys.A;
        public static readonly Keys RightKey = Keys.D;
        public static readonly Keys UpKey = Keys.W;
        public static readonly Keys DownKey = Keys.S;
        public static readonly Keys JumpKey = Keys.Space;
        public static readonly Keys PlayerDebugKey = Keys.LeftShift;

        public enum MouseButton
        {
            Left,
            Right,
            Middle
        }

        // KEYBOARD
        public static bool IsKeyPressed(Keys key)
        {
            return _keyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }
        public static bool IsKeyHeld(Keys key)
        {
            return _keyboardState.IsKeyDown(key);
        }
        public static bool IsKeyReleased(Keys key)
        {
            return !_keyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyDown(key);
        }
        public static int GetInputAxis(Keys key1, Keys key2) // Basically same as Godot
        {   
            if (_keyboardState.IsKeyDown(key1) && !_keyboardState.IsKeyDown(key2))
            {
                return -1;
            }
            else if (_keyboardState.IsKeyDown(key2) && !_keyboardState.IsKeyDown(key1))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        // KEYBOARD
        
        // MOUSE
        public static Vector2 GetMousePosition()
        {
            return new Vector2(_mouseState.X / screenScaleFactor.X, _mouseState.Y / screenScaleFactor.Y);
        }
        public static bool IsMouseButtonPressed(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return _mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released;
                case MouseButton.Middle:
                    return _mouseState.MiddleButton == ButtonState.Pressed && _previousMouseState.MiddleButton == ButtonState.Released;
                case MouseButton.Right:
                    return _mouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released;
                default:
                    return false;
            }
        }
        public static bool IsMouseButtonHeld(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return _mouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return _mouseState.MiddleButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return _mouseState.RightButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }
        public static bool IsMouseButtonReleased(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return _mouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return _mouseState.MiddleButton == ButtonState.Released && _previousMouseState.MiddleButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return _mouseState.RightButton == ButtonState.Released && _previousMouseState.RightButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }
        // MOUSE

        public static void Update()
        {
            _previousKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();

            _previousMouseState = _mouseState;
            _mouseState = Mouse.GetState();
        }
    }
}
