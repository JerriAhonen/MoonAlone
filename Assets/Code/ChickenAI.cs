using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenAI : MonoBehaviour {

    public int mood;    //0 = Normal, 1 = Loving, 2 = Fearfull

    //public float speed = 5;
    //public float directionChangeInterval = 1;
    //public float maxHeadingChange = 30;

    //CharacterController controller;
    //float heading;
    //Vector3 targetRotation;

    public Animator animControl;
    private float time;
    private float movementTimer = 5;

    private Vector3 newPos = Vector3.zero;
    

    // Use this for initialization
    void Start () {

        animControl = gameObject.GetComponent<Animator>();
        newPos = transform.position;

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
            
            // Set random initial rotation
           // heading = Random.Range(0, 360);
            //transform.eulerAngles = new Vector3(0, heading, 0);

            //StartCoroutine(NewHeading());
        }
	}
	
	// Update is called once per frame
	void Update () {

        time += Time.deltaTime;

        switch (mood)
        {
            case 0:
                //Wander();
                //if (!isInTower && !isThrown && !isFalling)
                //{
                    if (time > movementTimer)
                    {
                        //CalculateRandomLocation(wanderDistance);
                        time = 0f;
                        movementTimer = Random.Range(5, 10);
                    }
                //}

                Wander(newPos);
                Rotate();
                animControl.SetInteger("AnimParam", 0);
                break;
            case 1:

                break;
            case 2:

                break;
        }
	}

    private void CalculateRandomLocation(float max)
    {
        float x = Random.Range(-max, max);
        float z = Random.Range(-max, max);

        if (transform.position.x + x > -20f
            && transform.position.x + x < 20f
            && transform.position.z + z > -12
            && transform.position.z + z < 12)
        {
            newPos = new Vector3(transform.position.x + x, 1f, transform.position.z + z);
        }
    }

    public void Wander(Vector3 movement)
    {
        transform.position = Vector3.MoveTowards(transform.position, movement, 0.5f * Time.deltaTime);
    }
    
    void Rotate()
    {
        Vector3 direction = (newPos - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    


    
}
