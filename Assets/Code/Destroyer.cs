using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

    [SerializeField] private string _pickUpLayer = "PickUp";

	private GameObject _mainCamera;

    private ChickenSpawner _spawner;

	// Use this for initialization
	void Start () {
        _spawner = GameObject.Find("ChickenSpawner").GetComponent<ChickenSpawner>();
	}

    void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.layer == LayerMask.NameToLayer(_pickUpLayer)){
			Destroy(col.gameObject);

            _spawner.SpawnChicken();
		}
	}
}
