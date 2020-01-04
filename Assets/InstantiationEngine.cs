using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiationEngine : MonoBehaviour {


    //Reference to Prefab
    [SerializeField]
    public GameObject instantiateOne;

    [SerializeField]
    public  GameObject instantiateTwo;

    [SerializeField]
    public  GameObject instantiateThree;

	// Use this for initialization
	void Start () {
		

	}

    public  void InstantiateInstanceOne(Vector3 position)
    {
        
        if (instantiateOne != null)
        {
            Instantiate(instantiateOne, position, Quaternion.identity);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
