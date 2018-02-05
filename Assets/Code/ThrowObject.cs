using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour {

    [SerializeField]
    private string _pickUpLayer;

    [SerializeField]
    private Transform _pickUpPoint;

    private bool _isCarried = false;
    private bool _isPickuppable = false;

    private string _pickUpButton = "Fire1";
    private string _throwButton = "Fire2";
    
    private Transform _carriedObject;

    [SerializeField]
    private float _throwForce;

	// Use this for initialization
	void Start() {
        
	}
	
	// Update is called once per frame
	void Update() {
        bool pickUp = Input.GetButton(_pickUpButton);       // Which buttons should these be?
        bool throwIt = Input.GetButton(_throwButton);

        if (_isPickuppable && pickUp) {
            PickUp(_carriedObject);
        }

        if (_isCarried && throwIt) {
            Throw(_carriedObject);
        }
	}

    private void OnCollisionEnter(Collision collision) {        // Clips through object at first for some reason, FIX IT
        
        if (collision.gameObject.layer == LayerMask.NameToLayer(_pickUpLayer)) {        // Whole thing changed to use .distance so collision detection can be skipped?

            _isPickuppable = true;

            _carriedObject = collision.gameObject.transform;
        }
    }


    private void PickUp(Transform carriedObject) {
        carriedObject.parent = transform;

        carriedObject.position = _pickUpPoint.position;
        
        carriedObject.GetComponent<Rigidbody>().isKinematic = true;

        _isPickuppable = false;

        _isCarried = true;
    }

    private void Throw(Transform carriedObject) {
        Rigidbody rigidbody = carriedObject.gameObject.GetComponent<Rigidbody>();
        
        rigidbody.isKinematic = false;

        carriedObject.parent = null;

        rigidbody.AddForce(transform.forward.normalized * _throwForce, ForceMode.Impulse);     // CHANGE THIS to use something other than rigidbody + addforce, some sort of vector calc needed
        
        _isCarried = false;
    }
}
