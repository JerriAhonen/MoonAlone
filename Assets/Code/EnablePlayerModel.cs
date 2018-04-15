using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnablePlayerModel : MonoBehaviour {

    private GameObject[] characterList;
    private int index;
    public string csNumber;

    // Use this for initialization
    void Start () {
        index = PlayerPrefs.GetInt(csNumber);

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
	
	// Update is called once per frame
	void Update () {
		
	}
}
