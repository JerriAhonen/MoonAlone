using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : MonoBehaviour {

    public float numberInTower;
    public float yPos;
    
    public Chicken(int numberInTower, float yPos)
    {
        this.numberInTower = numberInTower;
        this.yPos = yPos;

        //TODO: Possibly Instatiate the chickens here.
    }
    
    public void Move(Vector3 dir)
    {
        transform.Translate(dir);
        
    }
}
