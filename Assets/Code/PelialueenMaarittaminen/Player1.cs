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

    public float throwSpeed = 10f;
    public GameObject thrownChicken;
    public bool isThrown = false;
    public bool isRising = true;

    public string horizontal = "Horizontal_P1";
    public string vertical = "Vertical_P1";
    public string fire1Button = "Fire1_P1";
    public string fire2Button = "Fire2_P1";
    public string jumpButton = "Jump_P1";

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        tower = GetComponent<Tower>();
    }

    void Update()
    {
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
            thrownChicken = tower.RemoveChicken();

            isThrown = true;
        }

        if (isThrown && (thrownChicken != null)) {
            Throw(thrownChicken);
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

    // Throws the chicken from the tower. HAS NOT REACHED ITS FINAL FORM!
    void Throw(GameObject chicken) {
        //Quaternion frontFacing = Quaternion.LookRotation(transform.forward, Vector3.up);

        //Quaternion throwRotation = Quaternion.Slerp(transform.rotation, frontFacing, 5f * Time.deltaTime);

        //chicken.transform.rotation = frontFacing;

        chicken.transform.position += (chicken.transform.forward * throwSpeed * Time.deltaTime);

        if ((chicken.transform.position.y < 6f) && isRising) {
            
            chicken.transform.position += (chicken.transform.up * throwSpeed * Time.deltaTime);
        }

        if (chicken.transform.position.y > 5f) {
            isRising = false;
        }

        if (!isRising) {
            chicken.transform.position -= (chicken.transform.up * throwSpeed * Time.deltaTime);
        }

        if (chicken.transform.position.y < 1f) {
            isThrown = false;
            isRising = true;
        }
    }
}
