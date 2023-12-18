using System.IO;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime; 
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks {
    [SerializeField] private string roomName = "LobbyRoom";
    [SerializeField] private string driver;
    [SerializeField] private string assistant;

    void Start() {
        CustomPrefabPool prefabPool = FindObjectOfType<CustomPrefabPool>();
        if (prefabPool != null) {
            PhotonNetwork.PrefabPool = prefabPool;
        } else {
            Debug.LogError("CustomPrefabPool not found in the scene.");
        }

        // Enable automatic scene synchronization
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected) {
            PhotonNetwork.ConnectUsingSettings(); // Connects to Photon using settings
        }
    }

    public override void OnConnectedToMaster() {
        Debug.Log("Connected to Photon Master Server");
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom() {
    Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
    Debug.Log("Player count in room: " + PhotonNetwork.CurrentRoom.PlayerCount);

    GameObject playerObj = null;

    // Check if this client has already instantiated a player object
    if (PhotonNetwork.LocalPlayer.TagObject == null) {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
            playerObj = PhotonNetwork.Instantiate(this.driver, new Vector3(0, 0, 0), Quaternion.identity);
        } else if (PhotonNetwork.CurrentRoom.PlayerCount == 2) {
            playerObj = PhotonNetwork.Instantiate(this.assistant, new Vector3(0, 0, 0), Quaternion.identity);
        } else {
            Debug.Log("Unexpected player count");
        }

        if (playerObj != null) {
            PhotonNetwork.LocalPlayer.TagObject = playerObj;
            DontDestroyOnLoad(playerObj);
        } else {
            Debug.Log("Player object was not instantiated.");
        }
    } else {
        Debug.Log("Player object already exists for this client.");
    }
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause) {
        Debug.LogError("Disconnected from Photon: " + cause);
    }
}
