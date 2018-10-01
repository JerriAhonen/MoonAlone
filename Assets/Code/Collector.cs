using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collector : MonoBehaviour {

    public Text intervalDebugText;

	public GameObject[] path = new GameObject[4];
	public ScoreManager scoreManager;
    private GameManager _gameManager;

    Vector3 _newDestination;
	private bool _goClockWise = true;
	public float speed;
	private int _i = 0;
    private bool _isMoving = true;
    private float _intervalTime;
    
	GameObject _originatingPlayer;
	private bool _receivedChicken;
    public GameObject floatingCollectorPointText;

    private Animator _anim;
    private CharacterController _controller;
    private Camera _mainCamera;

    [SerializeField]
    private LightBallPooling _lightBallPool;

    public ChickenSpawner spawner;

    public GameObject[] scoreBoards = new GameObject[4];
    public GameObject pointRight;   //LightBall routing points
    public GameObject pointLeft;
    public GameObject shutDownSmoke;
    public GameObject hooverParticle;
    public GameObject noChickensSign;
    private int _playerModel;

    public Vector3 RandomizeIntensity = new Vector3(5f, 5f, 5f);

    // Use this for initialization
    void Start () {
		_newDestination = path[0].transform.position;
        _controller = GetComponent<CharacterController>();
        _anim = gameObject.GetComponentInChildren<Animator>();
        
        _mainCamera = Camera.main;
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        spawner = GameObject.Find("ChickenSpawner").GetComponent<ChickenSpawner>();
        hooverParticle.SetActive(true);
        shutDownSmoke.SetActive(false);
        noChickensSign.SetActive(false);
        _intervalTime = Random.Range(20.0f, 30.0f);
    }

    // Update is called once per frame
    void Update ()
    {
        intervalDebugText.text = "ShutDown interval: " + _intervalTime.ToString("0.00");

        if (_isMoving)
        {
            Move();
        }
        ShutDown();
        if (_receivedChicken)
		{
			TakeChicken();
            if (floatingCollectorPointText)
            {
                ShowFloatingText();
            }
			_receivedChicken = false;
		}
		
	}

    private void ShowFloatingText()
    {
        GameObject go = Instantiate(floatingCollectorPointText, transform.position, Quaternion.identity, transform);
        FloatingCollectorPointText fcpt = go.GetComponent<FloatingCollectorPointText>();
        fcpt.playerModel = _gameManager.players[GetPlayerNum() - 1].GetComponentInChildren<EnablePlayerModel>().GetModelIndex();
    }

	private void TakeChicken()
	{
		scoreManager.AddChickenToCollector(_originatingPlayer);
	}

	/// <summary>
	/// OnCollisionEnter is called when this collider/rigidbody has begun
	/// touching another rigidbody/collider.
	/// </summary>
	/// <param name="other">The Collision data associated with this collision.</param>
	void OnCollisionEnter(Collision collision)
	{
        if(!_isMoving)
        {
            return;
        }
        GameObject collisionObject = collision.gameObject;

        if (collisionObject.GetComponent<Chicken>() != null) 
	    {
			Chicken collidingChicken = collisionObject.GetComponent<Chicken>();
			if (!collidingChicken.IsThrown || (collidingChicken.OriginatingPlayer == null))
			{
				Destroy(collision.gameObject);

                spawner.SpawnChicken();

                return;
			}
					
		_originatingPlayer = collidingChicken.OriginatingPlayer;
		_receivedChicken = true;

        FMODUnity.RuntimeManager.PlayOneShot("event:/Environment Sounds/env_collector_suck", _mainCamera.transform.position);

        Destroy(collision.gameObject);
        GetLightBall(GetPlayerNum());
        spawner.SpawnChicken();
        }
		else
		{
			return;
		}
	}

	private void Move()
	{
		bool doRotation = false;
        
		if (Vector3.Distance(transform.position, _newDestination) < 0.5f)
        {

            if (_i == path.Length - 1)
            {
                
                _goClockWise = false;
				doRotation = false;
            }
			else if (_i == 0)
			{
                
                _goClockWise = true;
				doRotation = false;
			}
			else 
			{
				doRotation = true;
			}
            if (_goClockWise) //left
            {
                _anim.SetInteger("AnimParam", 0);
                
                _i++;
            } 
			else if(!_goClockWise) // right
			{
                _anim.SetInteger("AnimParam", 1);
                _i--;
			}

            _newDestination = path[_i].transform.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, _newDestination, speed * Time.deltaTime);

		Vector3 relativePos = _newDestination - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
		if(doRotation)
		{
			if (_goClockWise)
        		transform.rotation = rotation;
			else
				transform.rotation = Quaternion.Inverse(rotation);
		}
		
	}

    private void ShutDown()
    {
        _intervalTime -= Time.deltaTime;

        if(_intervalTime <= 0)
        {
            if (!_isMoving)
            {
                Invoke("SetIsMovingTrue", 2.2f);
                _intervalTime = Random.Range(10.0f, 20.0f); // Change these to define the UPTIME of the chicken collector.
                _anim.SetTrigger("Restart");
                hooverParticle.SetActive(true);
                shutDownSmoke.SetActive(false);
            }
            else
            {
                _isMoving = false;
                _intervalTime = Random.Range(6.0f, 8.0f); // Change these to define the DOWNTIME of the chicken collector.
                _anim.SetTrigger("ShutDown");
                hooverParticle.SetActive(false);
                shutDownSmoke.SetActive(true);
                noChickensSign.SetActive(true);
            }
        }
    }

    private void SetIsMovingTrue()
    {
        _isMoving = true;
        noChickensSign.SetActive(false);
    }

    private int GetPlayerNum()
    {
        if (_originatingPlayer.transform.name == "P1")
        {
            return 1;
        }
        else if (_originatingPlayer.transform.name == "P2")
        {
            return 2;
        }
        else if (_originatingPlayer.transform.name == "P3")
        {
            return 3;
        }
        else if (_originatingPlayer.transform.name == "P4")
        {
            return 4;
        }
        else
        {
            Debug.LogError("Collector: GetPlayerNum returned 0");
            return 0;
        }
    }

	/// <summary>
	/// Callback to draw gizmos that are pickable and always drawn.
	/// </summary>
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;

		Gizmos.DrawLine(path[0].transform.position, path[1].transform.position);
		//Gizmos.DrawLine(path[1].transform.position, path[2].transform.position);
		//Gizmos.DrawLine(path[2].transform.position, path[3].transform.position);
	}

    public LightBall GetLightBall(int playerNum)
    {
        GameObject result = null;

        result = _lightBallPool.GetPooledObject();

        // If the pooled object was found, return that. Otherwise just return null.
        if (result != null)
        {

            LightBall lightBall = result.GetComponent<LightBall>();
            if (lightBall == null)
            {
                Debug.LogError("LightBall component could not be found " +
                    "from the object fetched from the pool.");
            }

            result.transform.position = transform.position;
            lightBall.startPos = transform.position;
            lightBall.target = scoreBoards[playerNum - 1].transform;

            // Determine the color of the lightball
            _playerModel = _gameManager.players[GetPlayerNum() - 1].GetComponentInChildren<EnablePlayerModel>().GetModelIndex();
            lightBall.playerModel = _playerModel;

            //If the LightBall's target is on the left of the score board, use Left Routing Point. Else use the right. 
            if (playerNum == 1 || playerNum == 2)
            {
                //                                                      Adding a little randomization to the routing point location to make effect/path seem arbitary.
                lightBall.routingPoint = pointLeft.transform.position + new Vector3(Random.Range(-RandomizeIntensity.x, RandomizeIntensity.x),
                                                Random.Range(-RandomizeIntensity.y, RandomizeIntensity.y),
                                                Random.Range(-RandomizeIntensity.z, RandomizeIntensity.z)); ;
            }
            else
            {
                lightBall.routingPoint = pointRight.transform.position + new Vector3(Random.Range(-RandomizeIntensity.x, RandomizeIntensity.x),
                                                Random.Range(-RandomizeIntensity.y, RandomizeIntensity.y),
                                                Random.Range(-RandomizeIntensity.z, RandomizeIntensity.z)); ;
            }   

            result.gameObject.SetActive(true);
            
            return lightBall;
        }
        return null;
    }

    public bool ReturnLightBall(LightBall lightBall)
    {
        return _lightBallPool.ReturnObject(lightBall.gameObject);
    }
}
