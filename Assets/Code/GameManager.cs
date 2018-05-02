using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    
    public GameObject[] players = new GameObject[4];                            // List of all players
    public int[] playerScores = new int[4];

    private GameObject[] _startChickens = new GameObject[12];
    private bool _startChickensEnabled = false;
    
    public RuntimeAnimatorController runtimeAnimatorController;                 // Player Animation controller
    public Avatar avatar;                                                       // Player Avatar

    public int readyCount;                                                      // Num of players ready
    public int noPlayerCount = 4;                                               // Num of players not playing

    public TextMeshPro timerText;                                               // Timer displayer during round
    public Canvas canvasRoundEnd;
    public TextMeshProUGUI transitionText;
    
    public Canvas canvasRoundStart;
    public TextMeshProUGUI gameStartTimerText;

    public float setTimer = 60.0f;                                              // Round lenght in seconds
    private float _timer;                                                        // Timer counting floats
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

    FMOD.Studio.EventInstance menuMusic;
    FMOD.Studio.EventInstance levelMusic;

    private void Start()
    {
        curRoundNum = PlayerPrefs.GetInt("CurrentRoundNumber");
        numberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers");
        _timer = setTimer;
        
        _displayRoundEndCanvas = false;
        canvasRoundEnd.gameObject.SetActive(false);

        levelMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music/level");

        if (GameObject.Find("MainMenu") != null)
            menuMusic = GameObject.Find("MainMenu").GetComponent<MainMenu>().menuMusic;

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

    private void Update()
    {
        if(!_playerGOsEnabled)
        {
            EnablePlayerGOs(numberOfPlayers);
            _playerGOsEnabled = true;
        }

        // Logic for when GAME RUNNING

        if(!roundStarted) 
        {
            gameStartTimerText.text = _startTime.ToString();
        }
        else
        {
            if (!_startChickensEnabled)
            {
                foreach(GameObject chicken in _startChickens)
                {
                    chicken.gameObject.SetActive(true);
                    chicken.GetComponent<Chicken>().isFalling = true;
                }
                _startChickensEnabled = true;
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
                GetRoundScore();

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
            canvasRoundEnd.gameObject.SetActive(true);
            if (PlayerPrefs.GetInt("CurrentRoundNumber") < NumOfRounds)
                transitionText.text = "Time until next round: " + countdownTime;
            else
                transitionText.text = "Time until main menu: " + countdownTime;
        }

        if (roundFinished && cooldownTimerFinished)
        {
            Debug.Log("EndRound();");
            _playerGOsEnabled = false;
            EndRound();
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

    public void GetRoundScore()
    {
        int[] scores = new int[numberOfPlayers];

        for (int i = 0; i < scores.Length; i++)
        {
            if (players[i].activeSelf)
                scores[i] = players[i].GetComponent<Tower>().chickenCount;
            else
                Debug.Log("GetRoundScore(); Player" + i + " isn't active");
        }

        System.Array.Sort(scores);

        for (int i = 0; i < scores.Length; i++)
        {
            string playerScore = "Player" + i + "Score";

            if (players[i].GetComponent<Tower>().chickenCount == 0)
            {
                PlayerPrefs.SetInt(playerScore, PlayerPrefs.GetInt(playerScore) + 0);
            }


            else if (players[i].GetComponent<Tower>().chickenCount == scores[scores.Length - 1])
            {
                PlayerPrefs.SetInt(playerScore, PlayerPrefs.GetInt(playerScore) + 3);
            }


            else if (players[i].GetComponent<Tower>().chickenCount == scores[scores.Length - 2])
            {
                PlayerPrefs.SetInt(playerScore, PlayerPrefs.GetInt(playerScore) + 2);
            }


            else if (players[i].GetComponent<Tower>().chickenCount == scores[scores.Length - 3])
            {
                PlayerPrefs.SetInt(playerScore, PlayerPrefs.GetInt(playerScore) + 1);
            }


            else if (players[i].GetComponent<Tower>().chickenCount == scores[scores.Length - 4])
            {
                PlayerPrefs.SetInt(playerScore, PlayerPrefs.GetInt(playerScore) + 0);
            }
        }

        for (int i = 0; i < playerScores.Length; i++)
        {
            playerScores[i] = PlayerPrefs.GetInt("Player" + i + "Score");
        }
    }


        //TODO: MAKE THIS WORK
        //public void GetRoundScore(){

        //    int[] scores = new int[4];

        //    // Fill scores[] with players' chicken counts
        //    for (int i = 0; i < players.Length; i++)
        //    {
        //        if (players[i].activeSelf)
        //            scores[i] = players[i].GetComponent<Tower>().chickenCount;
        //    }

        //    System.Array.Sort(scores);

        //    for (int i = 0; i < players.Length; i++)
        //    {
        //        if (players[i].activeSelf) {
        //            if ( players[i].GetComponent<Tower>().chickenCount == 0) {
        //                players[i].GetComponent<Player>().score = 0;
        //            } else if (players[i].GetComponent<Tower>().chickenCount == scores[3]) {
        //                players[i].GetComponent<Player>().score = readyCount - 1;
        //            } else if (players[i].GetComponent<Tower>().chickenCount == scores[2]) {
        //                players[i].GetComponent<Player>().score = readyCount - 2;
        //            } else if (players[i].GetComponent<Tower>().chickenCount == scores[1]) {
        //                players[i].GetComponent<Player>().score = readyCount - 3;
        //            } else if (players[i].GetComponent<Tower>().chickenCount == scores[0]) {
        //                players[i].GetComponent<Player>().score = readyCount - 4;
        //            }
        //        }
        //    }

        //    for (int i = 0; i < playerScores.Length; i++)
        //    {
        //        playerScores[i] += players[i].GetComponent<Player>().score;
        //    }

        //}

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
            //When all rounds are played, reset the roundNumber to 1.
            PlayerPrefs.SetInt("CurrentRoundNumber", 1);
            SceneManager.LoadScene("MainMenu");

            levelMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            menuMusic.release();
            levelMusic.release();

            Destroy(GameObject.Find("MainMenu"));
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
            yield return new WaitForSeconds(1f);
            
            canvasRoundStart.gameObject.SetActive(false);
        }
    }
}