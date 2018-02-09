using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAimCamera : MonoBehaviour {

    [SerializeField]
    public GameObject target;
    public float sensitivityX = 5.0f;
    public float sensitivityY = 4.0f;
    public float clampAngle = 70.0f;
    public float rotX;
    public float rotY;



    Vector3 offset;





    void Start()
    {
        //Set the cursor imvisible.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 rot = transform.localRotation.eulerAngles;
        rotX = rot.x;
        rotY = rot.y;
        
        //offset = target.transform.position - transform.position;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotX += mouseX * sensitivityX * Time.deltaTime;
        rotY += mouseY * sensitivityY * Time.deltaTime;

        rotY = Mathf.Clamp(rotY, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(rotY, rotX, 0.0f);
        transform.rotation = localRotation;

    }

    void LateUpdate()
    {
        
    }
}
