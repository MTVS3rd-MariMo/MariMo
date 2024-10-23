using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Y_GameManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    PhotonView pv;
    Y_BookController bookUI;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        bookUI = GameObject.Find("BookCanvas").GetComponent<Y_BookController>();

        StartCoroutine(SpawnPlayer());

        // OnPhotonSerializeView 에서 데이터 전송 빈도 수 설정하기 (per seconds)
        PhotonNetwork.SerializationRate = 30;
        // 대부분의 데이터 전송 빈도 수 설정하기 (per seconds)
        PhotonNetwork.SendRate = 30;
    }

    IEnumerator SpawnPlayer()
    {
        // 룸에 입장이 완료될 때까지 기다린다.
        yield return new WaitUntil(() => { return PhotonNetwork.InRoom; });

        // 현재 플레이어의 인덱스(순서)를 가져옴
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        // 지정된 스폰 지점의 위치 가져옴
        Vector3 spawnPosition = spawnPoints[playerIndex].position;

        // 플레이어를 해당 스폰 지점에 생성
        PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);
        
        if (bookUI != null)
        {
            bookUI.currentPlayerNum = playerIndex;
            //print("!!!!!!!!!!" + playerIndex);
            //RPC_AddPlayer(PhotonNetwork.NickName);
            bookUI.RPC_AddPlayer(playerIndex, PhotonNetwork.NickName);
            Debug.Log($"Player Spawned - Index: {playerIndex}, Name: {PhotonNetwork.NickName}");
        }
    }

    //public void RPC_AddPlayer(string playerName)
    //{
    //    pv.RPC("AddPlayer", RpcTarget.All, playerName);
    //}

    //[PunRPC]
    //void AddPlayer(string playerName)
    //{
    //    bookUI.playerNames.Add(playerName);
    //}


}
