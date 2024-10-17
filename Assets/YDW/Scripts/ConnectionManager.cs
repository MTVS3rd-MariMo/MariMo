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
        // 접속을 위한 설정
        if (LobbyUIController.lobbyUI.input_nickName.text.Length > 0)
        {
            PhotonNetwork.GameVersion = "1.0.0";
            PhotonNetwork.NickName = LobbyUIController.lobbyUI.input_nickName.text;
            PhotonNetwork.AutomaticallySyncScene = true;

            // 플레이어 캐릭터 바디 머티리얼 색상 선택

            // 접속을 서버에 요청하기
            PhotonNetwork.ConnectUsingSettings();
            LobbyUIController.lobbyUI.btn_login.interactable = false;
            int playerColor = LobbyUIController.lobbyUI.drop_playerColor.value;
            PhotonNetwork.LocalPlayer.CustomProperties.Add("CHARACTER_COLOR", playerColor);
        }
        
    }

    public override void OnConnected()
    {
        base.OnConnected();

        // 네임 서버에 접속이 완료되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        // 실패 원인을 출력한다.
        Debug.LogError("Disconnected from Server - " + cause);
        LobbyUIController.lobbyUI.btn_login.interactable = true;
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        // 마스터 서버에 접속이 완료되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");

        // 서버의 로비로 들어간다.
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        // 서버 로비에 들어갔음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        LobbyUIController.lobbyUI.ShowRoomPanel();
    }

    public void CreateRoom()
    {
        string roomName = LobbyUIController.lobbyUI.roomSetting[0].text;
        int playerCount = Convert.ToInt32(LobbyUIController.lobbyUI.roomSetting[1].text);

        if (roomName.Length > 0 && playerCount > 1)
        {
            // 나의 룸을 만든다.
            RoomOptions roomOpt = new RoomOptions();
            roomOpt.MaxPlayers = playerCount;
            roomOpt.IsOpen = true;
            roomOpt.IsVisible = true;

            // 룸의 커스텀 정보를 추가한다.
            // - 선택한 맵 번호를 룸 정보에 추가한다.
            // 키 값 등록하기
            roomOpt.CustomRoomPropertiesForLobby = new string[] { "MASTER_NAME", "PASSWORD", "SCENE_NUMBER"};
            
            // 키에 맞는 해시 테이블 추가하기
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
        // Join 관련 패널을 활성화한다.
        ChangePanel(1, 2);
    }

    /// <summary>
    /// 패널의 변경을 하기 위한 함수
    /// </summary>
    /// <param name="offIndex">꺼야될 패널 인덱스</param>
    /// <param name="onIndex">켜야될 패널 인덱스</param>
    void ChangePanel(int offIndex, int onIndex)
    {
        panelList[offIndex].SetActive(false);
        panelList[onIndex].SetActive(true);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        // 성공적으로 방이 개설되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        LobbyUIController.lobbyUI.PrintLog("방 만들어짐!");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // 성공적으로 방에 입장되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        LobbyUIController.lobbyUI.PrintLog("방에 입장 성공!");

        // 방에 입장한 친구들은 모두 1번 씬으로 이동하자!
       
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        // 룸에 입장이 실패한 이유를 출력한다.
        Debug.LogError(message);
        LobbyUIController.lobbyUI.PrintLog("입장 실패..." + message);
        
    }

    // 룸에 다른 플레이어가 입장했을 때의 콜백 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        string playerMsg = $"{newPlayer.NickName}님이 입장하셨습니다.";
        LobbyUIController.lobbyUI.PrintLog(playerMsg);
    }

    // 룸에 있던 다른 플레이어가 퇴장했을 때의 콜백 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        string playerMsg = $"{otherPlayer.NickName}님이 퇴장하셨습니다.";
        LobbyUIController.lobbyUI.PrintLog(playerMsg);
    }

    // 현재 로비에서 룸의 변경사항을 알려주는 콜백 함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        foreach(RoomInfo room in roomList)
        {
            // 만일, 갱신된 룸 정보가 제거 리스트에 있다면...
            if (room.RemovedFromList)
            {
                // cachedRoomList에서 해당 룸을 제거한다.
                cachedRoomList.Remove(room);
            }
            // 그렇지 않다면...
            else
            {
                // 만일, 이미 cachedRoomList에 있는 방이라면...
                if (cachedRoomList.Contains(room))
                {
                    // 기존 룸 정보를 제거한다.
                    cachedRoomList.Remove(room);
                }
                // 새 룸을 cachedRoomList에 추가한다.
                cachedRoomList.Add(room);
            }
        }

        // 기존의 모든 방 정보를 삭제한다.
        for(int i=0; i< scrollContent.childCount; i++)
        {
            Destroy(scrollContent.GetChild(i).gameObject);
        }

        foreach(RoomInfo room in cachedRoomList)
        {
            // cachedRoomList에 있는 모든 방을 만들어서 스크롤뷰에 추가한다.
            GameObject go = Instantiate(roomPrefab, scrollContent);
            RoomPanel roomPanel = go.GetComponent<RoomPanel>();
            roomPanel.SetRoomInfo(room);
            // 버튼에 방 입장 기능 연결하기
            roomPanel.btn_join.onClick.AddListener(() =>
            {
                PhotonNetwork.JoinRoom(room.Name);
            });
        }
    }

}
