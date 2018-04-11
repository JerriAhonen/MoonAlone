using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionState : GameManagerTest {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (csm.ready)
		{
			csm.EnableCharacterSelection(false, numOfPlayers);   
			ChangeScene(sceneState.round);
		}
	}

	void Exit (sceneState newState) {
		ChangeScene(newState);
	}
}
