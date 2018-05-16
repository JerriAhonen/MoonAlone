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
    private RectTransform musicSliderTransform;
    private RectTransform sfxSliderTransform;
    private float maxPos = 140f;
    private float minPos = 0f;

    private Camera _mainCamera;

    private FMOD.Studio.Bus _master;
    private FMOD.Studio.Bus _sfx;
    private FMOD.Studio.Bus _music;

    public string volumeAxis;
    public string cancelButton;

    public float sfxVolume;
    public float musicVolume;

	//public GameObject selector;
	//private Vector3 optionOnePos = new Vector2(785f, 568f);
	//private Vector3 optionTwoPos = new Vector2(785f, 460f);

    public GameObject sfxArrow;
    public GameObject musicArrow;

    public GameObject mainMenu;
    public bool movedSfxSlider = false;

    // Use this for initialization
    void Start () {
        _mainCamera = Camera.main;

        _master = FMODUnity.RuntimeManager.GetBus("bus:/");
        _sfx = FMODUnity.RuntimeManager.GetBus("bus:/Sfx");
        _music = FMODUnity.RuntimeManager.GetBus("bus:/Music");

        mainMenu = GameObject.Find("MainMenu");

        musicSliderTransform = musicSlider.GetComponent<RectTransform>();
        sfxSliderTransform = sfxSlider.GetComponent<RectTransform>();

        _sfx.getVolume(out sfxVolume, out sfxVolume);
        _music.getVolume(out musicVolume, out musicVolume);

        sfxSliderTransform.anchoredPosition = new Vector2(sfxVolume * 100f, sfxSliderTransform.anchoredPosition.y);
        musicSliderTransform.anchoredPosition = new Vector2(musicVolume * 100f, musicSliderTransform.anchoredPosition.y);
    }
    
    private void Update() {
        if (Input.GetAxisRaw(toggleAxis) == 1) {
            if (axisInUse == false) {
                axisInUse = true;
                cooldown = selectionCooldown;
                ToggleUp();
                FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/menu_change_selection", _mainCamera.transform.position);
                Debug.Log("+1 Switch! Index = " + index);
            }
        } else if (Input.GetAxisRaw(toggleAxis) == -1) {
            if (axisInUse == false) {
                axisInUse = true;
                cooldown = selectionCooldown;
                ToggleDown();
                FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/menu_change_selection", _mainCamera.transform.position);
                Debug.Log("-1 Switch! Index = " + index);
            }
        }

        if (cooldown >= 0.0f)
            cooldown -= Time.deltaTime;

        if (cooldown < 0.0f)
            axisInUse = false;

        switch (index) {
            case 0:
                //selector.transform.position = optionOnePos;   JOKU INDIKAATTORI MIKÄ SLIDER VALITTUNA
                //selector.transform.position = optionOnePos;
                sfxArrow.SetActive(true);
                musicArrow.SetActive(false);
                break;
            case 1:
                //selector.transform.position = optionTwoPos;
                //selector.transform.position = optionTwoPos;   JOKU INDIKAATTORI MIKÄ SLIDER VALITTUNA
                sfxArrow.SetActive(false);
                musicArrow.SetActive(true);
                break;
        }

        AdjustVolume();

        if (Input.GetButton(cancelButton)) {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/menu_cancel", _mainCamera.transform.position);

            mainMenu.GetComponent<MainMenu>().optionConfirmed = false;

            gameObject.SetActive(false);
        }
    }

    public void ToggleUp() {
        index--;
        if (index < 0)
            index = 1;
    }

    public void ToggleDown() {
        index++;
        if (index == 2)
            index = 0;
    }

    public void AdjustVolume() {
        optionIndex = index;

        switch (index) {
            case 0:
                Vector3 sfxSliderPos = sfxSliderTransform.anchoredPosition;

                sfxVolume = sfxSliderPos.x / 100f;

                if (Input.GetAxis(volumeAxis) < 0 && (sfxSliderPos.x > minPos)) {
                    sfxSliderPos.x--;

                    sfxSliderTransform.anchoredPosition = new Vector3(sfxSliderPos.x, sfxSliderPos.y, sfxSliderPos.z);

                    movedSfxSlider = true;
                }

                if (Input.GetAxis(volumeAxis) > 0 && (sfxSliderPos.x < maxPos)) {
                    sfxSliderPos.x++;
                    
                    sfxSliderTransform.anchoredPosition = new Vector3(sfxSliderPos.x, sfxSliderPos.y, sfxSliderPos.z);

                    movedSfxSlider = true;

                }

                _sfx.setVolume(sfxVolume);

                if ((Input.GetAxis(volumeAxis) == 0) && movedSfxSlider) {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/menu_sfx_slider", _mainCamera.transform.position);
                    movedSfxSlider = false;
                }

                break;
            case 1:
                Vector3 musicSliderPos = musicSliderTransform.anchoredPosition;

                musicVolume = musicSliderPos.x / 100f;

                if (Input.GetAxis(volumeAxis) < 0 && (musicSliderPos.x > minPos)) {
                    musicSliderPos.x--;
                    
                    musicSliderTransform.anchoredPosition = new Vector3(musicSliderPos.x, musicSliderPos.y, musicSliderPos.z);
                }

                if (Input.GetAxis(volumeAxis) > 0 && (musicSliderPos.x < maxPos)) {
                    musicSliderPos.x++;
                    
                    musicSliderTransform.anchoredPosition = new Vector3(musicSliderPos.x, musicSliderPos.y, musicSliderPos.z);
                }

                _music.setVolume(musicVolume);

                break;
        }
    }
}
