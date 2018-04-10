using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceHenHassle
{
    public class GameManagerTest : MonoBehaviour
    {


        public bool[] scenes = new bool[5];
        private int currentScene = 0;

        public int numOfPlayers;

        private bool timerFinished;

        public Camera MainCamera;
        private CameraControllerTest cc;
        private CSManagerTest csm;

        // Use this for initialization
        void Start()
        {
            cc = MainCamera.GetComponent<CameraControllerTest>();
            csm = GetComponent<CSManagerTest>();

            scenes[0] = true;
        }

        // Update is called once per frame
        void Update()
        {
            ChangeSceneWithC();

            if (scenes[0])
            {
                StartCooldownTimer(10);
                csm.EnableCharacterSelection(false, numOfPlayers);

                if (Input.GetKeyDown(KeyCode.Space))
                {    // Insert mainmenu option 1 here.
                    //StartCooldownTimer(10);
                    //if (timerFinished)
                        ChangeScene(1);
                }
                // Ask number of players
            }
            else if (scenes[1])
            {
                csm.EnableCharacterSelection(true, numOfPlayers);

                if (csm.ready)
                {
                    csm.EnableCharacterSelection(false, numOfPlayers);
                    
                    ChangeScene(2);
                }

            }





























        }

        public void ChangeScene(int newScene)   // Works 6.4.
        {
            // bool mainMenu;          // 0
            // bool characterSelect;   // 1
            // bool round;             // 2
            // bool roundScore;        // 3
            // bool gameOver;          // 4

            for (int i = 0; i < scenes.Length; i++)
            {
                scenes[i] = false;
            }

            scenes[newScene] = true;
            Debug.Log("Current scene = " + newScene);

            //Move the camera to the new location for each scene
            MoveCamera(newScene);   // Works 6.4.
        }

        // Move Camera
        // Communicates with the Main Camera's component "cc"
        public void MoveCamera(int sceneNum)
        {   // Works 6.4.
            cc.MoveCamera(sceneNum);
        }

        private void ChangeSceneWithC()
        {   // DEBUG METHOD
            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("Switch scene");
                currentScene++;
                if (currentScene == scenes.Length)
                {
                    currentScene = 0;
                }
                ChangeScene(currentScene);
            }
        }

        private void StartCooldownTimer(float cooldownTime)
        {
            timerFinished = false;
            while (cooldownTime > 0)
            {
                cooldownTime -= Time.deltaTime;
                Debug.Log("CooldownTimer: " + cooldownTime.ToString("f0"));
            }
            timerFinished = true;
        }
    }

}

