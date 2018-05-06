using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundScore : MonoBehaviour {

    private TextMeshProUGUI meshPro;
    private GameManager gameManager;
    public int playerNumber;
    private int playerModel;                //Defines the colour of the text to match the player model

    // Use this for initialization
    void Start () {
        meshPro = GetComponent<TextMeshProUGUI>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
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
                meshPro.text = "P1: " + gameManager.playerScores[0].ToString();
                break;
            case 2:
                meshPro.text = "P2: " + gameManager.playerScores[1].ToString();
                break;
            case 3:
                meshPro.text = "P3: " + gameManager.playerScores[2].ToString();
                break;
            case 4:
                meshPro.text = "P4: " + gameManager.playerScores[3].ToString();
                break;
        }
    }
}
