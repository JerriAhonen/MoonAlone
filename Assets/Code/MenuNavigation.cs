using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNavigation : MonoBehaviour {

    public Camera camera;

    public GameObject[] buttons;
    public int index;

    //public GameObject selectorCubes;
    //public GameObject[] buttonPositions = new GameObject[4];

    public string toggleAxis;
    public float selectionCooldown;
    private float cooldown;
    private bool axisInUse = false;
    public string confirmButton;
    private bool optionConfirmed = false;

    public Transform cameraPosMenu;
    public Transform cameraPosCharSel;

    // Use this for initialization
    void Start () {
        buttons = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            buttons[i] = transform.GetChild(i).gameObject;

        buttons[index].transform.position = buttons[index].transform.position - Vector3.forward / 2;

        camera.GetComponent<CameraController>().MoveCamera(cameraPosMenu);      // Move camera to Menu view
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
                    Debug.Log("-1 Switch! Index = " + index);
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


    }

    public void ToggleUp()
    {
        buttons[index].transform.position = buttons[index].transform.position + Vector3.forward / 2;

        index--;
        if (index < 0)
            index = buttons.Length - 1;

        buttons[index].transform.position = buttons[index].transform.position - Vector3.forward / 2;

    }

    public void ToggleDown()
    {
        buttons[index].transform.position = buttons[index].transform.position + Vector3.forward / 2;

        index++;
        if (index == buttons.Length)
            index = 0;

        buttons[index].transform.position = buttons[index].transform.position - Vector3.forward / 2;
    }

    public void Confirm()
    {
        optionConfirmed = true;
        camera.GetComponent<CameraController>().MoveCamera(cameraPosCharSel);      // Move camera to Char Sel view
    }

    public void EnterMainMenu()
    {
        optionConfirmed = false;
        index = 0;
    }
}
