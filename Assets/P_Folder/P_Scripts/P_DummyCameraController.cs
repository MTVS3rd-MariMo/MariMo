using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera cam;
    public Transform target;
    public float distance = 7f;
    public float delay = 10f;
    public float sitOffsetY = 1f;

    private float rotSpeed = 200f;
    private float curRotX = 0f;
    private float curRotY = 0f;
    private Vector3 dir;
    private Quaternion rotation;
    private Vector3 curPos;
    private Vector3 goPos;


    void Start()
    {
        Vector3 angles = cam.transform.eulerAngles;
        curRotX = angles.y;
        curRotY = angles.x;
        curPos = transform.position;

        // CamPos와 타겟 사이의 거리를 나타내는 백터
        dir = new Vector3(0, 0, -distance);
    }

    void Update()
    {
        CamMove();
        
    }


    void CamMove()
    {
        Vector3 lastPos;

        // CamPos 위치 설정
        transform.position = target.position + rotation * dir;
        lastPos = target.position + rotation * dir;

        goPos = lastPos;
        // 카메라의 위치 설정
        curPos = Vector3.Lerp(curPos, goPos, delay * Time.deltaTime);
        cam.transform.position = curPos;

        // 카메라의 방향 설정
        Vector3 lookAt = target.position;
        cam.transform.LookAt(lookAt);

    }

}
