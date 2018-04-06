using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGameManager : MonoBehaviour {


	public bool[] scenes = new bool[5];
	private int currentScene = 0;

	public Camera MainCamera;
	private DebugCameraController cm;
	
	// Use this for initialization
	void Start () {
		cm = MainCamera.GetComponent<DebugCameraController>();
	}
	
	// Update is called once per frame
	void Update () {
		ChangeSceneWithC();






























	}

	public void ChangeScene(int newScene)	// Works 6.4.
    {
        // bool mainMenu;          // 0
        // bool characterSelect;   // 1
        // bool round;             // 2
        // bool roundScore;        // 3
        // bool gameOver;          // 4

        for (int i = 0; i < scenes.Length; i++)
        {
                scenes[i] = false;
        }

        scenes[newScene] = true;
		Debug.Log("Current scene = " + newScene);

		//Move the camera to the new location for each scene
		MoveCamera( newScene );	// Works 6.4.
    }

	// Move Camera
	public void MoveCamera(int sceneNum) {	// Works 6.4.
		cm.MoveCamera( sceneNum );
	}

	private void ChangeSceneWithC() {	// DEBUG METHOD
		if (Input.GetKeyDown(KeyCode.C)) {
                Debug.Log("Switch scene");
				currentScene++;
				if (currentScene == scenes.Length) {
					currentScene = 0;
				}
                ChangeScene(currentScene);
        }
	}
}
