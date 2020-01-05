using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Script for the Bed Prefab
public class FurnitureObjectScript : MonoBehaviour,IFocusable, IInputClickHandler, IManipulationHandler
{

    [SerializeField]
    public Material mouseOverGlowEffect;

    private Transform currentObjPosition;
    private MeshRenderer[] childRenderer;
    private Vector3 originalPosition;
    private GameObject moveButton;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        currentObjPosition = GetComponent<Transform>();
        Vector3 centerPosition = currentObjPosition.localPosition;
        
        float dist = Vector3.Distance(Camera.main.transform.position, currentObjPosition.localPosition);

        //If player is more than 5meters away assume that they do not need the button and hide it
        if (dist >= 5.0f)
        {
            moveButton.SetActive(true);
            MoveModeButtonScript.ResetMoveButton();
            MoveModeButtonScript.isMoveToggled = false; //Hide button when too far and deactivate moving mode   
            OnFocusExit();
        }

    }

    private void Awake()
    {
        currentObjPosition = GetComponent<Transform>();
        Vector3 centerPosition = currentObjPosition.localPosition;

        //Spawn a Scale Button and set it to fale

        centerPosition.y += 1.0f;

        moveButton = GameObject.Find("MoveMode"); 
        moveButton.SetActive(false);

        childRenderer = GetComponentsInChildren<MeshRenderer>();
        
    }

    public void OnFocusEnter()
    {
        if (SelectionModeScript.isSelectionMode)
        {
            foreach (MeshRenderer mr in childRenderer)
            {
                Material currentMaterial = mr.material;
                List<Material> materials = new List<Material>();

                materials.Add(currentMaterial);
                materials.Add(mouseOverGlowEffect);

                mr.materials = materials.ToArray();
            }
        }


    }

    public void OnFocusExit()
    {
        if (SelectionModeScript.isSelectionMode)
        {
            foreach (MeshRenderer mr in childRenderer)
            {
                List<Material> materials = new List<Material>();
                materials.Add(mr.materials[0]);
                mr.materials = materials.ToArray();
            }
        }

    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        float distance = 3.0f;

        Vector3 centerPosition = currentObjPosition.localPosition;
        //Spawn a Scale Button and set it to fale
        centerPosition.y += 1.0f;

        Vector3 headPosition = Camera.main.transform.position + Camera.main.transform.forward * distance;
        headPosition.y = centerPosition.y;
        moveButton.transform.position = headPosition;

        moveButton.SetActive(true);


    }

    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        Debug.Log("Manipulation Detected");
        InputManager.Instance.AddGlobalListener(gameObject);
        if (MoveModeButtonScript.isMoveToggled)
        {
            originalPosition = transform.position;
        }
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        if (MoveModeButtonScript.isMoveToggled)
        {
            //Lock the Y coordinate coz idw make bed to move up
            Vector3 transformed = originalPosition + eventData.CumulativeDelta;
            transformed.y = originalPosition.y;
            transform.position = transformed;
            
        }
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        InputManager.Instance.RemoveGlobalListener(gameObject);
    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        OnManipulationCompleted(eventData);
    }
}
