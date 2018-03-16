using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float movementSpeed = 7.0f;
    public float turningSpeed = 800.0f;
    public float gravity = 16.0f;
    public float jumpForce = 8.0f;
    public float verticalVelocity;
    public float downwardsFallMultiplier;
    private int currentAnimationParam = 0;
    public bool isMoving;


    private Vector3 movement;

    public Tower tower;
    public GameObject chicken;

    public CharacterController controller;
	public Animator animControl;
    public MeshRenderer renderer;
    public CharacterSelection cs;

    public string _pickUpLayer = "PickUp";
    public string _playerLayer = "Player";

    private float _pressTime = 0;
    private float _throwTimer = 0;
    private bool _throwFar = false;
    private GameObject _enemy;
    private Vector3 _throwDirection;

    public string horizontal = "Horizontal_P1";
    public string vertical = "Vertical_P1";
    public string fire1Button = "Fire1_P1";
    public string fire2Button = "Fire2_P1";
    public string jumpButton = "Jump_P1";

    public GameObject mainCamera;
    
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        renderer = GetComponent<MeshRenderer>();
        tower = GetComponent<Tower>();
		animControl = gameObject.GetComponentInChildren<Animator>();
        mainCamera = GameObject.Find("Main Camera");
    }

    void Update()
    {
        if (animControl == null)
        {
            animControl = cs.animator;
            //Debug.Log("animControl = " + animControl);
        }
        
        Move();                                                                 // Player movement
        Throw();                                                                // Player throw
    }

    void Throw()
    {
        // Initialize press time with the moment in time the fire button was pressed down.
        if (Input.GetButtonDown(fire1Button)) {
            _pressTime = Time.time;
        }

        bool throwIt = Input.GetButtonUp(fire1Button);                        // Throw Input
        
        // Calculate how long the fire button was pressed.
        if (throwIt){
            _pressTime = Time.time - _pressTime;

            // If the press time was long, throw far.
            if (_pressTime > 0.2f) {
                _throwFar = true;
            } else {
                _throwFar = false;
            }
        }

        _throwTimer += Time.deltaTime;

        // If a pickuppable chicken has collided with the player.
        if (chicken != null)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Chicken Sounds/ChickenPickUpSuprise", mainCamera.transform.position);

            tower.AddChicken();                                                 // Adds chicken to Tower
            Destroy(chicken);                                                   // Destroys picked up chicken from scene
        }

        // If the throw button has been pressed, there are chickens in the tower 
        // and it has been over a second since the last throw, throw a chicken.
        if (throwIt && (tower.chickenCount > 0) && (_throwTimer > 1f))                // Check if there are chickens to throw
        {
            //FMODUnity.RuntimeManager.PlayOneShot("event:/Other Sounds/THROWTOBE!", mainCamera.transform.position);

            // If an enemy has triggered the aim collider, throw at the enemy 
            // and "forget" the enemy from autoaim. Else throw forward.
            if (_enemy != null) {
                _throwDirection = _enemy.transform.position - transform.position;

                _enemy = null;
            } else {
                _throwDirection = transform.forward;
            }

            tower.RemoveChicken(_throwDirection, true, _throwFar);           // Removes chicken from tower, Instantiates new and Throws it
            PlayAnimation(3);                                                // Plays Throw Animation

            // Reset throw timer.
            _throwTimer = 0f;
        }
    }

    void Move()
    {
        float moveHorizontal = Input.GetAxis(horizontal);                       // Horizontal Input
        float moveVertical = Input.GetAxis(vertical);                           // Vertical Input
        bool jump = Input.GetButtonDown(jumpButton);                            // Jump Input

		if (moveHorizontal == 0 && moveVertical == 0) {                          // If player not moving
            PlayAnimation (0);                                                   // Play's Idle animation
            isMoving = false;
		}
		else 
		{
			PlayAnimation(1);                                                   // Play's Run animation
            isMoving = true;
		}
        
        Jump(jump);                                                             // New Jump() Method

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

    void Jump(bool jump)
    {
        if (jump && controller.isGrounded)
        {
            if (tower.chickenCount > 5)                                         // Check if not too many chickens
            {
                verticalVelocity = jumpForce / 4f;                              // Decrease jump height
                Debug.Log("Cannot Jump, too many chicken!");
            }
            else
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Player Sounds/Jump", mainCamera.transform.position);

                verticalVelocity = jumpForce;
                PlayAnimation(2);                                               // Play Jump animation
            }
        }

        if(verticalVelocity > -50)                                              // No need to go smaller than this
        {
            verticalVelocity -= gravity * Time.deltaTime;                       // Always decrease verticalVelocity

            if (verticalVelocity < 0)                                           // Fall faster than go up
            {
                verticalVelocity -= gravity * downwardsFallMultiplier * Time.deltaTime;
            }
        }
    }

    void Rotate()
    {
        float step = turningSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(movement), step);
    }

    // If player hits something on the pick up layer, do things.
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.gameObject.layer == LayerMask.NameToLayer(_pickUpLayer)) {
            
            // If the hit object is a chicken and it is not thrown or falling, set that object for pick up.
            if (hit.gameObject.GetComponent<Chicken>() != null) {
                if (!hit.gameObject.GetComponent<Chicken>().isThrown 
                        && 
                    !hit.gameObject.GetComponent<Chicken>().isFalling) {
                        chicken = hit.gameObject;
                }
            }
            
        }
    }

    // If another player triggers the aim collider, remember the enemy for autoaim.
    private void OnTriggerStay(Collider collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer(_playerLayer)) {
            _enemy = collision.gameObject;
        }
    }

    // If another player exits the aim collider, drop the enemy from autoaim.
    private void OnTriggerExit(Collider collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer(_playerLayer)) {
            _enemy = null;
        }
    }

    public void PlayAnimation(int param)                                        // Changes current animation
    {
        if (animControl != null && param != currentAnimationParam)
        {
            animControl.SetInteger("AnimParam", param);                     // Set AnimParam to param 
            currentAnimationParam = param;

            switch (param)
            {
                case 0:
                    Debug.Log("Set animation Idle");
                    break;
                case 1:
                    Debug.Log("Set animation Run");
                    break;
                case 2:
                    Debug.Log("Set animation Jump");
                    break;
                case 3:
                    Debug.Log("Set animation Throw");
                    break;
                
            }
        }
    }
}
                                                                                // Idle = 0
                                                                                // Run = 1
                                                                                // Jump = 2
                                                                                // Throw = 3 
