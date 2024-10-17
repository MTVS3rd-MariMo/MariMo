using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera cam;
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

        if (Input.GetMouseButton(1))
            CamRot();
    }


    void CamRot()
    {
        float mx = Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;
        float my = Input.GetAxis("Mouse Y") * rotSpeed * Time.deltaTime;

        curRotX += mx;
        curRotY -= my;
        curRotY = Mathf.Clamp(curRotY, -30, 60);

        // CamPos의 궤도 회전
        rotation = Quaternion.Euler(curRotY, curRotX, 0);

        Vector3 lookAt = transform.position;
        transform.LookAt(lookAt);

    }

    void CamMove()
    {
        Vector3 lastPos;

        // CamPos 위치 설정
        transform.position = target.position + rotation * dir;
        lastPos = target.position + rotation * dir;
        lastPos = WallCheck(transform.position);

        goPos = lastPos;
        // 카메라의 위치 설정
        curPos = Vector3.Lerp(curPos, goPos, delay * Time.deltaTime);
        cam.transform.position = curPos;

        // 카메라의 방향 설정
        Vector3 lookAt = target.position;
        cam.transform.LookAt(lookAt);

    }

    Vector3 WallCheck(Vector3 pos)
    {
        RaycastHit hit;
        Vector3 dir = pos - target.position;
        float distance = Vector3.Distance(target.position, pos);

        if (Physics.Raycast(target.position, dir, out hit, distance, 1 << LayerMask.NameToLayer("Wall")))
        {
            return hit.point + (target.position - hit.point).normalized * 0.7f;
        }

        return pos;
    }
}
