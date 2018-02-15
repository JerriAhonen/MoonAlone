using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : MonoBehaviour {

    public float numberInTower;
    public float yPos;

    public float throwSpeed = 20f;

    public bool isThrown = false;
    
    public bool isRising = false;

    public Chicken(int numberInTower, float yPos)
    {
        this.numberInTower = numberInTower;
        this.yPos = yPos;

        //TODO: Possibly Instatiate the chickens here.
    }

    private void Update()
    {
        if (isThrown) {
            Fly();
        }
    }

    public void Move(Vector3 dir)
    {
        transform.Translate(dir);
        
    }

    void Fly() {
        //Quaternion frontFacing = Quaternion.LookRotation(transform.forward, Vector3.up);

        //Quaternion throwRotation = Quaternion.Slerp(transform.rotation, frontFacing, 5f * Time.deltaTime);

        //chicken.transform.rotation = frontFacing;

        transform.position += (transform.forward * Time.deltaTime);

        if ((transform.position.y < 5f) && isRising) {

            transform.position += (transform.up * Time.deltaTime);
        }

        if (transform.position.y > 4f) {
            isRising = false;
        }

        if (!isRising) {
            transform.position -= (transform.up * Time.deltaTime);
        }

        if (transform.position.y < 1f) {
            isThrown = false;
        }
    }

    public void SetThrow() {
        isThrown = true;
        isRising = true;
    }
}
