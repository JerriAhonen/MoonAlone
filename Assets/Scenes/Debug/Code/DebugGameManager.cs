using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGameManager : MonoBehaviour {


	public bool[] scenes = new bool[5];
	private int currentScene = 0;

	public Camera camera;
	private Transform targetTransform;
	public Transform MMcamPos;
	public Transform CScamPos;
	public Transform RcamPos;
	public Transform RScamPos;
	public Transform GOcamPos;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		ChangeSceneWithSpace();

	}

	public void ChangeScene(int newScene)	// Works.
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

		//Update the camera transform location when changing scenes
		switch (newScene) {
			case 0:
				targetTransform = MMcamPos;
				break;
			case 1:
				targetTransform = CScamPos;
				break;
			case 2:
				targetTransform = RcamPos;
				break;
			case 3:
				targetTransform = RScamPos;
				break;
			case 4:
				targetTransform = GOcamPos;
				break;
		}
		//Move the camera to the new location for each scene
		camera.GetComponent<CameraController>().MoveCamera( targetTransform );	// Works.
		
    }

	private void ChangeSceneWithSpace() {
		if (Input.GetKeyDown(KeyCode.Space)) {
                Debug.Log("Switch scene");
				currentScene++;
				if (currentScene == scenes.Length) {
					currentScene = 0;
				}
				
                ChangeScene(currentScene);
        }
	}
}
