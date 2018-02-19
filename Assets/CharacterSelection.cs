using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour {

    private GameObject[] characterList;
    private int index;
    public string vertical = "Vertical_P1";

    private void Start()
    {
        characterList = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            characterList[i] = transform.GetChild(i).gameObject;

        foreach (GameObject go in characterList)
            go.SetActive(false);
        

    }

    private void Update()
    {
        bool toggleUp = Input.GetKeyDown(KeyCode.W);
        bool toggleDown = Input.GetKeyDown(KeyCode.S);
        if (toggleUp)
            ToggleUp();
        else if (toggleDown)
            ToggleDown();

    }

    public void ToggleUp()
    {
        characterList[index].SetActive(false);

        index--;
        if (index < 0)
            index = characterList.Length - 1;

        characterList[index].SetActive(true);
    }

    public void ToggleDown()
    {
        characterList[index].SetActive(false);

        index++;
        if (index == characterList.Length)
            index = 0;

        characterList[index].SetActive(true);
    }

    public void Confirm()
    {

    }
}
