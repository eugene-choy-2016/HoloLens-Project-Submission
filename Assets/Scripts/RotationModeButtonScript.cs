using System.Collections;
using HoloToolkit.Unity.InputModule;
using System.Collections.Generic;
using UnityEngine;

public class RotationModeButtonScript : ManipulationModeButton, IInputClickHandler
{

    public const string ROTATION_MODE_DEACTIVATE = "Rotation\nMode";
    public const string ROTATION_MODE_ACTIVATE = "Rotation\nActivated";

    public static bool isRotationActivated = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!isRotationActivated && meshRenderer.materials.Length > 1)
        {
            List<Material> materials = new List<Material>();
            materials.Add(meshRenderer.materials[0]);
            meshRenderer.materials = materials.ToArray();
            GameObject.Find(gameObjectName).GetComponentInChildren<TextMesh>().text = deactivatedText;
        }
    }

    void Awake()
    {
        InitializeManipulationButton("RotationMode", ROTATION_MODE_ACTIVATE, ROTATION_MODE_DEACTIVATE);
        meshRenderer = GetComponent<MeshRenderer>();
        initialColor = meshRenderer.material.color;
    }

    public static void ToggleManipulationMode()
    {
        isRotationActivated = !isRotationActivated;

        if (isRotationActivated)
        {
            MoveModeButtonScript.isMoveActivated = false;
            ScaleModeButtonScript.isScaleActivated = false;
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        ToggleManipulationMode();
        if (isRotationActivated)
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
