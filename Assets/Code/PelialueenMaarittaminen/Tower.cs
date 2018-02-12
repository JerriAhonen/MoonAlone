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

        MoveChickensWithPlayer();
    }
    
    // Adds a chicken to the tower.
    public void AddChicken() {
        //Calculate the new chicken's position by the player's pos and the amount of chicken in the tower.
        Vector3 pos = transform.position + ((transform.up * chickenCount) + new Vector3(0, 1.5f, 0));

        //Instantiate a clone of the chicken prefab, so we can Destroy() it later.
        GameObject cloneChicken = Instantiate(chicken, pos, Quaternion.identity) as GameObject;
        //Parent it to the player so it moves with the player.
        cloneChicken.transform.parent = transform;

        tower.Add(cloneChicken);
        chickenCount = tower.Count;
    }

    // Removes a chicken from the tower, returns the removed chicken object.
    public GameObject RemoveChicken() {
        GameObject removedChicken = tower[chickenCount - 1].gameObject;

        //Remove parent link before Removing.
        removedChicken.transform.parent = null;

        tower.Remove(removedChicken);
        chickenCount--;

        return removedChicken.gameObject;
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