using HoloToolkit.Unity.InputModule;
using UnityEngine;
using HoloToolkit.Unity;

public class TagAlongButtonScript : MonoBehaviour, IFocusable, IInputClickHandler
{

    private string initialText;
    private MeshRenderer meshRenderer;
    private Color initialColor;
    private GameObject text;

    private const string TOGGLE_MODE_ACTIVATE = "Activate \n Toggle";

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void Awake()
    {
        text = GameObject.Find("TagAlongText");
        initialText = text.GetComponent<TextMesh>().text;   

        meshRenderer = GetComponent<MeshRenderer>();
        initialColor = meshRenderer.material.color;
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
        SimpleTagalong.isTagAlong = !SimpleTagalong.isTagAlong;
  

        if (!SimpleTagalong.isTagAlong)
        {
            text.GetComponent<TextMesh>().text = TOGGLE_MODE_ACTIVATE;
        }
        else
        {
            text.GetComponent<TextMesh>().text = initialText;
        }
    }

  
}
