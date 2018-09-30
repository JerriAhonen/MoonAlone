using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBall : MonoBehaviour {

    public Collector myCollector;
    public Vector3 startPos;
    public Vector3 secondStartPos;
    public Transform target;
    public Vector3 routingPoint;
    public int playerModel;

    public ParticleSystem[] ps;

    float startTime;
    bool goToRoutingPoint;

    private void Awake()
    {
        ps = GetComponentsInChildren<ParticleSystem>(true);
        //Debug.Log("Found the particle systems for lightball");
    }

    private void OnEnable()
    {
        startTime = Time.time;
        goToRoutingPoint = true;
    }

    private void Update()
    {
        MoveLightBall();

        // Change the color according to playermodel
        var main = ps[1].main;
        switch (playerModel)
        {
            case 1:
                main.startColor = new Color(0, 166, 189);
                break;
            case 2:
                main.startColor = new Color(163, 0, 0);
                break;
            case 3:
                main.startColor = Color.green;
                break;
            case 4:
                main.startColor = Color.yellow;
                break;
        }
    }

    void MoveLightBall()
    {
        float progress = Time.time - startTime;
        
        if (goToRoutingPoint)
        {
            transform.position = Vector3.Slerp(startPos, routingPoint, progress / 0.5f);

            if (Vector3.Distance(transform.position, routingPoint) <= 1f)
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
