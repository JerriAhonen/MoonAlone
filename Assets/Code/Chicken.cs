using System.Collections;
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
    
    private float time;
    private float movementTimer = 5;

    private Vector3 newPos = Vector3.zero;

    public GameObject _originatingPlayer;
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
    public string _terrainLayer;
    public bool _isGrounded = false;

    public Camera mainCamera;
    private ParticleSystem _featherParticles;
    private ParticleSystem _trailParticles;
    private ParticleSystem _cloudParticles;

    private float offset = 0.5f;
    private Vector3 _followOffset = new Vector3(1,0,0);

    public int mood;    //0 = Normal, 1 = Loving, 2 = Fearfull

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
        mainCamera = Camera.main;

        _groundLayer = "Ground";
        _pickUpLayer = "PickUp";
        _terrainLayer = "Terrain";

        _featherParticles = transform.Find("FlyingFeathers").GetComponent<ParticleSystem>();
        _trailParticles = transform.Find("WhiteTrail").GetComponent<ParticleSystem>();
        _cloudParticles = transform.Find("DropDownBurst").GetComponent<ParticleSystem>();

        if (Random.value > 0.8)         //20% chance
        {
            mood = 2;
        }
        else if (Random.value > 0.6)    //20% chnce
        {
            mood = 1;
        }
        else                            //60% chance
        {
            mood = 0;
        }
    }

    private void Update()
    {
        time += Time.deltaTime;

        if (!isInTower && !isThrown && !isFalling)
        {
            switch (mood)
            {
                case 0:     //0 = Normal
                    if (time > movementTimer)
                    {
                        CalculateRandomLocation(wanderDistance);
                        time = 0f;
                        movementTimer = Random.Range(5, 10);
                    }

                    Wander(newPos);
                    Rotate();
                    animControl.SetInteger("AnimParam", 0);
                    break;
                case 1:     //1 = Loving
                            // get newPos from playerPos - position.
                    if (time > movementTimer)
                    {
                        CalculateRandomLocation(wanderDistance);
                        time = 0f;
                        movementTimer = Random.Range(5, 10);
                    }

                    Wander(newPos);
                    Rotate();

                    animControl.SetInteger("AnimParam", 0);
                    break;
                case 2:     //2 = Fearfull
                            //Get new pos from position - player pos.
                    if (time > movementTimer)
                    {
                        CalculateRandomLocation(wanderDistance);
                        time = 0f;
                        movementTimer = Random.Range(5, 10);
                    }

                    Wander(newPos);
                    Rotate();

                    animControl.SetInteger("AnimParam", 0);
                    break;
            }
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

            _originatingPlayer = null;
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


    void Rotate()
    {
        Vector3 direction = (newPos - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void CalculateRandomLocation(float max)
    {
        float x = Random.Range(-max, max);
        float z = Random.Range(-max, max);

        if (transform.position.x + x > -20f
            && transform.position.x + x < 20f
            && transform.position.z + z > -12f
            && transform.position.z + z < 12f)
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

        // Apply that offset to get a target position.
        Vector3 targetPosition = chickenUnderneath.transform.localPosition + _followOffset;

        // Keep our y position unchanged.
        targetPosition.y = transform.localPosition.y;

        //This cancels all tower movement
        //if ((targetPosition - transform.localPosition).magnitude > 0.1f)
        
        // Smooth follow.    
        transform.localPosition += (targetPosition - transform.localPosition) * 0.1f;
    }

    public void SetTower(Tower tower, bool isInTower, GameObject towerOwner)
    {
        this.tower = tower;
        this.isInTower = isInTower;
        _originatingPlayer = towerOwner;
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
                _launchVelocity = 30f;
            } else {
                _launchVelocity = 17f;
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

        StartCoroutine(IgnoreTower(GetComponent<Rigidbody>()));

        // Reset fly time just in case.
        _flyTime = 0;
        
        // Set flight's horizontal trajectory. The horizontal trajectory stays constant through flight.
        _horizontalTrajectory = transform.forward * _launchVelocity * Mathf.Cos(_launchAngle * Mathf.Deg2Rad);
    }

    // Ignores tower chicken rigidbodies that interfere with flight trajectory during launch.
    IEnumerator IgnoreTower(Rigidbody rigid) {
        rigid.isKinematic = true;

        yield return new WaitForSeconds(0.2f);

        rigid.isKinematic = false;
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
                        GetComponentInParent<Player>().isHit = true;

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
                GetComponent<Rigidbody>().drag = 3f;
            }
            
            if (collisionObject.layer == LayerMask.NameToLayer(_terrainLayer)) {
                _isGrounded = true;

                _cloudParticles.Play();
            }
        }
    }
}
