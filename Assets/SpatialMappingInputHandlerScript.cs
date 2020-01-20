using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SpatialMappingInputHandlerScript : MonoBehaviour, IInputClickHandler
{
    GameObject funnyBed;
    // Use this for initialization
    void Start()
    {
        funnyBed = GameObject.Find("Bed_wToolTip");
        InputManager.Instance.AddGlobalListener(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        HomeScreenMainMenuScript.ShowMainMenu();
        GeneratePlanes.StartProcessing();
        gameObject.SetActive(false);
        funnyBed.SetActive(false);
    }
}
