using System.Collections.Generic;
using GameStates;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;
    public string sceneToLoad = "LobbyScene";
    
    [SerializeField] private TMP_InputField createRoomTMPInputField;
    
    [Header("Room List UI Panel")]
    [SerializeField] GameObject roomListPanel;
    [SerializeField] GameObject roomListEntryPrefab;
    [SerializeField] GameObject roomListParentGameObject;
    
    Dictionary<string, RoomInfo> cachedRoomList;
    Dictionary<string, GameObject> roomListGameObjects;

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
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListGameObjects = new Dictionary<string, GameObject>();
    }
    
    #endregion

    #region UI Callbacks

    public void OnCreateRoomButtonClicked()
    {
        CreateRoom(createRoomTMPInputField.text);
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        ActivatePanel(roomListPanel.name);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(sceneToLoad);
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();
        foreach (RoomInfo room in roomList)
        {
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList )
            {
                if (cachedRoomList.ContainsKey(room.Name))
                {
                    cachedRoomList.Remove(room.Name);
                }
            }
            else
            {
                if (cachedRoomList.ContainsKey(room.Name))
                {
                    cachedRoomList[room.Name] = room;
                }
                else
                {
                    cachedRoomList.Add(room.Name, room);
                }
            }
        }

        foreach (RoomInfo room in cachedRoomList.Values)
        {
            GameObject roomListEntryGameObject = Instantiate(roomListEntryPrefab, roomListParentGameObject.transform, true);
            roomListEntryGameObject.transform.localScale = Vector3.one;
            roomListEntryGameObject.transform.Find("RoomNameText").GetComponent<Text>().text = room.Name;
            roomListEntryGameObject.transform.Find("RoomPlayersText").GetComponent<Text>().text = room.PlayerCount+ " / "+ 4;
            roomListEntryGameObject.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(()=>OnJoinRoomButtonClicked(room.Name));
            roomListGameObjects.Add(room.Name,roomListEntryGameObject);
        }
    }
    
    public override void OnLeftLobby()
    {
        ClearRoomListView();
        cachedRoomList.Clear();
    }
    
    #endregion

    #region Public Methods

    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName == "" ? "default" : roomName);
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName == "" ? "default" : roomName);
    }

    #endregion
    
    #region Private Methods
        
    private void OnJoinRoomButtonClicked(string roomName)
    {
        JoinRoom(roomName);
    }
    
    private void ClearRoomListView()
    {
        foreach (var roomListGameObject in roomListGameObjects.Values)
        {
            Destroy(roomListGameObject);
        }
        roomListGameObjects.Clear();
    }
    
    void ActivatePanel(string panelToBeActivated)
    {
        roomListPanel.SetActive(panelToBeActivated.Equals(roomListPanel.name));
    }
        
    #endregion
}