using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace EgyptianWar
{
    public class Rules : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            #if UNITY_EDITOR
            if (Input.GetKey(KeyCode.Escape)) {
                UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();
            }
            #endif
        }

        public void OnBackClicked()
        {
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
