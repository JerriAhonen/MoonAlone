using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour {

    private int _index;

    [SerializeField] private string _toggleAxis;
    [SerializeField] private float _selectionCooldown;
    private float cooldown;
    private bool axisInUse = false;

    [SerializeField] private GameObject _musicSlider1;
    [SerializeField] private GameObject _sfxSlider1;
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

    [SerializeField] private string _volumeAxis;
    [SerializeField] private string _cancelButton;

    private float _sfxVolume;
    private float _musicVolume;

    [SerializeField] private GameObject _sfxArrow;
    [SerializeField] private GameObject _musicArrow;

    private GameObject _mainMenu;
    private bool _movedSfxSlider = false;

    // Use this for initialization
    void Start () {
        _mainCamera = Camera.main;

        _sfx = FMODUnity.RuntimeManager.GetBus("bus:/Sfx");
        _music = FMODUnity.RuntimeManager.GetBus("bus:/Music");

        _mainMenu = GameObject.Find("MainMenu");

        musicSlider1Transform = _musicSlider1.GetComponent<RectTransform>();
        sfxSlider1Transform = _sfxSlider1.GetComponent<RectTransform>();
        //musicSlider2Transform = musicSlider2.GetComponent<RectTransform>();
        //sfxSlider2Transform = sfxSlider2.GetComponent<RectTransform>();

        _sfx.getVolume(out _sfxVolume, out _sfxVolume);
        _music.getVolume(out _musicVolume, out _musicVolume);

        sfxSlider1Transform.anchoredPosition = new Vector2(_sfxVolume * 100f, sfxSlider1Transform.anchoredPosition.y);
        musicSlider1Transform.anchoredPosition = new Vector2(_musicVolume * 100f, musicSlider1Transform.anchoredPosition.y);
        //sfxSlider2Transform.right = new Vector2(sfxVolume * 100f - 40f, sfxSlider2Transform.anchoredPosition.y);
        //musicSlider2Transform.right = new Vector2(musicVolume * 100f - 40f, musicSlider2Transform.anchoredPosition.y);
    }
    
    private void Update() {
        if (Input.GetAxisRaw(_toggleAxis) == 1) {
            if (axisInUse == false) {
                axisInUse = true;
                cooldown = _selectionCooldown;
                ToggleUp();
                FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/menu_change_selection", _mainCamera.transform.position);
                Debug.Log("+1 Switch! Index = " + _index);
            }
        } else if (Input.GetAxisRaw(_toggleAxis) == -1) {
            if (axisInUse == false) {
                axisInUse = true;
                cooldown = _selectionCooldown;
                ToggleDown();
                FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/menu_change_selection", _mainCamera.transform.position);
                Debug.Log("-1 Switch! Index = " + _index);
            }
        }

        if (cooldown >= 0.0f)
            cooldown -= Time.deltaTime;

        if (cooldown < 0.0f)
            axisInUse = false;

        switch (_index) {
            case 0:
                _sfxArrow.SetActive(true);
                _musicArrow.SetActive(false);
                break;
            case 1:
                _sfxArrow.SetActive(false);
                _musicArrow.SetActive(true);
                break;
        }

        AdjustVolume();

        if (Input.GetButton(_cancelButton)) {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/menu_cancel", _mainCamera.transform.position);

            _mainMenu.GetComponent<MainMenu>().optionConfirmed = false;

            gameObject.SetActive(false);
        }
    }

    public void ToggleUp() {
        _index--;
        if (_index < 0)
            _index = 1;
    }

    public void ToggleDown() {
        _index++;
        if (_index == 2)
            _index = 0;
    }

    public void AdjustVolume() {
        switch (_index) {
            case 0:
                Vector3 sfxSliderPos = sfxSlider1Transform.anchoredPosition;

                _sfxVolume = sfxSliderPos.x / 100f;

                if (Input.GetAxis(_volumeAxis) < 0 && (sfxSliderPos.x > minPos)) {
                    sfxSliderPos.x--;

                    sfxSlider1Transform.anchoredPosition = new Vector3(sfxSliderPos.x, sfxSliderPos.y, sfxSliderPos.z);
                    //sfxSlider2Transform.anchoredPosition = new Vector3(sfxSliderPos.x, sfxSliderPos.y, sfxSliderPos.z);

                    _movedSfxSlider = true;
                }

                if (Input.GetAxis(_volumeAxis) > 0 && (sfxSliderPos.x < maxPos)) {
                    sfxSliderPos.x++;
                    
                    sfxSlider1Transform.anchoredPosition = new Vector3(sfxSliderPos.x, sfxSliderPos.y, sfxSliderPos.z);
                    //sfxSlider2Transform.anchoredPosition = new Vector3(sfxSliderPos.x, sfxSliderPos.y, sfxSliderPos.z);

                    _movedSfxSlider = true;

                }

                _sfx.setVolume(_sfxVolume);

                if ((Input.GetAxis(_volumeAxis) == 0) && _movedSfxSlider) {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/menu_sfx_slider", _mainCamera.transform.position);
                    _movedSfxSlider = false;
                }

                break;
            case 1:
                Vector3 musicSliderPos = musicSlider1Transform.anchoredPosition;

                _musicVolume = musicSliderPos.x / 100f;

                if (Input.GetAxis(_volumeAxis) < 0 && (musicSliderPos.x > minPos)) {
                    musicSliderPos.x--;
                    
                    musicSlider1Transform.anchoredPosition = new Vector3(musicSliderPos.x, musicSliderPos.y, musicSliderPos.z);
                    //musicSlider2Transform.anchoredPosition = new Vector3(musicSliderPos.x, musicSliderPos.y, musicSliderPos.z);
                }

                if (Input.GetAxis(_volumeAxis) > 0 && (musicSliderPos.x < maxPos)) {
                    musicSliderPos.x++;
                    
                    musicSlider1Transform.anchoredPosition = new Vector3(musicSliderPos.x, musicSliderPos.y, musicSliderPos.z);
                    //musicSlider2Transform.anchoredPosition = new Vector3(musicSliderPos.x, musicSliderPos.y, musicSliderPos.z);
                }

                _music.setVolume(_musicVolume);

                break;
        }
    }
}
