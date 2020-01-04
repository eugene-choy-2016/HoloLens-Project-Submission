using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddFurniture : MonoBehaviour, IFocusable, IInputClickHandler, ISpeechHandler
{
    private MeshRenderer meshRenderer;
    private Color initialColor;

    [SerializeField]
    private GameObject bedButton;
    private GameObject text;

    private string originalText;
    private bool isDrawerOpen;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        initialColor = meshRenderer.material.color;
        text = GameObject.Find("AddFurnitureText");
        originalText = text.GetComponent<TextMesh>().text;

        if(bedButton != null)
        {
            bedButton.SetActive(false);
        }
        
        

        isDrawerOpen = false;
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
        isDrawerOpen = !isDrawerOpen;
        bedButton.SetActive(isDrawerOpen);
        
        if (isDrawerOpen)
        {
            text.GetComponent<TextMesh>().text = "Cancel";
        }
        else
        {
            text.GetComponent<TextMesh>().text = originalText;
        }
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        throw new System.NotImplementedException();
    }



    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
