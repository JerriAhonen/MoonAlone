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


    public float animationTime = 5f;
    private float desiredNumber;
    private float initialNumber;
    private float currentNumber;

    public void SetNumber(float value)
    {
        //FIDDLE WITH THESE AND THE PLAYER PREFS

        initialNumber = currentNumber;
        desiredNumber = value;
    }

    public void AddToNumber(float value)
    {
        //FIDDLE WITH THESE AND THE PLAYER PREFS

        initialNumber = currentNumber;
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
	
	// Update is called once per frame
	void Update () {
        switch (playerNumber)
        {
            case 1:

                    // SetNumber(gameManager.playerScores[0]);
                    // PlayerPrefs.SetFloat("LastRoundScoreP1", desiredNumber);
                    AddToNumber(gameManager.playerScores[0] - PlayerPrefs.GetFloat("LastRoundScoreP1"));
                    PlayerPrefs.SetFloat("LastRoundScoreP1", desiredNumber);
                    //Lisää playerPrefseihin lastroundscore jotenkin


                meshPro.text = "P1: " + currentNumber.ToString() + "p";
                break;
            case 2:
                meshPro.text = "P2: " + gameManager.playerScores[1].ToString() + "p";
                break;
            case 3:
                meshPro.text = "P3: " + gameManager.playerScores[2].ToString() + "p";
                break;
            case 4:
                meshPro.text = "P4: " + gameManager.playerScores[3].ToString() + "p";
                break;
        }
    }

    private void OnApplicationQuit()
    {
        
        Debug.Log("Reset PlayerPrefs LastRoundScores");
    }
}
