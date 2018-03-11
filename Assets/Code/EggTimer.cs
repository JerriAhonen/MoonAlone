using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggTimer : MonoBehaviour {

    public float time;


	// Use this for initialization
	void Start () {
        time = 60f;
	}
	
	// Update is called once per frame
	void Update () {
        time -= Time.deltaTime;

        Rotate();

        //Debug.Log(time);
	}

    void Rotate()
    {
        transform.eulerAngles = Vector3.up * time * 60;
    }
}
