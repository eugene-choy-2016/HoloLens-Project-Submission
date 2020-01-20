using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInitializerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (GeneratePlanes.PlanesProcessed)
        {
            Debug.Log("Planes alr processed, place my fan");
            GeneratePlanes.generateCeilingFan();
        }
	}
	
	// Update is called once per frame
	void Update () {

	}
}
