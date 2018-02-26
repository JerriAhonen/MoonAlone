using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float movementSpeed = 5.0f;
    public float turningSpeed = 800.0f;
    public float gravity = 16.0f;
    public float jumpForce = 8.0f;
    public float verticalVelocity;

    private Vector3 movement;

    public Tower tower;
    public GameObject chicken;

    public CharacterController controller;
    public MeshRenderer renderer;

    public string _pickUpLayer;

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
    }

    void Update()
    {
        //Player movement.
        Move();

        bool pickUp = Input.GetButtonDown(fire2Button);
        bool throwIt = Input.GetButtonDown(fire1Button);
        
        if (pickUp && (chicken != null)) {
            tower.AddChicken();
    
            Destroy(chicken);
        } else {    // prevents player from picking up a chicken after colliding with it
            chicken = null;
        }

        if (throwIt && (tower.chickenCount > 0)) {
            tower.ThrowChicken(transform.forward);
        }
    }

    void Move()
    {
        float moveHorizontal = Input.GetAxis(horizontal);
        float moveVertical = Input.GetAxis(vertical);
        bool jump = Input.GetButtonDown(jumpButton);

        if (controller.isGrounded)
        {
            verticalVelocity = -gravity * Time.deltaTime;

            if (jump)
            {
                if (tower.chickenCount > 5)
                    Debug.Log("Cannot Jump, too many chicken!");
                else
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
}
