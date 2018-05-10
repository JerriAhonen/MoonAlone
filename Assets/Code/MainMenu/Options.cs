using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour {

    public int index;

    public string toggleAxis;
    public float selectionCooldown;
    private float cooldown;
    private bool axisInUse = false;
    public int optionIndex;

    public GameObject musicSlider;
    public GameObject sfxSlider;
    private Vector3 musicSliderPos;
    private Vector3 sfxSliderPos;

    private Camera _mainCamera;

    private FMOD.Studio.Bus _master;
    private FMOD.Studio.Bus _sfx;
    private FMOD.Studio.Bus _music;

    public string volumeAxis;
    public string cancelButton;

    public GameObject mainMenu;

	// Use this for initialization
	void Start () {
        _mainCamera = Camera.main;

        _master = FMODUnity.RuntimeManager.GetBus("bus:/");
        _sfx = FMODUnity.RuntimeManager.GetBus("bus:/Sfx");
        _music = FMODUnity.RuntimeManager.GetBus("bus:/Music");

        mainMenu = GameObject.Find("MainMenu");

        musicSliderPos = musicSlider.GetComponent<RectTransform>().localPosition;
        sfxSliderPos = sfxSlider.GetComponent<RectTransform>().localPosition;
    }

    // Update is called once per frame
    void Update() {
        //_master.setVolume(0.1f);

        if(Input.GetAxisRaw(toggleAxis) == 1) {
            if(axisInUse == false) {
                axisInUse = true;
                cooldown = selectionCooldown;
                ToggleUp();
                FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/menu_change_selection", _mainCamera.transform.position);
                Debug.Log("+1 Switch! Index = " + index);
            }
        } else if(Input.GetAxisRaw(toggleAxis) == -1) {
            if(axisInUse == false) {
                axisInUse = true;
                cooldown = selectionCooldown;
                ToggleDown();
                FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/menu_change_selection", _mainCamera.transform.position);
                Debug.Log("-1 Switch! Index = " + index);
            }
        }

        if(cooldown >= 0.0f)
            cooldown -= Time.deltaTime;

        if(cooldown < 0.0f)
            axisInUse = false;

        switch(index) {
            case 0:
                //selector.transform.position = optionOnePos;   JOKU INDIKAATTORI MIKÄ SLIDER VALITTUNA
                break;
            case 1:
                //selector.transform.position = optionTwoPos;   JOKU INDIKAATTORI MIKÄ SLIDER VALITTUNA
                break;
        }

        AdjustVolume();

        if (Input.GetButton(cancelButton)) {
            mainMenu.GetComponent<MainMenu>().optionConfirmed = false;

            gameObject.SetActive(false);
        }
    }

    public void ToggleUp() {
        index--;
        if(index < 0)
            index = 1;
    }

    public void ToggleDown() {
        index++;
        if(index == 2)
            index = 0;
    }

    public void AdjustVolume() {
        optionIndex = index;

        switch(index) {
            case 0:
                //FMODUnity.RuntimeManager.PlayOneShot("event:/Chicken Sounds/ChickenPickUpSurprise", _mainCamera.transform.position);

                if (Input.GetAxis(volumeAxis) < 0) {
                    sfxSliderPos.x--;
                    Debug.Log("slider pos = " + sfxSliderPos.x);
                    sfxSliderPos = new Vector3(sfxSliderPos.x, sfxSliderPos.y, sfxSliderPos.z);
                }

                if (Input.GetAxis(volumeAxis) > 0) {
                    sfxSliderPos.x++;
                    sfxSliderPos = new Vector3(sfxSliderPos.x, sfxSliderPos.y, sfxSliderPos.z);
                }

                break;
            case 1:
                Input.GetAxis(volumeAxis);
                break;
        }
    }
}
