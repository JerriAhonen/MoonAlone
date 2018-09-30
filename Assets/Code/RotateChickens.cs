using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateChickens : MonoBehaviour {

    [SerializeField] private Transform _target;
    [SerializeField] private float _orbitDistance = 10.0f;
    [SerializeField] private float _orbitDegreesPerSec = 360.0f;

    private Vector3 relativeDistance = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        if (_target != null)
        {
            relativeDistance = transform.position - _target.position;
        }
    }

    void Orbit()
    {
        if (_target != null)
        {
            // Keep us at the last known relative position
            transform.position = _target.position + relativeDistance;
            transform.RotateAround(_target.position, Vector3.up, _orbitDegreesPerSec * Time.deltaTime);
            // Reset relative position after rotate
            relativeDistance = transform.position - _target.position;
        }
    }

    void LateUpdate()
    {
        Orbit();
    }
}
