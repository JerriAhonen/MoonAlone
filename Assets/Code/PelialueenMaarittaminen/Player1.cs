using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1 : MonoBehaviour {

    public float movementSpeed = 5.0f;
    public float turningSpeed = 800.0f;
    public float gravity = 14.0f;
    public float jumpForce = 10.0f;
    public float verticalVelocity;

    private Vector3 movement;

    public Tower tower;
    public GameObject chicken;

    public CharacterController controller;

    public string _pickUpLayer;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        tower = GetComponent<Tower>();
    }

    void Update()
    {
        Move();

        bool pickUp = Input.GetButtonDown("Fire2");
        bool throwIt = Input.GetButtonDown("Fire1");
        
        if (pickUp && (chicken != null)) {
            tower.AddChicken();
    
            Destroy(chicken);
        } else {    // prevents player from picking up a chicken after colliding with it
            chicken = null;
        }

        if (throwIt && (tower.chickenCount > 0)) {
            Throw(tower.RemoveChicken());
        }
    }

    void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        bool jump = Input.GetButtonDown("Jump");

        if (controller.isGrounded)
        {
            verticalVelocity = -gravity * Time.deltaTime;

            if (jump)
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        Vector3 verticalMovement = new Vector3(0, verticalVelocity, 0);

        movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        //Only Update the player's rotation if he's moving. This way we keep the rotation.
        if (moveHorizontal != 0 || moveVertical != 0)
        {
            Rotate();
        }
        controller.Move(movement * movementSpeed * Time.deltaTime);
        controller.Move(verticalMovement * Time.deltaTime);
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

    // Throws the chicken from the tower.
    void Throw(GameObject chicken) {
        //TODO
    }
}

