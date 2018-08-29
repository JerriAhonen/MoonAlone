using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBall : MonoBehaviour {

    public Collector myCollector;
	
    void MoveLightBall()
    {
        ///Get collector position and correct scoreboard position.
        ///Move lightball to scoreboard
        ///Add random X movement to make lightballs have different paths.
    }

    public void CallReturnToPool()
    {
        myCollector.ReturnLightBall(this);
    }
}
