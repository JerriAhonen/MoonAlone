using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public int index;
    
    public string toggleAxis;
    public float selectionCooldown;
    private float cooldown;
    private bool axisInUse = false;
    public string confirmButton;
    public bool optionConfirmed = false;
    public int optionIndex;

    public GameObject selector;
    private Vector3 optionOnePos = new Vector3(2f, 2f, -4f);
    private Vector3 optionTwoPos = new Vector3(2f, 1f, -4f);
    private Vector3 optionThreePos = new Vector3(2f, 0f, -4f);
    private GameObject _mainCamera;

    public FMOD.Studio.EventInstance menuMusic;

    // Use this for initialization
    void Start () {
        selector.transform.position = optionOnePos;

        _mainCamera = GameObject.Find("Main Camera");

        menuMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music/menu");
        menuMusic.start();

        DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        if (!optionConfirmed)
        {
            if (Input.GetAxisRaw(toggleAxis) == 1)
            {
                if (axisInUse == false)
                {
                    axisInUse = true;
                    cooldown = selectionCooldown;
                    ToggleUp();
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Other Sounds/change_selection", _mainCamera.transform.position);
                    Debug.Log("+1 Switch! Index = " + index);
                }
            }
            else if (Input.GetAxisRaw(toggleAxis) == -1)
            {
                if (axisInUse == false)
                {
                    axisInUse = true;
                    cooldown = selectionCooldown;
                    ToggleDown();
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Other Sounds/change_selection", _mainCamera.transform.position);
                    Debug.Log("-1 Switch! Index = " + index);
                }
            }

            if (cooldown >= 0.0f)
                cooldown -= Time.deltaTime;

            if (cooldown < 0.0f)
                axisInUse = false;

            switch(index)
            {
                case 0:
                    selector.transform.position = optionOnePos;
                    break;
                case 1:
                    selector.transform.position = optionTwoPos;
                    break;
                case 2:
                    selector.transform.position = optionThreePos;
                    break;
            }

            bool confirm = Input.GetButtonDown(confirmButton);
            if (confirm)
                Confirm();
        }
    }

    public void ToggleUp()
    {
        index--;
        if (index < 0)
            index = 2;
    }

    public void ToggleDown()
    {
        index++;
        if (index == 3)
            index = 0;
    }

    public void Confirm()
    {
        optionConfirmed = true;
        optionIndex = index;

        switch(index)
        {
            case 0:
                FMODUnity.RuntimeManager.PlayOneShot("event:/Chicken Sounds/ChickenPickUpSurprise", _mainCamera.transform.position);

                SceneManager.LoadScene("CharacterSelection");
                break;
            case 1:
                break;
            case 2:
                break;
        }
        


    }
}
