using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {
    
    public GameObject chicken;
    public int chickenCount;

    public List<GameObject> tower = new List<GameObject>();
    
	// Update is called once per frame
	void Update () {

        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 pos = transform.position + ((transform.up * chickenCount) + new Vector3(0, 1.5f, 0));
            
            Instantiate(chicken, pos, Quaternion.identity);
            
            tower.Add(chicken);
            chickenCount = tower.Count;
            
        }
        //TODO
        MoveChickensWithPlayer();
    }

    //TODO: Make Chickens move.
    public void MoveChickensWithPlayer()
    {
        foreach (var chicken in tower)
        {
            chicken.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            //chicken.Move();
        }
    }
}
