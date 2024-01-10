using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _188898
{
    public class MainMenu : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            AudioListener.volume = 0.05f;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnLevel1ButtonPressed()
        {
            SceneManager.LoadScene("188898");
        }

        public void OnExitToDesktopButtonPressed()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif

            Application.Quit();
        }
    }
}
