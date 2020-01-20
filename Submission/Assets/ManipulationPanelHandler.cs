using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulationPanelHandler : MonoBehaviour {

    private static GameObject manipulationPanelObject;


    private void Awake()
    {
        manipulationPanelObject = this.gameObject;
    }
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public static void PanelSetActive()
    {
        manipulationPanelObject.SetActive(true);
   }

    public static void PanelSetInActive()
    {
        manipulationPanelObject.SetActive(false);
    }

    public static void Transform(Vector3 position)
    {
        manipulationPanelObject.transform.position = position;
    }
}
