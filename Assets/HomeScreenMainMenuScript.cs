using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScreenMainMenuScript : MonoBehaviour {

    public static GameObject thisObj;

	// Use this for initialization
	void Start () {
        thisObj = this.gameObject;
        thisObj.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void ShowMainMenu()
    {
        thisObj.SetActive(true);
    }

    public static void HideMainMenu()
    {
        thisObj.SetActive(false);
    }

}
