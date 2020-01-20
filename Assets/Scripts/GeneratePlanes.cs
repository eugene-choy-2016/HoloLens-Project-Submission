using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using System;

[RequireComponent(typeof(SurfaceMeshesToPlanes))]
[RequireComponent(typeof(RemoveSurfaceVertices))]
public class GeneratePlanes : MonoBehaviour {

    public static List<GameObject> wallPlanes;
    public static List<GameObject> floorPlanes; 
    public static List<GameObject> ceilingPlanes;

    public static bool PlanesProcessed = false;

    [SerializeField]
    public GameObject ceilingObject; //For the Prefab for Instantiating


    [SerializeField]
    public GameObject ceilingObjectModel; //For the model of the ceilingObject

    public static GameObject ceilingObject_s;
    public static GameObject ceilingObjectModel_s;

    private void Awake()
    {
        ceilingObject_s = ceilingObject;
        ceilingObjectModel_s = ceilingObjectModel;
    }

    public static void StartProcessing()
    {
        SurfaceMeshesToPlanes.Instance.MakePlanes();
        SurfaceMeshesToPlanes.Instance.MakePlanesComplete += OnPlanesCreated;

        wallPlanes = new List<GameObject>();
        floorPlanes = new List<GameObject>();

        PlanesProcessed = true;
    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private static void OnPlanesCreated(object source, EventArgs args)
    {

        RemoveSurfaceVertices.Instance.RemoveSurfaceVerticesWithinBounds(SurfaceMeshesToPlanes.Instance.ActivePlanes);
        SpatialMappingManager.Instance.DrawVisualMeshes = false;


        // Collection of wall planes that we can use to set vertical items on.
        wallPlanes = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Wall);
        floorPlanes = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Floor);
        ceilingPlanes = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Ceiling);

        Debug.Log("Wall Planes count: " + wallPlanes.Count);
        Debug.Log("Floor Planes count: " + floorPlanes.Count);
        Debug.Log("Ceiling Planes count: " + ceilingPlanes.Count);

        PlanesProcessed = true;
        generateCeilingFan();


    }

    public static void generateCeilingFan()
    {
        if (PlanesProcessed)
        {
            //If there is a ceiling
            GameObject nearestCeiling = GetNearestPlane(ceilingPlanes);

            //Attempt to Spawn Fan but not sure if it works since i couldnt get a ceiling plane
            if (nearestCeiling != null)
            {
                Vector3 headPosition = Camera.main.transform.position;
                headPosition = Camera.main.transform.position + Camera.main.transform.forward;
                headPosition.y = nearestCeiling.transform.position.y;
                InstantiateFan(ceilingObject_s, headPosition);
            }

            //if no ceiling, choose the nearest wall find out its height and spawn the fan on top of player directly w Y 
            //being the height of nearest wall + 1.5 times of its own size
            else
            {
                Debug.Log("No Ceiling object, spawning it on top of player");
                GameObject nearestWall = GetNearestPlane(wallPlanes);
                if (nearestWall != null)
                {
                    Debug.Log("Nearest wall height: " + nearestWall.transform.localScale.y);
                    Debug.Log("Nearest wall width: " + nearestWall.transform.localScale.x);
                    Debug.Log("Ceiling Object Actual Height: " + GetDimensions(ceilingObjectModel_s).y);

                    float height = nearestWall.transform.localScale.y + (GetDimensions(ceilingObjectModel_s).y * 1.5f);
                    Debug.Log("Height: " + height);
                    Vector3 headPosition = Camera.main.transform.position;
                    headPosition = Camera.main.transform.position + Camera.main.transform.forward;
                    headPosition.y = height;
                    InstantiateFan(ceilingObject_s, headPosition);
                }
            }
        }
       
    }

    private static Vector3 GetDimensions(GameObject obj)
    {
        Vector3 min = Vector3.one * Mathf.Infinity;
        Vector3 max = Vector3.one * Mathf.NegativeInfinity;

        Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 vert = mesh.vertices[i];
            min = Vector3.Min(min, vert);
            max = Vector3.Max(max, vert);
        }

        // the size is max-min multiplied by the object scale:
        return Vector3.Scale(max - min, obj.transform.localScale);
    }


    public static void InstantiateFan(GameObject fan,Vector3 position)
    {
        if (fan != null)
        {
            Instantiate(fan, position, Quaternion.identity);
        }
    }

    //Incase there is more than one floor element
    public static GameObject GetNearestPlane(List<GameObject> planeList)
    {

        if (planeList.Count <= 0)
        {
            return null;
        }

        Vector3 headPosition = Camera.main.transform.position;
        GameObject standingOn = null;
        float minDist = 999999999999;

        for (int i = 0; i < planeList.Count; i++)
        {
            GameObject currentPlane = planeList[i];
            Collider currentCollider = currentPlane.GetComponent<Collider>();
            Vector3 currentSpot = currentCollider.ClosestPointOnBounds(headPosition);

            float dist = Vector3.Distance(currentSpot, headPosition);

            if (dist < minDist)
            {
                minDist = dist;
                standingOn = currentPlane;
            }
        }

        return standingOn;
    }
}
