using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : MonoBehaviour {
    [SerializeField] private float _wanderDistance;

    private float _numberInTower;
    private Tower _tower;
    private float _yPos;

    private bool _isMoving;
    private bool _isInTower;

	private Animator _animControl;
    
    private float _moveTime;
    private float _reactionTime;
    private float _movementTimer = 5f;
    private float _reactionTimer;

    private Vector3 _newPos = Vector3.zero;

    public GameObject originatingPlayer;

    private float _gravity;
    private float _launchAngle;
    private float _launchVelocity;
    private float _flyTime;
    private Vector3 _verticalTrajectory;
    private Vector3 _horizontalTrajectory;

    public bool isThrown = false;
    public bool isFalling = false;

    [SerializeField] private string _groundLayer;
    [SerializeField] private string _pickUpLayer;
    [SerializeField] private string _terrainLayer;

    private bool _isGrounded = false;

    private Camera _mainCamera;

    private ParticleSystem _featherParticles;
    private ParticleSystem _trailParticles;
    private ParticleSystem _cloudParticles;
    private ParticleSystem _loveParticles;
    private ParticleSystem _fearParticles;

    private float _offset = 0.5f;
    private Vector3 _followOffset = new Vector3(1,0,0);

    public int mood;    //0 = Fearful, 1 = Loving, 2 = Chill
    public bool spottedPlayer = false;
    private float _movementSpeed;

    private GameObject _player;

    private string _flyBackLayer;

    public Chicken(int numberInTower, float yPos)
    {
        this._numberInTower = numberInTower;
        this._yPos = yPos;

        //TODO: Possibly Instantiate the chickens here.
    }

    private void Start()
    {
        _newPos = transform.position;
		_animControl = gameObject.GetComponent<Animator>();
        _mainCamera = Camera.main;

        _groundLayer = "Ground";
        _pickUpLayer = "PickUp";
        _terrainLayer = "Terrain";
        _flyBackLayer = "FlyBack";

        _featherParticles = transform.Find("FlyingFeathers").GetComponent<ParticleSystem>();
        _trailParticles = transform.Find("WhiteTrail").GetComponent<ParticleSystem>();
        _cloudParticles = transform.Find("DropDownBurst").GetComponent<ParticleSystem>();
        _loveParticles = transform.Find("Hearts").GetComponent<ParticleSystem>();
        _fearParticles = transform.Find("AlertParticle").GetComponent<ParticleSystem>();

        _reactionTimer = 2f;
    }

    float distance;

    private void Update()
    {
        _moveTime += Time.deltaTime;
        _reactionTime += Time.deltaTime;

        if (spottedPlayer && mood == 1) {
            _newPos = _player.transform.position;

            distance = Vector3.Distance(_newPos, transform.position);
        }

        if (!_isInTower && !isThrown && !isFalling)
        {
            switch (mood)
            {
                case 0:     //0 = Fearful
                    if (spottedPlayer) {
                        _movementSpeed = 4f;

                        if (_reactionTime > _reactionTimer) {
                            spottedPlayer = false;      // CAN YOU BREAK AND GO TO ELSE??
                        }
                    } else {
                        if (_moveTime > _movementTimer) {
                            CalculateRandomLocation(_wanderDistance);
                            _moveTime = 0f;
                            _movementTimer = Random.Range(5, 10);

                            _animControl.SetInteger("AnimParam", 0);

                            _movementSpeed = 0.5f;
                        }
                    }

                    Wander(_newPos, _movementSpeed);
                    Rotate();

                    break;
                case 1:     //1 = Loving
                    if (spottedPlayer) {
                        if (distance < 0f || distance > 3f) {
                            spottedPlayer = false;

                            _loveParticles.Stop();
                        }
                    } else {
                        if (_moveTime > _movementTimer) {
                            CalculateRandomLocation(_wanderDistance);
                            _moveTime = 0f;
                            _movementTimer = Random.Range(5, 10);

                            _animControl.SetInteger("AnimParam", 0);

                            _movementSpeed = 0.5f;
                        }
                    }

                    Wander(_newPos, _movementSpeed);
                    Rotate();
                    
                    break;
                case 2:     //2 = Chill
                    if (_moveTime > _movementTimer) {
                        CalculateRandomLocation(_wanderDistance);
                        _moveTime = 0f;
                        _movementTimer = Random.Range(5, 10);

                        _movementSpeed = 0.5f;
                    }

                    Wander(_newPos, _movementSpeed);
                    Rotate();

                    _animControl.SetInteger("AnimParam", 0);

                    break;
            }
        }

        if (isThrown || isFalling) {
            if (_featherParticles.isStopped || _trailParticles.isStopped) {
                _featherParticles.Play();
                _trailParticles.Play();
            }

			_animControl.SetInteger ("AnimParam", 1);
        }

		if (_isInTower) {
			_animControl.SetInteger ("AnimParam", 2);
            if (_tower != null)
                MovementInTower();
        }

        if (_isGrounded) {
            isThrown = false;
            isFalling = false;

            _flyTime = 0;

            // Unity gravity is turned back on for easy walk about and bouncing.
            GetComponent<Rigidbody>().useGravity = true;

            GetComponent<Rigidbody>().drag = 3f;

            _featherParticles.Stop();
            _trailParticles.Stop();

            _isGrounded = false;

            originatingPlayer = null;
        }
    }

    private void FixedUpdate() {
        if (isThrown || isFalling) {
            Fly();
        }
    }

    private void Wander(Vector3 movement, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, movement, speed * Time.deltaTime);
    }


    private void Rotate()
    {
        Vector3 direction = (_newPos - transform.position).normalized;
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
            _newPos = new Vector3(transform.position.x + x, 1f, transform.position.z + z);
        }
    }

    public void SpottedPlayer(GameObject player) {
        this._player = player;
        spottedPlayer = true;
        _reactionTime = 0f;

        switch (mood) {
            case 0:
                _fearParticles.Play();
                _newPos = transform.position - player.transform.position;

                _animControl.SetInteger("AnimParam", 3);
                break;
            case 1:
                _loveParticles.Play();

                _movementSpeed = 3f;

                _animControl.SetInteger("AnimParam", 3);
                break;
        }
    }

    public void MovementInTower() 
    {
        if (_tower.GetChickenUnderneath(this.gameObject) == null) {
            return;
        }

        GameObject chickenUnderneath = _tower.GetChickenUnderneath(this.gameObject);

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
        this._tower = tower;
        this._isInTower = isInTower;
        originatingPlayer = towerOwner;
    }

    // The chicken flies through the air either when thrown or falling.
    private void Fly() {
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
        this.originatingPlayer = originatingPlayer;
        
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
        if (_isInTower) {
            GameObject collisionObject = collision.gameObject;

            if (collisionObject.GetComponent<Chicken>() != null) {
                Chicken collidingChicken = collisionObject.GetComponent<Chicken>();

                if (collidingChicken.isThrown && (collidingChicken.originatingPlayer != originatingPlayer)) {
                    if (GetComponentInParent<Tower>() != null) {
                        GetComponentInParent<Player>().isHit = true;

                        GetComponentInParent<Tower>().Scatter(originatingPlayer);
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

    // If a chicken touches down on a fly back zone, set flight.
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer(_flyBackLayer)) {
            SetFlight(true, false, null);
        }
    }
}
