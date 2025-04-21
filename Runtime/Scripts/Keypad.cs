using UdonSharp;
using UnityEngine;
using VRC.Udon;
using UnityEngine.UI;
using VRC.SDKBase;
using TMPro;
using VRC.SDK3.StringLoading;
using VRC.Udon.Common.Interfaces;

namespace UwUtils
{
    [AddComponentMenu("UwUtils/Keypad System")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Keypad : UdonSharpBehaviour
    {

        private readonly string AUTHOR = "Reava_";
        private readonly string VERSION = "3.9 alpha";
        [Space]
        [SerializeField] private bool hideDoorsOnGranted = true;
        [SerializeField] private string keypadPassword = "8462";
        [Space]
        [SerializeField] private GameObject[] DoorObjects = new GameObject[0];
        [Tooltip("List of users that can just press the confirm button without the code to get permission")]
        [Space] public string[] allowList = new string[0];
        [Tooltip("List of users who even with the code cannot enter the code")]
        [SerializeField] private string[] denyList = new string[0];
        [Space]
        [Header("Sound settings")]
        [SerializeField] private bool useAudioFeedback = false;
        [SerializeField] private AudioSource feedbackSource = null;
        [SerializeField] private AudioClip soundDenied = null;
        [SerializeField] private AudioClip soundGranted = null;
        [SerializeField] private AudioClip soundButton = null;
        [Space]
        [Header("Text display")]
        [Space]
        [SerializeField] private string translationWaitcode = "PASSCODE";
        [SerializeField] private string translationDenied = "DENIED";
        [SerializeField] private string translationGranted = "GRANTED";
        [SerializeField] private TextMeshProUGUI internalKeypadDisplay = null;
        [Space]
        [Header("Scripts to send a custom event to on Clear/Deny/Granted actions")]
        [SerializeField] private UdonBehaviour[] programs;
        [Space]
        [Header("Extra Functions/Settings")]
        [Tooltip("Extra objects that will get turned on when granted (not affected by key seperation)")]
        [SerializeField] private GameObject[] ExtraObjectsToTurnOn = new GameObject[0];
        [Tooltip("Extra objects that will get turned off when granted (not affected by key seperation)")]
        [SerializeField] private GameObject[] ExtraObjectsToTurnOff = new GameObject[0];
        [Space]
        [Tooltip("If true, will automatically apply onGrant actions when an allowed user joins the world.")]
        [SerializeField] private bool OnJoinGrant = false;
        [Tooltip("Teleports the user to the location of the transform set under on grant (Does not apply when On Join Grant)")]
        [SerializeField] private bool teleportOnGrant = false;
        [SerializeField] private Transform teleportDestination;
        [Tooltip("This will give the user a tag other scripts can read, useful to pair with UwUtils scripts. (Empty to disable)")]
        [SerializeField] private string TagName = "vip";
        [Space]
        [Header("Advanced Section")]
        [Tooltip("When clear is hit after getting access, will revert the state of the 'doors' back to its original state.")]
        [SerializeField] private bool OnClearRevertDoors = true;
        [SerializeField] private bool useDisplay = true;
        [SerializeField] private int MaxInputLength = 8;
        [Tooltip("Will replace password being typed to this character to hide it")]
        [SerializeField] private bool HidePasswordTyped = true;
        [SerializeField] private char replacePassWithChar = '*';
        [Tooltip("name of the event to send to programs when the Clear button is pressed")]
        [SerializeField] private string eventNameOnClosed = "_keypadClosed";
        [Tooltip("name of the event to send to programs when the keypad Denies access")]
        [SerializeField] private string eventNameOnDenied = "_keypadDenied";
        [Tooltip("name of the event to send to programs when access is Granted")]
        [SerializeField] private string eventNameOnGranted = "_interact";
        [Space, Tooltip(" to update the allow list without updating the world")]
        [SerializeField] private bool useRemoteString = false;
        [Tooltip("You can use Pastebin RAW")]
        [SerializeField] private VRCUrl allowListLink = null;
        [SerializeField] private char splitRemoteStringWith = ',';
        [Space]
        [Header("Per passcode actions (Hover over)")]
        [Tooltip("When enabled, each code in 'additionalPasscodes' will toggle its own door in the 'additionalDoors' list, same for programsGranted")]
        [SerializeField] private bool additionalKeySeparation = false;
        [Space, SerializeField] private string[] additionalPasscodes = new string[0];
        [SerializeField] private GameObject[] additionalDoors = new GameObject[0];
        [Space]
        [Header("Warning: No support will be given if logging was disabled.")]
        [SerializeField] private bool enableLogging = true;

        // Debugging
        private string _keypadId;
        private string _prefix;

        // Keypad data storage
        private string _buffer;
        private string[] _solutions;
        private GameObject[] _doors;
        private string loadedString;
        private bool isGranted;
        private string[] strArr = new string[0];
        private bool isOnAllow = false;
        private string username = null;

        #region Util Functions
        private void Log(string value)
        {
            if (enableLogging)
            {
                Debug.Log(_prefix + value, gameObject);
            }
        }
        private void LogWarning(string value)
        {
            if (enableLogging)
            {
                Debug.LogWarning(_prefix + value, gameObject);
            }
        }
        private void LogError(string value)
        {
            if (enableLogging)
            {
                Debug.LogError(_prefix + value, gameObject);
            }
        }
        private void Die()
        {
            this.gameObject.GetComponent<UdonBehaviour>().enabled = false;
        }
        #endregion Util Functions

        public void Start()
        {
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            _keypadId = (Random.value * 15847).ToString().Substring(0, 4) + "(" + gameObject.name + ")";
            _prefix = "[Reava_/UwUtils/Keypad] [K-" + _keypadId + "] ";
            // Override disableDebugging here
            Debug.Log(_prefix + "Starting Keypad... Made by @" + AUTHOR + ". Version " + VERSION + ".");

            _buffer = "";

            if ((keypadPassword.Length < 1 || keypadPassword.Length > MaxInputLength) || keypadPassword == null)
            {
                LogError("Solution was empty or longer than "+ MaxInputLength +" ! Generating unusable password for security.");
                keypadPassword = Random.value.ToString();
            }

            if (internalKeypadDisplay == null && useDisplay)
            {
                LogError("Display is not set! Switching to not using a display, please set this correctly or check references !");
                useDisplay = false;
            }

            if (allowList == null)
            {
                LogError("Allow list was null, setting to empty list...");
                allowList = new string[0];
            }
            if (denyList == null)
            {
                LogError("Deny list was null, setting to empty list...");
                denyList = new string[0];
            }

            if (allowList.Length > 999)
            {
                LogError("Allow list was larger than 999, this is most likely unintentional, resetting to 0.");
                allowList = new string[0];
            }

            if (denyList.Length > 999)
            {
                LogError("Allow list was larger than 999, this is most likely unintentional, resetting to 0.");
                denyList = new string[0];
            }

            if (additionalPasscodes == null)
            {
                LogError("Additional Solutions list was null, setting to empty list...");
                additionalPasscodes = new string[0];
            }
            if (DoorObjects == null)
            {
                LogError("Additional Doors list was null, setting to empty list...");
                DoorObjects = new GameObject[0];
            }

            if (additionalPasscodes.Length > 999)
            {
                LogError("Additional Solutions list was larger than 999, this is most likely unintentional, resetting to 0.");
                additionalPasscodes = new string[0];
            }
            if (DoorObjects.Length > 999)
            {
                LogError("Additional Doors list was larger than 999, this is most likely unintentional, resetting to 0.");
                DoorObjects = new GameObject[0];
            }

            if (additionalKeySeparation && additionalPasscodes.Length != additionalDoors.Length)
            {
                LogError("Key separation was enabled, but the number of additional solutions is not equal to the number of additional doors, " +
                    "resetting to False. Please read the documentation what this setting does.");
                additionalKeySeparation = false;
            }

            if (additionalDoors.Length > 999)
            {
                LogError("Additional doors list was larger than 999, this is most likely unintentional, resetting to 0.");
                denyList = new string[0];
            }
            Log("Additional key separation is: " + additionalKeySeparation);

            if((useRemoteString && allowListLink == null) || splitRemoteStringWith == '\0')
            {
                LogError("Using remote string without a character to split the string with or no link to the remote string, this is not supported, disabling remote string loading. Please read the documentation what this setting does.");
                useRemoteString = false;
            }

            if(replacePassWithChar == '\0' || replacePassWithChar.ToString().Length > 1)
            {
                LogWarning("Invalid or character to hide the password with, this is unsupported, please disable 'Hide password' instead. Resetting to default config for this field.");
                replacePassWithChar = '*';
            }

            if(translationWaitcode == null || translationDenied == null || translationGranted == null)
            {
                LogError("Translation code cannot be empty, resetting empty values.");
                if (translationWaitcode == null) translationWaitcode = "PASSCODE";
                if (translationDenied == null) translationDenied = "DENIED";
                if (translationGranted == null) translationGranted = "GRANTED";
            }

            // Merge primary solution/door with additional solutions/doors.
            // This makes coding and loops more streamlined.
            _solutions = new string[additionalPasscodes.Length + 1];
            _doors = new GameObject[DoorObjects.Length + 1];
            _solutions[0] = keypadPassword;
            for (int i = 0; i != additionalPasscodes.Length; i++)
            {
                _solutions[i + 1] = additionalPasscodes[i];
            }
            for (int i = 0; i != DoorObjects.Length; i++)
            {
                _doors[i + 1] = DoorObjects[i];
            }
            username = Networking.LocalPlayer == null ? "UnityEditor" : Networking.LocalPlayer.displayName;
            if(useDisplay) internalKeypadDisplay.text = translationWaitcode;
            if (OnJoinGrant && allowList != null)
            {
                foreach (string u in allowList)
                {
                    if (u != null && u == username) _grantEvent();
                }
            }
            Log("Keypad started!");
            if(useRemoteString && allowListLink != null) VRCStringDownloader.LoadUrl(allowListLink, (IUdonEventReceiver)this);
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            loadedString += result.Result;
            if(loadedString != null) strArr = loadedString.Split(splitRemoteStringWith);
            if (OnJoinGrant && strArr.Length > 0)
            {
                foreach (string u in strArr)
                {
                    if (u == null) continue;
                    if (u == username)
                    {
                        _grantEvent();
                    }
                }
            }
            string[] tempList = new string[allowList.Length + strArr.Length];
            if (enableLogging) Log("String successfully loaded: " + loadedString);
        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            LogError("String loading failed: " + result.Error + "| Error Code: " + result.ErrorCode);
        }

        public void _TogglePassVisibility()
        {
            HidePasswordTyped = !HidePasswordTyped;
            if(_buffer.Length > 0)
            {
                PrintPassword();
            }
        }

        // ReSharper disable once InconsistentNaming
        private void CLR()
        {
            Log("Passcode CLEAR!");
            if (useDisplay) internalKeypadDisplay.text = translationWaitcode;
            if(OnClearRevertDoors && isGranted)
            {
                foreach (GameObject door in _doors)
                {
                    if (door == null) continue;
                    door.SetActive(hideDoorsOnGranted);
                }
                isGranted = false;
                Networking.LocalPlayer.SetPlayerTag("rank", "Visitor");
            }
            _relayToPrograms(0); // Relay closed event to programs
            _buffer = "";
        }
        private void _grantEvent()
        {
            Log(isOnAllow ? "GRANTED through allow list!" : "Passcode GRANTED!");
            if (useDisplay) internalKeypadDisplay.text = translationGranted;
            if (TagName != null) Networking.LocalPlayer.SetPlayerTag("rank", TagName);
            foreach (GameObject door in _doors)
            {
                if (door != null) door.SetActive(!hideDoorsOnGranted);
            }
            if(ExtraObjectsToTurnOn.Length > 0) // Turn on objects in ExtraObjectsToTurnOn array if there are any
            {
                Log(ExtraObjectsToTurnOn.Length + " Extra Objects to turn ON found, executing");
                foreach (GameObject o in ExtraObjectsToTurnOn)
                {
                    if (o == null) continue;
                    o.SetActive(true);
                }
            }
            if (ExtraObjectsToTurnOff.Length > 0) // Turn off objects in ExtraObjectsToTurnOn array if there are any
            {
                Log(ExtraObjectsToTurnOff.Length + " Extra Objects to turn OFF found, executing");
                foreach (GameObject o in ExtraObjectsToTurnOff)
                {
                    if (o == null) continue;
                    o.SetActive(false);
                }
            }
            if (soundGranted != null && useAudioFeedback && feedbackSource) feedbackSource.PlayOneShot(soundGranted);
            _relayToPrograms(2); // Relay granted to programs.
            isGranted = true;
        }

        public void _relayToPrograms(int result)
        {
            if (programs != null)
            {
                Log(programs.Length + " programs to relay to found, executing");
                switch (result)
                {
                    case 0:
                        foreach (UdonBehaviour prog in programs)
                        {
                            prog.SetProgramVariable("keypadCode", _buffer);
                            prog.SendCustomEvent(eventNameOnClosed);
                        }
                        break;
                    case 1:
                        foreach (UdonBehaviour prog in programs)
                        {
                            prog.SetProgramVariable("keypadCode", _buffer);
                            prog.SendCustomEvent(eventNameOnDenied);
                        }
                        break;
                    case 2:
                        foreach (UdonBehaviour prog in programs)
                        {
                            prog.SetProgramVariable("keypadCode", _buffer);
                            prog.SendCustomEvent(eventNameOnGranted);
                        }
                        break;
                }
                
            }
        }

        // ReSharper disable once InconsistentNaming
        private void OK()
        {
            isOnAllow = false;
            var isOnDeny = false;
            username = Networking.LocalPlayer == null ? "UnityEditor" : Networking.LocalPlayer.displayName;
            // Check if user is on allow list
            foreach (string entry in allowList)
            {
                if (entry == username) isOnAllow = true;
            }
            if(strArr.Length > 0)
            {
                foreach (string entry in strArr)
                {
                    if (entry == username) isOnAllow = true;
                }
            }
            // Check if user is on deny list
            foreach (string entry in denyList)
            {
                if (entry == username) isOnDeny = true;
            }
            bool isCorrect = false;
            for (int i = 0; i != _solutions.Length; i++)
            {
                if (_solutions[i] != _buffer) continue;
                isCorrect = true;
            }
            if (additionalKeySeparation)
            {
                for (var i = 0; i != additionalPasscodes.Length; i++)
                {
                    if (additionalPasscodes[i] != _buffer) continue;
                    if (i < additionalDoors.Length)
                    {
                        additionalDoors[i].SetActive(!hideDoorsOnGranted);
                    }
                }
            }
                // Check if pass is correct and not on deny, or if is on allow list.
            if ((isCorrect && !isOnDeny) || isOnAllow)
            {
                _grantEvent();
                if (teleportOnGrant && (teleportDestination != null)) Networking.LocalPlayer.TeleportTo(teleportDestination.position, teleportDestination.rotation);
                if (isOnAllow) _buffer = ""; // Clears buffer only if is on allow to avoid double press resetting the states on normal behavior
            }
            else
            {
                // Do not announce to user that they are on deny list.
                Log("Passcode DENIED!");
                if (useDisplay) internalKeypadDisplay.text = translationDenied;
                foreach (GameObject door in _doors)
                {
                    if (door == null) continue;
                    door.SetActive(hideDoorsOnGranted);
                }
                if (soundDenied != null && useAudioFeedback && feedbackSource) feedbackSource.PlayOneShot(soundDenied);
                _relayToPrograms(1); // Relay denied event to programs
                isGranted = false;
                _buffer = "";
            }
        }

        private void PrintPassword()
        {
            if (HidePasswordTyped)
            {
                string pass = replacePassWithChar.ToString();
                for (var i = 1; i < _buffer.Length; i++)
                {
                    pass += " " + replacePassWithChar;
                }
                if (useDisplay) internalKeypadDisplay.text = pass;
            }
            else
            {
                if (useDisplay) internalKeypadDisplay.text = _buffer;
            }
        }

        public void ButtonInput(string inputValue)
        {
            if (inputValue == "CLR")
            {
                CLR();
            }
            else if (inputValue == "OK")
            {
                if(isGranted && teleportOnGrant && teleportDestination != null) Networking.LocalPlayer.TeleportTo(teleportDestination.position, teleportDestination.rotation);
                OK();
            }
            else if (inputValue == "SHOW")
            {
                _TogglePassVisibility();
            }
            else
            {
                if (_buffer.Length == MaxInputLength)
                {
                    Log("Input Limit reached!");
                }
                else
                {
                    _buffer += inputValue;
                    PrintPassword();
                    Log("Buffer appended: " + inputValue);
                    if (soundButton != null && useAudioFeedback && feedbackSource) feedbackSource.PlayOneShot(soundButton);
                }
            }
        }

    }
}