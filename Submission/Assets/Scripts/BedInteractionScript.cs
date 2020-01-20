using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is just a random easter egg script for the Bed at the Main Menu
public class BedInteractionScript : MonoBehaviour, IFocusable {

    private GameObject bedTooltip;

    public void OnFocusEnter()
    {
        bedTooltip.SetActive(true);
    }

    public void OnFocusExit()
    {
        bedTooltip.SetActive(false);
    }

    // Use this for initialization
    void Start () {
        bedTooltip = GameObject.Find("BedToolTip");
        bedTooltip.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
