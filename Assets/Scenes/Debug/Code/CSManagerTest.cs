using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceHenHassle
{
    public class CSManagerTest : MonoBehaviour
    {
        [SerializeField]
        private int numOfPlayers;
        public int numOfReady;

        public bool ready;
        
        public CSTest[] cs = new CSTest[4];

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (numOfReady == numOfPlayers)
            {
                ready = true;
            }
                

        }

        // Disable and enable character selection, according to how many players are playing.
        public void EnableCharacterSelection(bool enable, int numOfPlayers) 
        {
            this.numOfPlayers = numOfPlayers;

            if (enable)
            {
                ready = false;  // When enabling cs, reset the ready param.
                numOfReady = 0;     // Reset the ready count.
            }

            for (int i = 0; i < numOfPlayers; i++)
            {
                if (cs != null)
                {
                    if (enable)
                    {
                        cs[i].enabled = true;
                    }
                    else
                    {
                        cs[i].enabled = false;
                    }
                }
            }

        }
    }

}



