using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Y_GameManager : MonoBehaviourPun
{
    public static Y_GameManager instance;

    public Transform[] spawnPoints;
    Y_BookController bookUI;

    // 각 플레이어에 할당할 VideoRenderTexture 파일 경로 배열
    public string[] videoRendererPaths = new string[]
    {
        @"C:\Users\Admin\MariMo\Assets\YDW\VideoPlayer\ChromaKey1.renderTexture",
        @"C:\Users\Admin\MariMo\Assets\YDW\VideoPlayer\ChromaKey2.renderTexture",
        @"C:\Users\Admin\MariMo\Assets\YDW\VideoPlayer\ChromaKey3.renderTexture",
        @"C:\Users\Admin\MariMo\Assets\YDW\VideoPlayer\ChromaKey4.renderTexture"
    };

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        bookUI = GameObject.Find("BookCanvas").GetComponent<Y_BookController>();
        StartHttp();

        // OnPhotonSerializeView 에서 데이터 전송 빈도 수 설정하기 (per seconds)
        PhotonNetwork.SerializationRate = 30;
        // 대부분의 데이터 전송 빈도 수 설정하기 (per seconds)
        PhotonNetwork.SendRate = 30;

        StartCoroutine(SpawnPlayer());
        //
        //StartCoroutine(AA());
    }

    void StartHttp()
    {
        int lessonMaterialId = 0;

        // 수업자료 id 받기
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("lessonMaterialId", out object lessonMaterialNum))
        {
            //print("여기 들어왔다");
            lessonMaterialId = Convert.ToInt32(lessonMaterialNum);
            //print("lessonMaterialId : " + lessonMaterialId);
            Debug.Log("Joined Room with lessonMaterial ID: " + lessonMaterialId);


        }
        else
        {
            Debug.LogError("lessonMaterial ID not found in the room properties.");
        }

        // 방 id 받기
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("lessonId", out object lesson))
        {
            Y_HttpRoomSetUp.GetInstance().userlessonId = Convert.ToInt32(lesson);
            Debug.Log("Joined Room with room ID: " + Y_HttpRoomSetUp.GetInstance().userlessonId);

            // 수업에 유저 등록
            StartCoroutine(Y_HttpRoomSetUp.GetInstance().SendLessonId(lessonMaterialId));
        }
        else
        {
            Debug.LogError("room ID not found in the room properties.");
        }


        if (PhotonNetwork.CurrentRoom.PlayerCount == 5)
        {
            // 포톤 RPC로 전체 호출
            StartCoroutine(Y_HttpRoomSetUp.GetInstance().GetUserIdList());
        }
    }

    public int playerCount = 0;

    IEnumerator AA()
    {
        yield return new WaitUntil(() => { return PhotonNetwork.InRoom; });
        Debug.LogWarning("RPC  시작!!");

        photonView.RPC(nameof(AddPlayerCnt), RpcTarget.AllBuffered);
        Debug.LogWarning("RPC  끝!!");
    }


    [PunRPC]
    void AddPlayerCnt()
    {
        playerCount++;
        if(playerCount == 5)
        {
            Debug.LogWarning("스폰 시작!!");

            StartCoroutine(SpawnPlayer());
        }
    }

    IEnumerator SpawnPlayer()
    {
        // 룸에 입장이 완료될 때까지 기다린다.
        //yield return new WaitUntil(() => { return playerCount == 5; });
        
        //yield return new WaitForSeconds(5);
        Debug.LogWarning("스폰됐다!!");
        // 현재 플레이어의 인덱스(순서)를 가져옴 (원래 네명일 땐 - 1 였는데 이제 5명이라 -2 (선생님이 언제나 Master, ActorNum = 1))
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 2;

        // 지정된 스폰 지점의 위치 가져옴 
        if(playerIndex >= 0)
        {
            Vector3 spawnPosition = spawnPoints[playerIndex].position;
            // 플레이어를 해당 스폰 지점에 생성
            GameObject player = PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);
        }
        else if(PhotonNetwork.IsMasterClient)
        {
            GameObject player = PhotonNetwork.Instantiate("Player", spawnPoints[4].position, Quaternion.identity);
            player.GetComponent<MeshRenderer>().enabled = false;
            player.GetComponent<BoxCollider>().enabled = false;
        }
        
        //VideoPlayer videoPlayer = player.GetComponentInChildren<VideoPlayer>();
        //RawImage rawImage = player.GetComponentInChildren<RawImage>();

        if (bookUI != null)
        {
            bookUI.currentPlayerNum = playerIndex;
            if (playerIndex >= 0)
            {
                bookUI.RPC_AddPlayerNames(playerIndex, PhotonNetwork.NickName);
            }

            // RenderTexture 로드
            //RenderTexture renderTexture = Resources.Load<RenderTexture>(videoRendererPaths[playerIndex]);
            //videoPlayer.targetTexture = renderTexture;
            //rawImage.texture = renderTexture;

        }
        yield return null;
        //SetUpVideoRenderer(player, playerIndex);




        if (PhotonNetwork.CurrentRoom.PlayerCount == 5)
        {
            photonView.RPC("GetPlayerObjects", RpcTarget.All);
        }
    }

    [PunRPC]
    public void GetPlayerObjects()
    {
        Y_SetCamera y_SetCamera = FindObjectOfType<Y_SetCamera>();

        Debug.LogWarning("다섯명이 다 들어왔다");
        int i = 0;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            GameObject playerObject = y_SetCamera.FindPlayerObjectByActorNumber(player.ActorNumber).gameObject;
            y_SetCamera.students[i] = playerObject;
            i++;

        }

        y_SetCamera.isFive = true;
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

    public GameObject hotSeat;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hotSeat.SetActive(true);
            //photonView.RPC(nameof(AddPlayerCnt), RpcTarget.AllBuffered);
        }
    }
}

