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

    public GameObject musicSlider1;
    public GameObject sfxSlider1;
    //public GameObject musicSlider2;
    //public GameObject sfxSlider2;

    private RectTransform musicSlider1Transform;
    private RectTransform sfxSlider1Transform;
    //private RectTransform musicSlider2Transform;
    //private RectTransform sfxSlider2Transform;

    private float maxPos = 160f;
    private float minPos = 0f;

    private Camera _mainCamera;

    private FMOD.Studio.Bus _sfx;
    private FMOD.Studio.Bus _music;

    public string volumeAxis;
    public string cancelButton;

    public float sfxVolume;
    public float musicVolume;

    public GameObject sfxArrow;
    public GameObject musicArrow;

    public GameObject mainMenu;
    public bool movedSfxSlider = false;

    // Use this for initialization
    void Start () {
        _mainCamera = Camera.main;

        _sfx = FMODUnity.RuntimeManager.GetBus("bus:/Sfx");
        _music = FMODUnity.RuntimeManager.GetBus("bus:/Music");

        mainMenu = GameObject.Find("MainMenu");

        musicSlider1Transform = musicSlider1.GetComponent<RectTransform>();
        sfxSlider1Transform = sfxSlider1.GetComponent<RectTransform>();
        //musicSlider2Transform = musicSlider2.GetComponent<RectTransform>();
        //sfxSlider2Transform = sfxSlider2.GetComponent<RectTransform>();

        _sfx.getVolume(out sfxVolume, out sfxVolume);
        _music.getVolume(out musicVolume, out musicVolume);

        sfxSlider1Transform.anchoredPosition = new Vector2(sfxVolume * 100f, sfxSlider1Transform.anchoredPosition.y);
        musicSlider1Transform.anchoredPosition = new Vector2(musicVolume * 100f, musicSlider1Transform.anchoredPosition.y);
        //sfxSlider2Transform.right = new Vector2(sfxVolume * 100f - 40f, sfxSlider2Transform.anchoredPosition.y);
        //musicSlider2Transform.right = new Vector2(musicVolume * 100f - 40f, musicSlider2Transform.anchoredPosition.y);
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
                sfxArrow.SetActive(true);
                musicArrow.SetActive(false);
                break;
            case 1:
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
                Vector3 sfxSliderPos = sfxSlider1Transform.anchoredPosition;

                sfxVolume = sfxSliderPos.x / 100f;

                if (Input.GetAxis(volumeAxis) < 0 && (sfxSliderPos.x > minPos)) {
                    sfxSliderPos.x--;

                    sfxSlider1Transform.anchoredPosition = new Vector3(sfxSliderPos.x, sfxSliderPos.y, sfxSliderPos.z);
                    //sfxSlider2Transform.anchoredPosition = new Vector3(sfxSliderPos.x, sfxSliderPos.y, sfxSliderPos.z);

                    movedSfxSlider = true;
                }

                if (Input.GetAxis(volumeAxis) > 0 && (sfxSliderPos.x < maxPos)) {
                    sfxSliderPos.x++;
                    
                    sfxSlider1Transform.anchoredPosition = new Vector3(sfxSliderPos.x, sfxSliderPos.y, sfxSliderPos.z);
                    //sfxSlider2Transform.anchoredPosition = new Vector3(sfxSliderPos.x, sfxSliderPos.y, sfxSliderPos.z);

                    movedSfxSlider = true;

                }

                _sfx.setVolume(sfxVolume);

                if ((Input.GetAxis(volumeAxis) == 0) && movedSfxSlider) {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/menu_sfx_slider", _mainCamera.transform.position);
                    movedSfxSlider = false;
                }

                break;
            case 1:
                Vector3 musicSliderPos = musicSlider1Transform.anchoredPosition;

                musicVolume = musicSliderPos.x / 100f;

                if (Input.GetAxis(volumeAxis) < 0 && (musicSliderPos.x > minPos)) {
                    musicSliderPos.x--;
                    
                    musicSlider1Transform.anchoredPosition = new Vector3(musicSliderPos.x, musicSliderPos.y, musicSliderPos.z);
                    //musicSlider2Transform.anchoredPosition = new Vector3(musicSliderPos.x, musicSliderPos.y, musicSliderPos.z);
                }

                if (Input.GetAxis(volumeAxis) > 0 && (musicSliderPos.x < maxPos)) {
                    musicSliderPos.x++;
                    
                    musicSlider1Transform.anchoredPosition = new Vector3(musicSliderPos.x, musicSliderPos.y, musicSliderPos.z);
                    //musicSlider2Transform.anchoredPosition = new Vector3(musicSliderPos.x, musicSliderPos.y, musicSliderPos.z);
                }

                _music.setVolume(musicVolume);

                break;
        }
    }
}
