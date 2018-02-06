using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour {

    [SerializeField]
    private string _pickUpLayer;

    [SerializeField]
    private Transform _pickUpPoint;

    private bool _isCarried = false;
    private bool _isPickuppable = false;

    private string _pickUpButton = "Fire1";
    private string _dropButton = "Fire2";
    
    private Transform _pickuppableObject;

    [SerializeField]
    private float _throwForce = 25;

    [SerializeField]
    private float _groundLevel = 0.25f;

	// Use this for initialization
	void Start() {
        
	}
	
	// Update is called once per frame
	void Update() {
        bool pickUp = Input.GetButton(_pickUpButton);
        bool drop = Input.GetButton(_dropButton);

        if (_pickuppableObject != null) {
            if (_isPickuppable && pickUp) {
                PickUp(_pickuppableObject);
            }

            if (Input.GetButtonUp(_pickUpButton)) {
                _isCarried = true;
            }

            if (_isCarried && pickUp) {
                Throw(_pickuppableObject);
            }

            if (_isCarried && drop) {
                Drop(_pickuppableObject);
            }
        }
    }
    
    // If the player collides with a pick up layer object
    private void OnCollisionEnter(Collision collision) {        // Clips through object at first for some reason, FIX IT
        
        if (collision.gameObject.layer == LayerMask.NameToLayer(_pickUpLayer)) {        // Whole thing changed to use .distance so collision detection can be skipped?

            _isPickuppable = true;

            _pickuppableObject = collision.gameObject.transform;
        }
    }

    // Picks up an object to be carried.
    private void PickUp(Transform pickuppableObject) {      // Dude can pick things up with his ass! Make a separate collider for hand?
        pickuppableObject.parent = transform;

        pickuppableObject.position = _pickUpPoint.position;
        
        pickuppableObject.GetComponent<Rigidbody>().isKinematic = true;

        _isPickuppable = false;
    }

    // Throws the carried object.
    private void Throw(Transform carriedObject) {
        Rigidbody rigidbody = carriedObject.GetComponent<Rigidbody>();
        
        rigidbody.isKinematic = false;

        carriedObject.parent = null;

        rigidbody.AddForce(transform.forward.normalized * _throwForce, ForceMode.Impulse);     
        // CHANGE THIS to use something other than rigidbody + addforce, some sort of vector calc needed
        // direction has to be taken from the player

        carriedObject = null;

        _isCarried = false;
    }

    // Drops the carried object.
    private void Drop(Transform carriedObject) {
        Rigidbody rigidbody = carriedObject.GetComponent<Rigidbody>();

        rigidbody.isKinematic = false;

        carriedObject.parent = null;

        carriedObject.position = new Vector3 (carriedObject.position.x, _groundLevel, carriedObject.position.z);

        carriedObject = null;

        _isCarried = false;
    }
}
