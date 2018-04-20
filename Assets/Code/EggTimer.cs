using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggTimer : MonoBehaviour {

    public float time;
    public float speed;

	// Use this for initialization
	void Start () {
        time = 60f;
        speed = 10f;
	}
	
	// Update is called once per frame
	void Update () {
        time -= Time.deltaTime;

        Rotate();

        //Debug.Log(time);
	}

    void Rotate()
    {
        //transform.rotation = Vector3.up * time * 60;
        transform.Rotate(Vector3.forward, speed * Time.deltaTime);
    }
}
