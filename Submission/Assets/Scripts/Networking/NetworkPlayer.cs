using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SharingWithUNET;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Script that handles the responsibilities of the "Player Object" in the network.
/// Based on a cleaned up version of MRTK's PlayerController.cs
/// </summary>
public class NetworkPlayer : NetworkBehaviour
{
    

    /*
     * Statics
     */
    /// <summary>
    /// Instance of the NetworkPlayer that represents the local player.
    /// </summary>
    public static NetworkPlayer Instance { get; private set; }

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
                // Place posters
                AutoPlacePosters.Instance.BeginAutoPlacement();
            }
            else    // We are not the server
            {
                // Introduction to Networked Experiences: Exercise 7.2
                /************************************************************/
                // Call “CmdRequestSync()” to request a sync with the server
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

    /// <summary>
    /// Update function
    /// </summary>
    private void Update()
    {
        /*
         * Non-local Player Section (AKA this player is from another client)
         */
        // If we aren't the local player, we just need to make sure that the position of this object is set properly
        // so that we properly render their avatar in our world.
        if (!isLocalPlayer)
        {
            if (string.IsNullOrEmpty(PlayerName) == false)
            {
                // Interpolate movement
                transform.localPosition = Vector3.Lerp(transform.localPosition, localPosition, 0.3f);
                // Snap rotation
                transform.localRotation = localRotation;
            }

            return;
        }

        /*
         * Local Player Section (AKA this player is the player we created from this client)
         */

        // if our anchor established state has changed, update everyone
        if (AnchorEstablished != anchorManager.AnchorEstablished)
        {
            CmdSendAnchorEstablished(anchorManager.AnchorEstablished);
        }

        // if our anchor isn't established, we shouldn't bother sending transforms.
        if (AnchorEstablished == false)
        {
            return;
        }

        // If this player object is the one we control then we need to update our worldPosition and then set our 
        // local (to the shared world anchor) position for other clients to update our position in their world.
        transform.position = CameraCache.Main.transform.position;
        transform.rotation = CameraCache.Main.transform.rotation;

        // Send our current local position and rotation so that it can be used to sync on other clients
        CmdTransform(transform.localPosition, transform.localRotation);
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

    #region Network Transform
    /// <summary>
    /// Sets the localPosition and localRotation on clients.
    /// </summary>
    /// <param name="postion">the localPosition to set</param>
    /// <param name="rotation">the localRotation to set</param>
    [Command(channel = 1)]
    public void CmdTransform(Vector3 postion, Quaternion rotation)
    {
        localPosition = postion;
        localRotation = rotation;
    }
    #endregion

    /// <summary>
    /// Command function called by the client and executed on the server to request setting the 
    /// transform values of the posters
    /// </summary>
    /// <param name="holoLens">If true, the hololens will be the poster we are positioning</param>
    /// <param name="car">If true, the car will be the poster we are positioning</param>
    /// <param name="engine">If true, the engine will be the poster we are positioning</param>
    /// <param name="localPosition">The local position to set the poster to</param>
    /// <param name="localRotation">The local rotation to set the poster to</param>
    [Command]
    public void CmdSetPosterTransform(bool holoLens, bool car, bool engine, Vector3 localPosition, Quaternion localRotation)
    {
        RpcSetPosterTransform(holoLens, car, engine, localPosition, localRotation);
    }

    /// <summary>
    /// ClientRpc function called by the server and executed on the clients to set the transform values of the posters
    /// </summary>
    /// <param name="holoLens">If true, the hololens will be the poster we are positioning</param>
    /// <param name="car">If true, the car will be the poster we are positioning</param>
    /// <param name="engine">If true, the engine will be the poster we are positioning</param>
    /// <param name="localPosition">The local position to set the poster to</param>
    /// <param name="localRotation">The local rotation to set the poster to</param>
    [ClientRpc]
    public void RpcSetPosterTransform(bool holoLens, bool car, bool engine, Vector3 localPosition, Quaternion localRotation)
    {
        PosterManager.Instance.SetPosterTransform(holoLens, car, engine, localPosition, localRotation);
    }

    /// <summary>
    /// Command function called by the client and executed on the server to request setting the currently showed model
    /// </summary>
    /// <param name="hololens">Whether to show the hololens model</param>
    /// <param name="car">Whether to show the car model</param>
    /// <param name="engine">Whether to show the engine model</param>
    [Command]
    public void CmdShowModel(bool hololens, bool car, bool engine)
    {
        // Introduction to Networked Experiences: Exercise 5.1.a
        /************************************************************/
        // Call "RpcShowModel()"
        RpcShowModel(hololens, car, engine);

    }

    /// <summary>
    /// ClientRpc function called by the server and executed on all clients to set the currently showed model
    /// </summary>
    /// <param name="hololens">Whether to show the hololens model</param>
    /// <param name="car">Whether to show the car model</param>
    /// <param name="engine">Whether to show the engine model</param>
    [ClientRpc]
    public void RpcShowModel(bool hololens, bool car, bool engine)
    {
        // Introduction to Networked Experiences: Exercise 5.1.b
        /************************************************************/
        // Call ModelLibrary's instance's "ShowModel()"
        ModelLibrary.Instance.ShowModel(hololens, car, engine);

    }

    // Introduction to Networked Experiences: Exercise 6.1.a
    /************************************************************/
    // Create “CmdUpdateModelTransform(Vector3 modelLocalPosition, Quaternion modelLocalRotation)” with a [Command] attribute
    [Command]
    public void CmdUpdateModelTransform(Vector3 modelLocalPosition, Quaternion modelLocalRotation)
    {
        RpcUpdateModelTransform(modelLocalPosition, modelLocalRotation);
    }


    // Introduction to Networked Experiences: Exercise 6.1.c
    /************************************************************/
    // Call "RpcUpdateModelTransform()"



    // Introduction to Networked Experiences: Exercise 6.1.b
    /************************************************************/
    // Create “RpcUpdateModelTransform(Vector3 modelLocalPosition, Quaternion modelLocalRotation)” with a [ClientRpc] attribute
    [ClientRpc]
    public void RpcUpdateModelTransform(Vector3 modelLocalPosition, Quaternion modelLocalRotation)
    {
        ModelLibrary.Instance.gameObject.transform.localPosition = modelLocalPosition;
        ModelLibrary.Instance.gameObject.transform.localRotation = modelLocalRotation;

    }

    // Introduction to Networked Experiences: Exercise 6.1.d.i
    /************************************************************/
    // Set the “ModelLibrary” instance’s local position to “modelLocalPosition”


    // Introduction to Networked Experiences: Exercise 6.1.d.ii
    /************************************************************/
    // Set the “ModelLibrary” instance’s local rotation to “modelLocalRotation”



    /// <summary>
    /// Command function called by the client and executed on the server to request for a sync of all the variables
    /// </summary>
    [Command]
    public void CmdRequestSync()
    {
        // Introduction to Networked Experiences: Exercise 7.1.a
        /************************************************************/
        // Call “RpcShowModel()” and pass in the boolean variables from ModelLibrary's instance that 
        // describe the current model shown to update the current active model
        RpcShowModel(ModelLibrary.Instance.IsHoloLensActive(), ModelLibrary.Instance.IsCarActive(), ModelLibrary.Instance.IsEngineActive());

        // Introduction to Networked Experiences: Exercise 7.1.b
        /************************************************************/
        // Call "RpcSetModelTransform()" with the ModelLibrary's instance's transform values
        // to update the position and rotation
        RpcUpdateModelTransform(ModelLibrary.Instance.transform.localPosition, ModelLibrary.Instance.transform.localRotation);


        // Introduction to Networked Experiences: Exercise 7.1.c
        /************************************************************/
        // Call PosterManager’s  instance’s “RequestSyncPosters()” to request a sync to update the poster positions
        PosterManager.Instance.RequestSyncPosters();
    }
}
