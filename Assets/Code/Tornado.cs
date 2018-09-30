﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : MonoBehaviour {

    private float time;
    private float movementTimer = 1;

    [SerializeField]
    private float wanderDistance;

    private Vector3 newPos = Vector3.zero;

    private Camera _mainCamera;

    // Use this for initialization
    private void Start () {
        newPos = transform.position;

        _mainCamera = Camera.main;
    }
	
	// Update is called once per frame
	private void Update () {
        time += Time.deltaTime;

        if (time > movementTimer) {
            SetMovement();
        }

        Wander(newPos);
    }

    private void SetMovement() {
        do {
            CalculateRandomLocation(wanderDistance);
        } while (newPos.z > 6f);

        time = 0f;
        movementTimer = Random.Range(1, 3);
    }

    private void Wander(Vector3 movement) {
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

        if (collision.gameObject.layer == LayerMask.NameToLayer("PickUp")) {
            GameObject collisionObject = collision.gameObject;

            // Basically only chickens on the ground get affected.
            if (collisionObject.GetComponent<Chicken>() != null && collisionObject.GetComponent<Chicken>().originatingPlayer == null) {
                collisionObject.GetComponent<Chicken>().SetFlight(true, false, null);
                FMODUnity.RuntimeManager.PlayOneShot("event:/Action Sounds/action_throw_cartoon", _mainCamera.transform.position);
            }
        }
    }
}
