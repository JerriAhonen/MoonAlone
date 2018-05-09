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
	private bool recievedChicken;


	// Use this for initialization
	void Start () {
		newDestination = path[0].transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Move();
		if(recievedChicken)
		{
			TakeChicken();
			recievedChicken = false;
		}
		
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
				if (!collidingChicken.isThrown)
				{
					Destroy(collision.gameObject);
					return;
				}
					
				originatingPlayer = collidingChicken._originatingPlayer;
				recievedChicken = true;
				Destroy(collision.gameObject);
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
                i++;
            } 
			else if(!goClockWise)
			{
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
		Gizmos.DrawLine(path[1].transform.position, path[2].transform.position);
		Gizmos.DrawLine(path[2].transform.position, path[3].transform.position);
	}
}
