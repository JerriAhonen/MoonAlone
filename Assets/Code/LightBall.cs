using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBall : MonoBehaviour {

    public Collector myCollector;
    public Vector3 startPos;
    public Transform target;

    float startTime;

    private void OnEnable()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        MoveLightBall();
    }

    void MoveLightBall()
    {
        float progress = Time.time - startTime;
        transform.position = Vector3.Slerp(startPos, target.position, progress / 1f);

        //TODO add 2 points ontop of both lights to route the lightballs through them.






        if (transform.position == target.position)
        {
            CallReturnToPool();
        }
        
        ///Get collector position and correct scoreboard position.
        ///Move lightball to scoreboard
        ///Add random X movement to make lightballs have different paths.
    }

    public void CallReturnToPool()
    {
        myCollector.ReturnLightBall(this);
    }
}
