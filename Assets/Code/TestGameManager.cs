using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestGameManager : MonoBehaviour
{

    
    public bool[] chosenCharacters = new bool[6];                               // List of taken players during char selection

    public GameObject[] players = new GameObject[4];                            // List of all players
    public CharacterSelection[] cs = new CharacterSelection[4];                 // List of all character selectors
    public GameObject[] spawnPoints = new GameObject[4];                        // List of player spawn points
    public bool[] scenes = new bool[5];
    public int[] playerScores = new int[4];

    public Camera camera;                                                       // The Main Camera
    public Transform cameraPosMainMenu;                                         // Pos of camera during menu
    public Transform cameraPosCharacterSelection;                               // Pos of camera during char selection
    public Transform cameraPosRound;                                            // Pos of camera during round
    public Transform cameraPosGameOver;                                         // Pos of camera when the game's over
    public RuntimeAnimatorController runtimeAnimatorController;                 // Player Animation controller
    public Avatar avatar;                                                       // Player Avatar

    public MenuNavigation menuNavigation;

    public string cancelButton = "Fire2_P1";

    public int readyCount;                                                      // Num of players ready
    public int noPlayerCount = 4;                                               // Num of players not playing

    public TextMeshPro timerText;                                                  // Timer displayer during round
    public TextMeshPro winnerText;                                                 // Text displayed after round

    public float setTimer = 60.0f;                                              // Round lenght in seconds
    private float timer;                                                        // Timer counting floats
    public int timeLeft;                                                        // Time displayed on timer
    public int numberOfRounds;                                                  // How many rounds are played
    public int roundNum;                                                        // Number of current round

    public bool roundStarted;                                                    // Is the round started
    public bool roundFinished;                                                   // Is the round finished

    public int NumOfPlayersNeededToStartGame_DEBUG;                             // Debug value for how many players needed to start game

    public int countDownTimer;
    public bool displayCountDownTimer;

    private void Start()
    {
        camera.GetComponent<CameraController>().MoveCamera(cameraPosMainMenu);  // Start from the main menu

        menuNavigation.enabled = true;

        // Disable all player controls during character selection.
        DisablePlayerControls();

        timer = setTimer;

        scenes[0] = true;                                                       // Set the scene to Main Menu

        foreach (var cs in cs)
        {
            cs.UseTestGameManager = true;
        }

        EnableOrDisableCS(false);                                               // Disable cs
    }

    private void Update()
    {
        if (scenes[0])
        {
            // MAIN MENU

            DisablePlayerControls();
            camera.GetComponent<CameraController>().MoveCamera( cameraPosMainMenu );
            menuNavigation.enabled = true;
            EnableOrDisableCS(false);

            if ( menuNavigation.optionConfirmed ){
                switch (menuNavigation.optionIndex){
                    case 0:    // Case option 1 = Character Selection

                        // TODO: Round Amount Selection

                        menuNavigation.enabled = false;
                        ChangeScene(1);    // Scene 1 = Character Selection
                        
                        break;
                    case 1:
                        //Enter Options
                        break;
                    case 2:
                        //Enter Credits
                        break;
                    case 3:
                        //Enter Exit
                        break;
                }
            }
        }

        if (scenes[1])
        {
            // CHARACTER SELECTION

            EnableOrDisableCS(true);

            camera.GetComponent<CameraController>().MoveCamera( cameraPosCharacterSelection );
            //Add animators (?)
            Debug.Log("CS not ready.");
            if (readyCount >= NumOfPlayersNeededToStartGame_DEBUG && noPlayerCount == (4 - readyCount))
            {
                Debug.Log("CS ready.");
                // // Redo character selection
                // if (CountDownTimer(3, true)) {
                    
                //     for (int i = 0; i < cs.Length; i++)
                //     {
                //         cs[i].UnConfirm();  //Undo character confirmation on all players.
                //     }
                // //Start round
                // } else {
                //     ChangeScene(2);
                //     AddAnimatorsToPlayers(readyCount);  //Add animators to players
                // }

                ChangeScene(2);
                //EnableOrDisableCS(false);
                //AddAnimatorsToPlayers(readyCount);  //Add animators to players
            }

            // Return to the main menu
            if (Input.GetButtonDown(cancelButton)) {
                Debug.Log("Back to mainmenu");
                ChangeScene(0);
            }
                
        }

        if (scenes[2])
        {
            // ROUND

            camera.GetComponent<CameraController>().MoveCamera( cameraPosRound );
            
            //CountDownTimer(3, false);           //Timer before round start, no cancelling
            StartGame(readyCount);
            
            roundStarted = true;
            roundNum++;
            roundFinished = false;

            timerText.text = "";
            winnerText.text = "";

            

            // Game logic until timer runs out

            if (timer >= 0.0f && !roundFinished)
            {
                timer -= Time.deltaTime;
                timeLeft = System.Convert.ToInt32(timer);                      // Convert float to int to get seconds
                timerText.text = timeLeft.ToString();
            }

            // Winning condition = the player with the most chickens when timeLeft = 0.
            if (timeLeft == 0)
            {
                roundFinished = true;                                               // Set the round to finished


                DisablePlayerControls();                                            // Stop the players from moving
                timer = setTimer;                                                   // Reset the timer for next round
                timeLeft = System.Convert.ToInt32(timer);                           // Reset TimeLeft

                timerText.text = "Time's up!";
                //winnerText.text = "Player " + playerNum + " wins with " + winningScore + " chickens!";
            }

            DisablePlayerControls();
            ChangeScene(3);
        }

        if (scenes[3])
        {
            // ROUND SCORE

            RoundOver();
            // Display round score (who won? who 2nd 3rd and 4th)
            // timer like 5 seconds

            CountDownTimer(5, false);

            if(roundNum == numberOfRounds) {
                ChangeScene(4); //Game Over
            } else {
                ChangeScene(2); //Next Round
            }
        }

        if (scenes[4])
        {
            // GAME OVER

            camera.GetComponent<CameraController>().MoveCamera( cameraPosGameOver );
            // Display who won
            CountDownTimer(10, false);
            GameOver();
            
            //  if ( press main menu )
            //  ChangeScene[0]
            //  if ( press play again )
            //  ChangeScene[2]
        }
    }

    public void CanStartGame() {

    }

    void StartGame(int playerCount)                                                 // Player position and scripts and camera
    {
        for (int i = 0; i < playerCount; i++)
        {
            players[i].transform.position = spawnPoints[i].transform.position;      // Teleport to starting position 
            players[i].GetComponent<Player>().enabled = true;                       // Enable Player script
        }

        //camera.GetComponent<CameraController>().MoveCamera(cameraPosRound);      // Move camera to Game view
    }

    public void GameOver() {
        // TODO: Display winners in gameOver Screen

        ChangeScene(0);
    }

    private void DisablePlayerControls()                                            // Disable player GameObject
    {
        foreach (var player in players)
        {
            player.GetComponent<Player>().enabled = false;
        }
    }

    private void EnableOrDisableCS(bool enable) {
        
        if(enable) {
            foreach (var cs in cs)
            {
                cs.enabled = true;
            }
        } else {
            foreach (var cs in cs)
            {
                cs.enabled = false;
            }
        }
        
    }

    public void AddAnimatorsToPlayers(int numOfPlayers){
        switch (numOfPlayers)
            {
                case 1:
                    AddAnimator(0);
                    players[1].SetActive(false);
                    players[2].SetActive(false);
                    players[3].SetActive(false);
                    break;
                case 2:
                    AddAnimator(0);
                    AddAnimator(1);
                    players[2].SetActive(false);
                    players[3].SetActive(false);
                    
                    break;
                case 3:
                    AddAnimator(0);
                    AddAnimator(1);
                    AddAnimator(2);
                    players[3].SetActive(false);
                    break;
                case 4:
                    AddAnimator(0);
                    AddAnimator(1);
                    AddAnimator(2);
                    AddAnimator(3);
                    break;
            }
    }


    // Adds an Animator, AnimationController and an Avatar to the player[index]
    public void AddAnimator(int index)
    {
        // Finds the selected model through cs (CharacterSelector) and add's the Animator component to it.
        Animator animator = cs[index].transform.GetChild(cs[index].selectedCharacter).gameObject.AddComponent<Animator>() as Animator;
        animator.runtimeAnimatorController = runtimeAnimatorController;
        animator.avatar = avatar;
    }

    public void ChangeScene(int newScene)                                       // Changes the scene boolean
    {
        // bool mainMenu;          // 0
        // bool characterSelect;   // 1
        // bool round;             // 2
        // bool roundScore;        // 3
        // bool gameOver;          // 4

        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i] == true)
                scenes[i] = false;
        }

        scenes[newScene] = true;
    }

    public bool CountDownTimer(int amount, bool enableCancel){      // Returns true if countdown was cancelled

        displayCountDownTimer = true;   // Display the timer onscreen.
        float i = 0;
        countDownTimer = amount;

        while(amount > 0){
            if(enableCancel){
                if (Input.GetButtonDown(cancelButton)){
                    return true;
                }
            }
            while(i < 1){
                i += Time.deltaTime;
            }
            countDownTimer -= 1;
        }
        displayCountDownTimer = false;  // Hide the timer.
        return false;
    }

    public void RoundOver() {
        GetRoundScore();

        // TODO: Display Score
    }

    public void GetRoundScore(){

        int[] scores = new int[4];

        // Fill scores[] with players' chicken counts
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].activeSelf)
                scores[i] = players[i].GetComponent<Tower>().chickenCount;
        }

        System.Array.Sort(scores);  //Sort the array from smallest to highest

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].activeSelf) {
                //If the player has 0 chickens, score = 0
                if ( players[i].GetComponent<Tower>().chickenCount == 0) {
                    players[i].GetComponent<Player>().score = 0;
                }
                // Else, give score according to ranking and player count
                else if (players[i].GetComponent<Tower>().chickenCount == scores[3]) {
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
        
        //Add round score to playerscore
        for (int i = 0; i < playerScores.Length; i++)
        {
            playerScores[i] += players[i].GetComponent<Player>().score;
        }
    }
}