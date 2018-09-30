using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Arrows : MonoBehaviour {

    [SerializeField] private Button button;

    // Use this for initialization
    void Start () {
        button = GetComponent<Button>();
	}
	
	public void PressButton() {
        button.Select();

        StartCoroutine(Wait());
    }

    private IEnumerator Wait() {
        yield return new WaitForSeconds(0.1f);

        EventSystem.current.SetSelectedGameObject(null);
    }
}
