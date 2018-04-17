﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {
    
    private Player player;
    public GameObject chicken;
    public int chickenCount;
    float chickenOffset;

    public List<GameObject> tower = new List<GameObject>();

    public string _towerLayer = "Tower";
    public string _pickUpLayer = "PickUp";

    public bool doSidewaysWobble;
    public float chickenOffsetZMultiplier = 0;   //Front - Back axis
    public float chickenOffsetZMultiplierAdder;
    public float chickenOffsetXMultiplier = 0;   //Right - Left axis
    public float chickenOffsetXMultiplierAdder;

    float timer = 0;

    public float maxTilt = 1;
    public float wobbleDistanceSetter;
    public float wobbleDistance;
    public float oldRot;

    private bool calculateNewTowerTiltPos;

    void Start()
    {
        player = GetComponent<Player>();
        wobbleDistance = wobbleDistanceSetter;    
    }

    float cttt = 0;

    void Update () {

        if (cttt == 0)
            calculateNewTowerTiltPos = false;

        //TowerTiltv2();
        TowerTilt();

        cttt += Time.deltaTime;
        if (cttt > 1f)
        {
            cttt = 0;
            calculateNewTowerTiltPos = true;
        }
            
    }
    
    // Adds a chicken to the tower.
    public void AddChicken() {
        //Calculate the new chicken's position by the player's pos and the amount of chicken in the tower.
        Vector3 pos = transform.position + (((transform.up / 1.5f) * chickenCount) + new Vector3(0, 0.8f, 0));
        
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

    // Removes a chicken from the tower, removing it from the tower list and 
    // setting it up for flight in the Chicken script.
    public void RemoveChicken(Vector3 flightDirection, bool toBeThrown, bool flyFar, GameObject originatingPlayer) {
        GameObject removedChicken = tower[chickenCount - 1].gameObject;

        // Remove parent link before Removing.
        removedChicken.transform.parent = null;

        tower.Remove(removedChicken);
        chickenCount--;

        Vector3 clonePosition;

        // Create a clone of the removed chicken at a position slightly in front
        // of the first chicken if the chicken is thrown and at removed chicken's position if not.
        if (toBeThrown) {
            clonePosition = transform.position + transform.forward * 2f + new Vector3(0f, 0.5f, 0);
        } else {
            clonePosition = removedChicken.transform.position;
        }
        
        GameObject cloneChicken = Instantiate(chicken, clonePosition, Quaternion.identity);

        // Tell the chicken it is not in a tower.
        Chicken chickenController = cloneChicken.GetComponent<Chicken>();
        chickenController.IsInTower = false;

        // Turn clone chicken to face the direction the player is facing so that
        // it is thrown in the right direction.
        cloneChicken.transform.rotation = Quaternion.LookRotation(flightDirection, Vector3.up);

        Destroy(removedChicken);

        // Change chicken's layer back to the pick up layer before throw.
        cloneChicken.layer = LayerMask.NameToLayer(_pickUpLayer);

        cloneChicken.GetComponent<Rigidbody>().useGravity = false;
        
        chickenController.SetFlight(toBeThrown, flyFar, originatingPlayer);
    }

    //------------------------------------------------------------------------------------------------\\

    private float riskOfCollapse; //The higher the tower, the greater the risk of collapsing. Collapses when reaches 100.
    private List<Vector3> oldPos = new List<Vector3>();

    //------------------------------------------------------------------------------------------------\\

    public void TowerTiltv2()
    {
        //------------------------------------------------------------------------------------------------\\

        if (player.isMoving)
        {
            riskOfCollapse += 0.1f * Time.deltaTime;
        }
        else if (riskOfCollapse > 0f)
        {
            riskOfCollapse -= 0.2f * Time.deltaTime;
        }

        float height = tower.Count;         //The heigher the tower, the more it should tilt.

        //------------------------------------------------------------------------------------------------\\

        // Calculating a position for every chicken in the tower one by one
        for (int i = 0; i < tower.Count; i++)
        {
            
                GameObject chicken = tower[i];
                Vector3 pos = new Vector3(transform.position.x,
                                                    chicken.transform.position.y,
                                                    transform.position.z);

                //oldPos[i] = pos;    //Save the position for clamping the next round of movement
                Vector3 newPos = Vector3.zero;


                if (player.isMoving)                //The tower should tilt more, if the player is moving
                {
                    //TODO: Tower tilting/swaying incrementally as the player moves or the tower height augments 


                }
                else
                {
                    //TODO: Calm the tower back down.

                    newPos = new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
                }
            
            
            chicken.transform.position += newPos;

        }

        //------------------------------------------------------------------------------------------------\\
    }



    public void TowerTilt(){
        
        for (int i = 0; i < tower.Count; i++)
        {

            GameObject chicken = tower[i];

            Vector3 chickenPos = new Vector3(transform.position.x, 
                                                chicken.transform.position.y, 
                                                transform.position.z);

            if (player.isMoving) {           // If the player is moving, add tilt to the tower.
                if(timer < maxTilt){        // increase tilt until maximum
                    timer += Time.deltaTime;
                    chickenOffsetZMultiplier += chickenOffsetZMultiplierAdder;

                    // Sideways movement
                    if (doSidewaysWobble) {
                        if (oldRot < transform.rotation.eulerAngles.y) {    //Agle ++

                            chickenOffsetXMultiplier += chickenOffsetXMultiplierAdder;

                        } else if (oldRot > transform.rotation.eulerAngles.y) { //Angle --
                            
                            chickenOffsetXMultiplier -= chickenOffsetXMultiplierAdder;
                        }
                    }
                }
            } 
            else {                    // If the player stops, bring the tower back to straight
                if(timer > 0){          // Bring the timer back to zero
                    timer -= Time.deltaTime;
                    chickenOffsetZMultiplier -= chickenOffsetZMultiplierAdder;
                    


                    if (doSidewaysWobble){
                        if (chickenOffsetXMultiplier > 0)
                            chickenOffsetXMultiplier -= chickenOffsetXMultiplierAdder;
                        else if (chickenOffsetXMultiplier < 0)
                            chickenOffsetXMultiplier += chickenOffsetXMultiplierAdder;
                    }
                    
                } else {        
                    
                    timer = 0;
                    chickenOffsetZMultiplier = 0;
                    chickenOffsetXMultiplier = 0;
                }
            }

            // Bumch of maths to make the chicken tilt incremental the higher the chicken is.
            chickenPos -= transform.forward * i * 
                (chickenOffsetZMultiplier + i * (chickenOffsetZMultiplier / 10));

            if (doSidewaysWobble){
                chickenPos += transform.right * i *
                    (chickenOffsetXMultiplier + i * (chickenOffsetXMultiplier / 10));
            }
            


            chicken.transform.position = chickenPos;
        }

        oldRot = transform.rotation.eulerAngles.y;
    }

    // Scatter the chickens on impact, removing them from the tower one by one 
    // starting from the top.
    public void Scatter(GameObject originatingPlayer) {
        // foreach-loop would cause an InvalidOperationException!
        for (int index = chickenCount - 1; index >= 0; index--) {
            RemoveChicken(tower[index].transform.forward, false, false, originatingPlayer);
        }
    }
}