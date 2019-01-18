using System;//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scorch_Keybindings
{
    public class KeyBinding : MonoBehaviour
    {
        /* The Keybinding class
         * The player controller needs access to the jumpKey, fireKey, etc
         * Saved in PlayerPrefs
         * 
         */


        //KeyCodes
        public KeyCode pauseKey = KeyCode.Escape;
        public KeyCode memoKey = KeyCode.Tab;
        public KeyCode confirmKey = KeyCode.Return;
        public KeyCode callKey = KeyCode.G;
        public KeyCode interactKey = KeyCode.E;
        public KeyCode crouchKey = KeyCode.LeftControl;
        public KeyCode keyToRebind = KeyCode.None;

        public enum Actions { None, Pause, Memo, Confirm, Call, Interact, Crouch }
        Actions actions;
        Actions actionToRebind = Actions.None;

        private bool listening = false;


        //UI
        [SerializeField]
        private Text pauseKeyText, memoKeyText, confirmKeyText, callKeyText, interactKeyText, crouchKeyText;

        [SerializeField]
        private Button pauseKeyButton;

        // Use this for initialization
        void Start()
        {
            // Retrieve the Keybindings from the PlayerPrefs
            pauseKey = (KeyCode)PlayerPrefs.GetInt("KeyPause");
            memoKey = (KeyCode)PlayerPrefs.GetInt("KeyMemo");
            confirmKey = (KeyCode)PlayerPrefs.GetInt("KeyConfirm"); //, (int)KeyCode.Return
            callKey = (KeyCode)PlayerPrefs.GetInt("KeyCall");
            interactKey = (KeyCode)PlayerPrefs.GetInt("KeyInteract");
            crouchKey = (KeyCode)PlayerPrefs.GetInt("KeyCrouch");

            // Update the UI Text for the keys
            pauseKeyText.text = pauseKey.ToString();
            memoKeyText.text = memoKey.ToString();
            confirmKeyText.text = confirmKey.ToString();
            callKeyText.text = callKey.ToString();
            interactKeyText.text = interactKey.ToString();
            crouchKeyText.text = crouchKeyText.ToString();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(pauseKey))
            {
                Debug.Log("KB Pause");
            }

            if (Input.GetKeyDown(memoKey))
            {
                Debug.Log("KB Memoboard");
            }

            if (Input.GetKey(confirmKey))
            {
                Debug.Log("KB Confirmed");
            }

            if (listening)
            {
                KeyCode newKey = IdentifyPressedKeyCode();

                //When a key was pressed
                if (newKey != KeyCode.None)
                {
                    //public enum Actions { None, Pause, Memo, Confirm, Call }

                    //Update the control
                    switch (actionToRebind)
                    {
                        case 0:
                            //None
                            break;
                        case (Actions)1: //Pause
                            RebindPause(newKey);
                            break;
                        case (Actions)2: //Memo
                            RebindMemo(newKey);
                            break;
                        case (Actions)3: //Confirm
                            RebindConfirm(newKey);
                            break;
                        case (Actions)4: //Call
                            RebindCall(newKey);
                            break;
                        case (Actions)5: //Interact
                            RebindInteract(newKey);
                            break;
                        case (Actions)6: //Crouch
                            RebindCrouch(newKey);
                            break;
                    }

                    //stop listening
                    listening = false;
                }


            }
        }


        public void StartListening()
        {
            Debug.Log("Started listening");
            listening = true;
        }


        //Updating the action which we will rebind
        public void SetActionToRebind(int i)
        {
            Debug.Log("Updated the action to rebind");
            actionToRebind = (Actions)i;
            StartListening();
        }

        //public enum Actions { None, Pause, Memo, Confirm, Call }
        //Rebinding the key
        public void RebindPause(KeyCode newKey)
        {
            pauseKey = newKey;
            pauseKeyText.text = newKey.ToString();
            PlayerPrefs.SetInt("KeyPause", (int)newKey);
        }

        public void RebindMemo(KeyCode newKey)
        {
            memoKey = newKey;
            memoKeyText.text = newKey.ToString();
            PlayerPrefs.SetInt("KeyMemo", (int)newKey);
        }

        public void RebindConfirm(KeyCode newKey)
        {
            confirmKey = newKey;
            confirmKeyText.text = newKey.ToString();
            PlayerPrefs.SetInt("KeyConfirm", (int)newKey);
        }

        public void RebindCall(KeyCode newKey)
        {
            callKey = newKey;
            callKeyText.text = newKey.ToString();
            PlayerPrefs.SetInt("KeyCall", (int)newKey);
        }

        public void RebindInteract(KeyCode newKey)
        {
            interactKey = newKey;
            interactKeyText.text = newKey.ToString();
            PlayerPrefs.SetInt("KeyInteract", (int)newKey);
        }

        public void RebindCrouch(KeyCode newKey)
        {
            crouchKey = newKey;
            crouchKeyText.text = newKey.ToString();
            PlayerPrefs.SetInt("KeyCrouch", (int)newKey);
        }

        //Used for listening, very unoptimized
        public KeyCode IdentifyPressedKeyCode()
        {
            foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keycode))
                {
                    Debug.Log("KeyCode down: " + keycode);
                    return keycode;
                }
            }
            return KeyCode.None;
        }



        //Player Preferences Saving and Loading
        public void SaveAllPlayerPrefs()
        {
            PlayerPrefs.Save();
            Debug.Log("All player prefernces were saved");
        }
    }
}