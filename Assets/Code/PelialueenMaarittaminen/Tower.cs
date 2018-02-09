using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {
    
    public GameObject chicken;
    public int chickenCount;
    float chickenOffset;

    public List<GameObject> tower = new List<GameObject>();
    
	// Update is called once per frame
	void Update () {

        if (Input.GetButtonDown("Fire2"))
        {
            Vector3 pos = transform.position + ((transform.up * chickenCount) + new Vector3(0, 1.5f, 0));
            
            GameObject cloneChicken = Instantiate(chicken, pos, Quaternion.identity) as GameObject;
            
            tower.Add(cloneChicken);
            chickenCount = tower.Count;
        }
        if (Input.GetButtonDown("Fire1") && chickenCount > 0)
        {
            GameObject removedChicken = tower[chickenCount - 1].gameObject;

            tower.Remove(removedChicken);
            chickenCount--;
            
            Destroy(removedChicken.gameObject);
        }
            MoveChickensWithPlayer();
    }
    
    public void MoveChickensWithPlayer()
    {
        //Start the offset from 0;
        chickenOffset = 0;

        foreach (var chicken in tower)
        {
            Vector3 chickenPos = new Vector3(transform.position.x, chicken.transform.position.y, transform.position.z);

            //Adding slight tilt to tower
            chickenPos -= transform.forward * chickenOffset;
            chicken.transform.position = chickenPos;

            chickenOffset += 0.1f;
        }
    }
}
