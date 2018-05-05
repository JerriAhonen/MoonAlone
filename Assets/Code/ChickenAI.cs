using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenAI : MonoBehaviour {

    public int mood;    //0 = Normal, 1 = Loving, 2 = Fearfull

    public float speed = 5;
    public float directionChangeInterval = 1;
    public float maxHeadingChange = 30;

    //CharacterController controller;
    float heading;
    Vector3 targetRotation;

    // Use this for initialization
    void Start () {
        if (Random.value > 0.8)     //20% chance
        {
            mood = 2;
        }
        else if(Random.value > 0.6) //20% chnce
        {
            mood = 1;
        }
        else                        //60% chance
        {
            mood = 0;

            //controller = GetComponent<CharacterController>();

            // Set random initial rotation
            heading = Random.Range(0, 360);
            transform.eulerAngles = new Vector3(0, heading, 0);

            StartCoroutine(NewHeading());
        }
	}
	
	// Update is called once per frame
	void Update () {
        switch (mood)
        {
            case 0:
                Wander();
                break;
            case 1:

                break;
            case 2:

                break;
        }
	}

    void Wander()
    {
        transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * directionChangeInterval);
        var forward = transform.TransformDirection(Vector3.forward);
        //controller.SimpleMove(forward * speed);
        transform.Translate(Vector3.forward * speed);
    }
    
    IEnumerator NewHeading()
    {
        while (true)
        {
            NewHeadingRoutine();
            yield return new WaitForSeconds(directionChangeInterval);
        }
    }

    void NewHeadingRoutine()
    {
        var floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
        var ceil = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
        heading = Random.Range(floor, ceil);
        targetRotation = new Vector3(0, heading, 0);
    }


    
}
