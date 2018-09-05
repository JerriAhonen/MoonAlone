using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundScore : MonoBehaviour {

    private TextMeshProUGUI meshPro;
    private GameManager gameManager;
    private ScoreManager scoreManager;
    public int playerNumber;
    private int playerModel;                //Defines the colour of the text to match the player model
    private int numOfPlayers;

    public float animationTime = 50f;
    private float desiredNumber;
    private float initialNumber;
    private float currentNumber;

    private bool numberSet = false;

    public void SetNumber(float value)
    {
        //FIDDLE WITH THESE AND THE PLAYER PREFS
        //initialNumber = PlayerPrefs.GetFloat("LastRoundScoreP"+player);
        initialNumber = currentNumber;
        desiredNumber = value;
    }

    //NOT USED ATM
    public void AddToNumber(float value, string player)
    {
        //FIDDLE WITH THESE AND THE PLAYER PREFS

        initialNumber = PlayerPrefs.GetFloat("LastRoundScoreP"+player);
        desiredNumber += value;
    }

    public void ScrollNumbers()
    {
        if (currentNumber != desiredNumber)
        {
            if (initialNumber < desiredNumber)
            {
                currentNumber += (animationTime * Time.deltaTime) * (desiredNumber - initialNumber);
                if(currentNumber >= desiredNumber)
                    currentNumber = desiredNumber;
            }
        }
    }




    // Use this for initialization
    void Start () {
        meshPro = GetComponent<TextMeshProUGUI>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        scoreManager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();
        playerModel = gameManager.players[playerNumber - 1].GetComponentInChildren<EnablePlayerModel>().GetModelIndex();
        numberSet = false;
        numOfPlayers = gameManager.numberOfPlayers;

        if (playerNumber <= numOfPlayers)
        {
            switch (playerModel)
            {
                case 1:
                    meshPro.color = new Color(0, 166, 189);
                    break;
                case 2:
                    meshPro.color = new Color(163, 0, 0);
                    break;
                case 3:
                    meshPro.color = Color.green;
                    break;
                case 4:
                    meshPro.color = Color.yellow;
                    break;
            }
        }
        else
        {
            meshPro.color = Color.gray;
        }
    }
	
	// Update is called once per frame
	void Update () {
        switch (playerNumber)
        {
            case 1:
                
                if (!numberSet)
                {
                    SetNumber(gameManager.playerScores[0]);
                    PlayerPrefs.SetFloat("LastRoundScoreP1", desiredNumber);
                    numberSet = true;
                }
                ScrollNumbers();

                meshPro.text = "P1: " + currentNumber.ToString("0") + "p";
                break;
            case 2:

                if (!numberSet)
                {
                    SetNumber(gameManager.playerScores[1]);
                    PlayerPrefs.SetFloat("LastRoundScoreP2", desiredNumber);
                    numberSet = true;
                }
                ScrollNumbers();

                meshPro.text = "P2: " + currentNumber.ToString("0") + "p";
                break;
            case 3:
                if (!numberSet)
                {
                    SetNumber(gameManager.playerScores[2]);
                    PlayerPrefs.SetFloat("LastRoundScoreP3", desiredNumber);
                    numberSet = true;
                }
                ScrollNumbers();
                meshPro.text = "P3: " + currentNumber.ToString("0") + "p";
                break;
            case 4:
                if (!numberSet)
                {
                    SetNumber(gameManager.playerScores[3]);
                    PlayerPrefs.SetFloat("LastRoundScoreP4", desiredNumber);
                    numberSet = true;
                }
                ScrollNumbers();
                meshPro.text = "P4: " + currentNumber.ToString("0") + "p";
                //meshPro.text = "P4: " + gameManager.playerScores[3].ToString() + "p";
                break;
        }
    }

    private void OnApplicationQuit()
    {
        
        Debug.Log("Reset PlayerPrefs LastRoundScores");
    }
}
