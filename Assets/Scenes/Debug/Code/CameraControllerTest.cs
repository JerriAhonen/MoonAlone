using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    public class CameraControllerTest : MonoBehaviour
    {

        private Transform targetTransform;
        public bool cameraMoved = false;
        public Transform MMcamPos;
        public Transform CScamPos;
        public Transform RcamPos;
        public Transform RScamPos;
        public Transform GOcamPos;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (cameraMoved)
            {
                transform.position = Vector3.Slerp(transform.position,
                    targetTransform.position, Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    targetTransform.rotation, Time.deltaTime);

            }
        }

        public void MoveCamera(int i)
        {
            cameraMoved = true;

            //Update the camera transform location when changing scenes
            switch (i)
            {
                case 0:
                    targetTransform = MMcamPos;
                    break;
                case 1:
                    targetTransform = CScamPos;
                    break;
                case 2:
                    targetTransform = RcamPos;
                    break;
                case 3:
                    targetTransform = RScamPos;
                    break;
                case 4:
                    targetTransform = GOcamPos;
                    break;
            }
        }
}

