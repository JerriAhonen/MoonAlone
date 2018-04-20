using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnablePlayerModel : MonoBehaviour {

    private GameObject[] characterList;
    private int index;
    public string csNumber;

    void Awake()
    {
        index = PlayerPrefs.GetInt(csNumber);
    }

    // Use this for initialization
    void Start () {
        
        characterList = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            characterList[i] = transform.GetChild(i).gameObject;

        //Set all the models to nonActive (not visible)
        foreach (GameObject go in characterList)
            go.SetActive(false);

        //Set the first model to Active (visible)
        if (characterList[index])
            characterList[index].SetActive(true);
    }

    public GameObject GetActivePlayerModel()
    {
        Debug.Log("Fetching chosen model from: " + csNumber);

        if (characterList[index] != null)
        {
            Debug.Log("Model found from: " + csNumber);
            return characterList[index];
        }
            
        else
        {
            Debug.Log("No model to be found from: " + csNumber);
            return null;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
