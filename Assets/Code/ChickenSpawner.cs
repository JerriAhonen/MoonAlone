using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenSpawner : MonoBehaviour {

    [SerializeField] private GameObject[] spawnerRoute = new GameObject[4];
    [SerializeField] private GameObject _prefabLoving;
    [SerializeField] private GameObject _prefabFearful;
    [SerializeField] private GameObject _prefabChill;

    private GameObject _prefabToSpawn;

    [SerializeField] private float _spawnTimer;
    [SerializeField] private float _speed;

    [Tooltip ("Amount of chickens to spawn")]
    [SerializeField] private int _amountToSpawn;

    private int chickensSpawned;
    private float timer;
    private int i = 0;
    
    private Vector3 newDestination;

    private GameObject mainCamera;

    // Use this for initialization
    void Start () {
        newDestination = spawnerRoute[0].transform.position;
        timer = _spawnTimer;
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

        transform.position = Vector3.MoveTowards(transform.position, newDestination, _speed * Time.deltaTime);
        timer -= Time.deltaTime;

        if(timer < 0 && chickensSpawned < _amountToSpawn)
        {
            SpawnChicken();
            chickensSpawned++;
            timer = _spawnTimer;
        }
	}

    public void SpawnChicken()
    {
        int mood;

        if (Random.value > 0.8)         //20% chance chill
        {
            mood = 2;
            _prefabToSpawn = _prefabChill;
        } else if(Random.value > 0.6)    //20% chance loving
          {
            mood = 1;
            _prefabToSpawn = _prefabLoving;
        } else                            //60% chance fearful
          {
            mood = 0;
            _prefabToSpawn = _prefabFearful;
        }
        
        GameObject cloneChicken = Instantiate(_prefabToSpawn, transform.position, Quaternion.identity);

        cloneChicken.GetComponent<Chicken>().Mood = mood;

        cloneChicken.GetComponent<Chicken>().IsFalling = true;

        FMODUnity.RuntimeManager.PlayOneShot("event:/Action Sounds/action_chicken_spawn", mainCamera.transform.position);
    }
}
