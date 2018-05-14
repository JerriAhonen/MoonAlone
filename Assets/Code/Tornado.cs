using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : MonoBehaviour {

    private float time;
    private float movementTimer = 1;
    public float wanderDistance;

    private Vector3 newPos = Vector3.zero;

    private Camera _mainCamera;

    public Transform tornadoPath;
    private List<Transform> tornadoPoints;

    // Use this for initialization
    void Start () {
        newPos = transform.position;

        _mainCamera = Camera.main;

        foreach (Transform child in tornadoPath) {
            //tornadoPoints.Add(child);
        }
    }
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;

        if (time > movementTimer) {
            CalculateRandomLocation(wanderDistance);
            time = 0f;
            movementTimer = Random.Range(1, 3);
        }

        Wander(newPos);
    }

    public void Wander(Vector3 movement) {
        transform.position = Vector3.MoveTowards(transform.position, movement, 2f * Time.deltaTime);
    }

    private void CalculateRandomLocation(float max) {
        float x = Random.Range(-max, max);
        float z = Random.Range(-max, max);

        if (transform.position.x + x > -20f && transform.position.x + x < 20f && transform.position.z + z > -12 && transform.position.z + z < 12) {
            newPos = new Vector3(transform.position.x + x, 1f, transform.position.z + z);
        }
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            GameObject collisionObject = collision.gameObject;

            if (collisionObject.GetComponent<Player>() != null) {
                collisionObject.GetComponent<Player>().isHit = true;

                if (collisionObject.GetComponent<Tower>() != null) {
                    collisionObject.GetComponent<Tower>().Scatter(collision.gameObject);
                }
            }
        }

        if(collision.gameObject.layer == LayerMask.NameToLayer("PickUp")) {
            GameObject collisionObject = collision.gameObject;

            // Basically only chickens on the ground get affected.
            if (collisionObject.GetComponent<Chicken>() != null && collisionObject.GetComponent<Chicken>()._originatingPlayer == null) {
                collisionObject.GetComponent<Chicken>().SetFlight(true, false, null);
                //FMODUnity.RuntimeManager.PlayOneShot("event:/Other Sounds/Throw", _mainCamera.transform.position);
            }
        }
    }
}
