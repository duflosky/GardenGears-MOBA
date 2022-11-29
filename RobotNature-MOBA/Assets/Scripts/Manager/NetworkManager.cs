using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;
    public string sceneToLoad = "LobbyScene"; 

    #region Unity Methods
    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    
    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("ServeurLobby");
    }

    public void CreateRoom(string roomName)
    {
        if (roomName == "") PhotonNetwork.CreateRoom("default");
        else PhotonNetwork.CreateRoom(roomName);
    }

    public void JoinRoom(string roomName)
    {
        if (roomName == "") PhotonNetwork.JoinRoom("default");
        else PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(sceneToLoad);
    }
    
    #endregion
}