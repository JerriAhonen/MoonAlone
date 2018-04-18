﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : MonoBehaviour
{
    public float numberInTower;
    private Tower tower;
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

    private GameObject _originatingPlayer;
    private float _gravity;
    private float _launchAngle;
    private float _launchVelocity;
    private float _flyTime;
    private Vector3 _verticalTrajectory;
    private Vector3 _horizontalTrajectory;
    public bool isThrown = false;
    public bool isFalling = false;
    public string _groundLayer;
    public string _pickUpLayer;
    public bool _isGrounded = false;

    public GameObject mainCamera;
    private ParticleSystem _featherParticles;
    private ParticleSystem _trailParticles;

    private float offset = 0.5f;

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

        _groundLayer = "Ground";
        _pickUpLayer = "PickUp";

        _featherParticles = this.transform.Find("FlyingFeathers").GetComponent<ParticleSystem>();
        _trailParticles = this.transform.Find("WhiteTrail").GetComponent<ParticleSystem>();
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
            if (_featherParticles.isStopped || _trailParticles.isStopped) {
                _featherParticles.Play();
                _trailParticles.Play();
            }

			animControl.SetInteger ("AnimParam", 1);
        }

		if (isInTower) {
			animControl.SetInteger ("AnimParam", 2);
            if (tower != null)
                MovementInTower();
        }

        if (_isGrounded) {
            isThrown = false;
            isFalling = false;

            _flyTime = 0;
            
            // Unity gravity is turned back on for easy walk about and bouncing.
            GetComponent<Rigidbody>().useGravity = true;

            _featherParticles.Stop();
            _trailParticles.Stop();

            _isGrounded = false;
        }
    }

    private void FixedUpdate() {
        if (isThrown || isFalling) {
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

    public void MovementInTower() 
    {
        if (tower.GetChickenUnderneath(this.gameObject) == null) {
            return;
        }

        GameObject chickenUnderneath = tower.GetChickenUnderneath(this.gameObject);

        // Vector3 diff = transform.position - chickenUnderneath.transform.position;

        // diff.y = 0.0f;
        // transform.position = chickenUnderneath.transform.position + diff.normalized * offset;

        // transform.position += new Vector3(0.0f, 1 * tower.GetChickenIndex(this.gameObject),0);

        // Apply that offset to get a target position.
        Vector3 targetPosition = chickenUnderneath.transform.localPosition * offset;

        // Keep our y position unchanged.
        targetPosition.y = transform.localPosition.y;

        // Smooth follow.    
        transform.position += (targetPosition - transform.localPosition) * 0.1f;
        
        //transform.position = new Vector3(0.0f, verticalOffset, 0.0f) + player.position + diff.normalized * distanceWanted;


        // Vector3 newPos = new Vector3(
        //     chickenUnderneath.transform.position.x , 
        //     transform.position.y, 
        //     chickenUnderneath.transform.position.z
        // );

        //newPos.x = objectUnderneath.transform.position.x - offset;

        //newPos = newPos * Mathf.Sin(Time.deltaTime);

        //transform.position = newPos;
        //transform.Translate(newPos, Space.Self);
















    }

    public void SetTower(Tower tower)
    {
        this.tower = tower;
    }

    // The chicken flies through the air either when thrown or falling.
    void Fly() {
        // Set flight's vertical trajectory. Gravity affects the vertical trajectory at every point in fly time.
        _verticalTrajectory.y = _launchVelocity * Mathf.Sin(_launchAngle * Mathf.Deg2Rad) - _gravity * _flyTime;
        // Time spent in flight always increases.
        _flyTime += Time.deltaTime;

        transform.position += _verticalTrajectory * Time.deltaTime;
        transform.position += _horizontalTrajectory * Time.deltaTime;
    }

    // Set the parameters for flight either when thrown or falling.
    public void SetFlight(bool toBeThrown, bool flyFar, GameObject originatingPlayer) {
        // Remember who threw you or whose tower you fell from.
        _originatingPlayer = originatingPlayer;
        
        // NOTE! Rocket launch curved trajectory chickens can be achieved by using minus gravity!
        _gravity = 50f;
        
        if (toBeThrown) {
            _launchAngle = 30f;
            
            if (flyFar) {
                _launchVelocity = 17f;
            } else {
                _launchVelocity = 11f;
            }

            isThrown = true;
        } else {
            _launchAngle = 10f;
            _launchVelocity = 8f;

            // Introduce more angular drag so that Unity Ridigbody physics don't make the chickens go apeshit.
            // Trying to make the Rigidbodies NOT react with their own forces proved difficult.
            GetComponent<Rigidbody>().drag = 3f;

            isFalling = true;
        }

        // Reset fly time just in case.
        _flyTime = 0;
        
        // Set flight's horizontal trajectory. The horizontal trajectory stays constant through flight.
        _horizontalTrajectory = transform.forward * _launchVelocity * Mathf.Cos(_launchAngle * Mathf.Deg2Rad);
    }

    private void OnCollisionEnter(Collision collision) {
        // If a tower chicken collides with a thrown chicken and the thrown chicken
        // did not originate from the player whose tower the chicken is in, scatter 
        // chickens.
        if (isInTower) {
            GameObject collisionObject = collision.gameObject;

            if (collisionObject.GetComponent<Chicken>() != null) {
                Chicken collidingChicken = collisionObject.GetComponent<Chicken>();

                if (collidingChicken.isThrown && (collidingChicken._originatingPlayer != _originatingPlayer)) {
                    if (GetComponentInParent<Tower>() != null) {
                        //FMODUnity.RuntimeManager.PlayOneShot("event:/Other Sounds/HITTOBE!", mainCamera.transform.position);

                        GetComponentInParent<Tower>().Scatter(_originatingPlayer);
                    }
                }
            }
        }

        if (isFalling || isThrown) {
            GameObject collisionObject = collision.gameObject;

            if (collisionObject.layer == LayerMask.NameToLayer(_pickUpLayer)) {
                // Introduce drag so chickens don't bounce as much.
                GetComponent<Rigidbody>().drag = 2f;
                collisionObject.GetComponent<Rigidbody>().drag = 2f;
            }
            
            if (collisionObject.layer == LayerMask.NameToLayer(_groundLayer)) {
                _isGrounded = true;
            }
        }
    }
}
