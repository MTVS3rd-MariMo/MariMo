using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Reflection;
using System;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using Org.BouncyCastle.Bcpg;


public class ConnectionManager : MonoBehaviourPunCallbacks
{
    public GameObject roomPrefab;
    public Transform scrollContent;
    int lessonMaterialId = 0;

    List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    void Start()
    {
        Screen.SetResolution(640, 480, FullScreenMode.Windowed);
        Y_SoundManager.instance.PlayBgmSound(Y_SoundManager.EBgmType.BGM_LOGIN);
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha6)) CreateRoom("테스트", Y_HttpRoomSetUp.GetInstance().userlessonId, 1);
    }

    public void StartLogin()
    {
        
        if (LobbyController.lobbyUI.input_nickName.text.Length > 0)
        {
            // 접속을 위한 설정
            PhotonNetwork.GameVersion = "1.0.0";
            PhotonNetwork.NickName = LobbyController.lobbyUI.input_nickName.text;
            PhotonNetwork.AutomaticallySyncScene = false;
            print("접속 설정 완료");

            // 접속을 서버에 요청하기
            PhotonNetwork.ConnectUsingSettings(); // 위 정보를 바탕으로 네임 서버에서 마스터 서버로 연결하는 함수.
                                                  // 네임 서버: 커넥션 관리, NetID 구분하여 클라이언트 구분.
                                                  // 마스터 서버: 유저들 간의 Match making 을 해주는 공간. 룸을 만들고 룸에 조인을 하고 룸의 플레이어끼리 플레이를 하는 식. 방장의 씬을 기준으로 설정하고 나의 씬에 동기화
            print("서버에 요청 중....");

            //LobbyController.lobbyUI.btn_login.interactable = false; // 도원 알파 시연용 수정
            //int playerColor = LobbyUIController.lobbyUI.drop_playerColor.value;
            //PhotonNetwork.LocalPlayer.CustomProperties.Add("CHARACTER_COLOR", playerColor);
        }
        
    }

    public override void OnConnected()
    {
        base.OnConnected();

        // 네임 서버에 접속이 완료되었음을 알려준다.
        //print(MethodInfo.GetCurrentMethod().Name + " is Call!");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        // 실패 원인을 출력한다.
        Debug.LogError("Disconnected from Server - " + cause);
        LobbyController.lobbyUI.btn_login.interactable = true;
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        // 마스터 서버에 접속이 완료되었음을 알려준다.
        //print(MethodInfo.GetCurrentMethod().Name + " is Call!");

        // 서버의 로비로 들어간다.
        PhotonNetwork.JoinLobby();

        //// 연결 준비 상태 확인 후 방 생성
        //if (PhotonNetwork.IsConnectedAndReady)
        //{
        //    CreateRoom();
        //}
        //else
        //{
        //    StartCoroutine(WaitAndCreateRoom());
        //}

    }

    //IEnumerator WaitAndCreateRoom()
    //{
    //    // 연결 상태가 준비될 때까지 대기
    //    while (!PhotonNetwork.IsConnectedAndReady)
    //    {
    //        yield return null;
    //    }
    //    CreateRoom();
    //}

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        // 서버 로비에 들어갔음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        //LobbyController.lobbyUI.ShowRoomPanel();

        PhotonNetwork.LoadLevel(1);
    }

    //public void CreateRoom(string roomname)
    //{
    //    //string roomName = LobbyUIController.lobbyUI.roomSetting[0].text;
    //    //int playerCount = Convert.ToInt32(LobbyUIController.lobbyUI.roomSetting[1].text);
    //    string roomName = roomname; /////////////////////////////////// To. 효근 : 나중에 선생님이 입력한 Inputbox.text 값으로 바꿔놓으면 됨!
    //    int playerCount = 5;

    //    if (roomName.Length > 0 && playerCount > 1)
    //    {
    //        // 나의 룸을 만든다.
    //        RoomOptions roomOpt = new RoomOptions();
    //        roomOpt.MaxPlayers = playerCount;
    //        roomOpt.IsOpen = true;
    //        roomOpt.IsVisible = true;

    //        // 룸의 커스텀 정보를 추가한다.
    //        // - 선택한 맵 번호를 룸 정보에 추가한다.
    //        // 키 값 등록하기
    //        roomOpt.CustomRoomPropertiesForLobby = new string[] { "MASTER_NAME", "ROOM_ID" }; // , "PASSWORD", "SCENE_NUMBER"

    //        // 키에 맞는 해시 테이블 추가하기
    //        Hashtable roomTable = new Hashtable();
    //        roomTable.Add("MASTER_NAME", PhotonNetwork.NickName);

    //        // 서버에서 Room_ID 받아오고 -> 받아올 때까지 기다려
    //        // 해시테이블에 추가

    //        //roomTable.Add("PASSWORD", 1234);
    //        //roomTable.Add("SCENE_NUMBER", LobbyUIController.lobbyUI.drop_mapSelection.value + 2);
    //        roomOpt.CustomRoomProperties = roomTable;

    //        PhotonNetwork.CreateRoom(roomName, roomOpt, TypedLobby.Default);

    //    }
    //}

    //public void CreateRoom(string roomname, int lessonId, int lessonMaterialNum)
    //{
    //    string roomName = roomname;
    //    int playerCount = 5;

    //    if (roomName.Length > 0 && playerCount > 1)
    //    {
    //        // 나의 룸을 만든다.
    //        RoomOptions roomOpt = new RoomOptions();
    //        roomOpt.MaxPlayers = playerCount;
    //        roomOpt.IsOpen = true;
    //        roomOpt.IsVisible = true;

    //        // 룸의 커스텀 정보를 추가한다.
    //        // - 선택한 맵 번호를 룸 정보에 추가한다.
    //        // 키 값 등록하기
    //        // 키에 맞는 해시 테이블 추가하기
    //        Hashtable roomTable = new Hashtable();
    //        roomTable.Add("MASTER_NAME", PhotonNetwork.NickName);
    //        roomTable.Add("lessonId", lessonId.ToString());
    //        roomTable.Add("lessonMaterialId", lessonMaterialNum.ToString());
    //        roomOpt.CustomRoomPropertiesForLobby = new string[] { "MASTER_NAME", "lessonId", "lessonMaterialId" };
    //        roomOpt.CustomRoomProperties = roomTable;

    //        PhotonNetwork.CreateRoom(roomName, roomOpt, TypedLobby.Default);
    //    }
    //}

    public void JoinRoom()
    {
        // Join 관련 패널을 활성화한다.
        //ChangePanel(1, 2);

        //PhotonNetwork.LoadLevel(1);

        // 수업에 들어온 유저가 서버에 본인의 userId를 보낸다.


        // 5명이 다 들어오면 (일단 테스트 땐 4명)
        // 참가자들dl lessonId로 참가자들의 userId리스트를 받는다.

        // 수업에서 사용할 수업자료 호출 (퀴즈 같은 거)


    }


    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        // 성공적으로 방이 개설되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        LobbyController.lobbyUI.PrintLog("방 만들어짐!");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // 성공적으로 방에 입장되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        //LobbyController.lobbyUI.PrintLog("방에 입장 성공!");


        // 잠만지워
        //PhotonNetwork.LoadLevel(1);
        PhotonNetwork.LoadLevel(2);

        //StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        yield return new WaitForSeconds(5f);

        
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        // 룸에 입장이 실패한 이유를 출력한다.
        Debug.LogError(message);
        LobbyController.lobbyUI.PrintLog("입장 실패..." + message);

    }

    // 룸에 다른 플레이어가 입장했을 때의 콜백 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        string playerMsg = $"{newPlayer.NickName}님이 입장하셨습니다.";
        LobbyController.lobbyUI.PrintLog(playerMsg);

        if(PhotonNetwork.CurrentRoom.PlayerCount == 5)
        {
            // 포톤 RPC로 전체 호출
            StartCoroutine(Y_HttpRoomSetUp.GetInstance().GetUserIdList());
        }
    }

    // 룸에 있던 다른 플레이어가 퇴장했을 때의 콜백 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        string playerMsg = $"{otherPlayer.NickName}님이 퇴장하셨습니다.";
        LobbyController.lobbyUI.PrintLog(playerMsg);
    }


    public GameObject[] buttons;

    // 현재 로비에서 룸의 변경사항을 알려주는 콜백 함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (Y_HttpLogIn.GetInstance().isTeacher) return;

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

        // 버튼 UI 업데이트
        for (int i = 0; i < buttons.Length; i++)
        {
            // 버튼 클릭 리스너 초기화
            buttons[i].GetComponent<Button>().onClick.RemoveAllListeners();

            // 룸이 있는 경우 버튼 설정
            if (i < cachedRoomList.Count)
            {
                RoomInfo room = cachedRoomList[i];
                buttons[i].SetActive(true);
                buttons[i].GetComponentInChildren<TMP_Text>().text = room.Name;

                // 클릭 리스너에 방 입장 기능 추가
                buttons[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    PhotonNetwork.JoinRoom(room.Name);
                });
            }
            else
            {
                // 룸이 없으면 버튼 비활성화
                buttons[i].SetActive(false);
            }
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        // 잠만
        //SceneManager.LoadScene(0);
        SceneManager.LoadScene(1);

        Y_HttpLogIn.GetInstance().ReturnLobby();
    }

}
