using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CSManager : MonoBehaviour {

    public CharacterSelection[] cs = new CharacterSelection[4];
    public bool[] chosenCharacters = new bool[6];                               // List of taken players during char selection

    public int readyCount;                                                      // Num of players ready
    public int noPlayerCount = 4;                                               // Num of players not playing

    public int NumOfPlayersNeededToStartGame_DEBUG;

    private Camera _mainCamera;

    // Use this for initialization
    void Start () {
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update () {
        if (readyCount >= NumOfPlayersNeededToStartGame_DEBUG && noPlayerCount == (4 - readyCount))
        {
            PlayerPrefs.SetInt("NumberOfPlayers", NumOfPlayersNeededToStartGame_DEBUG);

            SceneManager.LoadScene("Round_Level1");

            FMODUnity.RuntimeManager.PlayOneShot("event:/Chicken Sounds/ChickenCluckSeries", _mainCamera.transform.position);
        }
    }
}
