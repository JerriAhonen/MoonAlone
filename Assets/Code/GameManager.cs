using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    
    //TODO: Make this work
    public bool[] chosenCharacters = new bool[6];                               // List of taken players during char selection

    public GameObject[] players = new GameObject[4];                            // List of all players
    public CharacterSelection[] cs = new CharacterSelection[4];                 // List of all character selectors
    public GameObject[] spawnPoints = new GameObject[4];                        // List of player spawn points

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

        //Disable all player controls during character selection.
        DisablePlayerControls();

        timer = setTimer;
    }

    private void Update()
    {
        // Logic for when CHARACTER SELECTION RUNNING
        if (!gameStarted)
        {
            if (readyCount >= NumOfPlayersNeededToStartGame_DEBUG && noPlayerCount == (4 - readyCount))
            {
                //Debug.Log("Start Game!");
                StartGame(readyCount);
                gameStarted = true;
                gameFinished = false;

                timerText.text = "";
                winnerText.text = "";

                //Deactivate all players that are not needed
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

                timerText.text = "Time's up!";
                winnerText.text = "Player " + playerNum + " wins with " + winningScore + " chickens!";
            }

        }
    }

    void StartGame(int playerCount)
    {
        for (int i = 0; i < playerCount; i++)
        {
            players[i].transform.position = spawnPoints[i].transform.position;      // Teleport to starting position 
            players[i].GetComponent<Player>().enabled = true;                       // Enable Player script
        }

        camera.GetComponent<CameraController>().MoveCamera(cameraPosGameplay);      // Move camera to Game view
    }

    private void DisablePlayerControls()
    {
        foreach (var player in players)
        {
            player.GetComponent<Player>().enabled = false;                          // Disable player GameObject
        }
    }

    public void AddAnimator(int index)                                              // Adds an Animator and AnimationController to the player
    {

        Animator animator = cs[index].transform.GetChild(cs[index].selectedCharacter).gameObject.AddComponent<Animator>() as Animator;
        animator.runtimeAnimatorController = runtimeAnimatorController;
        animator.avatar = avatar;
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