using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {

    private GameObject _target;
    //private GameObject _targetEffect;

    // Use this for initialization
    void Start()
    {
        //_target = transform.Find("target").gameObject;
        _target = this.gameObject;
        //_targetEffect = transform.Find("INSERT_EFFECT_NAME_HERE").gameObject;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PickUp"))
        {
            if (collision.gameObject.GetComponent<Chicken>() != null)
            {
                if (collision.gameObject.GetComponent<Chicken>().isThrown)
                {
                    Destroy(_target.gameObject);

                    //_targetEffect.SetActive(true);

                    gameObject.layer = 0;
                }
            }
        }
    }
}
