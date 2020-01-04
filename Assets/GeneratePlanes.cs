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


    }
}
