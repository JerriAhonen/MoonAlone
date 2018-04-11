using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase : MonoBehaviour {


    private bool cooldownTimerFinished;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator StartCooldownTimer(float cooldownTime)
    {
        while(cooldownTime > 0) {
            yield return new WaitForSeconds(1f);

            cooldownTime--;
            Debug.Log("cooldown time: " + cooldownTime);    
        }
        if (cooldownTime <= 0) {
            Debug.Log("CooldownTimer Finished!");
            cooldownTimerFinished = true;
        }
        
    }
}
