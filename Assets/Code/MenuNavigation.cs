using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNavigation : MonoBehaviour {

    public GameObject[] buttons;
    public int index;

    public GameObject selectorCubes;
    public GameObject[] buttonPositions = new GameObject[4];

    public string toggleAxis;
    public float selectionCooldown;
    private float cooldown;
    private bool axisInUse = false;
    public string confirmButton;
    private bool optionConfirmed = false;
    // Use this for initialization
    void Start () {
        buttons = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            buttons[i] = transform.GetChild(i).gameObject;
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
        
        index--;
        if (index < 0)
            index = buttons.Length - 1;

        selectorCubes.transform.position = buttonPositions[index].transform.position;

    }

    public void ToggleDown()
    {
        
        index++;
        if (index == buttons.Length)
            index = 0;

        selectorCubes.transform.position = buttonPositions[index].transform.position;
    }

    public void Confirm()
    {
        optionConfirmed = true;
    }

    public void EnterMainMenu()
    {
        optionConfirmed = false;
        index = 0;
    }
}
