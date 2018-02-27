using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    //TODO: Make this work
    public bool[] chosenCharacters = new bool[6];

    public GameObject[] players = new GameObject[4];
    public CharacterSelection[] cs = new CharacterSelection[4];
    public GameObject[] spawnPoints = new GameObject[4];

    public Camera camera;
    public Transform cameraPosMainMenu;
    public Transform cameraPosCharacterSelection;
    public Transform cameraPosGameplay;

    public int readyCount;
    public int noPlayerCount = 4;
    
    public bool gameStarted;

    public int NumOfPlayersNeededToStartGame_DEBUG;

    private void Start()
    {
        camera.GetComponent<CameraController>().MoveCamera(cameraPosCharacterSelection);

        //Disable all player controls during character selection.
        foreach (var player in players)
        {
            player.GetComponent<Player>().enabled = false;
        }
    }

    private void Update()
    {
        if (!gameStarted)
        {
            if (readyCount >= NumOfPlayersNeededToStartGame_DEBUG && noPlayerCount == (4 - readyCount))
            {
                //Debug.Log("Start Game!");
                StartGame(readyCount);
                gameStarted = true;
            }
        }
    }

    void StartGame(int playerCount)
    {
        for (int i = 0; i < playerCount; i++)
        {
            players[i].transform.position = spawnPoints[i].transform.position; 
            players[i].GetComponent<Player>().enabled = true;
        }

        camera.GetComponent<CameraController>().MoveCamera(cameraPosGameplay);
    }

    
}
