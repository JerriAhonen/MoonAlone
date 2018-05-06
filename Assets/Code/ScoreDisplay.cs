using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour {

    public bool isCounter;

    private GameManager gameManager;
    public int playerNumber;                //To get the score of the right player
    private int playerModel;                //Defines the colour of the text to match the player model
    private TextMeshPro meshPro;            //The score variable
    private Tower tower;
    
	// Use this for initialization
	void Start () {
        meshPro = GetComponent<TextMeshPro>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        playerModel = gameManager.players[playerNumber - 1].GetComponentInChildren<EnablePlayerModel>().GetModelIndex();
        tower = gameManager.players[playerNumber - 1].GetComponent<Tower>();

        switch (playerModel)
        {
            case 1:
				meshPro.color = new Color(0,166,189);
                break;
            case 2:
				meshPro.color = new Color(163,0,0);
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

        if (isCounter)
        {
            meshPro.text = tower.chickenCount.ToString();
        }
    }
}
