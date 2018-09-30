using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public bool debugAnimations = false;

    public float setMovementSpeed;
    public float movementSpeed;
    public float turningSpeed = 800.0f;
    public float gravity = 16.0f;
    public float jumpForce = 8.0f;
    public float verticalVelocity;
    public float downwardsFallMultiplier;
    private int currentAnimationParam = 0;
    public bool isMoving;
    public int score;

    private Vector3 movement;

    private Tower _tower;
    public GameObject chicken;

    private CharacterController _controller;
	private Animator _animControl;
    public CharacterSelection cs;

    public string _pickUpLayer = "PickUp";
    public string _playerLayer = "Player";

    private float _pressTime = 0;
    private float _animTimer;
    private float _throwTimer = 0;
    private bool _readyToThrow = true;
    private bool _throwFar = false;
    private GameObject _enemy;
    private Vector3 _throwDirection;

    public string horizontal;
    public string vertical;
    public string fireShortButton;
    public string fireLongButton;
    //public string jumpButton = "Jump_P1";

    public bool isHit = false;
    public bool isIncapacitated = false;

    private GameObject _hitEffect;
    private GameObject _hitBirdEffect;
    private GameObject _chargeEffect;

    private bool _isWindingUp = false;
    private bool _isThrowing = false;
    private bool _throwNow = false;

    private Camera _mainCamera;
    public Camera playerCamera;
    public float shakeTime = 0.1f;
    float timeStop = 0;
    float shakeAmount = 0.1f;
    Vector3 originPosition;

    private FMOD.Studio.EventInstance _run;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _tower = GetComponent<Tower>();
		_animControl = gameObject.GetComponentInChildren<Animator>();

        _hitEffect = transform.Find("Hit").gameObject;
        _hitBirdEffect = transform.Find("Rotating chickens").gameObject;
        _chargeEffect = transform.Find("ChargeShot").gameObject;

        _mainCamera = Camera.main;
        movementSpeed = setMovementSpeed;

        _run = FMODUnity.RuntimeManager.CreateInstance("event:/Player Sounds/player_run");
    }

    void Update()
    {
        if (_animControl == null)
            _animControl = gameObject.GetComponentInChildren<Animator>();

        if (!isIncapacitated) {
            
            if (_tower.chickenCount > 5)
            {
                movementSpeed = setMovementSpeed - ((_tower.chickenCount - 5) / 4);
            }
            
            Move();

            // If a pickuppable chicken has collided with the player.
            if (chicken != null) {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Chicken Sounds/chicken_pickup", _mainCamera.transform.position);

                _tower.AddChicken(gameObject, chicken);
                Destroy(chicken);
            }

            if (_tower.chickenCount > 0) {
                Throw();
            }

            if (isHit) {
                PlayAnimation(4);

                StartCoroutine(GetHit());
            }
        }

        // Here instead of LateUpdate since it makes throw input response feel a bit faster.
        if (_isThrowing) {
            PlayAnimation(3);

            // Reset throw timer.
            _throwTimer = 0f;

            _isThrowing = false;

            _readyToThrow = true;
        }
        
        if (Time.time < timeStop)
        {
            Vector3 rand = Random.insideUnitSphere;
            _mainCamera.transform.localPosition = originPosition + rand * shakeAmount;
            playerCamera.transform.localPosition = originPosition + rand * shakeAmount;
        }
    }

    private void LateUpdate() {
        if(isMoving && _isWindingUp) {
            PlayAnimation(6);
        }

        if(isMoving && !_isWindingUp) {
            PlayAnimation(1);
        }

        if(!isMoving && _isWindingUp) {
            PlayAnimation(5);
        }

        if(!isMoving && !_isWindingUp) {
            PlayAnimation(0);
        }
        
        // Makes sure throw animation doesn't get stuck.
        if(!_isThrowing && _isWindingUp && (_tower.chickenCount == 0)) {
            _isWindingUp = false;
        }

        // Makes sure charge effect doesn't get stuck as active.
        if(_chargeEffect.activeInHierarchy && (_tower.chickenCount == 0)) {
            _chargeEffect.SetActive(false);
        }
    }

    void Throw()
    {
        if (Input.GetButtonDown(fireShortButton) && _readyToThrow && !isIncapacitated && !Input.GetButton(fireLongButton)) {
            _throwFar = false;
            _throwNow = true;
        } else if (!Input.GetButton(fireShortButton)) {
            // Initialize press time with the moment in time the fire button was pressed down.
            if (Input.GetButtonDown(fireLongButton) && _readyToThrow && !isIncapacitated) {
                _pressTime = Time.time;

                _chargeEffect.SetActive(true);

                _isWindingUp = true;
            }

            // Calculate how long the fire button was pressed.
            if (Input.GetButtonUp(fireLongButton) && _readyToThrow && _isWindingUp) {

                _chargeEffect.SetActive(false);

                _isWindingUp = false;

                _pressTime = Time.time - _pressTime;

                // If the press time was long, throw far.
                if (_pressTime > 1.5f) {
                    _throwFar = true;
                    _throwNow = true;
                } else {
                    _throwNow = false;
                    _throwFar = false;
                }
            }
        }
        
        _throwTimer += Time.deltaTime;

        // If the throw button has been pressed, there are chickens in the tower 
        // and it has been over the limit since the last throw, throw a chicken.
        if (_throwNow && (_throwTimer > 0.3f) && _readyToThrow)                // Check if there are chickens to throw
        {
            _isThrowing = true;
            _readyToThrow = false;
            _throwNow = false;

            // If an enemy has triggered the aim collider, throw at the enemy 
            // and "forget" the enemy from autoaim. Else throw forward.
            if (_enemy != null) {
                _throwDirection = _enemy.transform.position - transform.position;

                _enemy = null;
            } else {
                _throwDirection = transform.forward;
            }

            // Remove chicken from the tower to be thrown.
            _tower.RemoveChicken(_throwDirection, true, _throwFar, gameObject);

            //FMODUnity.RuntimeManager.PlayOneShot("event:/Action Sounds/action_throw", mainCamera.transform.position);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Action Sounds/action_throw_cartoon", _mainCamera.transform.position);

            //FMODUnity.RuntimeManager.PlayOneShot("event:/Player Sounds/Throw", mainCamera.transform.position);
            //FMODUnity.RuntimeManager.PlayOneShot("event:/Other Sounds/Throw", mainCamera.transform.position);
        }
    }

    IEnumerator GetHit() {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Action Sounds/action_hit", _mainCamera.transform.position);
        //FMODUnity.RuntimeManager.PlayOneShot("event:/Player Sounds/Hit1", mainCamera.transform.position);

        ShakeNow();

        isHit = false;
        isIncapacitated = true;

        ResetPlayer();

        _hitEffect.SetActive(true);
        _hitBirdEffect.SetActive(true);

        yield return new WaitForSeconds(1f);

        _hitEffect.SetActive(false);
        _hitBirdEffect.SetActive(false);
        isIncapacitated = false;

        PlayAnimation(0);

        movementSpeed = setMovementSpeed;
    }

    public void ShakeNow() {
        originPosition = _mainCamera.transform.position;
        timeStop = Time.time + shakeTime;
    }

    public void ResetPlayer() {
        isMoving = false;

        _chargeEffect.SetActive(false);
        _isWindingUp = false;

        _throwFar = false;
        _throwNow = false;

        _isThrowing = false;
        _readyToThrow = true;
        _throwTimer = 0f;

        _run.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    void Move()
    {
        float moveHorizontal = Input.GetAxis(horizontal);                       // Horizontal Input
        float moveVertical = Input.GetAxis(vertical);                           // Vertical Input
        //bool jump = Input.GetButtonDown(jumpButton);                            // Jump Input

		if (moveHorizontal == 0 && moveVertical == 0) {                          // If player not moving
            isMoving = false;

            _run.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		else 
		{
            isMoving = true;

            FMOD.Studio.PLAYBACK_STATE state;
            _run.getPlaybackState(out state);

            if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
                _run.start();
        }

        Jump(false);                                                             // New Jump() Method

        Vector3 verticalMovement = new Vector3(0, verticalVelocity, 0);         // Get vertical movement in Vector3 form

        movement = new Vector3(moveHorizontal, 0.0f, moveVertical);             // Get the movement Vector3
        movement = Vector3.ClampMagnitude(movement, 1.0f);                      // Eliminate faster diagonal movement

        //Only Update the player's rotation if he's moving. This way we keep the rotation.
        if (moveHorizontal != 0 || moveVertical != 0)
        {
            Rotate();
        }
        _controller.Move(movement * movementSpeed * Time.deltaTime);             // Move player on X and Z
        _controller.Move(verticalMovement * Time.deltaTime);                     // Move player on Y
    }

    void Jump(bool jump)
    {
        if (jump && _controller.isGrounded)
        {
            if (_tower.chickenCount > 5)                                         // Check if not too many chickens
            {
                verticalVelocity = jumpForce / 4f;                              // Decrease jump height
                Debug.Log("Cannot Jump, too many chicken!");
            }
            else
            {
                //FMODUnity.RuntimeManager.PlayOneShot("event:/Player Sounds/Jump", mainCamera.transform.position);

                verticalVelocity = jumpForce;
                PlayAnimation(2);                                               // Play Jump animation
            }
        }

        if (verticalVelocity > -50)                                              // No need to go smaller than this
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
                if (!hit.gameObject.GetComponent<Chicken>().IsThrown 
                        && 
                    !hit.gameObject.GetComponent<Chicken>().IsFalling) {
                        chicken = hit.gameObject;
                }
            }
            
        }
    }
    
    private void OnTriggerStay(Collider collider) {
        // If another player triggers the aim collider, remember the enemy for autoaim.
        if(collider.gameObject.layer == LayerMask.NameToLayer(_playerLayer)) {
            if (!(collider is BoxCollider)) {
                _enemy = collider.gameObject;
            }
        }

        // If a chicken triggers the collider, player has been spotted.
        if (collider.gameObject.layer == LayerMask.NameToLayer(_pickUpLayer)) {
            if (collider.gameObject.GetComponent<Chicken>() != null) {
                Chicken detectedChicken = collider.gameObject.GetComponent<Chicken>();

                if (!detectedChicken.IsThrown && !detectedChicken.IsFalling && detectedChicken.Mood != 2 && !detectedChicken.SpottedPlayer) {
                    detectedChicken.SpotPlayer(gameObject);
                }
            }
        }
    }

    // If another player exits the aim collider, drop the enemy from autoaim.
    private void OnTriggerExit(Collider collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer(_playerLayer)) {
            if (_enemy != null) {
                _enemy = null;
            }
        }
    }

    // Play animation and return the animation length.
    public void PlayAnimation(int param)                                   // Changes current animation
    {
        //Debug.Log("Set animation to " + param);

        if (_animControl != null && param != currentAnimationParam)
        {
            
            _animControl.SetInteger("AnimParam", param);                     // Set AnimParam to param
            
            currentAnimationParam = param;

            if (debugAnimations) 
            {
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
                    case 4:
                        Debug.Log("Set animation to Fall");
                        break;
                    case 5:
                        Debug.Log("Set animation to Wind Up Idle");
                        break;
                    case 6:
                        Debug.Log("Set animation to Wind Up Run");
                        break;

                }
            }
            
        }
    }
}
                                                                                // Idle = 0
                                                                                // Run = 1
                                                                                // Jump = 2
                                                                                // Throw = 3 
                                                                                // Fall = 4
                                                                                // Wind Up Idle = 5
                                                                                // Wind Up Run = 6