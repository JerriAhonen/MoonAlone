using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenSpawner : MonoBehaviour {

    public GameObject[] spawnerRoute = new GameObject[4];
    public GameObject prefabToSpawn;
    public GameManager gm;
    public float spawnTimer;
    private float timer;
    public float speed;
    public int i = 0;
    
    Vector3 newDestination;

    public GameObject mainCamera;

    // Use this for initialization
    void Start () {
        newDestination = spawnerRoute[0].transform.position;
        timer = spawnTimer;
        mainCamera = GameObject.Find("Main Camera");
    }
	
	// Update is called once per frame
	void Update () {
        
        if (Vector3.Distance(transform.position, newDestination) < 1f)
        {
            if (i == spawnerRoute.Length - 1)
            {
                i = 0;
            }
            else
            {
                i++;
            }

            newDestination = spawnerRoute[i].transform.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, newDestination, speed * Time.deltaTime);
        timer -= Time.deltaTime;

        if(timer < 0 && gm.gameStarted)
        {
            SpawnChicken();
            timer = spawnTimer;
        }
	}

    void SpawnChicken()
    {
        GameObject cloneChicken = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);

        FMODUnity.RuntimeManager.PlayOneShot("event:/Other Sounds/Spawn", mainCamera.transform.position);
    }
}
