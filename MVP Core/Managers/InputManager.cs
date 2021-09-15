using Microsoft.Xna.Framework.Input;
using MVP_Core.Components;
using MVP_Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MVP_Core.Managers
{
    public class InputManager
    {
        private static bool isInitialized = false;
        private static List<Keys> oldPressedKeys;
        private static Dictionary<Keys, InputActionEvent> actions = new Dictionary<Keys, InputActionEvent>();
        
        public static void Initialize(XElement parameters)
        {
            actions = new Dictionary<Keys, InputActionEvent>();
            //foreach(XElement key in parameters.Descendants())
            //{
            //    Keys XNAKey;
            //    if (Enum.TryParse(key.Name.LocalName, out XNAKey))
            //    {
            //        actions.Add(XNAKey, new InputActionEvent(key.Value));
            //    }
            //}

            oldPressedKeys = Keyboard.GetState().GetPressedKeys().ToList();
            isInitialized = true;
        }

        public static void PollStateChanges()
        {
            List<Keys> newPressedKeys = Keyboard.GetState().GetPressedKeys().ToList();
            foreach (Keys key in oldPressedKeys.Except(newPressedKeys))
            {
                ProcessKeyRelease(key);
            }
            IEnumerable<Keys> keysToProcess = newPressedKeys.Except(oldPressedKeys);
            foreach (Keys key in keysToProcess)
            {
                ProcessKeyPress(key);
            }
            oldPressedKeys = newPressedKeys;
        }

        public static void RegisterInput(Keys key, InputActionEvent action)
        {
            if (!actions.ContainsKey(key))
                actions.Add(key, action);
        }

        private static void ProcessKeyPress(Keys key)
        {
            Console.WriteLine("Processing" + Guid.NewGuid());
            if (actions.ContainsKey(key))
                actions[key].SetState(InputActionEvent.States.START);
        }

        private static void ProcessKeyRelease(Keys key)
        {
            if (actions.ContainsKey(key))
                actions[key].SetState(InputActionEvent.States.STOP);
        }

        public static void RunKeyActions(UpdateComponent controlledComponent)
        {
            foreach (var pair in actions)
            {
                if (pair.Value.state != InputActionEvent.States.NULL && pair.Value.state != InputActionEvent.States.HELD)
                {
                    controlledComponent.Invoke(pair.Value.action.ToString() + pair.Value.state.ToString());
                    if (pair.Value.state == InputActionEvent.States.START)
                        pair.Value.state = InputActionEvent.States.HELD;
                    if (pair.Value.state == InputActionEvent.States.STOP)
                        pair.Value.state = InputActionEvent.States.NULL;
                }
            }
        }

        public static string ActionState(Keys key)
        {
            return actions[key].action + ": " + actions[key].state;
        }
    }
}
