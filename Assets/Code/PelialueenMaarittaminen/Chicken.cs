using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : MonoBehaviour
{

    public float numberInTower;
    public float yPos;
    public float wanderDistance;
    public bool isMoving;
    public bool isInTower;

    public bool IsInTower {
        set
        {
            isInTower = value;
        }
    }


    private float time;
    private float movementTimer = 5;

    private Vector3 newPos = Vector3.zero;

    public float throwDistance = 8f;
    public float throwHeight = 5f;

    public bool isThrown = false;
    public bool isRising = false;
    public string _pickUpLayer;

    public Chicken(int numberInTower, float yPos)
    {
        this.numberInTower = numberInTower;
        this.yPos = yPos;

        //TODO: Possibly Instatiate the chickens here.
    }

    private void Start()
    {
        newPos = transform.position;
    }

    private void Update()
    {
        time += Time.deltaTime;

        if (!isInTower && !isThrown)
        {

            if (time > movementTimer)
            {
                CalculateRandomLocation(wanderDistance);
                time = 0f;
                movementTimer = Random.Range(5, 10);
            }

            Wander(newPos);
        }

        if (isThrown) {
            Fly();
        }
    }

    public void Move(Vector3 dir)
    {
        transform.Translate(dir);

    }

    public void Wander(Vector3 movement)
    {
        transform.position = Vector3.MoveTowards(transform.position, movement, 0.5f * Time.deltaTime);
    }

    private void CalculateRandomLocation(float max)
    {
        float x = Random.Range(-max, max);
        float z = Random.Range(-max, max);
        
        if (transform.position.x + x > -20f && transform.position.x + x < 20f && transform.position.z + z > -12 && transform.position.z + z < 12)
        {
            newPos = new Vector3(transform.position.x + x, 1f, transform.position.z + z);
        }
    }

    // The chicken flies through the air.
    void Fly() {
        //Quaternion frontFacing = Quaternion.LookRotation(transform.forward, Vector3.up);

        //Quaternion throwRotation = Quaternion.Slerp(transform.rotation, frontFacing, 5f * Time.deltaTime);

        //chicken.transform.rotation = frontFacing;

        transform.position += (transform.forward * throwDistance * Time.deltaTime);
        transform.position += (transform.up * throwHeight * Time.deltaTime);

        //if((transform.position.y < 5f) && isRising) {

        //    transform.position += (transform.up * Time.deltaTime);
        //}

        //if(transform.position.y > 4f) {
        //    isRising = false;
        //}

        //if(!isRising) {
        //    transform.position -= (transform.up * Time.deltaTime);
        //}

        if (transform.position.y < 1f) {
            isThrown = false;
        }
    }

    // Set the parameters for throw.
    public void SetThrow() {
        isThrown = true;
        isRising = true;
        throwDistance = 8f;
        throwHeight = 5f;
    }

    private void OnCollisionEnter(Collision collision) {
        // If a tower chicken collides with a thrown chicken, scatter chickens.
        if (isInTower) {
            if (collision.gameObject.GetComponent<Chicken>() != null) {
                if (collision.gameObject.GetComponent<Chicken>().isThrown) {
                    GetComponentInParent<Tower>().Scatter();        // CAUSES NULL REFERENCE EXCEPTION WHEN THROWEES CHICKENS GET THROWN BACK TOWARDS THROWER
                }
            }
        }
    }
}
