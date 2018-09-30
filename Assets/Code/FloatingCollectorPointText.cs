using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingCollectorPointText : MonoBehaviour {

    public float destroyTime = 3f;
    public Vector3 offset = new Vector3(0, 5, 0);
    public Vector3 RandomizeIntensity = new Vector3(0.5f, 0.5f, 0);
    public int playerModel;
    public TextMeshPro textMeshPro;

    // Use this for initialization
    void Start () {
        Destroy(gameObject, destroyTime);
        transform.rotation = Camera.main.transform.rotation;
        transform.localPosition += offset;
        transform.localPosition += new Vector3(Random.Range(-RandomizeIntensity.x, RandomizeIntensity.x),
                                                Random.Range(-RandomizeIntensity.y, RandomizeIntensity.y),
                                                0); //Do not randomize the z axis on a text object :D
        textMeshPro = GetComponent<TextMeshPro>();
    }
    
    private void Update()
    {
        switch (playerModel)
        {
            case 1:
                textMeshPro.color = new Color(0, 166, 189);
                break;
            case 2:
                textMeshPro.color = new Color(163, 0, 0);
                break;
            case 3:
                textMeshPro.color = Color.green;
                break;
            case 4:
                textMeshPro.color = Color.yellow;
                break;
        }
    }
    
}
