using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    
    //TODO: Make this work
    public bool[] chosenCharacters = new bool[6];                               // List of taken players during char selection

    public GameObject[] players = new GameObject[4];                            // List of all players
    public CharacterSelection[] cs = new CharacterSelection[4];                 // List of all character selectors
    public GameObject[] spawnPoints = new GameObject[4];                        // List of player spawn points
    public bool[] scenes = new bool[5];                                         // <- Not yet in use

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

    public int winningScore = -1;                                                    // Highest score after round
    
    public bool gameStarted;                                                    // Is the round started
    public bool gameFinished;                                                   // Is the round finished

    public int NumOfPlayersNeededToStartGame_DEBUG;                             // Debug value for how many players needed to start game

    private void Start()
    {
        camera.GetComponent<CameraController>().MoveCamera(cameraPosCharacterSelection);

        // Disable all player controls during character selection.
        DisablePlayerControls();

        timer = setTimer;

        scenes[0] = true;                                                       // Set the scene to Main Menu <- Not yet in use
    }

    private void Update()
    {
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
                if (readyCount < 4)
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
            if(timer >= 0.0f && !gameFinished)
            {
                timer -= Time.deltaTime;
                timeLeft = System.Convert.ToInt32(timer);                      // Convert float to int to get seconds
        		//if(timeLeft < 5)													// Set when the timer shows
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
                    if(player.activeSelf == true)                                   // Check if player active
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

    public void ChangeScene(int from, int to)                                       // Changes the scene boolean <- Not yet in use
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