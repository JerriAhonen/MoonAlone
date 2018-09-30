using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public bool debugAnimations = false;

    [SerializeField] private float _setMovementSpeed;
    private float _movementSpeed;

    [SerializeField] private float _turningSpeed = 800.0f;
    [SerializeField] private float _gravity = 10.0f;
    [SerializeField] private float _jumpForce = 5.0f;
    [SerializeField] private float _verticalVelocity;
    [SerializeField] private float _downwardsFallMultiplier;

    private int _currentAnimationParam = 0;
    private bool _isMoving;

    public bool IsMoving {
        get {
            return _isMoving;
        }
    }

    private int _score;

    public int Score {
        get {
            return _score;
        }
        set {
            _score = value;
        }
    }

    private Vector3 _movement;

    private Tower _tower;
    private GameObject _chicken;

    private CharacterController _controller;
	private Animator _animControl;

    [SerializeField] private string _pickUpLayer = "PickUp";
    [SerializeField] private string _playerLayer = "Player";

    private float _pressTime = 0;
    private float _animTimer;
    private float _throwTimer = 0;
    private bool _readyToThrow = true;
    private bool _throwFar = false;
    private GameObject _enemy;
    private Vector3 _throwDirection;

    [SerializeField] private string _horizontal;
    [SerializeField] private string _vertical;
    [SerializeField] private string _fireShortButton;
    [SerializeField] private string _fireLongButton;
    //[SerializeField] private string _jumpButton = "Jump_P1";

    private bool _isHit = false;

    public bool IsHit {
        get {
            return _isHit;
        }
        set {
            _isHit = value;
        }
    }

    private bool _isIncapacitated = false;

    private GameObject _hitEffect;
    private GameObject _hitBirdEffect;
    private GameObject _chargeEffect;

    private bool _isWindingUp = false;
    private bool _isThrowing = false;
    private bool _throwNow = false;

    private Camera _mainCamera;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private float _shakeTime = 0.2f;

    private float _timeStop = 0;
    private float _shakeAmount = 0.1f;
    private Vector3 _originPosition;

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
        _movementSpeed = _setMovementSpeed;

        _run = FMODUnity.RuntimeManager.CreateInstance("event:/Player Sounds/player_run");
    }

    void Update()
    {
        if (_animControl == null)
            _animControl = gameObject.GetComponentInChildren<Animator>();

        if (!_isIncapacitated) {
            
            if (_tower.chickenCount > 5)
            {
                _movementSpeed = _setMovementSpeed - ((_tower.chickenCount - 5) / 4);
            }
            
            Move();

            // If a pickuppable chicken has collided with the player.
            if (_chicken != null) {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Chicken Sounds/chicken_pickup", _mainCamera.transform.position);

                _tower.AddChicken(gameObject, _chicken);
                Destroy(_chicken);
            }

            if (_tower.chickenCount > 0) {
                Throw();
            }

            if (_isHit) {
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
        
        if (Time.time < _timeStop)
        {
            Vector3 rand = Random.insideUnitSphere;
            _mainCamera.transform.localPosition = _originPosition + rand * _shakeAmount;
            _playerCamera.transform.localPosition = _originPosition + rand * _shakeAmount;
        }
    }

    private void LateUpdate() {
        if(_isMoving && _isWindingUp) {
            PlayAnimation(6);
        }

        if(_isMoving && !_isWindingUp) {
            PlayAnimation(1);
        }

        if(!_isMoving && _isWindingUp) {
            PlayAnimation(5);
        }

        if(!_isMoving && !_isWindingUp) {
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
        if (Input.GetButtonDown(_fireShortButton) && _readyToThrow && !_isIncapacitated && !Input.GetButton(_fireLongButton)) {
            _throwFar = false;
            _throwNow = true;
        } else if (!Input.GetButton(_fireShortButton)) {
            // Initialize press time with the moment in time the fire button was pressed down.
            if (Input.GetButtonDown(_fireLongButton) && _readyToThrow && !_isIncapacitated) {
                _pressTime = Time.time;

                _chargeEffect.SetActive(true);

                _isWindingUp = true;
            }

            // Calculate how long the fire button was pressed.
            if (Input.GetButtonUp(_fireLongButton) && _readyToThrow && _isWindingUp) {

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

        _isHit = false;
        _isIncapacitated = true;

        ResetPlayer();

        _hitEffect.SetActive(true);
        _hitBirdEffect.SetActive(true);

        yield return new WaitForSeconds(1f);

        _hitEffect.SetActive(false);
        _hitBirdEffect.SetActive(false);
        _isIncapacitated = false;

        PlayAnimation(0);

        _movementSpeed = _setMovementSpeed;
    }

    public void ShakeNow() {
        _originPosition = _mainCamera.transform.position;
        _timeStop = Time.time + _shakeTime;
    }

    public void ResetPlayer() {
        _isMoving = false;

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
        float moveHorizontal = Input.GetAxis(_horizontal);                       // Horizontal Input
        float moveVertical = Input.GetAxis(_vertical);                           // Vertical Input
        //bool jump = Input.GetButtonDown(jumpButton);                            // Jump Input

		if (moveHorizontal == 0 && moveVertical == 0) {                          // If player not moving
            _isMoving = false;

            _run.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		else 
		{
            _isMoving = true;

            FMOD.Studio.PLAYBACK_STATE state;
            _run.getPlaybackState(out state);

            if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
                _run.start();
        }

        Jump(false);                                                             // New Jump() Method

        Vector3 verticalMovement = new Vector3(0, _verticalVelocity, 0);         // Get vertical movement in Vector3 form

        _movement = new Vector3(moveHorizontal, 0.0f, moveVertical);             // Get the movement Vector3
        _movement = Vector3.ClampMagnitude(_movement, 1.0f);                      // Eliminate faster diagonal movement

        //Only Update the player's rotation if he's moving. This way we keep the rotation.
        if (moveHorizontal != 0 || moveVertical != 0)
        {
            Rotate();
        }
        _controller.Move(_movement * _movementSpeed * Time.deltaTime);             // Move player on X and Z
        _controller.Move(verticalMovement * Time.deltaTime);                     // Move player on Y
    }

    void Jump(bool jump)
    {
        if (jump && _controller.isGrounded)
        {
            if (_tower.chickenCount > 5)                                         // Check if not too many chickens
            {
                _verticalVelocity = _jumpForce / 4f;                              // Decrease jump height
                Debug.Log("Cannot Jump, too many chicken!");
            }
            else
            {
                //FMODUnity.RuntimeManager.PlayOneShot("event:/Player Sounds/Jump", mainCamera.transform.position);

                _verticalVelocity = _jumpForce;
                PlayAnimation(2);                                               // Play Jump animation
            }
        }

        if (_verticalVelocity > -50)                                              // No need to go smaller than this
         {
            _verticalVelocity -= _gravity * Time.deltaTime;                       // Always decrease verticalVelocity

            if (_verticalVelocity < 0)                                           // Fall faster than go up
            {
                 _verticalVelocity -= _gravity * _downwardsFallMultiplier * Time.deltaTime;
            }
        }
    }

    void Rotate()
    {
        float step = _turningSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_movement), step);
    }

    // If player hits something on the pick up layer, do things.
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.gameObject.layer == LayerMask.NameToLayer(_pickUpLayer)) {
            
            // If the hit object is a chicken and it is not thrown or falling, set that object for pick up.
            if (hit.gameObject.GetComponent<Chicken>() != null) {
                if (!hit.gameObject.GetComponent<Chicken>().IsThrown 
                        && 
                    !hit.gameObject.GetComponent<Chicken>().IsFalling) {
                        _chicken = hit.gameObject;
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

        if (_animControl != null && param != _currentAnimationParam)
        {
            
            _animControl.SetInteger("AnimParam", param);                     // Set AnimParam to param
            
            _currentAnimationParam = param;

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