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

    public ChickenSpawner spawner;

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
}
