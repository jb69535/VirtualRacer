using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Photon.Realtime;


public class RoleSelection : MonoBehaviourPunCallbacks
{
    private string player1Role = "";
    private string player2Role = "";
    private bool isPlayer1SelectingRole = false;
    private bool isPlayer2SelectingRole = false;
    public bool isPLayerDriver;

    // Event code for game starting notification
    private const byte GameStartingEventCode = 101;

    private void Awake() {
        PhotonNetwork.AutomaticallySyncScene = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
    }

    private void Update() {
        if (PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom != null) {
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

            if (playerCount == 1) {
                HandleSinglePlayerInput();
            } else if (playerCount == 2) {
                HandleMultiPlayerInput();
            }
            if(Input.GetKeyDown(KeyCode.Return)){
                Debug.Log("press key return");
            }

            if (Input.GetKeyDown(KeyCode.Return) && AreRolesProperlySelected()) {
                Debug.Log("Starting game");
                StartGame();
            }
        }
        playerDriver();
    }

    private void HandleSinglePlayerInput() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Debug.Log("Player 1 is selecting");
            isPlayer1SelectingRole = true;
        }

        if (isPlayer1SelectingRole) {
            if (Input.GetKeyDown(KeyCode.Z)) {
                Debug.Log("Player 1: Driver selected");
                player1Role = "Driver";
                isPLayerDriver = true;
                SetRoleForPlayer("Player1", player1Role);
                isPlayer1SelectingRole = false;
            }
        }
    }

    private void HandleMultiPlayerInput() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            isPlayer1SelectingRole = true;
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            isPlayer2SelectingRole = true;
        }

        if (isPlayer1SelectingRole) {
            if (Input.GetKeyDown(KeyCode.Z)) {
                player1Role = "Driver";
                SetRoleForPlayer("Player1", player1Role);
                isPLayerDriver = true;
                isPlayer1SelectingRole = false;
            } else if (Input.GetKeyDown(KeyCode.C)) {
                player1Role = "Assistant";
                SetRoleForPlayer("Player1", player1Role);
                isPlayer1SelectingRole = false;
                isPLayerDriver = false;
            }
        }

        if (isPlayer2SelectingRole) {
            if (Input.GetKeyDown(KeyCode.Z)) {
                player2Role = "Driver";
                SetRoleForPlayer("Player2", player2Role);
                isPLayerDriver = true;
                isPlayer2SelectingRole = false;
            } else if (Input.GetKeyDown(KeyCode.C)) {
                player2Role = "Assistant";
                SetRoleForPlayer("Player2", player2Role);
                isPlayer2SelectingRole = false;
                isPLayerDriver = false;
            }
        }
    }

    public bool playerDriver()
    {
        return isPLayerDriver;
    }

    private void SetRoleForPlayer(string playerId, string role) {
        var playerProperties = new ExitGames.Client.Photon.Hashtable { { playerId, role } };
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
        Debug.Log(playerId + " role chosen: " + role);
    }

    private bool AreRolesProperlySelected() {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
            return player1Role == "Driver";
        } else if (PhotonNetwork.CurrentRoom.PlayerCount == 2) {
            return true;
        }
        return false;
    }

    private void StartGame() {
        if (PhotonNetwork.IsMasterClient) {
            // Send a custom event to notify that the game is starting
            PhotonNetwork.RaiseEvent(GameStartingEventCode, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            PhotonNetwork.LoadLevel("RallyMap2");
        }
    }

    private void OnEventReceived(EventData photonEvent) {
        if (photonEvent.Code == GameStartingEventCode) {
            Debug.Log("Received game starting notification");
            // Additional actions when game is starting, like showing a notification
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "RallyMap2") {
            Debug.Log(PhotonNetwork.LocalPlayer.NickName + " entered the NewPhysicsTesting scene");
        }
    }
}
