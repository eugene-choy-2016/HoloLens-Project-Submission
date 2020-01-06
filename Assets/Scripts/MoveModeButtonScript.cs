using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveModeButtonScript : ManipulationModeButton, IInputClickHandler
{

    public const string MOVE_MODE_DEACTIVATE = "Move\nMode";
    public const string MOVE_MODE_ACTIVATE = "Move\nActivated";


    public static bool isMoveActivated = false;

    public static void ToggleManipulationMode()
    {
        isMoveActivated = !isMoveActivated;

        if (isMoveActivated)
        {
            ScaleModeButtonScript.isScaleActivated = false;         
        }
    }
    
    public void OnInputClicked(InputClickedEventData eventData)
    {
        ToggleManipulationMode();
        if (isMoveActivated)
        {
            Material currentMaterial = meshRenderer.material;
            List<Material> materials = new List<Material>();

            materials.Add(currentMaterial);
            materials.Add(mouseOverGlowEffect);

            meshRenderer.materials = materials.ToArray();

            GameObject.Find(gameObjectName).GetComponentInChildren<TextMesh>().text = activatedText;
        }
        else
        {
            List<Material> materials = new List<Material>();
            materials.Add(meshRenderer.materials[0]);
            meshRenderer.materials = materials.ToArray();
            GameObject.Find(gameObjectName).GetComponentInChildren<TextMesh>().text = deactivatedText;
        }
    }

    public static void ResetMoveButton()
    {

        List<Material> materials = new List<Material>();
        MeshRenderer meshRenderer = GameObject.Find("ManipulationPanel").transform.Find("MoveMode").gameObject.GetComponentInChildren<MeshRenderer>();
        materials.Add(meshRenderer.materials[0]);
        meshRenderer.materials = materials.ToArray();
        GameObject.Find("ManipulationPanel").transform.Find("MoveMode").gameObject.GetComponentInChildren<TextMesh>().text = MOVE_MODE_DEACTIVATE;
        

        //Unfocus after resetting
        meshRenderer.material.DisableKeyword("_EMISSION");
        meshRenderer.material.SetColor("_EmissionColor", Color.white);
        meshRenderer.material.color = initialColor;


    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!isMoveActivated && meshRenderer.materials.Length > 1)
        {
            List<Material> materials = new List<Material>();
            materials.Add(meshRenderer.materials[0]);
            meshRenderer.materials = materials.ToArray();
            GameObject.Find(gameObjectName).GetComponentInChildren<TextMesh>().text = deactivatedText;
        }
    }
   
    void Awake()
    {
        InitializeManipulationButton("MoveMode", MOVE_MODE_ACTIVATE, MOVE_MODE_DEACTIVATE);
        meshRenderer = GetComponent<MeshRenderer>();
        initialColor = meshRenderer.material.color;
    }
}
