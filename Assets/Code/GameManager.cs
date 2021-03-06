﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    
    public ScoreManager scoreManager;
    public GameObject[] players = new GameObject[4];                            // List of all players
    public int[] playerScores = new int[4];

    private GameObject[] _startChickens = new GameObject[15];
    private bool _startChickensEnabled = false;
    
    public RuntimeAnimatorController runtimeAnimatorController;                 // Player Animation controller
    public Avatar avatar;                                                       // Player Avatar

    public int readyCount;                                                      // Num of players ready
    public int noPlayerCount = 4;                                               // Num of players not playing

    public TextMeshPro timerText;                                               // Timer displayer during round
    public Canvas canvasRoundEnd;
    public Canvas canvasGameOver;
    public TextMeshProUGUI transitionText;
    public TextMeshProUGUI roundText;
    
    public Canvas canvasRoundStart;
    public TextMeshProUGUI gameStartTimerText;

    public Canvas canvasInput;

    public float setTimer = 60.0f;                                              // Round lenght in seconds
    private float _timer;                                                       // Timer counting floats
    public int timeLeft;                                                        // Time displayed on timer
    public int NumOfRounds = 2;
    public int curRoundNum;                                                     // Number of current round

    public int winningScore = -1;                                               // Highest score after round
    
    public bool roundStarted;                                                   // Is the round started
    public bool roundFinished;                                                  // Is the round finished

    public int numberOfPlayers;

    public bool cooldownTimerFinished = false;
    private int countdownTime;

    private bool _startTimerFinished = false;
    private int _startTime;

    private bool _displayRoundEndCanvas;
    private bool _displayGameStartCanvas;
    private bool _playerGOsEnabled = false;

    public EggTimer eggTimer;

    FMOD.Studio.EventInstance menuMusic;
    FMOD.Studio.EventInstance levelMusic;

    private Camera _mainCamera;

    public string confirmButton;    //Player 1 button A.      
    private bool confirmedExitToMainMenu;

    public string menuButton = "Menu";
    
    private void Start()
    {
        eggTimer = GameObject.Find("egg").gameObject.GetComponentInChildren<EggTimer>();

        curRoundNum = PlayerPrefs.GetInt("CurrentRoundNumber");
        numberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers");
        _timer = setTimer;

        if (curRoundNum == 1)
            canvasInput.enabled = true;
        else
            canvasInput.enabled = false;
        
        _displayRoundEndCanvas = false;
        canvasRoundEnd.gameObject.SetActive(false);
        canvasGameOver.gameObject.SetActive(false);

        levelMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music/music_level");

        if (GameObject.Find("MainMenu") != null)
            menuMusic = GameObject.Find("MainMenu").GetComponent<MainMenu>().menuMusic;

        _mainCamera = Camera.main;

        menuMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        levelMusic.start();

        

        int i = 0;
        foreach(GameObject chicken in GameObject.FindGameObjectsWithTag("Chicken"))
        {
            _startChickens[i] = chicken;
            i++;
            chicken.gameObject.SetActive(false);
        }

        StartCoroutine(StartTimer(3));
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

        AddAnimators(num);
    }
    
    //Called when Quiting Play mode.
    private void OnApplicationQuit()
    {
        ResetPlayerPrefs();
        Debug.Log("Reset PlayerPrefs RoundNumber and PlayerScores");
    }

    private void Update()
    {
        if(!_playerGOsEnabled && !roundFinished)
        {
            EnablePlayerGOs(numberOfPlayers);
            _playerGOsEnabled = true;
        }

        // Logic for when GAME RUNNING

        if(!roundStarted) 
        {
            EnablePlayerScripts(false);
            gameStartTimerText.text = _startTime.ToString();
            roundText.text = "Round " + curRoundNum + "/3";
        }
        else
        {
            roundText.text = "";
            if (!_startChickensEnabled)
            {
                foreach(GameObject chicken in _startChickens)
                {
                    chicken.gameObject.SetActive(true);
                    chicken.GetComponent<Chicken>().IsFalling = true;
                }
                _startChickensEnabled = true;
                EnablePlayerScripts(true);
            }
            
            if (_timer >= 0.0f && !roundFinished)
            {
                _timer -= Time.deltaTime;
                timeLeft = System.Convert.ToInt32(_timer);                       // Convert float to int to get seconds
                //if(timeLeft < 5)												// Set when the timer shows
                timerText.text = timeLeft.ToString();
            }

            // Winning condition = the player with the most chickens when timeLeft = 0.
            if (timeLeft == 0 && !roundFinished)
            {
                scoreManager.GetRoundScore();
                EnablePlayerScripts(false);

                roundFinished = true;                                           // Set the round to finished
                    
                _timer = setTimer;                                               // Reset the timer for next round
                timeLeft = System.Convert.ToInt32(_timer % 60);                  // Reset TimeLeft

                //timerText.text = "Time's up!";

                Debug.Log("Started CooldownTimer Coroutine");
                StartCoroutine(CooldownTimer(5));

                _displayRoundEndCanvas = true;
            }
        }

        if (_displayRoundEndCanvas)
        {
            if (PlayerPrefs.GetInt("CurrentRoundNumber") == NumOfRounds)
            {
                canvasGameOver.gameObject.SetActive(true);
            }
            else
            {
                canvasRoundEnd.gameObject.SetActive(true);
                
                transitionText.text = "Time until next round: " + countdownTime;
            }

            
        }

        if (roundFinished && cooldownTimerFinished)
        {
            //cooldownTimerFinished = false;
            //Debug.Log("EndRound();");
            _playerGOsEnabled = false;
            EndRound();
        }

        if (Input.GetButtonDown(menuButton)) {
            ResetPlayerPrefs();
            SceneManager.LoadScene("MainMenu");

            levelMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            menuMusic.release();
            levelMusic.release();

            Destroy(GameObject.Find("MainMenu"));
        }
    }

    public void ResetPlayerPrefs()
    {
        //Reset the round number to 1
        PlayerPrefs.SetInt("CurrentRoundNumber", 1);

        //Reset all player scores to 0 when a new game is started
        PlayerPrefs.SetInt("Player0Score", 0);
        PlayerPrefs.SetInt("Player1Score", 0);
        PlayerPrefs.SetInt("Player2Score", 0);
        PlayerPrefs.SetInt("Player3Score", 0);
    }

    private void EnablePlayerScripts(bool enable)
    {
        foreach (GameObject player in players)
        {
            if(player.activeSelf)
            {
                if (enable == false) {
                    player.GetComponent<Player>().ResetPlayer();
                }

                player.GetComponent<Player>().enabled = enable;
            }
        }
    }

    public void AddAnimators(int num)
    {
        for (int i = 0; i < num; i++)
        {
            Animator animator = players[i].GetComponentInChildren<EnablePlayerModel>().GetActivePlayerModel().AddComponent<Animator>() as Animator;
            animator.runtimeAnimatorController = runtimeAnimatorController;
            animator.avatar = avatar;
        }
    }

    
    private void EndRound()
    {
        if(PlayerPrefs.GetInt("CurrentRoundNumber") < NumOfRounds)
        {
            //Add 1 to the current roundNumber in PlayerPrefs.
            PlayerPrefs.SetInt("CurrentRoundNumber", PlayerPrefs.GetInt("CurrentRoundNumber") + 1);
            SceneManager.LoadScene("Round_Level1");

            levelMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        else
        {
            if (Input.GetButtonDown(confirmButton))
            {
                ResetPlayerPrefs();
                SceneManager.LoadScene("MainMenu");

                levelMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                menuMusic.release();
                levelMusic.release();

                Destroy(GameObject.Find("MainMenu"));
            }
            
        }
            
    }

    private IEnumerator CooldownTimer(int cooldownTime)
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

    private IEnumerator StartTimer(int time)
    {
        _startTime = time;
        canvasRoundStart.gameObject.SetActive(true);

        while (_startTime > 0)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Other Sounds/other_countdown", _mainCamera.transform.position);

            yield return new WaitForSeconds(1f);
            
            _startTime--;
            _startTimerFinished = false;
            roundStarted = false;
            Debug.Log("Time until start: " + _startTime);
        }
        if (_startTime <= 0)
        {
            Debug.Log("Start timer Finished!");
            _startTimerFinished = true;
            roundStarted = true;

            gameStartTimerText.text = "GO!";

            FMODUnity.RuntimeManager.PlayOneShot("event:/Other Sounds/other_go", _mainCamera.transform.position);

            eggTimer.enabled = enabled;

            yield return new WaitForSeconds(1f);
            canvasInput.enabled = false;
            canvasRoundStart.gameObject.SetActive(false);
        }
    }
}