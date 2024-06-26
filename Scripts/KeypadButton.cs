using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UwUtils;

// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable once CheckNamespace

namespace UwUtils
{
    [AddComponentMenu("UwUtils/Keypad Button")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class KeypadButton : UdonSharpBehaviour
    {

        public Keypad keypad = null;
        public string buttonValue = null;
        public bool disableDebugging = false;

        private string _buttonId;
        private string _prefix;

        #region Util Functions
        private void Log(string value)
        {
            if (disableDebugging != true)
            {
                Debug.Log(_prefix + value);
            }
        }
        private void LogError(string value)
        {
            if (disableDebugging != true)
            {
                Debug.LogError(_prefix + value);
            }
        }
        private void Die()
        {
            // Crash.
            // ReSharper disable once PossibleNullReferenceException
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            ((string)null).ToString();
        }
        #endregion Util Functions

        public void Start()
        {
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            _buttonId = (Random.value * 15847).ToString().Substring(0, 4);
            _prefix = "[UdonKeypad] [b-" + _buttonId + "] ";
            // need to replace prefix and id stuff to match that of parent's keypad 
            Log("Loading button... Value: " + buttonValue);
            if (keypad == null)
            {
                LogError("Keypad is not set! Trying to dynamically find object...");
                var obj = GameObject.Find("Keypad");
                if (obj != null)
                {
                    keypad = (Keypad)obj.GetComponent(typeof(UdonBehaviour));
                }
                else
                {
                    LogError("Keypad is not set and could not dynamically find parent! Dying...");
                    Die();
                }
            }

            if (buttonValue == null)
            {
                LogError("Keypad is not set! Resetting to calculated value: " + gameObject.name.Substring(12));
                buttonValue = gameObject.name.Substring(12);
            }

            if (buttonValue.Length != 1 && buttonValue != "OK" && buttonValue != "CLR" && buttonValue != "SHOW")
            {
                LogError("Button has invalid value! Resetting to 'X'. OldValue: '" + buttonValue + "'.");
                buttonValue = "X";
            }

            Log("Button started! Value: " + buttonValue);
        }

        public override void Interact()
        {
            Log("Key pressed: " + buttonValue + " (interact)");
            if (Networking.LocalPlayer != null/* && Networking.LocalPlayer.IsUserInVR()*/) // Why was it checking for user in vr???
            {
                ButtonPressed();
            }
        }

        public void OnMouseDown()
        {
            Log("Key pressed: " + buttonValue + " (OnMouseDown)");
            ButtonPressed();
        }

        private void ButtonPressed()
        {
            keypad.ButtonInput(buttonValue);
        }
    }
}