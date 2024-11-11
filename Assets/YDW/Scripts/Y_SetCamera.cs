using Cinemachine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        playerMove = GetComponent<Y_PlayerMove>();

        // 메인 카메라 찾기
        Camera mainCamera = Camera.main;
        // 버츄얼 카메라 찾기
        tpc = FindObjectOfType<ThirdPersonCamera>();

        // 플레이어간의 평균값 찾기
        playerAverage = GameObject.Find("PlayerAverage").transform;

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
    }

    bool isFive = false;
    public GameObject[] students = new GameObject[4];

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 5 && !isFive)
        {
            Debug.LogWarning("다섯명이 다 들어왔다");
            int i = 0;
            foreach(var player in PhotonNetwork.PlayerList)
            {
                if(player.ActorNumber > 1)
                {
                    GameObject playerObject = FindPlayerObjectByActorNumber(player.ActorNumber);
                    students[i] = playerObject;
                    i++;
                }
            }

            isFive = true;
        }

        if (isFive)
        {
            for(int i = 0; i < students.Length; i++)
            {
                Debug.LogWarning("2222 students == Null ? " + (students[i] == null) + " 이 때 i 는 몇? : " + i);
            }

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
        for(int i = 0; i < playerPositions.Length; i++)
        {
            //Debug.LogWarning("students == Null ? " + (students[i] == null) + " 이 때 i 는 몇? : " + i);
            playerPositions[i] = students[i].transform.position;
        }
        //int index = 0;
        //Debug.LogWarning("루프 시작");
        //foreach (var player in PhotonNetwork.PlayerList)
        //{
        //    GameObject playerObject = FindPlayerObjectByActorNumber(player.ActorNumber);
        //    Debug.LogWarning("플레이어 찾았다");
        //    Debug.LogWarning("PlayerObject 가 널인가? : " + playerObject != null);
        //    Debug.LogWarning("ActorNum 이 1보다 큰가? : " + (PhotonNetwork.LocalPlayer.ActorNumber > 1));
        //    if (playerObject != null && PhotonNetwork.LocalPlayer.ActorNumber > 1)
        //    {
        //        Debug.LogWarning("index ? : " + index);
        //        playerPositions[index] = playerObject.transform.position;
        //        Debug.LogWarning("index 늘렸다 : " + index);
        //        index++;
        //    }
        //}
        //index = 0;

        // 플레이어들의 평균 위치 계산
        Vector3 averagePosition = Vector3.zero;
        foreach (var pos in playerPositions)
        {
            averagePosition += pos;
        }
        averagePosition /= playerPositions.Length;

        // playerAverage의 위치 업데이트
        playerAverage.position = averagePosition;

        // ThirdPersonCamera에 새로운 위치 설정
        tpc.SetPlayer(playerAverage);
    }

    private GameObject FindPlayerObjectByActorNumber(int actorNumber)
    {
        foreach (PhotonView view in FindObjectsOfType<PhotonView>())
        {
            if (view.Owner != null && view.Owner.ActorNumber == actorNumber)
            {
                return view.gameObject;
            }
        }
        return null;
    }
}
