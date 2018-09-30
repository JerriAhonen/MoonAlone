using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenSpawner : MonoBehaviour {

    public GameObject[] spawnerRoute = new GameObject[4];
    public GameObject prefabLoving;
    public GameObject prefabFearful;
    public GameObject prefabChill;
    public GameObject prefabToSpawn;
    public float spawnTimer;
    public float speed;

    [Tooltip ("Amount of chickens to spawn")]
    public int amountToSpawn;
    private int chickensSpawned;

    private float timer;
    private int i = 0;
    
    Vector3 newDestination;

    private GameObject mainCamera;

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

        if(timer < 0 && chickensSpawned < amountToSpawn)
        {
            SpawnChicken();
            chickensSpawned++;
            timer = spawnTimer;
        }
	}

    public void SpawnChicken()
    {
        int mood;

        if (Random.value > 0.8)         //20% chance chill
        {
            mood = 2;
            prefabToSpawn = prefabChill;
        } else if(Random.value > 0.6)    //20% chance loving
          {
            mood = 1;
            prefabToSpawn = prefabLoving;
        } else                            //60% chance fearful
          {
            mood = 0;
            prefabToSpawn = prefabFearful;
        }
        
        GameObject cloneChicken = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);

        cloneChicken.GetComponent<Chicken>().Mood = mood;

        cloneChicken.GetComponent<Chicken>().IsFalling = true;

        FMODUnity.RuntimeManager.PlayOneShot("event:/Action Sounds/action_chicken_spawn", mainCamera.transform.position);
    }
}
