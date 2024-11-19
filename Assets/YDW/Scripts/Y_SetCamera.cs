using Cinemachine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Y_SetCamera : MonoBehaviour
{
    // 카메라 위치 Transform
    public Transform cameraTransform;
    // 버츄얼 카메라
    private CinemachineVirtualCamera virtualCamera;

    PhotonView pv;
    public Transform playerAverage;
    public ThirdPersonCamera tpc;

    Y_PlayerMove playerMove;
    private void Awake()
    {
        // 플레이어간의 평균값 찾기
        playerAverage = GameObject.Find("PlayerAverage").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        playerMove = GetComponent<Y_PlayerMove>();

        // 메인 카메라 찾기
        Camera mainCamera = Camera.main;
        // 버츄얼 카메라 찾기
        tpc = FindObjectOfType<ThirdPersonCamera>();

        if (playerAverage == null)
        {
            Debug.LogError("PlayerAverage 오브젝트를 찾을 수 없습니다.");
        }

        if (pv.IsMine)
        {
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
                //print("메인 카메라 있다");
            }
            else
            {
                //print("메인 카메라 내놔");
            }

            if (tpc != null)
            {
                tpc.SetPlayer(this.transform);
            }
            else
            {
                //print("버츄얼 카메라 내놔");
            }


        }

        Y_GameManager.instance.SetPlayerObject(pv);
    }

    public bool isFive = false;
    public GameObject[] students = new GameObject[5];


    // Update is called once per frame
    void Update()
    {
        if (isFive && pv.IsMine)
        {
            UpdateCameraPosition();
        }
    }

    private void UpdateCameraPosition()
    {
        if (playerAverage == null)
        {
            Debug.LogWarning("playerAverage가 null입니다. 초기화되지 않았습니다.");
            return; // playerAverage 또는 tpc가 null일 경우, 더 이상 진행하지 않음
        }
        if(tpc == null)
        {
            Debug.LogWarning("tpc가 null입니다. 초기화되지 않았습니다.");
            return; // playerAverage 또는 tpc가 null일 경우, 더 이상 진행하지 않음
        }


        // 모든 플레이어의 위치 가져오기
        Vector3[] playerPositions = new Vector3[4];
        for(int i = 1; i <= playerPositions.Length; i++)
        {
            //Debug.LogWarning("students == Null ? " + (students[i] == null) + " 이 때 i 는 몇? : " + i);
            playerPositions[i-1] = Y_GameManager.instance.students[i].transform.position;
        }

        // 플레이어들의 평균 위치 계산
        Vector3 averagePosition = Vector3.zero;
        foreach (var pos in playerPositions)
        {
            averagePosition += pos;
        }
        averagePosition /= (float)playerPositions.Length;

        // playerAverage의 위치 업데이트
        playerAverage.position = averagePosition;

        // ThirdPersonCamera에 새로운 위치 설정
        tpc.SetPlayer(playerAverage);
    }

    public PhotonView FindPlayerObjectByActorNumber(int actorNumber)
    {
        PhotonView pview = null;
        Y_PlayerMove[] playerMoves = FindObjectsOfType<Y_PlayerMove>();
        
        for (int i = 0; i < playerMoves.Length; i++)
        {
            Debug.LogError(playerMoves.Length + " i : " + i);
            Debug.LogError("view.pv.Owner 는? : " + (playerMoves[i].GetComponent<PhotonView>().Owner != null) 
                + " view.pv.Owner.ActorNumber 는? : " + (playerMoves[i].GetComponent<PhotonView>().Owner.ActorNumber == actorNumber) 
                + " 이 때 ActorNum 은? : " + (playerMoves[i].pv.Owner.ActorNumber));
            if (playerMoves[i].GetComponent<PhotonView>().Owner != null && (playerMoves[i].GetComponent<PhotonView>().Owner.ActorNumber == actorNumber))
            {
                Debug.LogError("반환했다~!");
                pview = playerMoves[i].GetComponent<PhotonView>();
            }
        }
        return pview;
    }
}
