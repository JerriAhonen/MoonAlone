using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

	public GameManager gameManager;
	private Tower tower;
	
	public int[] collectorChickenCount = new int[4];

	// Use this for initialization
	void Start () {
		//gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//playerNum: 1 = totalChickenCount[0];
	public void AddChickenToCollector(GameObject player)
	{
		collectorChickenCount[System.Array.IndexOf(gameManager.players, player)] += 1;
	}

	public int GetPlayerTotalChickenCount(int playerNum)
	{
		tower = gameManager.players[playerNum - 1].GetComponent<Tower>();
		return tower.chickenCount + collectorChickenCount[playerNum - 1];
	}

	public void GetRoundScore()
    {
        int[] scores = new int[gameManager.numberOfPlayers];

        for (int i = 0; i < scores.Length; i++)
        {
            if (gameManager.players[i].activeSelf)
                scores[i] = gameManager.players[i].GetComponent<Tower>().chickenCount + collectorChickenCount[i];
            else
                Debug.Log("GetRoundScore(); Player" + i + " isn't active");
        }

        System.Array.Sort(scores);

        for (int i = 0; i < scores.Length; i++)
        {
            string playerScore = "Player" + i + "Score";
			int chickenCount = gameManager.players[i].GetComponent<Tower>().chickenCount;

            if (chickenCount + collectorChickenCount[i] == 0)
            {
                PlayerPrefs.SetInt(playerScore, PlayerPrefs.GetInt(playerScore) + 0);
            }


            else if (chickenCount + collectorChickenCount[i] == scores[scores.Length - 1])
            {
                PlayerPrefs.SetInt(playerScore, PlayerPrefs.GetInt(playerScore) + 3);
            }


            else if (chickenCount + collectorChickenCount[i] == scores[scores.Length - 2])
            {
                PlayerPrefs.SetInt(playerScore, PlayerPrefs.GetInt(playerScore) + 2);
            }


            else if (chickenCount + collectorChickenCount[i] == scores[scores.Length - 3])
            {
                PlayerPrefs.SetInt(playerScore, PlayerPrefs.GetInt(playerScore) + 1);
            }


            else if (chickenCount + collectorChickenCount[i] == scores[scores.Length - 4])
            {
                PlayerPrefs.SetInt(playerScore, PlayerPrefs.GetInt(playerScore) + 0);
            }
        }

        for (int i = 0; i < gameManager.playerScores.Length; i++)
        {
            gameManager.playerScores[i] = PlayerPrefs.GetInt("Player" + i + "Score");
        }
    }
}
