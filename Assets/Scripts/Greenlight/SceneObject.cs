using UnityEngine;


namespace Scorch_SceneObject
{
    /// <summary>
    ///  Static Game Objects
    ///  Originally planned as Singleton Pattern, but not used as one
    /// </summary>
    [System.Serializable]
    public class SceneObject : MonoBehaviour
    {
        private static Conversation c_Instance;
        private static AnsweringMachine am_Instance;
        private static VManager v_Instance;
        private static AudioManager a_Instance;

        public static VManager VManager
        {
            get
            {
                if (v_Instance == null)
                {
                    v_Instance = (VManager)FindObjectOfType(typeof(VManager));
                    if (v_Instance == null)
                    {
                        GameObject singleton = new GameObject();
                        v_Instance = singleton.AddComponent<VManager>();
                        singleton.name = "(singleton) " + typeof(VManager).ToString();
                    }
                }
                return v_Instance;
            }
        }
       public static Conversation conversation
        {
            get
            {
                if (c_Instance == null)
                {
                    c_Instance = (Conversation)FindObjectOfType(typeof(Conversation));
                    if (c_Instance == null)
                    {
                        GameObject singleton = new GameObject();
                        c_Instance = singleton.AddComponent<Conversation>();
                        singleton.name = "(singleton) " + typeof(Conversation).ToString();
                    }

                }

                return c_Instance;
            }
        }
        public static AnsweringMachine answeringMachine
        {
            get
            {

                if (am_Instance == null)
                {
                    am_Instance = (AnsweringMachine)FindObjectOfType(typeof(AnsweringMachine));
                    if (am_Instance == null)
                    {
                        GameObject singleton = new GameObject();
                        am_Instance = singleton.AddComponent<AnsweringMachine>();
                        //am_Instance = new AnsweringMachine(vManager);
                        singleton.name = "(singleton) " + typeof(AnsweringMachine).ToString();
                    }
                }

                return am_Instance;
            }
        }
        public static AudioManager audioManager
        {
            get
            {
                if (a_Instance == null)
                {
                    a_Instance = (AudioManager)FindObjectOfType(typeof(AudioManager));

                    if (a_Instance == null)
                    {
                        GameObject singleton = new GameObject();
                        a_Instance = singleton.AddComponent<AudioManager>();
                        singleton.name = "(singleton) " + typeof(AudioManager).ToString();
                    }
                }

                return a_Instance;
            }
        }

        private static bool applicationIsQuitting = false;

        public void OnDestroy() {
            applicationIsQuitting = true;
        }



    }

}