using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    //public TestGameManager testGameManager;
    public CSManager csManager;
    public Animator animator;

    public bool UseTestGameManager;
    public string csNumber;

    private Camera _mainCamera;
    public Animator animControl;

    public Button arrowUp;
    public Button arrowDown;
    public Light spotlight;
    
    private void Start()
    {
        index = 0;

        csManager = GetComponentInParent<CSManager>();
        animControl = gameObject.GetComponentInChildren<Animator>();
        spotlight.gameObject.SetActive(false);
        characterList = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            characterList[i] = transform.GetChild(i).gameObject;

        //Set all the models to nonActive (not visible)
        foreach (GameObject go in characterList)
            go.SetActive(false);

        //Set the first model to Active (visible)
        if (characterList[index])
            characterList[index].SetActive(true);

        _mainCamera = Camera.main;
    }

    private void Update()
    {

        //animControl = gameObject.GetComponentInChildren<Animator>();
        animControl = gameObject.transform.GetChild(index).GetComponent<Animator>();
            

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

                    FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/menu_change_selection", _mainCamera.transform.position);

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

                    FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/menu_change_selection", _mainCamera.transform.position);

                    Debug.Log("-1 Switch!");
                }
            }

            if (cooldown >= 0.0f)
                cooldown -= Time.deltaTime;

            if (cooldown < 0.0f)
                axisInUse = false;

            if (Input.GetButtonDown(confirmButton)) {
                Debug.Log("Confirm button pressed.");
                Confirm();

            }
        }
    }

    public void ToggleUp()
    {
        arrowUp.onClick.Invoke();
        characterList[index].SetActive(false);

        //Set the index to be free, so other players can choose it.
        //gameManager.chosenCharacters[index] = false;


        index--;
        if (index < 1)
            index = characterList.Length - 2;

        while (csManager.chosenCharacters[index])
        {
            index--;
            if (index < 1)
                index = characterList.Length - 2;
        }

        //Set the index to be taken, so other players can't choose it.
        //gameManager.chosenCharacters[index] = true;

        characterList[index].SetActive(true);
    }
    
    public void ToggleDown()
    {
        arrowDown.onClick.Invoke();
        characterList[index].SetActive(false);

        //Set the index to be free, so other players can choose it.
        //gameManager.chosenCharacters[index] = false;

        index++;
        if (index == characterList.Length - 1)
            index = 1;

        while (csManager.chosenCharacters[index])
        {
            index++;
            if (index == characterList.Length - 1)
                index = 1;
        }

        //Set the index to be taken, so other players can't choose it.
        //gameManager.chosenCharacters[index] = true;

        characterList[index].SetActive(true);
    }
    
    public void Confirm()
    {
        // Can't choose "no player" as character. Can't choose character that has already been chose.
        Debug.Log("Character Confirmed");
        if (index != 0 && !csManager.chosenCharacters[index]) {
			selectedCharacter = index;
            csManager.chosenCharacters[index] = true;

            animControl.SetInteger("AnimParam", 5);
            spotlight.gameObject.SetActive(true);
            //Save the selected character in PlayerPrefs.
            PlayerPrefs.SetInt(csNumber, index);

            index = characterList.Length - 1;
			//characterList[index].SetActive(true);

			characterConfirmed = true;

            FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/menu_confirm", _mainCamera.transform.position);

            csManager.readyCount++;
			csManager.noPlayerCount--;
		}
    }

    public int GetIndex()
    {
        return index;
    }

    public bool GetCharacterConfirmed()
    {
        return characterConfirmed;
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

    //  public void TestConfirm()
    //  {
    //      // Can't choose "no player" as character. Can't choose character that has already been chose.
    //      Debug.Log("Character Confirmed");
    //      if (index != 0 && !testGameManager.chosenCharacters[index]) {
    //	selectedCharacter = index;
    //          testGameManager.chosenCharacters[index] = true;

    //          index = characterList.Length - 1;
    //	characterList[index].SetActive(true);

    //	characterConfirmed = true;

    //	testGameManager.readyCount++;
    //	testGameManager.noPlayerCount--;
    //}
    //  }

    //public void UnConfirm()
    //{
    //    for (int i = 0; i < testGameManager.chosenCharacters.Length; i++)
    //    {
    //        testGameManager.chosenCharacters[i]= false;
    //    }

    //    //Set all the models to nonActive (not visible)
    //    foreach (GameObject go in characterList)
    //        go.SetActive(false);

    //    //Set the first model to Active (visible)
    //    if (characterList[0])
    //        characterList[0].SetActive(true);

    //    characterConfirmed = false;

    //    testGameManager.readyCount--;
    //    testGameManager.noPlayerCount++;

    //    index = 0;
    //}

}
