using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {
    
    public GameObject chicken;
    public int chickenCount;
    float chickenOffset;

    public List<GameObject> tower = new List<GameObject>();

    public string _towerLayer = "Tower";
    public string _pickUpLayer = "PickUp";

    // Update is called once per frame
    void Update () {

        MoveChickensWithPlayer();
    }
    
    // Adds a chicken to the tower.
    public void AddChicken() {
        //Calculate the new chicken's position by the player's pos and the amount of chicken in the tower.
        Vector3 pos = transform.position + (((transform.up / 1.5f) * chickenCount) + new Vector3(0, 1.0f, 0));

        // Possibility to put all chickens face the same way
        // Quaternion forward = Quaternion.LookRotation(transform.forward);

        //Instantiate a clone of the chicken prefab, so we can Destroy() it later.
        GameObject cloneChicken = Instantiate(chicken, pos, Quaternion.identity) as GameObject;
        //Parent it to the player so it moves with the player.
        cloneChicken.transform.parent = transform;

        Rigidbody cloneRigidbody = cloneChicken.GetComponent<Rigidbody>();

        cloneRigidbody.useGravity = false;

        // Freeze chicken rigidbody rotation and position so that it is not affected by unwanted collisions.
        cloneRigidbody.constraints = 
            RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;

        //Tell the chicken it is in a tower.
        Chicken chickenController = cloneChicken.GetComponent<Chicken>();
        chickenController.IsInTower = true;

        tower.Add(cloneChicken);
        chickenCount = tower.Count;

        // Change chicken's layer to the tower layer so that it can't be picked up by players.
        cloneChicken.layer = LayerMask.NameToLayer(_towerLayer);
    }

    // Throws a chicken from the tower, removing it from the tower list and 
    // setting it up for flight in the Chicken script. The throw direction is
    // given as a parameter.
    public void RemoveChicken(Vector3 flightDirection, bool toBeThrown) {
        GameObject removedChicken = tower[chickenCount - 1].gameObject;

        // Remove parent link before Removing.
        removedChicken.transform.parent = null;

        tower.Remove(removedChicken);
        chickenCount--;

        // NICE TO HAVE: RE-SORT TOWER SO THE CHICKENS DROP DOWN ONE INDEX WITH SUITABLE BOUNCE

        // Create a clone of the removed chicken at a position slightly in front
        // of the first chicken.
        GameObject cloneChicken = Instantiate(chicken, 
            transform.position + transform.forward * 1.5f + new Vector3(0f, 1.5f, 0), 
            Quaternion.identity);

        // Tell the chicken it is not in a tower.
        Chicken chickenController = cloneChicken.GetComponent<Chicken>();
        chickenController.IsInTower = false;

        // Turn clone chicken to face the direction the player is facing so that
        // it is thrown in the right direction.
        cloneChicken.transform.rotation = Quaternion.LookRotation(flightDirection, Vector3.up);

        Destroy(removedChicken);

        // Change chicken's layer back to the pick up layer before throw.
        cloneChicken.layer = LayerMask.NameToLayer(_pickUpLayer);

        cloneChicken.GetComponent<Rigidbody>().useGravity = true;
        
        cloneChicken.GetComponent<Chicken>().SetFlight(toBeThrown);
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

    // Scatter the chickens on impact, removing them from the tower one by one 
    // starting from the top.
    public void Scatter() {
        // foreach-loop would cause an InvalidOperationException!
        for (int index = chickenCount - 1; index >= 0; index--) {
            RemoveChicken(tower[index].transform.forward, false);
        }
    }
}