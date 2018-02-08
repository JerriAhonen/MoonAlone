using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1 : MonoBehaviour {

    public float movementSpeed = 5;
    public float turningSpeed = 360;

    private Vector3 movement;

    public Tower tower;
    public GameObject chicken;

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        float step = turningSpeed * Time.deltaTime;

        //Only Update the player's rotation if he's moving. This way we keep the rotation.
        if (moveHorizontal != 0 || moveVertical != 0)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(movement), step);
        }
        
        transform.Translate(movement * movementSpeed * Time.deltaTime, Space.World);
    }
    
}

