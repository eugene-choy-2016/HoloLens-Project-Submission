using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Script for the Bed Prefab
public class BedObjectScript : MonoBehaviour,IFocusable, IInputClickHandler {

    [SerializeField]
    public GameObject MoveButton;

    [SerializeField]
    public Material mouseOverGlowEffect;

    private Transform currentObjPosition;
    private MeshRenderer[] childRenderer;


    private GameObject instantiatedScaleButton;

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
            instantiatedScaleButton.SetActive(false);
            OnFocusExit();
        }

    }

    private void Awake()
    {
        currentObjPosition = GetComponent<Transform>();
        Vector3 centerPosition = currentObjPosition.localPosition;

        //Spawn a Scale Button and set it to fale
        centerPosition.y += 1.0f;
        instantiatedScaleButton = Instantiate(ScaleButton, centerPosition, Quaternion.identity);
        instantiatedScaleButton.SetActive(false);

        childRenderer = GetComponentsInChildren<MeshRenderer>();
        
    }

    public void OnFocusEnter()
    {

        foreach(MeshRenderer mr in childRenderer)
        {
            Material currentMaterial = mr.material;
            List<Material> materials = new List<Material>();

            materials.Add(currentMaterial);
            materials.Add(mouseOverGlowEffect);

            mr.materials = materials.ToArray();
        }

    }

    public void OnFocusExit()
    {
        foreach (MeshRenderer mr in childRenderer)
        {
            List<Material> materials = new List<Material>();
            materials.Add(mr.materials[0]);
            mr.materials = materials.ToArray();
        }

    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        instantiatedScaleButton.SetActive(true);
    }
}
