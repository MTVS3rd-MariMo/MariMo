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

public class P_RoomMgr : MonoBehaviourPunCallbacks
{
    public P_RoomCreate p_creat;

    public void CreateRoom(string roomname, int lessonId, int lessonMaterialNum)
    {
        string roomName = roomname; 
        int playerCount = p_creat.players;

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
            // 키에 맞는 해시 테이블 추가하기
            Hashtable roomTable = new Hashtable();
            roomTable.Add("MASTER_NAME", PhotonNetwork.NickName);
            roomTable.Add("lessonId", lessonId.ToString());
            roomTable.Add("lessonMaterialId", lessonMaterialNum.ToString());
            roomOpt.CustomRoomPropertiesForLobby = new string[] { "MASTER_NAME", "lessonId", "lessonMaterialId" };
            roomOpt.CustomRoomProperties = roomTable;

            bool roomCreated = PhotonNetwork.CreateRoom(roomName, roomOpt, TypedLobby.Default);

            Debug.Log($"PhotonNetwork.CreateRoom called. Success? {roomCreated}");
        }
    }
}
