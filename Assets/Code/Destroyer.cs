using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

	public string _pickUpLayer = "PickUp";
	public GameObject mainCamera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// OnCollisionEnter is called when this collider/rigidbody has begun
	/// touching another rigidbody/collider.
	/// </summary>
	/// <param name="other">The Collision data associated with this collision.</param>
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.layer == LayerMask.NameToLayer(_pickUpLayer)){
			Destroy(col.gameObject);
			//FMODUnity.RuntimeManager.PlayOneShot("event:/Chicken Sounds/ChickenThrowScream", mainCamera.transform.position);
		}
	}
}
