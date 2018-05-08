using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRoute : MonoBehaviour {

	public bool pingPong;
	public GameObject[] path;

	public Color color;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDrawGizmos()
	{
		Gizmos.color = color;

		for (int i = 0; i < path.Length; i++)
		{
			if (i == path.Length - 1) {
				if (pingPong)
					return;
				else
					Gizmos.DrawLine(path[i].transform.position, path[0].transform.position);		
			}
			else
				Gizmos.DrawLine(path[i].transform.position, path[i + 1].transform.position);	
		}
	}
}
