using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Parent class for All Manipulation Mode Buttons for Gaze
public class ManipulationModeButton : MonoBehaviour,IFocusable
{
    [SerializeField]
    public Material mouseOverGlowEffect;
    protected MeshRenderer meshRenderer;
    public static Color initialColor;

    protected string gameObjectName;
    protected string activatedText;
    protected string deactivatedText;


    //initialze the button to enable interactions
    public void InitializeManipulationButton(string _gameObjectName, string activated,string deactivated)
    {
        gameObjectName = _gameObjectName;
        activatedText = activated;
        deactivatedText = deactivated;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Awake()
    {

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


}
