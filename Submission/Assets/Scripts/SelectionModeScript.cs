using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionModeScript : MonoBehaviour, IFocusable, IInputClickHandler
{

    [SerializeField]
    public Material mouseOverGlowEffect;
    public static bool isSelectionMode = false;

    private MeshRenderer meshRenderer;
    private Color initialColor;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        initialColor = meshRenderer.material.color;
    }


    public static void ToggleSelectionMode()
    {
        isSelectionMode = !isSelectionMode;
    }

    public void OnFocusEnter()
    {
        meshRenderer.material.EnableKeyword("_EMISSION");
        meshRenderer.material.SetColor("_EmissionColor", Color.green);
        meshRenderer.material.color = Color.green;
    }

    public void OnFocusExit()
    {
        meshRenderer.material.DisableKeyword("_EMISSION");
        meshRenderer.material.SetColor("_EmissionColor", Color.white);
        meshRenderer.material.color = initialColor;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        ToggleSelectionMode();   
        if (isSelectionMode)
        {
            Material currentMaterial = meshRenderer.material;
            List<Material> materials = new List<Material>();

            materials.Add(currentMaterial);
            materials.Add(mouseOverGlowEffect);

            meshRenderer.materials = materials.ToArray();
        }
        else
        {
            List<Material> materials = new List<Material>();
            materials.Add(meshRenderer.materials[0]);
            meshRenderer.materials = materials.ToArray();
        }
    }
}
