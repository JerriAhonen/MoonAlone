using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeEgg : MonoBehaviour
{

    public float time;
    public float speed;

    public float shakeSpeed;
    private float shakeTime;

    public bool shakeRight = true;
    public bool shakeLeft = false;

    // Use this for initialization
    void Start()
    {
        shakeTime = 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if (time >= 0)
        {
            Rotate();
        }
        else
        {
            Shake();
        }


        //Debug.Log(time);
    }

    void Rotate()
    {

       // transform.Rotate(Vector3.forward, speed * Time.deltaTime);
    }

    void Shake()
    {

        if (shakeRight)
        {
            transform.Rotate(Vector3.forward, -shakeSpeed * Time.deltaTime);
            shakeTime -= Time.deltaTime;
            if (transform.rotation.eulerAngles.z > 1)
            {
                shakeTime = 0.01f;
                shakeLeft = true;
                shakeRight = false;
            }
        }
        else if (shakeLeft)
        {
            transform.Rotate(Vector3.forward, shakeSpeed * Time.deltaTime);
            shakeTime -= Time.deltaTime;
            if (transform.rotation.eulerAngles.z < -1)
            {
                shakeTime = 0.01f;
                shakeLeft = false;
                shakeRight = true;
            }
        }
    }
}
