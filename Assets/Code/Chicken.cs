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

    private float _gravity;
    private float _launchAngle;
    private float _v0;
    private float _flyTime;
    private Vector3 _verticalTrajectory;
    private Vector3 _horizontalTrajectory;
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

        _gravity = 80f;
        _launchAngle = 30f;
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
        // Set flight's vertical trajectory.
        _verticalTrajectory.y = _v0 * Mathf.Sin(_launchAngle * Mathf.Deg2Rad) - _gravity * _flyTime;
        
        _flyTime += Time.deltaTime;

        transform.position += _verticalTrajectory * Time.deltaTime;
        transform.position += _horizontalTrajectory * Time.deltaTime;
        
        if (transform.position.y < 0.1f) {
             isThrown = false;
             isFalling = false;
             _flyTime = 0;
        }
    }

    // Set the parameters for flight (throw / fall).
    public void SetFlight(bool toBeThrown, bool flyFar) {
        if (toBeThrown) {
            if (flyFar) {
                _v0 = 22f;
            } else {
                _v0 = 16f;
            }

            isThrown = true;
        } else {
            // TODO: Make falling look like falling instead of an explosion, doesn't really use Fly() as much as just Rigidbodies being Rigidbodies at the moment.
            isFalling = true;
        }

        _flyTime = 0;
        
        // Set flight's horizontal trajectory.
        _horizontalTrajectory = transform.forward * _v0 * Mathf.Cos(_launchAngle * Mathf.Deg2Rad);
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

    // THE CHICKEN LAUNCH FLIGHT:
    //
    // void Fly() {
    //     _flyTime += Time.deltaTime;
        
    //     if (verticalVelocity > -50) {                                           // No need to go smaller than this
    //         verticalVelocity -= gravity * Time.deltaTime;                       // Always decrease verticalVelocity

    //         if (verticalVelocity > 0) {                                         // Fall faster than go up
    //             verticalVelocity += gravity * upwardsMultiplier * Time.deltaTime;
    //         }
    //     }

    //     Vector3 verticalMovement = new Vector3(0, verticalVelocity, 0);         // Get vertical movement in Vector3 form

    //     transform.position += (transform.forward * throwForce * Time.deltaTime);

    //     if (_flyTime < 0.1f) {
    //         transform.position += (verticalMovement * Time.deltaTime);
    //     } else {
    //         transform.position -= (verticalMovement * Time.deltaTime);
    //     }
        
    //     if (transform.position.y < 0.1f) {
    //         isThrown = false;
    //         isFalling = false;
    //         _flyTime = 0;
    //     }
    // }
}
