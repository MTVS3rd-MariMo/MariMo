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
    public Animator anim;
    //public GameObject Fence;
    public GameObject coli_Fence;
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

        GameObject fenceObj = GameObject.Find("Fence");
        anim = fenceObj.GetComponent<Animator>();
        anim.enabled = false;
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


        //particle_Destroy = GetComponent<ParticleSystem>();
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
        if (playerIndex >= 0)
        {
            Vector3 spawnPosition = spawnPoints[playerIndex].position;
            // 플레이어를 해당 스폰 지점에 생성
            GameObject player = PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);
        }
        else if (PhotonNetwork.IsMasterClient)
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

    public List<GameObject> players = new List<GameObject>(5);
    PhotonView myPhotonView = null;

    public void SetPlayerObject(PhotonView go)
    {
        players[go.Owner.ActorNumber - 1] = go.gameObject;
        if (go.IsMine)
        {
            myPhotonView = go;
        }

        if (players.Count >= 5 && players.All(player => player != null))
        {
            myPhotonView.GetComponent<Y_SetCamera>().isFive = true;
            for (int i = 0; i < players.Count; i++)
            {
                players[i].GetComponent<Y_PlayerMove>().isFive = true;
                if (i > 0) students[i - 1] = players[i];
            }
        }

    }

    public List<GameObject> students = new List<GameObject>(4);
    public Transform[] startPos = new Transform[4];

    // 오브젝트 애니메이션
    public GameObject Ani_Object;
    public GameObject Drawing_School;

    public IEnumerator SetStartPos()
    {
        bool[] playersInPosition = new bool[students.Count]; // 각 플레이어의 도달 상태를 추적
        int i = 0;

        while (true)
        {
            bool allPlayersInPosition = true;

            for (i = 0; i < students.Count; i++)
            {
                if (playersInPosition[i]) continue; // 이미 도달한 플레이어는 무시

                GameObject student = students[i];
                Vector3 playerStartPos = startPos[i].position;
                playerStartPos.y = student.transform.position.y;

                //float distanceSqr = (student.transform.position - playerStartPos).sqrMagnitude; // 제곱 거리
                //if (distanceSqr < 1f) // 0.1f^2 = 0.01
                //{
                //    student.transform.position = playerStartPos; // 정확히 위치 고정
                //    playersInPosition[i] = true; // 도달 상태 업데이트
                //}
                if (Vector3.Distance(student.transform.position, playerStartPos) < 0.5f) // 도달 여부 확인
                {
                    student.transform.position = playerStartPos; // 정확히 위치 고정
                    playersInPosition[i] = true; // 도달 상태 업데이트
                }
                else
                {
                    allPlayersInPosition = false;
                    student.transform.position = Vector3.Lerp(student.transform.position, playerStartPos, 0.1f); // 부드럽게 이동
                }
            }

            if (allPlayersInPosition)
            {
                K_LobbyUiManager.instance.isAllArrived = true;
                Ani_Object.SetActive(true);
                Drawing_School.SetActive(false);
                Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_3D_OBJECT_01);
                //myPhotonView.GetComponent<Y_SetCamera>().isFive = true;
                break; // 모든 플레이어가 위치에 도달하면 코루틴 종료
            }

            yield return null;
        }
    }

    public GameObject hotSeat;
    public bool afterbook = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hotSeat.SetActive(true);
            //photonView.RPC(nameof(AddPlayerCnt), RpcTarget.AllBuffered);
        }

        if (afterbook)
        {
            StartCoroutine(SetStartPos());
            afterbook = false;
        }
    }

    public void RPC_Unlock()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("Unlock", RpcTarget.All);

        }
    }

    [PunRPC]
    public void Unlock()
    {
        StartCoroutine(UnlockBarrierAfterKeyUI());
    }


    // 왕 아이콘 1초 후 -> 투명벽 열렸다는 딜레이 함수
    public IEnumerator UnlockBarrierAfterKeyUI()
    {
        // 딜레이줘야함 (마지막 활동 끝나고) HZ 원래 2초였는데 베타 시연용 4초로 변경
        yield return new WaitForSeconds(4f);

        //VC_Fence.SetActive(true); HZ
        // 마지막 키 활성화
        K_KeyUiManager.instance.EndKeyUi();
        // HZ 키 애니메이션 다 보여주고 cameraON
        yield return new WaitForSeconds(4f);
        VC_Fence.SetActive(true);
        // 사운드
        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_KEY);
        yield return new WaitForSeconds(2f);

        // 애니메이션 !!!!!!!!!!!!!!!!!!!!
        anim.enabled = true;
        anim.SetTrigger("Unlock");
        //anim.Play("Fence_Animation");
        particle_Destroy.Play();


        // 파티클 시간만큼 3초로 딜레이
        //yield return new WaitForSeconds(3f);

        // 먼지 파티클 끝난 후 sparks, glow 재생
        
        // 파티클 시간만큼 딜레이
        yield return new WaitForSeconds(5f);

        VC_Fence.SetActive(false);
        // Fence 사운드
        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_FENCE_ON);
        OpenBarrier();
    }

    void OpenBarrier()
    {
        print("문열림");
        Destroy(barrier);

        coli_Fence.GetComponent<BoxCollider>().enabled = false;
        isBarrierOpened = true;

        // 문 열렸다는 이미지 
        K_KeyUiManager.instance.img_doorOpen.gameObject.SetActive(true);
        // 문 열렸다는 이미지 2초뒤에 꺼줘
        K_KeyUiManager.instance.StartCoroutine(K_KeyUiManager.instance.HideOpenDoor(2f));

        // 왕 아이콘을 먼저 띄워줘야함..
        // 문열렸다는 안내 이미지 함수 -> 코루틴까지 처리하고

    }
}

