using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveModeButtonScript : MonoBehaviour, IFocusable, IInputClickHandler
{
    private MeshRenderer meshRenderer;
    private Color initialColor;


    public const string MOVE_MODE_DEACTIVATE = "Move \n Mode";
    private const string MOVE_MODE_ACTIVATE = "Move \n Activated";

    [SerializeField]
    public Material mouseOverGlowEffect;

    //Static variable to decide if Furniture should be moved
    public static bool isMoveToggled = false;

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

    public static void ToggleMoveMode()
    {
        isMoveToggled = !isMoveToggled;
    }


    public void OnInputClicked(InputClickedEventData eventData)
    {
        ToggleMoveMode();
        if (isMoveToggled)
        {
            Material currentMaterial = meshRenderer.material;
            List<Material> materials = new List<Material>();

            materials.Add(currentMaterial);
            materials.Add(mouseOverGlowEffect);

            meshRenderer.materials = materials.ToArray();

            GameObject.Find("MoveMode").GetComponentInChildren<TextMesh>().text = MOVE_MODE_ACTIVATE;
        }
        else
        {
            List<Material> materials = new List<Material>();
            materials.Add(meshRenderer.materials[0]);
            meshRenderer.materials = materials.ToArray();
            GameObject.Find("MoveMode").GetComponentInChildren<TextMesh>().text = MOVE_MODE_DEACTIVATE;
        }
    }

    public static void ResetMoveButton()
    {

        List<Material> materials = new List<Material>();
        MeshRenderer meshRenderer = GameObject.Find("MoveMode").GetComponentInChildren<MeshRenderer>();
        materials.Add(meshRenderer.materials[0]);
        meshRenderer.materials = materials.ToArray();
        GameObject.Find("MoveMode").GetComponentInChildren<TextMesh>().text = MOVE_MODE_DEACTIVATE;
        GameObject.Find("MoveMode").SetActive(false);
    }

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
}
