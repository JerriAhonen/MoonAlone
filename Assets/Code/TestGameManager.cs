﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameManager : MonoBehaviour
{

    //TODO: Make this work
    public bool[] chosenCharacters = new bool[6];                               // List of taken players during char selection

    public GameObject[] players = new GameObject[4];                            // List of all players
    public CharacterSelection[] cs = new CharacterSelection[4];                 // List of all character selectors
    public GameObject[] spawnPoints = new GameObject[4];                        // List of player spawn points
    public bool[] scenes = new bool[5];

    public Camera camera;                                                       // The Main Camera
    public Transform cameraPosMainMenu;                                         // Pos of camera during menu
    public Transform cameraPosCharacterSelection;                               // Pos of camera during char selection
    public Transform cameraPosGameplay;                                         // Pos of camera during round

    public RuntimeAnimatorController runtimeAnimatorController;                 // Player Animation controller
    public Avatar avatar;                                                       // Player Avatar

    public int readyCount;                                                      // Num of players ready
    public int noPlayerCount = 4;                                               // Num of players not playing

    public TextMesh timerText;                                                  // Timer displayer during round
    public TextMesh winnerText;                                                 // Text displayed after round

    public float setTimer = 60.0f;                                              // Round lenght in seconds
    private float timer;                                                        // Timer counting floats
    public int timeLeft;                                                        // Time displayed on timer
    public int roundNum;                                                        // Number of current round

    public int winningScore;                                                    // Highest score after round

    public bool gameStarted;                                                    // Is the round started
    public bool gameFinished;                                                   // Is the round finished

    public int NumOfPlayersNeededToStartGame_DEBUG;                             // Debug value for how many players needed to start game

    private void Start()
    {
        camera.GetComponent<CameraController>().MoveCamera(cameraPosCharacterSelection);

        // Disable all player controls during character selection.
        DisablePlayerControls();

        timer = setTimer;

        scenes[0] = true;                                                       // Set the scene to Main Menu
    }

    private void Update()
    {
        /*
        if (scenes[0])
        {
             MAIN MENU

             DisablePlayerControls();
             camera.GetComponent<CameraController>().MoveCamera( cameraPosMAINMENU );

             if ( press play )
             ChangeScene(1);

             If ( press quit )
             Quit();
        }

        if (scenes[1])
        {
             CHARACTER SELECTION

             camera.GetComponent<CameraController>().MoveCamera( cameraPosCharacterSelection );
             Add animators (?)

             if ( all playing players ready )
             ChangeScene[2];

             if ( press back )
             ChangeScene[0];
        }

        if (scenes[2])
        {
             ROUND

             camera.GetComponent<CameraController>().MoveCamera( cameraPosGameplay );

             CountDown(3)     <- Count down before match starts. 3 seconds.
             add animators (?)

             Game logic until timer runs out
             DisablePlayerControls();
             ChangeScene[3];
        }

        if (scenes[3])
        {
             ROUND SCORE

             Display round score (who won? who 2nd 3rd and 4th)
             timer like 5 seconds
             if ( rounds = loppu)
             ChangeScene[4];
             else ChangeScene[2];

        }

        if (scenes[4])
        {
             GAME OVER

             camera.GetComponent<CameraController>().MoveCamera( cameraPosGAMEOVER );
             Display who won

             if ( press main menu )
             ChangeScene[0]
             if ( press play again )
             ChangeScene[2]
        }
        */




        // Logic for when CHARACTER SELECTION RUNNING
        if (!gameStarted)
        {
            if (readyCount >= NumOfPlayersNeededToStartGame_DEBUG && noPlayerCount == (4 - readyCount))
            {
                StartGame(readyCount);
                gameStarted = true;
                gameFinished = false;

                timerText.text = "";
                winnerText.text = "";

                // Deactivate all players that are not needed
                // TODO: Clean this up.
                if (readyCount < 4)                                                 // Add animators to all needed models
                {
                    switch (noPlayerCount)
                    {
                        case 1:
                            players[3].SetActive(false);
                            AddAnimator(2);
                            AddAnimator(1);
                            AddAnimator(0);
                            break;
                        case 2:
                            players[3].SetActive(false);
                            players[2].SetActive(false);
                            AddAnimator(1);
                            AddAnimator(0);
                            break;
                        case 3:
                            players[3].SetActive(false);
                            players[2].SetActive(false);
                            players[1].SetActive(false);
                            AddAnimator(0);
                            break;
                    }
                }
            }
        }
        // Logic for when GAME RUNNING
        else
        {
            if (timer >= 0.0f && !gameFinished)
            {
                timer -= Time.deltaTime;
                timeLeft = System.Convert.ToInt32(timer % 60);                      // Convert float to int to get seconds
                timerText.text = timeLeft.ToString();
            }

            // Winning condition = the player with the most chickens when timeLeft = 0.
            if (timeLeft == 0)
            {
                int playerNum = 0;
                int playerScore = 0;

                int i = 0;
                foreach (var player in players)                                     // Go through all players
                {
                    if (player.activeSelf == true)                                   // Check if player active
                    {
                        i++;                                                        // Start counting from P1
                        playerScore = player.GetComponentInChildren<Tower>().chickenCount;
                        if (playerScore > winningScore)                             // Find highest chicken count
                        {
                            winningScore = playerScore;
                            playerNum = i;
                        }
                    }
                }

                gameFinished = true;                                                // Set the round to finished

                DisablePlayerControls();                                            // Stop the players from moving
                timer = setTimer;                                                   // Reset the timer for next round
                timeLeft = System.Convert.ToInt32(timer % 60);                      // Reset TimeLeft

                timerText.text = "Time's up!";
                winnerText.text = "Player " + playerNum + " wins with " + winningScore + " chickens!";
            }
        }
    }

    void StartGame(int playerCount)                                                 // Player position and scripts and camera
    {
        for (int i = 0; i < playerCount; i++)
        {
            players[i].transform.position = spawnPoints[i].transform.position;      // Teleport to starting position 
            players[i].GetComponent<Player>().enabled = true;                       // Enable Player script
        }

        camera.GetComponent<CameraController>().MoveCamera(cameraPosGameplay);      // Move camera to Game view
    }

    private void DisablePlayerControls()                                            // Disable player GameObject
    {
        foreach (var player in players)
        {
            player.GetComponent<Player>().enabled = false;
        }
    }

    // Adds an Animator, AnimationController and an Avatar to the player
    public void AddAnimator(int index)
    {
        // Finds the selected model through cs (CharacterSelector) and add's the Animator component to it.
        Animator animator = cs[index].transform.GetChild(cs[index].selectedCharacter).gameObject.AddComponent<Animator>() as Animator;
        animator.runtimeAnimatorController = runtimeAnimatorController;
        animator.avatar = avatar;
    }

    public void ChangeScene(int from, int to)                                       // Changes the scene boolean
    {
        // bool mainMenu;          // 0
        // bool characterSelect;   // 1
        // bool round;             // 2
        // bool roundScore;        // 3
        // bool gameOver;          // 4

        scenes[from] = false;
        scenes[to] = true;
    }


}


//public void AddAnimator(int index)                                              // Adds an Animator and AnimationController to the player
//{
//    GameObject go = players[index].gameObject;                                  // Reference to player
//    go = go.transform.GetChild(0).gameObject;                                   // Reference to characterSelector
//    for (int i = 0; i < go.transform.childCount; i++)                           // Go through all childs
//    {
//        if (go.transform.GetChild(i).gameObject.activeSelf)                     // Check if child is active
//        {
//            Animator animator = go.transform.GetChild(i).gameObject.AddComponent<Animator>() as Animator;
//            animator.runtimeAnimatorController = runtimeAnimatorController;
//        }
//    }
//}