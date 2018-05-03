using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggTimer : MonoBehaviour {
    public float time;
    public float speed;

    public float shakeSpeed;
    private float shakeTime;

    public bool shakeRight = true;
    public bool shakeLeft = false;
    public Animator animControl;

	// Use this for initialization
	void Start () {
        shakeTime = 0.01f;
        animControl = GetComponentInParent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        time -= Time.deltaTime;
        if (time >= 0)
        {
            Rotate();
            animControl.SetInteger("EggAnimParam", 0);
        }
        else {
            Shake();
        }
        

        //Debug.Log(time);
	}

    void Rotate()
    {
       
        transform.Rotate(Vector3.forward, speed * Time.deltaTime);
    }

    void Shake() {

        animControl.SetInteger("EggAnimParam", 1);
        /*if (shakeRight)
        {
            transform.Rotate(Vector3.forward, -shakeSpeed * Time.deltaTime);
            shakeTime -= Time.deltaTime;
            if (shakeTime < 0)
            {
                shakeTime = 0.001f;
                shakeLeft = true;
                shakeRight = false;
            }
        }
        else if (shakeLeft)
        {
            transform.Rotate(Vector3.forward, shakeSpeed * Time.deltaTime);
            shakeTime -= Time.deltaTime;
            if (shakeTime < 0)
            {
                shakeTime = 0.001f;
                shakeLeft = false;
                shakeRight = true;
            }
       
        }
        */
    }
}
