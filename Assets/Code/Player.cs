﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float movementSpeed = 7.0f;
    public float turningSpeed = 800.0f;
    public float gravity = 16.0f;
    public float jumpForce = 8.0f;
    public float verticalVelocity;

    private Vector3 movement;

    public Tower tower;
    public GameObject chicken;

    public CharacterController controller;
	public Animator animControl;
    public MeshRenderer renderer;

    public string _pickUpLayer = "PickUp";

    public string horizontal = "Horizontal_P1";
    public string vertical = "Vertical_P1";
    public string fire1Button = "Fire1_P1";
    public string fire2Button = "Fire2_P1";
    public string jumpButton = "Jump_P1";

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        renderer = GetComponent<MeshRenderer>();
        tower = GetComponent<Tower>();
		animControl = gameObject.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Move();                                                                 // Player movement.

        bool pickUp = Input.GetButtonDown(fire2Button);                         // Pick up Input
        bool throwIt = Input.GetButtonDown(fire1Button);                        // Throw Input

        if (pickUp && (chicken != null))
        {
            tower.AddChicken();                                                 // Adds chicken to Tower
            Destroy(chicken);                                                   // Destroys picked up chicken from scene
        }
        else
        {   
            chicken = null;                                                     // Prevents player from picking up chicken after colliding with it
        }

        if (throwIt && (tower.chickenCount > 0))                                // Check if there are chickens to throw
        {
            tower.RemoveChicken(transform.forward, true);                       // Removes chicken from tower, Instantiates new and Throws it
            PlayAnimation(3);                                                   // Play's Throw Animation
        }
    }

    void Move()
    {
        float moveHorizontal = Input.GetAxis(horizontal);                       // Horizontal Input
        float moveVertical = Input.GetAxis(vertical);                           // Vertical Input
        bool jump = Input.GetButtonDown(jumpButton);                            // Jump Input

        if (moveHorizontal == 0 && moveVertical == 0)                           // If player not moving
            PlayAnimation(0);                                                   // Play's Idle animation
        else
            PlayAnimation(1);                                                   // Play's Run animation
        
		if (controller.isGrounded)
        {
            verticalVelocity = -gravity * Time.deltaTime;                       // Always decrease verticalVelocity

            if (jump)
            {
				if (tower.chickenCount > 5) {                                   // Check if not too many chickens
					verticalVelocity = jumpForce / 4f;                          // Decrease jump height
					Debug.Log ("Cannot Jump, too many chicken!");
				} else {
					verticalVelocity = jumpForce;
                    PlayAnimation(2);                                           // Play Jump animation
				}
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;                       // Always decrease verticalVelocity
        }

        Vector3 verticalMovement = new Vector3(0, verticalVelocity, 0);         // Get vertical movement in Vector3 form

        movement = new Vector3(moveHorizontal, 0.0f, moveVertical);             // Get the movement Vector3
        movement = Vector3.ClampMagnitude(movement, 1.0f);                      // Eliminate faster diagonal movement

        //Only Update the player's rotation if he's moving. This way we keep the rotation.
        if (moveHorizontal != 0 || moveVertical != 0)
        {
            Rotate();
        }
        controller.Move(movement * movementSpeed * Time.deltaTime);             // Move player on X and Z
        controller.Move(verticalMovement * Time.deltaTime);                     // Move player on Y
    }

    void Rotate()
    {
        float step = turningSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(movement), step);
    }

    // If player hits something on the pick up layer, do things.
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.gameObject.layer == LayerMask.NameToLayer(_pickUpLayer)) {

            chicken = hit.gameObject;
        }
    }

    public void PlayAnimation(int param)                                        // Changes current animation
    {
        if (animControl != null)
            animControl.SetInteger("AnimParam", param);                         // Set AnimParam to param 
    }
}
                                                                                // Idle = 0
                                                                                // Run = 1
                                                                                // Jump = 2
                                                                                // Throw = 3 
