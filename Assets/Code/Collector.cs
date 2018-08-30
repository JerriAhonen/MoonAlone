using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour {

	public GameObject[] path = new GameObject[4];
	public ScoreManager scoreManager;

	Vector3 newDestination;
	private bool goClockWise = true;
	public float speed;
	private int i = 0;

	GameObject originatingPlayer;
	private bool receivedChicken;
    public GameObject FloatingCollectorPointText;

    private Animator anim;
    private CharacterController controller;
    private Camera _mainCamera;

    [SerializeField]
    private LightBallPooling lightBallPool;

    public ChickenSpawner spawner;

    public GameObject[] scoreBoards = new GameObject[4];
    public GameObject pointRight;   //LightBall routing points
    public GameObject pointLeft;

    public Vector3 RandomizeIntensity = new Vector3(5f, 5f, 5f);

    // Use this for initialization
    void Start () {
		newDestination = path[0].transform.position;
        controller = GetComponent<CharacterController>();
        anim = gameObject.GetComponentInChildren<Animator>();

        _mainCamera = Camera.main;

        spawner = GameObject.Find("ChickenSpawner").GetComponent<ChickenSpawner>();
    }

    // Update is called once per frame
    void Update () {
		Move();
        
        if (receivedChicken)
		{
			TakeChicken();
            if (FloatingCollectorPointText)
            {
                ShowFloatingText();
            }
			receivedChicken = false;
		}
		
	}

    private void ShowFloatingText()
    {
        Instantiate(FloatingCollectorPointText, transform.position, Quaternion.identity, transform);
    }

	private void TakeChicken()
	{
		scoreManager.AddChickenToCollector(originatingPlayer);
	}

	/// <summary>
	/// OnCollisionEnter is called when this collider/rigidbody has begun
	/// touching another rigidbody/collider.
	/// </summary>
	/// <param name="other">The Collision data associated with this collision.</param>
	void OnCollisionEnter(Collision collision)
	{
        GameObject collisionObject = collision.gameObject;

        if (collisionObject.GetComponent<Chicken>() != null) 
	    {
			Chicken collidingChicken = collisionObject.GetComponent<Chicken>();
			if (!collidingChicken.isThrown || (collidingChicken._originatingPlayer == null))
			{
				Destroy(collision.gameObject);

                spawner.SpawnChicken();

                return;
			}
					
		originatingPlayer = collidingChicken._originatingPlayer;
		receivedChicken = true;

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



		if (Vector3.Distance(transform.position, newDestination) < 0.5f)
        {

            if (i == path.Length - 1)
            {
                
                goClockWise = false;
				doRotation = false;
            }
			else if (i == 0)
			{
                
                goClockWise = true;
				doRotation = false;
			}
			else 
			{
				doRotation = true;
			}
            if (goClockWise)
            {
                anim.SetInteger("AnimParam", 0);
                
                i++;
            } 
			else if(!goClockWise)
			{
                anim.SetInteger("AnimParam", 1);
                i--;
			}

            newDestination = path[i].transform.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, newDestination, speed * Time.deltaTime);
		
		// Vector3 heading = newDestination - transform.position;
		// heading = Quaternion.Euler(0, 90, 0) * heading;
		// //heading.Normalize();
		// Quaternion rotation = Quaternion.LookRotation(heading);
		// transform.rotation = rotation;

		Vector3 relativePos = newDestination - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
		if(doRotation)
		{
			if (goClockWise)
        		transform.rotation = rotation;
			else
				transform.rotation = Quaternion.Inverse(rotation);
		}
		
	}

    private int GetPlayerNum()
    {
        if (originatingPlayer.transform.name == "P1")
        {
            return 1;
        }
        else if (originatingPlayer.transform.name == "P2")
        {
            return 2;
        }
        else if (originatingPlayer.transform.name == "P3")
        {
            return 3;
        }
        else if (originatingPlayer.transform.name == "P4")
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

        result = lightBallPool.GetPooledObject();

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
        return lightBallPool.ReturnObject(lightBall.gameObject);
    }
}
