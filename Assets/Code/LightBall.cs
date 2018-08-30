using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBall : MonoBehaviour {

    public Collector myCollector;
    public Vector3 startPos;
    public Vector3 secondStartPos;
    public Transform target;
    public Transform routingPoint;

    float startTime;
    bool goToRoutingPoint;

    private void OnEnable()
    {
        startTime = Time.time;
        goToRoutingPoint = true;
    }

    private void Update()
    {
        MoveLightBall();
    }

    void MoveLightBall()
    {
        float progress = Time.time - startTime;
        

        //TODO add 2 points ontop of both lights to route the lightballs through them.

        if (goToRoutingPoint)
        {
            transform.position = Vector3.Slerp(startPos, routingPoint.position, progress / 0.5f);


            if (Vector3.Distance(transform.position, routingPoint.position) <= 1f)
            {
                goToRoutingPoint = false;
                secondStartPos = transform.position;
                startTime = Time.time;
            }
        }
        else
        {
            transform.position = Vector3.Slerp(secondStartPos, target.position, progress / 0.5f);
        }




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
