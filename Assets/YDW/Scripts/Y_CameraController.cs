using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Y_CameraController : MonoBehaviour
{
    //CameraFollow followCam;

    public Vector3 nearOffset;
    public Vector3 farOffset;
    public float distance = 5;
    public float rotSpeed = 300;

    Vector3 minLocalPos;
    Vector3 maxLocalPos;

    // 회전 값
    float rotY;
    float rotX;
    public bool useRotY;
    public bool useRotX;


    // 내가 다시 씀

    public Transform player;

    public float xSpeed = 120f;
    public float ySpeed = 120f;
    public float yminLimit = -20f;
    public float ymaxLimit = 80f;

    public float x = 0f;
    public float y = 0f;


    void Start()
    {
        // 메인 카메라
        //followCam = Camera.main.transform.GetComponent<CameraFollow>();
    }

    void Update()
    {
        if (player && EventSystem.current.currentSelectedGameObject == null)
        {
            Vector3 position = player.position;
            transform.position = position;
        }
    }
}
