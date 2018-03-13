using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : MonoBehaviour
{

    public float numberInTower;
    public float yPos;
    public float wanderDistance;
    public float turningSpeed;
    public bool isMoving;
    public bool isInTower;

	public Animator animControl;

    public bool IsInTower {
        set
        {
            isInTower = value;
        }
    }

    private float time;
    private float movementTimer = 5;

    private Vector3 newPos = Vector3.zero;

    public float throwSpeed = 8f;
    public float throwHeight = 5f;

    public bool isThrown = false;
    public bool isFalling = false;
    public string _pickUpLayer = "PickUp";

    public GameObject mainCamera;

    public Chicken(int numberInTower, float yPos)
    {
        this.numberInTower = numberInTower;
        this.yPos = yPos;

        //TODO: Possibly Instantiate the chickens here.
    }

    private void Start()
    {
        newPos = transform.position;
		animControl = gameObject.GetComponent<Animator>();
        mainCamera = GameObject.Find("Main Camera");
    }

    private void Update()
    {
        time += Time.deltaTime;

        if (!isInTower && !isThrown && !isFalling)
        {

            if (time > movementTimer)
            {
                CalculateRandomLocation(wanderDistance);
                time = 0f;
                movementTimer = Random.Range(5, 10);
            }

            //Rotate(newPos);
            Wander(newPos);
			animControl.SetInteger ("AnimParam", 0);
        }

        if (isThrown || isFalling) {
            Fly();
			animControl.SetInteger ("AnimParam", 1);
        }

		if (isInTower)
			animControl.SetInteger ("AnimParam", 2);
    }

    public void Move(Vector3 dir)
    {
        transform.Translate(dir);

    }

    public void Wander(Vector3 movement)
    {
        transform.position = Vector3.MoveTowards(transform.position, movement, 0.5f * Time.deltaTime);
    }

    // Make this work
    void Rotate(Vector3 movement)
    {
        float step = turningSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(movement.normalized), step);
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
        transform.position += (transform.forward * throwSpeed * Time.deltaTime);
        transform.position += (transform.up * throwHeight * Time.deltaTime);
        
        if (transform.position.y < 0.1f) {
            isThrown = false;
            isFalling = false;
        }
    }

    // Set the parameters for flight (throw / fall).
    public void SetFlight(bool toBeThrown) {
        if (toBeThrown) {
            mainCamera = GameObject.Find("Main Camera");
            // TODO: Make throw faster
            isThrown = true;
            throwSpeed = 8f;
            throwHeight = 5f;
        } else {
            // TODO: Make falling look like falling instead of an explosion, doesn't really use Fly() as much as just Rigidbodies being Rigidbodies at the moment.
            isFalling = true;
            throwSpeed = 0f;
            throwHeight = 0f;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        // If a tower chicken collides with a thrown chicken, scatter chickens.
        if (isInTower) {
            if (collision.gameObject.GetComponent<Chicken>() != null) {
                if (collision.gameObject.GetComponent<Chicken>().isThrown) {
                    if (GetComponentInParent<Tower>() != null) {
                        //FMODUnity.RuntimeManager.PlayOneShot("event:/Other Sounds/HITTOBE!", mainCamera.transform.position);

                        GetComponentInParent<Tower>().Scatter();
                    }
                }
            }
        }
    }

    // Played when chicken is thrown off map, logic missing
    //FMODUnity.RuntimeManager.PlayOneShot("event:/Chicken Sounds/ChickenThrowScream", mainCamera.transform.position);
}
