using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

	public string _pickUpLayer = "PickUp";
	public GameObject mainCamera;

    public ChickenSpawner spawner;

	// Use this for initialization
	void Start () {
        mainCamera = Camera.main.gameObject;

        spawner = GameObject.Find("ChickenSpawner").GetComponent<ChickenSpawner>();
	}

    void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.layer == LayerMask.NameToLayer(_pickUpLayer)){
			Destroy(col.gameObject);
            //FMODUnity.RuntimeManager.PlayOneShot("event:/Chicken Sounds/ChickenThrowScream", mainCamera.transform.position);

            spawner.SpawnChicken();
		}
	}
}
