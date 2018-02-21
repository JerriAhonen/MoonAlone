using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    //TODO: Make this work
    public bool[] chosenCharacters = new bool[6];

    public int readyCount;
    public int noPlayerCount = 4;

    private void Update()
    {
        if(readyCount >= 2 && noPlayerCount == (4 - readyCount))
        {
            Debug.Log("Start Game!");
        }
    }

    
}
