using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGameManager : MonoBehaviour {


	public bool[] scenes = new bool[5];
	public int currentScene = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
                Debug.Log("Switch scene");
				currentScene++;
				if (currentScene == scenes.Length) {
					currentScene = 0;
				}
				
                ChangeScene(currentScene);
            }
	}

	public void ChangeScene(int newScene)                                       // Changes the scene boolean
    {
        // bool mainMenu;          // 0
        // bool characterSelect;   // 1
        // bool round;             // 2
        // bool roundScore;        // 3
        // bool gameOver;          // 4

        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i] == true)
                scenes[i] = false;
        }

        scenes[newScene] = true;
		Debug.Log("Current scene = " + newScene);
    }
}
