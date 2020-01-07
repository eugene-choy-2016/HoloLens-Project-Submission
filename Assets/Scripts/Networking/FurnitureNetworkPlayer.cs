using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SharingWithUNET;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.Networking;

public class FurnitureNetworkPlayer : NetworkBehaviour
{

    [SerializeField]
    public GameObject bedPrefab;

    [SerializeField]
    public GameObject vasePrefab;



    /*
     * Statics
     */
    /// <summary>
    /// Instance of the NetworkPlayer that represents the local player.
    /// </summary>
    public static FurnitureNetworkPlayer Instance { get; private set; }

    /*
     * Members
     */
    // Anchors
    /// <summary>
    /// Tracks if the player associated with the script has found the shared anchor
    /// </summary>
    [SyncVar(hook = "AnchorEstablishedChanged")]
    bool AnchorEstablished;

    /// <summary>
    /// The transform of the shared world anchor.
    /// </summary>
    private Transform sharedWorldAnchorTransform;

    // User
    /// <summary>
    /// Tracks the player name.
    /// </summary>
    [SyncVar(hook = "PlayerNameChanged")]
    string PlayerName;

    //#pragma warning disable 0414
    /// <summary>
    /// Keeps track of the player's IP address
    /// </summary>
    [SyncVar(hook = "PlayerIpChanged")]
    string PlayerIp;
    //#pragma warning restore 0414


    // Transform
    /// <summary>
    /// The position relative to the shared world anchor.
    /// </summary>
    [SyncVar]
    private Vector3 localPosition;

    /// <summary>
    /// The rotation relative to the shared world anchor.
    /// </summary>
    [SyncVar]
    private Quaternion localRotation;

    /*
     * Components
     */
    /// <summary>
    /// Script that handles finding, creating, and joining sessions.
    /// </summary>
    private NetworkDiscoveryWithAnchors networkDiscovery;
    /// <summary>
    /// The anchor manager.
    /// </summary>
    private UNetAnchorManager anchorManager;

    #region MonoBehaviour
    /// <summary>
    /// Initialization 
    /// </summary>
    void Awake()
    {
        // Get Components
        networkDiscovery = NetworkDiscoveryWithAnchors.Instance;
        anchorManager = UNetAnchorManager.Instance;
        
    }

    /// <summary>
    /// Resource Initialization
    /// </summary>
    private void Start()
    {
        if (SharedCollection.Instance == null)
        {
            Debug.LogError("This script required a SharedCollection script attached to a GameObject in the scene");
            Destroy(this);
            return;
        }

        // This instance of the NetworkPlayer belongs to us
        if (isLocalPlayer)
        {
            InitializeLocalPlayer();

            // Introduction to Networked Experiences: Exercise 4.5
            /************************************************************/
            // Activate the shared collection gameObject
            SharedCollection.Instance.gameObject.SetActive(true);

            // We are the server
            if (isServer)
            {
               
            }
            else
            {
                CmdRequestSync();
                SpatialMappingManager.Instance.DrawVisualMeshes = false;
            }
        }
        else
        {
            Debug.Log("remote player");
            // Set to red so that we can differentiate other players from this client's own player
            GetComponentInChildren<MeshRenderer>().material.color = Color.red;

            // Initialize Anchor Establishment variables
            AnchorEstablishedChanged(AnchorEstablished);
        }

        // We will be using the Shared Collection object as the world anchor
        sharedWorldAnchorTransform = SharedCollection.Instance.gameObject.transform;

        // Parent ourselves to it
        transform.SetParent(sharedWorldAnchorTransform);
    }


    private void Update()
    {
      
    }
    #endregion

