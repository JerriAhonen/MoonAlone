using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CSManager : MonoBehaviour {

    public CharacterSelection[] cs = new CharacterSelection[4];
    public bool[] chosenCharacters = new bool[6];                               // List of taken players during char selection

    public int readyCount;                                                      // Num of players ready
    public int noPlayerCount = 4;                                               // Num of players not playing

    public bool canPlayWithOnePlayer;

    private Camera _mainCamera;

    private int _startTime;
    public string cancelButton = "Cancel";
    public bool isStarted = false;

    private bool tutorialCanvasTimerEnded;
    private bool tutorialCanvasTimerStarted;

    FMOD.Studio.EventInstance menuMusic;

    //public Canvas canvasGameStart;
    public TextMeshProUGUI gameStartTimerText;

    public GameObject gameRulesScreen;

    // Use this for initialization
    void Start () {
        if(GameObject.Find("MainMenu") != null)
            menuMusic = GameObject.Find("MainMenu").GetComponent<MainMenu>().menuMusic;

        _mainCamera = Camera.main;

        if (gameRulesScreen == null) {
            gameRulesScreen = GameObject.Find("SplashScreenCanvas");
        }

        gameRulesScreen.SetActive(false);
        tutorialCanvasTimerEnded = false;
        tutorialCanvasTimerStarted = false;
    }

    // Update is called once per frame
    void Update () {
        
        if (Input.GetButtonDown(cancelButton))
        {
            SceneManager.LoadScene("MainMenu");
            Destroy(GameObject.Find("MainMenu"));

            menuMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        // ************* THIS IS FOR DEBUGGING WITH ONE PLAYER ************* //

        if (canPlayWithOnePlayer)
        {
            if (cs[0].GetCharacterConfirmed()   //Player one always needs to choose a character.
        &&
        (cs[1].GetIndex() == 0 || cs[1].GetCharacterConfirmed())
        &&
        (cs[2].GetIndex() == 0 || cs[2].GetCharacterConfirmed())
        &&
        (cs[3].GetIndex() == 0 || cs[3].GetCharacterConfirmed()))
            {
                if (!isStarted)
                {
                    StartCoroutine(StartTimer(3));          // QUICK FIX FOR 61 x START

                    isStarted = true;
                }

            }
            else
            {
                gameStartTimerText.text = "";
            }
        }
        else
        // ************* THIS IS FOR THE NORMAL GAME, AT LEAST 2 PLAEYRS NEEDED ************* //
        {
            if (cs[0].GetCharacterConfirmed()   
                &&
                (cs[1].GetCharacterConfirmed())
                &&
                (cs[2].GetIndex() == 0 || cs[2].GetCharacterConfirmed())
                &&
                (cs[3].GetIndex() == 0 || cs[3].GetCharacterConfirmed()))
            {
                if (!isStarted)
                {
                    StartCoroutine(StartTimer(3));          // QUICK FIX FOR 61 x START

                    isStarted = true;
                }

            }
            else
            {
                gameStartTimerText.text = "";
            }
        }

        if (tutorialCanvasTimerEnded || (tutorialCanvasTimerStarted && Input.GetButtonDown("FireShort_P1")))
        {
            StopCoroutine("TutorialCanvasStartTimer");
            SceneManager.LoadScene("Round_Level1");
        }


    }

    private IEnumerator StartTimer(int time)
    {
        _startTime = time;

        while (_startTime > 0)
        {
            //gameStartTimerText.text = "Game starting in " + _startTime.ToString("0");

            yield return new WaitForSeconds(1f);
            
            _startTime--;
            
            Debug.Log("Time until Game start: " + _startTime);
        }
        if (_startTime <= 0)
        {
            //FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/menu_start_game", _mainCamera.transform.position);

            Debug.Log("Game start timer Finished!");

            //gameStartTimerText.text = "Game starting in " + _startTime.ToString();

            PlayerPrefs.SetInt("NumberOfPlayers", readyCount);

            //_startTime = 5;

            StartCoroutine(TutorialCanvasStartTimer(5));
            tutorialCanvasTimerStarted = true;

            gameRulesScreen.SetActive(true);

            /*
            while(_startTime > 0) {
                if (Input.GetButtonDown("FireShort_P1"))
                {
                    break;
                }

                Debug.Log("Tutorial canvas timer: " + _startTime);
                yield return new WaitForSeconds(1f);
                
                _startTime--;
            }
            */

            
        }
    }

    private IEnumerator TutorialCanvasStartTimer(int time)
    {
        _startTime = time;

        while (_startTime > 0)
        {
            Debug.Log("Tutorial canvas timer: " + _startTime);
            yield return new WaitForSeconds(1f);

            _startTime--;
        }
        tutorialCanvasTimerEnded = true;
    }
}
