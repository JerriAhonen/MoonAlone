using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuState {

	GameManagerTest gmt;

    public MainMenuState(GameManagerTest gmt)
    {
		this.gmt = gmt;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	public void Update () {
		
		if (Input.GetKeyDown(KeyCode.Space))    // Insert mainmenu option 1 here.
		{   
			Debug.Log("Exit MainMenu");
			Exit();
		}
	}

	void Exit () {
	}
}
