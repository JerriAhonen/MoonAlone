using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private Transform targetTransform;
    public bool cameraMoved = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (cameraMoved)
        {
            transform.position = Vector3.Slerp(transform.position, targetTransform.position, Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetTransform.rotation, Time.deltaTime);
            
        }
    }

    public void MoveCamera(Transform target)
    {
        cameraMoved = true;
        targetTransform = target;
    }
}
