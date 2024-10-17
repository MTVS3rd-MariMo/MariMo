using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Reflection;
using System;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class ConnectionManager : MonoBehaviourPunCallbacks
{
    public GameObject roomPrefab;
    public Transform scrollContent;
    public GameObject[] panelList;

    List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    void Start()
    {
        Screen.SetResolution(640, 480, FullScreenMode.Windowed);
    }

    void Update()
    {
        
    }

    public void StartLogin()
    {
        // ������ ���� ����
        if (LobbyUIController.lobbyUI.input_nickName.text.Length > 0)
        {
            PhotonNetwork.GameVersion = "1.0.0";
            PhotonNetwork.NickName = LobbyUIController.lobbyUI.input_nickName.text;
            PhotonNetwork.AutomaticallySyncScene = true;

            // �÷��̾� ĳ���� �ٵ� ��Ƽ���� ���� ����

            // ������ ������ ��û�ϱ�
            PhotonNetwork.ConnectUsingSettings();
            LobbyUIController.lobbyUI.btn_login.interactable = false;
            int playerColor = LobbyUIController.lobbyUI.drop_playerColor.value;
            PhotonNetwork.LocalPlayer.CustomProperties.Add("CHARACTER_COLOR", playerColor);
        }
        
    }

    public override void OnConnected()
    {
        base.OnConnected();

        // ���� ������ ������ �Ϸ�Ǿ����� �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        // ���� ������ ����Ѵ�.
        Debug.LogError("Disconnected from Server - " + cause);
        LobbyUIController.lobbyUI.btn_login.interactable = true;
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        // ������ ������ ������ �Ϸ�Ǿ����� �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");

        // ������ �κ�� ����.
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        // ���� �κ� ������ �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        LobbyUIController.lobbyUI.ShowRoomPanel();
    }

    public void CreateRoom()
    {
        string roomName = LobbyUIController.lobbyUI.roomSetting[0].text;
        int playerCount = Convert.ToInt32(LobbyUIController.lobbyUI.roomSetting[1].text);

        if (roomName.Length > 0 && playerCount > 1)
        {
            // ���� ���� �����.
            RoomOptions roomOpt = new RoomOptions();
            roomOpt.MaxPlayers = playerCount;
            roomOpt.IsOpen = true;
            roomOpt.IsVisible = true;

            // ���� Ŀ���� ������ �߰��Ѵ�.
            // - ������ �� ��ȣ�� �� ������ �߰��Ѵ�.
            // Ű �� ����ϱ�
            roomOpt.CustomRoomPropertiesForLobby = new string[] { "MASTER_NAME", "PASSWORD", "SCENE_NUMBER"};
            
            // Ű�� �´� �ؽ� ���̺� �߰��ϱ�
            Hashtable roomTable = new Hashtable();
            roomTable.Add("MASTER_NAME", PhotonNetwork.NickName);
            roomTable.Add("PASSWORD", 1234);
            roomTable.Add("SCENE_NUMBER", LobbyUIController.lobbyUI.drop_mapSelection.value + 2);
            roomOpt.CustomRoomProperties = roomTable;

            PhotonNetwork.CreateRoom(roomName, roomOpt, TypedLobby.Default);
            
        }
    }

    public void JoinRoom()
    {
        // Join ���� �г��� Ȱ��ȭ�Ѵ�.
        ChangePanel(1, 2);
    }

    /// <summary>
    /// �г��� ������ �ϱ� ���� �Լ�
    /// </summary>
    /// <param name="offIndex">���ߵ� �г� �ε���</param>
    /// <param name="onIndex">�Ѿߵ� �г� �ε���</param>
    void ChangePanel(int offIndex, int onIndex)
    {
        panelList[offIndex].SetActive(false);
        panelList[onIndex].SetActive(true);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        // ���������� ���� �����Ǿ����� �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        LobbyUIController.lobbyUI.PrintLog("�� �������!");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // ���������� �濡 ����Ǿ����� �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        LobbyUIController.lobbyUI.PrintLog("�濡 ���� ����!");

        // �濡 ������ ģ������ ��� 1�� ������ �̵�����!
       
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        // �뿡 ������ ������ ������ ����Ѵ�.
        Debug.LogError(message);
        LobbyUIController.lobbyUI.PrintLog("���� ����..." + message);
        
    }

    // �뿡 �ٸ� �÷��̾ �������� ���� �ݹ� �Լ�
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        string playerMsg = $"{newPlayer.NickName}���� �����ϼ̽��ϴ�.";
        LobbyUIController.lobbyUI.PrintLog(playerMsg);
    }

    // �뿡 �ִ� �ٸ� �÷��̾ �������� ���� �ݹ� �Լ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        string playerMsg = $"{otherPlayer.NickName}���� �����ϼ̽��ϴ�.";
        LobbyUIController.lobbyUI.PrintLog(playerMsg);
    }

    // ���� �κ񿡼� ���� ��������� �˷��ִ� �ݹ� �Լ�
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        foreach(RoomInfo room in roomList)
        {
            // ����, ���ŵ� �� ������ ���� ����Ʈ�� �ִٸ�...
            if (room.RemovedFromList)
            {
                // cachedRoomList���� �ش� ���� �����Ѵ�.
                cachedRoomList.Remove(room);
            }
            // �׷��� �ʴٸ�...
            else
            {
                // ����, �̹� cachedRoomList�� �ִ� ���̶��...
                if (cachedRoomList.Contains(room))
                {
                    // ���� �� ������ �����Ѵ�.
                    cachedRoomList.Remove(room);
                }
                // �� ���� cachedRoomList�� �߰��Ѵ�.
                cachedRoomList.Add(room);
            }
        }

        // ������ ��� �� ������ �����Ѵ�.
        for(int i=0; i< scrollContent.childCount; i++)
        {
            Destroy(scrollContent.GetChild(i).gameObject);
        }

        foreach(RoomInfo room in cachedRoomList)
        {
            // cachedRoomList�� �ִ� ��� ���� ���� ��ũ�Ѻ信 �߰��Ѵ�.
            GameObject go = Instantiate(roomPrefab, scrollContent);
            RoomPanel roomPanel = go.GetComponent<RoomPanel>();
            roomPanel.SetRoomInfo(room);
            // ��ư�� �� ���� ��� �����ϱ�
            roomPanel.btn_join.onClick.AddListener(() =>
            {
                PhotonNetwork.JoinRoom(room.Name);
            });
        }
    }

}
