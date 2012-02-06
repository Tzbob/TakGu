using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Globalization;
using System.Diagnostics;

namespace TakGu
{

    public class InputUtility
    {
        static public bool IsKeyToggled(Keys key, KeyboardState oldState, KeyboardState newState)
        {
            return newState.IsKeyUp(key) && oldState.IsKeyDown(key);
        }

        static public void ExecuteOnToggle(Keys key, KeyboardState oldState, KeyboardState newState, Utility.Delegate_Paramless execute)
        {
            if(IsKeyToggled(key, oldState, newState))
                execute();
        }

        static public void ExecuteOnToggle(List<Keys> keys, KeyboardState oldState, KeyboardState newState, Utility.Delegate_Paramless execute)
        {
            bool isOneToggled = false;

            foreach (Keys key in keys)
                isOneToggled |= IsKeyToggled(key, oldState, newState) && !IsModifierUsed(newState);

            if(isOneToggled)
                execute();
        }

        static public void ExecuteOnToggle(List<Tuple<Keys, Keys>> keys, KeyboardState oldState, KeyboardState newState, Utility.Delegate_Paramless execute)
        {
            bool isOneToggled = false;

            foreach (Tuple<Keys,Keys> key in keys)
                isOneToggled |= IsKeyToggled(key.Item2, oldState, newState) && newState.IsKeyDown(key.Item1);

            if(isOneToggled)
                execute();
        }

        static public bool IsButtonToggled(Buttons button, GamePadState oldState, GamePadState newState)
        {
            return newState.IsButtonUp(button) && oldState.IsButtonDown(button);
        }

        static public void ExecuteOnToggle(Buttons button, GamePadState oldState, GamePadState newState, Utility.Delegate_Paramless execute)
        {
            if(IsButtonToggled(button, oldState, newState))
                execute();
        }

        static public void ExecuteOnToggle(List<Buttons> buttons, GamePadState oldState, GamePadState newState, Utility.Delegate_Paramless execute)
        {
            bool isOneToggled = false;

            foreach (Buttons button in buttons)
                isOneToggled |= IsButtonToggled(button, oldState, newState);

            if (isOneToggled)
                execute();
        }

        static public bool IsModifierUsed(KeyboardState kState)
        {
            return kState.IsKeyDown(Keys.LeftAlt)
                || kState.IsKeyDown(Keys.LeftControl)
                || kState.IsKeyDown(Keys.LeftWindows)
                || kState.IsKeyDown(Keys.LeftShift)
                || kState.IsKeyDown(Keys.RightAlt)
                || kState.IsKeyDown(Keys.RightControl)
                || kState.IsKeyDown(Keys.RightWindows)
                || kState.IsKeyDown(Keys.RightShift);
        }

        static public string TypeText(KeyboardState oldState, KeyboardState newState, string text, int maxchars, string def)
        {
            Keys[] oldKeys = oldState.GetPressedKeys();
            Keys[] newKeys = newState.GetPressedKeys();

            foreach (Keys key in oldKeys)
            {
                if (!newKeys.Contains(key))
                {
                    if (key == Keys.Back)
                    {
                        if (text.Length > 0)
                            text = text.Remove(text.Length - 1);
                    }
                    else if (text.Length <= maxchars)
                    {
                        if (key == Keys.Space)
                        {
                            text += "_";
                        }
                        else if (key.ToString().Length == 1)
                        {
                            if (text == def)
                                text = "";

                            text += key.ToString();
                        }
                    }
                }
            }
            return text;
        }
    }
}