    #region NetworkBehaviour
    /// <summary>
    /// Called when the local player starts.  In general the side effect should not be noticed
    /// as the players' avatar is always rendered on top of their head.
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        // Set our colour to blue so that we can easily identify ourselves from other players
        GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
    }
    #endregion

    #region Network Anchoring
    /// <summary>
    /// Sent from a local client to the host to update if the shared
    /// anchor has been found.
    /// </summary>
    /// <param name="Established">true if the shared anchor is found</param>
    [Command]
    private void CmdSendAnchorEstablished(bool Established)
    {
        AnchorEstablished = Established;
        if (Established && !UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque && !isLocalPlayer)
        {
            Debug.Log("remote device likes the anchor");
#if UNITY_WSA
            // Notify the anchor manager that the world anchor has been established on this player's client
            anchorManager.AnchorFoundRemotely();
#endif
        }
    }

    /// <summary>
    /// Called when the anchor is either lost or found
    /// </summary>
    /// <param name="update">true if the anchor is found</param>
    void AnchorEstablishedChanged(bool update)
    {
        Debug.LogFormat("AnchorEstablished for {0} was {1} is now {2}", PlayerName, AnchorEstablished, update);

        // Set the flag of whether the anchor was established
        AnchorEstablished = update;
        // only draw the mesh for the player if the anchor is found.
        GetComponentInChildren<MeshRenderer>().enabled = update;
    }
    #endregion

    #region User
    /// <summary>
    /// Sets up all of the local player information
    /// </summary>
    private void InitializeLocalPlayer()
    {
        if (isLocalPlayer)
        {
            Debug.Log("Setting instance for local player ");
            Instance = this;
            Debug.LogFormat("Set local player name {0} IP {1}", networkDiscovery.broadcastData, networkDiscovery.LocalIp);
            CmdSetPlayerName(networkDiscovery.broadcastData);
            CmdSetPlayerIp(networkDiscovery.LocalIp);
        }

    }
    #endregion

    #region Network User
    /// <summary>
    /// Called to set the player name
    /// </summary>
    /// <param name="playerName">The name to update to</param>
    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        PlayerName = playerName;
    }

    /// <summary>
    /// Called when the player name changes.
    /// </summary>
    /// <param name="update">the updated name</param>
    void PlayerNameChanged(string update)
    {
        Debug.LogFormat("Player name changing from {0} to {1}", PlayerName, update);
        PlayerName = update;
        // Special case for spectator view
        if (PlayerName.ToLower() == "spectatorviewpc")
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Command for changing the Player's IP on the server
    /// </summary>
    /// <param name="playerIp">The ip address to change to</param>
    private void CmdSetPlayerIp(string playerIp)
    {
        PlayerIp = playerIp;
    }

    /// <summary>
    /// Called when the player IP address changes
    /// </summary>
    /// <param name="update">The updated IP address</param>
    void PlayerIpChanged(string update)
    {
        PlayerIp = update;
    }
    #endregion


    [Command]
    public void CmdCreateFurniture(string objectName, Vector3 modelLocalPosition, Quaternion modelLocalRotation)
    {
        Debug.Log("Get Furniture Name");
        Debug.Log(objectName);
        RpcCreateFurniture(objectName, modelLocalPosition, modelLocalRotation);
    }

    // For Instance Updates
    [Command]
    public void CmdUpdateFurnitureTransform(string gameObjectName, Vector3 modelLocalPosition, Quaternion modelLocalRotation, Vector3 localScale)
    {
        RpcUpdateFurnitureTransform(gameObjectName,modelLocalPosition, modelLocalRotation, localScale);
    }

    [ClientRpc]
    public void RpcCreateFurniture(string objectName, Vector3 modelLocalPosition, Quaternion modelLocalRotation)
    {
        Debug.Log("RPC Called");
        Debug.Log(objectName);

        if (objectName.Contains("Queen_Bed_FitToRoom"))
        {
            GameObject go = Instantiate(bedPrefab, modelLocalPosition, modelLocalRotation);
            go.transform.SetParent(this.transform.parent.transform);
            go.name = objectName;
            

        }
        else if (objectName.Contains("Vase_1_Prefab"))
        {
            GameObject go = Instantiate(vasePrefab, modelLocalPosition, modelLocalRotation);       
            go.transform.SetParent(this.transform.parent.transform);
            go.name = objectName;

        }
    }

    [ClientRpc]
    public void RpcUpdateFurnitureTransform(string gameObjectName,Vector3 modelLocalPosition, Quaternion modelLocalRotation, Vector3 localScale)
    {
        Debug.Log(gameObjectName + " manipulated");
        GameObject furnitureObj = GameObject.Find(gameObjectName);

        if(furnitureObj != null)
        {
            Debug.Log(gameObjectName + " found and ready to transform");
        }

        //change position too
        furnitureObj.transform.localPosition = modelLocalPosition;
        furnitureObj.transform.localRotation = modelLocalRotation;
        furnitureObj.transform.localScale = localScale;
    }

    //Request sync ask for all the objects and respawn
    [Command]
    public void CmdRequestSync()
    {
        Transform parent = this.transform.parent;
        int childCount = this.transform.parent.childCount;

        for (int i = 0; i < childCount; i++)
        {
            GameObject child = parent.GetChild(i).gameObject;
            RpcCreateFurniture(child.name, child.transform.localPosition, child.transform.localRotation);
            
        }
    }



}
