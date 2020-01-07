using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFurniture : MonoBehaviour, IFocusable, IInputClickHandler, ISpeechHandler
{
    private MeshRenderer meshRenderer;
    private Color initialColor;

    //Reference to Prefab
    [SerializeField]
    public GameObject Furniture;

    [SerializeField]
    public bool isMulti = false;

    [SerializeField]
    public GameObject parentObject;

    public void InstantiateFurniture(Vector3 position)
    {

        if (Furniture != null)
        {
            if (!isMulti)
            {
                Instantiate(Furniture, position, Quaternion.identity); 
            }
            else
            {
                if(parentObject != null && FurnitureNetworkPlayer.Instance != null)
                {
                    Debug.Log("Instantiating Object in the Network");
                    Debug.Log("Name of GO");
                    Debug.Log(Furniture.name);
                    FurnitureNetworkPlayer.Instance.CmdCreateFurniture(Furniture.name, position, Quaternion.identity);
                }
                else
                {
                    if (FurnitureNetworkPlayer.Instance == null)
                    {
                        Debug.LogError("FurnitureNetworkPlayer is null");
                    }

                    else
                    {
                        Debug.LogError("Parent cannot be null if Multiplayer mode is set to true in CreateFurniture.cs");
                    }
                    
                }

            }
            
        }
    }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        initialColor = meshRenderer.material.color;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
        float distance = 2.0f;
        List<GameObject> floorList = GeneratePlanes.floorPlanes;
        GameObject floor = GetFloorPlaneStandingOn(floorList);


        //Instantiate Furniture position infront of player
        Vector3 headPosition = Camera.main.transform.position;
        headPosition = Camera.main.transform.position + Camera.main.transform.forward * distance;
        headPosition.y = floor.transform.position.y ;

        
        InstantiateFurniture(headPosition);

    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    //Incase there is more than one floor element
    public GameObject GetFloorPlaneStandingOn(List<GameObject> floorList)
    {
        Vector3 headPosition = Camera.main.transform.position;
        GameObject standingOn = null;
        float minDist = 999999999999;

        for(int i = 0; i < floorList.Count; i++)
        {
            GameObject currentFloorPlane = floorList[i];
            Collider currentCollider = currentFloorPlane.GetComponent<Collider>();
            Vector3 currentSpot = currentCollider.ClosestPointOnBounds(headPosition);

            float dist = Vector3.Distance(currentSpot, headPosition);

            if(dist < minDist)
            {
                minDist = dist;
                standingOn = currentFloorPlane;
            }
        }

        return standingOn;
    }
}
