using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceHenHassle
{
    public class CSTest : MonoBehaviour
    {

        //private GameManagerDebug gameManager;
        private CSManagerTest csm;
        private GameObject[] characterList;
        private int index;
        public string toggleAxis;
        public string confirmButton;
        private bool axisInUse = false;
        public float selectionCooldown;
        private float cooldown;
        private bool characterConfirmed = false;
        public int selectedCharacter;

        // Use this for initialization
        void Start()
        {
            //gameManager = GetComponentInParent<GameManagerDebug>();
            csm = GetComponentInParent<CSManagerTest>();

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

        // Update is called once per frame
        void Update()
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

                if (Input.GetButtonDown(confirmButton))
                {
                    Debug.Log("Confirm button pressed.");
                    Confirm();

                }
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
            Debug.Log("Character Confirmed");
            if (index != 0 /*&& !gameManager.chosenCharacters[index]*/)
            {
                selectedCharacter = index;
                //gameManager.chosenCharacters[index] = true;

                index = characterList.Length - 1;
                characterList[index].SetActive(true);

                characterConfirmed = true;
                Debug.Log("number of players ready: " + csm.numOfReady);
                csm.numOfReady++;
                Debug.Log("number of players ready: " + csm.numOfReady);
                Debug.Log("SetcsToReady();");

                //gameManager.readyCount++;
                //gameManager.noPlayerCount--;
            }
        }
    }

}

