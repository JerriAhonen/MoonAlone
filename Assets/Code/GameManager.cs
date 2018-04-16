using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    
    public GameObject[] players = new GameObject[4];                            // List of all players
    public int[] playerScores = new int[4];
    
    public RuntimeAnimatorController runtimeAnimatorController;                 // Player Animation controller
    public Avatar avatar;                                                       // Player Avatar

    public int readyCount;                                                      // Num of players ready
    public int noPlayerCount = 4;                                               // Num of players not playing

    public TextMeshPro timerText;                                               // Timer displayer during round
    //public TextMeshPro winnerText;                                              // Text displayed after round
    public TextMeshPro nextRound;                                               // Text saying what happens next after round finished

    public float setTimer = 60.0f;                                              // Round lenght in seconds
    private float timer;                                                        // Timer counting floats
    public int timeLeft;                                                        // Time displayed on timer
    public int NumOfRounds = 2;
    public int curRoundNum;                                                     // Number of current round

    public int winningScore = -1;                                               // Highest score after round
    
    public bool roundStarted;                                                   // Is the round started
    public bool roundFinished;                                                  // Is the round finished

    public int numberOfPlayers;

    public bool cooldownTimerFinished = false;
    private int countdownTime;

    private bool displayTransitionText;

    private void Start()
    {
        //camera.GetComponent<CameraController>().MoveCamera(cameraPosCharacterSelection);

        curRoundNum = PlayerPrefs.GetInt("CurrentRoundNumber");
        numberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers");
        timer = setTimer;

        EnablePlayerGOs(numberOfPlayers);
        displayTransitionText = false;
    }

    private void EnablePlayerGOs (int num)
    {
        //Disable all players first
        foreach (GameObject player in players)
        {
            player.SetActive(false);
        }

        //Enable "num" amount of players
        for (int i = 0; i < num; i++)
        {
            players[i].SetActive(true);
        }
    }

    private void Update()
    {
        // Logic for when GAME RUNNING
        
        if (timer >= 0.0f && !roundFinished)
        {
            timer -= Time.deltaTime;
            timeLeft = System.Convert.ToInt32(timer);                       // Convert float to int to get seconds
            //if(timeLeft < 5)												// Set when the timer shows
            timerText.text = timeLeft.ToString();
        }

        // Winning condition = the player with the most chickens when timeLeft = 0.
        if (timeLeft == 0 && !roundFinished)
        {
            GetRoundScore();

            roundFinished = true;                                           // Set the round to finished
                
            timer = setTimer;                                               // Reset the timer for next round
            timeLeft = System.Convert.ToInt32(timer % 60);                  // Reset TimeLeft

            timerText.text = "Time's up!";
            //winnerText.text = "Player " + playerNum + " wins with " + winningScore + " chickens!";

            Debug.Log("Started CooldownTimer Coroutine");
            StartCoroutine(StartCooldownTimer(5));

            displayTransitionText = true;



            //Debug.Log("EndRound();");
            //EndRound();
        }

        if (displayTransitionText)
        {
            if (PlayerPrefs.GetInt("CurrentRoundNumber") < NumOfRounds)
                nextRound.text = "Time until next round: " + countdownTime;
            else
                nextRound.text = "Time until main menu: " + countdownTime;
        }

        if (roundFinished && cooldownTimerFinished)
        {
            Debug.Log("EndRound();");
            EndRound();
        }
    }

    // Adds an Animator, AnimationController and an Avatar to the player
    //public void addAnimators(int index)
    //{

    //    for (int i = 0; i < numberOfPlayers; i++)
    //    {
    //        Animator animator = cs[index].transform.getchild(cs[index].selectedcharacter).gameobject.addcomponent<Animator>() as Animator;
    //    }

    //    // finds the selected model through cs (characterselector) and add's the animator component to it.
        
    //    animator.runtimeanimatorcontroller = runtimeanimatorcontroller;
    //    animator.avatar = avatar;
    //}

        //TODO: MAKE THIS WORK
    public void GetRoundScore(){

        int[] scores = new int[4];

        // Fill scores[] with players' chicken counts
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].activeSelf)
                scores[i] = players[i].GetComponent<Tower>().chickenCount;
        }

        System.Array.Sort(scores);

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].activeSelf) {
                if ( players[i].GetComponent<Tower>().chickenCount == 0) {
                    players[i].GetComponent<Player>().score = 0;
                } else if (players[i].GetComponent<Tower>().chickenCount == scores[3]) {
                    players[i].GetComponent<Player>().score = readyCount - 1;
                } else if (players[i].GetComponent<Tower>().chickenCount == scores[2]) {
                    players[i].GetComponent<Player>().score = readyCount - 2;
                } else if (players[i].GetComponent<Tower>().chickenCount == scores[1]) {
                    players[i].GetComponent<Player>().score = readyCount - 3;
                } else if (players[i].GetComponent<Tower>().chickenCount == scores[0]) {
                    players[i].GetComponent<Player>().score = readyCount - 4;
                }
            }
        }
        
        for (int i = 0; i < playerScores.Length; i++)
        {
            playerScores[i] += players[i].GetComponent<Player>().score;
        }

    }

    private void EndRound()
    {
        if(PlayerPrefs.GetInt("CurrentRoundNumber") < NumOfRounds)
        {
            //Add 1 to the current roundNumber in PlayerPrefs.
            PlayerPrefs.SetInt("CurrentRoundNumber", PlayerPrefs.GetInt("CurrentRoundNumber") + 1);
            SceneManager.LoadScene("Round_Level1");
        }
        else
        {
            //When all rounds are played, reset the roundNumber to 1.
            PlayerPrefs.SetInt("CurrentRoundNumber", 1);
            SceneManager.LoadScene("MainMenu");
        }
            
    }

    private IEnumerator StartCooldownTimer(int cooldownTime)
    {
        countdownTime = cooldownTime;

        while (countdownTime > 0)
        {
            yield return new WaitForSeconds(1f);

            countdownTime--;
            cooldownTimerFinished = false;
            Debug.Log("cooldown time: " + countdownTime);
        }
        if (countdownTime <= 0)
        {
            Debug.Log("CooldownTimer Finished!");
            cooldownTimerFinished = true;
        }

    }

    //void StartGame(int playerCount)                                                 // Player position and scripts and camera
    //{
    //    for (int i = 0; i < playerCount; i++)
    //    {
    //        players[i].transform.position = spawnPoints[i].transform.position;      // Teleport to starting position 
    //        players[i].GetComponent<Player>().enabled = true;                       // Enable Player script
    //    }

    //    //camera.GetComponent<CameraController>().MoveCamera(cameraPosGameplay);      // Move camera to Game view
    //}

    //-------OLD CODE-------\\

    //TODO: Make this work
    //public bool[] chosenCharacters = new bool[6];                             // List of taken players during char selection

    //public CharacterSelection[] cs = new CharacterSelection[4];               // List of all character selectors
    //public GameObject[] spawnPoints = new GameObject[4];                        // List of player spawn points
    //public bool[] scenes = new bool[5];                                         // <- Not yet in use

    //public Camera camera;                                                       // The Main Camera
    //public Transform cameraPosMainMenu;                                         // Pos of camera during menu
    //public Transform cameraPosCharacterSelection;                               // Pos of camera during char selection
    //public Transform cameraPosGameplay;                                         // Pos of camera during round

    // Disable all player controls during character selection.
    //DisablePlayerControls();

    //foreach (var cs in cs)
    //{
    //    cs.UseTestGameManager = false;
    //}

    //scenes[0] = true;                                                       // Set the scene to Main Menu <- Not yet in use


    // int playerNum = 0;
    // int playerScore = 0;

    // int i = 0;
    // foreach (var player in players)                                     // Go through all players
    // {
    //     if (player.activeSelf == true)                                   // Check if player active
    //     {
    //         i++;                                                        // Start counting from P1
    //         playerScore = player.GetComponentInChildren<Tower>().chickenCount;
    //         if (playerScore > winningScore)                             // Find highest chicken count
    //         {
    //             winningScore = playerScore;
    //             playerNum = i;
    //         }
    //     }
    // }


    // Logic for when CHARACTER SELECTION RUNNING
    //else
    //{
    //    if (readyCount >= NumOfPlayersNeededToStartGame_DEBUG && noPlayerCount == (4 - readyCount))
    //    {
    //        StartGame(readyCount);
    //        roundStarted = true;
    //        roundFinished = false;

    //        timerText.text = "";
    //        winnerText.text = "";

    //        // Deactivate all players that are not needed
    //        // TODO: Clean this up.

    //        switch (noPlayerCount)
    //        {
    //            case 0:
    //                AddAnimator(3);
    //                AddAnimator(2);
    //                AddAnimator(1);
    //                AddAnimator(0);
    //                break;
    //            case 1:
    //                players[3].SetActive(false);
    //                AddAnimator(2);
    //                AddAnimator(1);
    //                AddAnimator(0);
    //                break;
    //            case 2:
    //                players[3].SetActive(false);
    //                players[2].SetActive(false);
    //                AddAnimator(1);
    //                AddAnimator(0);
    //                break;
    //            case 3:
    //                players[3].SetActive(false);
    //                players[2].SetActive(false);
    //                players[1].SetActive(false);
    //                AddAnimator(0);
    //                break;
    //        }
    //    }
    //}





    //private void DisablePlayerControls()                                            // Disable player GameObject
    //{
    //    foreach (var player in players)
    //    {
    //        player.GetComponent<Player>().enabled = false;                          
    //    }
    //}

    

    //public void ChangeScene(int from, int to)                                       // Changes the scene boolean <- Not yet in use
    //{
    //    // bool mainMenu;          // 0
    //    // bool characterSelect;   // 1
    //    // bool round;             // 2
    //    // bool roundScore;        // 3
    //    // bool gameOver;          // 4

    //    scenes[from] = false;
    //    scenes[to] = true;
    //}
}