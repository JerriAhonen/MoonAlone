﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haystack : MonoBehaviour {

    private GameObject _haystack;
    private GameObject _hayEffect;

    // Use this for initialization
    void Start() {
        _haystack = transform.Find("haystack").gameObject;
        _hayEffect = transform.Find("HayPoof").gameObject;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PickUp"))
        {
            if (collision.gameObject.GetComponent<Chicken>() != null)
            {
                if (collision.gameObject.GetComponent<Chicken>().IsThrown 
                    && 
                    collision.gameObject.GetComponent<Chicken>().OriginatingPlayer != null)
                {
                    Destroy(_haystack.gameObject);

                    _hayEffect.SetActive(true);

                    gameObject.layer = 0;
                }
            }
        }
    }
}
