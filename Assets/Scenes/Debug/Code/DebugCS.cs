using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCS : MonoBehaviour {


	public string confirmButton;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown(confirmButton)) {
			Debug.Log("Confirm button pressed.");
		}
	}
}
