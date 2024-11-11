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
                print("메인 카메라 있다");
            }
            else
            {
                print("메인 카메라 내놔");
            }

            if (tpc != null)
            {
                tpc.SetPlayer(this.transform);
            }
            else
            {
                print("버츄얼 카메라 내놔");
            }


        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 5)
        {
            UpdateCameraPosition();
            //playerMove.movable = true;
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
        int index = 0;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            GameObject playerObject = FindPlayerObjectByActorNumber(player.ActorNumber);
            if (playerObject != null)
            {
                //if(PhotonNetwork.LocalPlayer.ActorNumber > 1) playerPositions[index] = playerObject.transform.position;
                index++;
            }
        }

        if (index == 0)
        {
            Debug.LogWarning("플레이어를 찾지 못했습니다.");
            return;
        }

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
