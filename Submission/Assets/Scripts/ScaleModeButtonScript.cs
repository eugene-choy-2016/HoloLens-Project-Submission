using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleModeButtonScript : ManipulationModeButton, IInputClickHandler
{

    public const string SCALE_MODE_DEACTIVATE = "Scale\nMode";
    public const string SCALE_MODE_ACTIVATE = "Scale\nActivated";

    public static bool isScaleActivated = false;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!isScaleActivated && meshRenderer.materials.Length > 1)
        {
            List<Material> materials = new List<Material>();
            materials.Add(meshRenderer.materials[0]);
            meshRenderer.materials = materials.ToArray();
            GameObject.Find(gameObjectName).GetComponentInChildren<TextMesh>().text = deactivatedText;
        }
	}

    void Awake()
    {

        InitializeManipulationButton("ScaleMode", SCALE_MODE_ACTIVATE, SCALE_MODE_DEACTIVATE);
        meshRenderer = GetComponent<MeshRenderer>();
        initialColor = meshRenderer.material.color;
    }

    public static void ToggleManipulationMode()
    {
        isScaleActivated = !isScaleActivated;

        if (isScaleActivated)
        {
            MoveModeButtonScript.isMoveActivated = false;
            RotationModeButtonScript.isRotationActivated = false;
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        ToggleManipulationMode();
        if (isScaleActivated)
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

}
