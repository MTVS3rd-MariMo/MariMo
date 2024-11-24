using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Video;

public class Y_GameManager : MonoBehaviourPun
{
    public static Y_GameManager instance;

    public Transform[] spawnPoints;
    Y_BookController bookUI;

    public string[] urls = new string[4];

    public GameObject barrier;
    private bool isBarrierOpened = false;
    private Animation anim;
    public GameObject Fence;
    public ParticleSystem particle_Destroy;
    public GameObject VC_Fence;

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

        anim = GetComponent<Animation>();
        anim.enabled = false;
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
            GameObject player = PhotonNetwork.Instantiate("Teacher", spawnPoints[4].position, Quaternion.identity);
            //yield return new WaitUntil(() => player != null);

            //player.GetComponent<MeshRenderer>().enabled = false;
            //player.GetComponent<BoxCollider>().enabled = false;
            //player.GetComponent<NavMeshAgent>().enabled = false;
        }

        if (bookUI != null)
        {
            bookUI.currentPlayerNum = playerIndex;
            if (playerIndex >= 0)
            {
                bookUI.RPC_AddPlayerNames(playerIndex, PhotonNetwork.NickName);
            }

        }
        yield return null;
    }

    public List<GameObject> students = new List<GameObject>(5);
    PhotonView myPhotonView = null;

    public void SetPlayerObject(PhotonView go)
    {
        students[go.Owner.ActorNumber - 1] = go.gameObject;
        if(go.IsMine)
        {
            myPhotonView = go;
        }

        if(students.Count >= 5 && students.All(student => student != null))
        {
            myPhotonView.GetComponent<Y_SetCamera>().isFive = true;
            for (int i = 0; i < students.Count; i++)
            {
                students[i].GetComponent<Y_PlayerMove>().isFive = true;
            }
        }

    }

    

    public GameObject hotSeat;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hotSeat.SetActive(true);
            //photonView.RPC(nameof(AddPlayerCnt), RpcTarget.AllBuffered);
        }
    }

    public void RPC_Unlock()
    {
        photonView.RPC("Unlock", RpcTarget.All);
    }

    [PunRPC]
    public void Unlock()
    {
        StartCoroutine(UnlockBarrierAfterKeyUI());
    }


    // 왕 아이콘 1초 후 -> 투명벽 열렸다는 딜레이 함수
    public IEnumerator UnlockBarrierAfterKeyUI()
    {
        VC_Fence.SetActive(true);
        //K_KeyUiManager.instance.EndKeyUi();
        // 사운드
        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_KEY);
        yield return new WaitForSeconds(2f);

        // 애니메이션 !!!!!!!!!!!!!!!!!!!!
        anim.enabled = true;
        anim.Play("Fence_Animation");
        particle_Destroy.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        VC_Fence.SetActive(false);
        OpenBarrier();
    }

    void OpenBarrier()
    {
        print("문열림");
        Destroy(barrier);

        isBarrierOpened = true;

        // 문 열렸다는 이미지 
        K_KeyUiManager.instance.img_doorOpen.gameObject.SetActive(true);
        // 문 열렸다는 이미지 2초뒤에 꺼줘
        K_KeyUiManager.instance.StartCoroutine(K_KeyUiManager.instance.HideOpenDoor(2f));

        // 왕 아이콘을 먼저 띄워줘야함..
        // 문열렸다는 안내 이미지 함수 -> 코루틴까지 처리하고

    }
}

