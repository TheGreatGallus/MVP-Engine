using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVP_Editor.Util
{
    public static class MouseUtil
    {
        public static bool wasClicked(this ButtonState newState, ButtonState oldState)
        {
            return newState == ButtonState.Pressed && oldState == ButtonState.Released;
        }
        public static bool wasHeld(this ButtonState newState, ButtonState oldState)
        {
            return newState == ButtonState.Pressed && oldState == ButtonState.Pressed;
        }
        public static bool wasClickedOrHeld(this ButtonState newState, ButtonState oldState)
        {
            return newState.wasClicked(oldState) || newState.wasHeld(oldState);
        }
        public static bool wasReleased(this ButtonState newState, ButtonState oldState)
        {
            return newState == ButtonState.Released && oldState != ButtonState.Released;
        }
        public static bool isFocused(this MouseState newState)
        {
            return newState.LeftButton == ButtonState.Pressed || newState.MiddleButton == ButtonState.Pressed
                || newState.RightButton == ButtonState.Pressed;
        }
        public static bool wasUnfocused(this MouseState newState)
        {
            return newState.LeftButton == ButtonState.Released && newState.MiddleButton == ButtonState.Released
                && newState.RightButton == ButtonState.Released;
        }
    }
}
