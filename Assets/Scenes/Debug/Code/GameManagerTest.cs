using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerTest : MonoBehaviour
{

    public enum sceneState{
        mainMenu = 0,
        characterSelection = 1,
        round = 2,
        roundScore = 3,
        gameOver = 4
    }
    public sceneState state;

    //public bool[] scenes = new bool[5];
    private int currentScene = 0;
    public int numOfPlayers;
    public Camera MainCamera;
    private CameraControllerTest cc;
    public CSManagerTest csm;
    MainMenuState mmstate;
    
    // Use this for initialization
    void Start()
    {
        cc = MainCamera.GetComponent<CameraControllerTest>();
        csm = GetComponent<CSManagerTest>();
        mmstate = new MainMenuState(this);
        //scenes[0] = true;
        state = sceneState.mainMenu;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeSceneWithC();

        switch(state) {
            case sceneState.mainMenu: 
                csm.EnableCharacterSelection(false, numOfPlayers);
                // Ask number of players
                if (mmstate != null) {
                    mmstate.Update();
                }


                break;
            case sceneState.characterSelection:
                
                csm.EnableCharacterSelection(true, numOfPlayers);

                
                break;
            case sceneState.round:

                

                break;
            
        }

        // if (scenes[0])
        // {
        //     csm.EnableCharacterSelection(false, numOfPlayers);

        //     
        // }
        // else if (scenes[1])
        // {
        //     

        // }

        //StartCoroutine(StartCooldownTimer(10));

    }

    private MainMenuState InitMainMenu() {
        return new MainMenuState(this);
    }

    public void ChangeScene(sceneState newState) {
        state = newState;
        Debug.Log("Current scene = " + state);

        MoveCamera((int)newState);
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
            Debug.Log(" 'C' pressed, switch scene");
            currentScene++;
            if (currentScene == 5)
            {
                currentScene = 0;
            }
            ChangeScene((sceneState)currentScene);  //Cast the current scene as enum
        }
    }

    
}

