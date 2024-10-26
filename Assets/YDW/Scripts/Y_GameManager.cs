﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Y_GameManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    PhotonView pv;
    Y_BookController bookUI;

    // 각 플레이어에 할당할 VideoRenderTexture 파일 경로 배열
    public string[] videoRendererPaths = new string[]
    {
        @"C:\Users\Admin\MariMo\Assets\YDW\VideoPlayer\ChromaKey1.renderTexture",
        @"C:\Users\Admin\MariMo\Assets\YDW\VideoPlayer\ChromaKey2.renderTexture",
        @"C:\Users\Admin\MariMo\Assets\YDW\VideoPlayer\ChromaKey3.renderTexture",
        @"C:\Users\Admin\MariMo\Assets\YDW\VideoPlayer\ChromaKey4.renderTexture"
    };

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
        GameObject player = PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);
        //VideoPlayer videoPlayer = player.GetComponentInChildren<VideoPlayer>();
        //RawImage rawImage = player.GetComponentInChildren<RawImage>();

        if (bookUI != null)
        {
            bookUI.currentPlayerNum = playerIndex;
            bookUI.RPC_AddPlayer(playerIndex, PhotonNetwork.NickName);

            // RenderTexture 로드
            //RenderTexture renderTexture = Resources.Load<RenderTexture>(videoRendererPaths[playerIndex]);
            //videoPlayer.targetTexture = renderTexture;
            //rawImage.texture = renderTexture;

        }

        //SetUpVideoRenderer(player, playerIndex);
    }

    //private void SetUpVideoRenderer(GameObject player, int index)
    //{
    //    VideoPlayer videoPlayer = player.GetComponentInChildren<VideoPlayer>();
    //    RawImage rawImage = player.GetComponentInChildren<RawImage>();

    //    if (videoPlayer != null && rawImage != null)
    //    {
    //        // 비디오 파일 경로 설정
    //        if (index < videoRendererPaths.Length)
    //        {
    //            videoPlayer.url = videoRendererPaths[index];
    //        }
    //        else
    //        {
    //            Debug.LogWarning("비디오 파일 경로가 설정되지 않았습니다.");
    //        }

    //        // Video Player의 RenderTexture를 Raw Image에 설정
    //        rawImage.texture = videoPlayer.targetTexture;
    //    }
    //}
}
