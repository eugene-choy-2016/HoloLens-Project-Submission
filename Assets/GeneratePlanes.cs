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

    

    [SerializeField]
    public GameObject ceilingObject; //For the Prefab for Instantiating

    [SerializeField]
    public GameObject ceilingObjectModel; //For the model of the ceilingObject  

    private void Awake()
    {
        
    }

    // Use this for initialization
    void Start () {
        SurfaceMeshesToPlanes.Instance.MakePlanes();
        SurfaceMeshesToPlanes.Instance.MakePlanesComplete += OnPlanesCreated;

        wallPlanes = new List<GameObject>();
        floorPlanes = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnPlanesCreated(object source, EventArgs args)
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

        //If there is a ceiling
        GameObject nearestCeiling = GetNearestPlane(ceilingPlanes);

        //Attempt to Spawn Fan but not sure if it works since i couldnt get a ceiling plane
        if(nearestCeiling != null)
        {
            Vector3 headPosition = Camera.main.transform.position;
            headPosition = Camera.main.transform.position + Camera.main.transform.forward;
            headPosition.y = nearestCeiling.transform.position.y;
            InstantiateFan(ceilingObject, headPosition);
        }

        //if no ceiling, choose the nearest wall find out its height and spawn the fan on top of player directly w Y 
        //being the height of nearest wall + 1.5 times of its own size
        else
        {
            Debug.Log("No Ceiling object, spawning it on top of player");
            GameObject nearestWall = GetNearestPlane(wallPlanes);
            if(nearestWall != null)
            {
                Debug.Log("Nearest wall height: " + nearestWall.transform.localScale.y);
                Debug.Log("Nearest wall width: " + nearestWall.transform.localScale.x);
                Debug.Log("Ceiling Object Actual Height: " + GetDimensions(ceilingObjectModel).y);
   
                float height = nearestWall.transform.localScale.y + (GetDimensions(ceilingObjectModel).y*1.5f);
                Debug.Log("Height: " + height);
                Vector3 headPosition = Camera.main.transform.position;
                headPosition = Camera.main.transform.position + Camera.main.transform.forward;
                headPosition.y = height;
                InstantiateFan(ceilingObject, headPosition);
            }
        }

    }

    private Vector3 GetDimensions(GameObject obj)
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


    public void InstantiateFan(GameObject fan,Vector3 position)
    {
        if (fan != null)
        {
            Instantiate(fan, position, Quaternion.identity);
        }
    }

    //Incase there is more than one floor element
    public GameObject GetNearestPlane(List<GameObject> planeList)
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
