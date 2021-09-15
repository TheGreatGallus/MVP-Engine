using Microsoft.Xna.Framework.Input;

namespace MVP_Editor.Util
{
    public static class KeyboardUtil
    {
        public static bool wasPressed(this KeyboardState newState, KeyboardState oldState, Keys key)
        {
            return newState.IsKeyDown(key) && oldState.IsKeyUp(key);
        }
        public static bool wasHeld(this KeyboardState newState, KeyboardState oldState, Keys key)
        {
            return newState.IsKeyDown(key) && oldState.IsKeyDown(key);
        }
        public static bool wasPressedOrHeld(this KeyboardState newState, KeyboardState oldState, Keys key)
        {
            return newState.wasPressed(oldState, key) && newState.wasPressed(oldState, key);
        }
        public static bool wasReleased(this KeyboardState newState, KeyboardState oldState, Keys key)
        {
            return newState.IsKeyUp(key) && oldState.IsKeyDown(key);
        }
        public static bool isShortcutPressed(this KeyboardState newState, Keys key, Keys modifier)
        {
            return newState.IsKeyDown(key) && newState.IsKeyDown(modifier);
        }
    }
}
