﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour {

    private GameObject[] characterList;
    private int index;
    public string toggleAxis;
    public string confirmButton;
    private bool axisInUse = false;
    public float selectionCooldown;
    private float cooldown;
    private bool characterConfirmed = false;
    public int selectedCharacter;

    public TestGameManager gameManager;
    public Animator animator;
    
    private void Start()
    {
        gameManager = GetComponentInParent<TestGameManager>();

        characterList = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            characterList[i] = transform.GetChild(i).gameObject;

        //Set all the models to nonActive (not visible)
        foreach (GameObject go in characterList)
            go.SetActive(false);

        //Set the first model to Active (visible)
        if (characterList[0])
            characterList[0].SetActive(true);
    }

    private void Update()
    {
        //Once confirmed, cannot change anymore.
        if (!characterConfirmed)
        {
            if (Input.GetAxisRaw(toggleAxis) == 1)
            {
                if (axisInUse == false)
                {
                    axisInUse = true;
                    cooldown = selectionCooldown;
                    ToggleUp();
                    Debug.Log("+1 Switch!");
                }
            }
            else if (Input.GetAxisRaw(toggleAxis) == -1)
            {
                if (axisInUse == false)
                {
                    axisInUse = true;
                    cooldown = selectionCooldown;
                    ToggleDown();
                    Debug.Log("-1 Switch!");
                }
            }

            if (cooldown >= 0.0f)
                cooldown -= Time.deltaTime;

            if (cooldown < 0.0f)
                axisInUse = false;

            bool confirm = Input.GetButtonDown(confirmButton);
            if (confirm)
                Confirm();
        }

        if (gameManager.roundStarted && characterConfirmed)
        {
            DisableReadyCube();

            // Set the animator so Player.cs can get it.
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
        }
        else if (gameManager.roundStarted && !characterConfirmed)
        {
            //Set all the models to nonActive (not visible)
            foreach (GameObject go in characterList)
                go.SetActive(false);
        }
    }

    public void ToggleUp()
    {
        characterList[index].SetActive(false);

        //Set the index to be free, so other players can choose it.
        //gameManager.chosenCharacters[index] = false;

        index--;
        if (index < 0)
            index = characterList.Length - 2;

        //Set the index to be taken, so other players can't choose it.
        //gameManager.chosenCharacters[index] = true;

        characterList[index].SetActive(true);
    }
    
    public void ToggleDown()
    {
        characterList[index].SetActive(false);

        //Set the index to be free, so other players can choose it.
        //gameManager.chosenCharacters[index] = false;

        index++;
        if (index == characterList.Length - 1)
            index = 0;

        //Set the index to be taken, so other players can't choose it.
        //gameManager.chosenCharacters[index] = true;

        characterList[index].SetActive(true);
    }
    
    public void Confirm()
    {
        // Can't choose "no player" as character. Can't choose character that has already been chose.
        if (index != 0 && !gameManager.chosenCharacters[index]) {
			selectedCharacter = index;
            gameManager.chosenCharacters[index] = true;

            index = characterList.Length - 1;
			characterList[index].SetActive(true);

			characterConfirmed = true;


			gameManager.readyCount++;
			gameManager.noPlayerCount--;
		}
    }

    public void UnConfirm()
    {
        for (int i = 0; i < gameManager.chosenCharacters.Length; i++)
        {
            gameManager.chosenCharacters[i]= false;
        }

        //Set all the models to nonActive (not visible)
        foreach (GameObject go in characterList)
            go.SetActive(false);

        //Set the first model to Active (visible)
        if (characterList[0])
            characterList[0].SetActive(true);

        characterConfirmed = false;

        gameManager.readyCount--;
        gameManager.noPlayerCount++;

        index = 0;
    }

    public GameObject getSelectedCharacter()
    {
        return characterList[selectedCharacter];
    }

    public void DisableReadyCube()
    {
        index = characterList.Length - 1;
        characterList[index].SetActive(false);
    }
}
