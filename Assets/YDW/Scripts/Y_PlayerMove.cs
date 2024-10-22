using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Utilities;

public class Y_PlayerMove : MonoBehaviour, IPunObservable
{
    public float speed;
    CharacterController conn;

    public NavMeshAgent agent;
    int layerMaskGround;

    public GameObject test;

    PhotonView pv;
    Vector3 myPos;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        speed = 20;
        conn = GetComponent<CharacterController>();
        layerMaskGround = LayerMask.GetMask("Ground");
        agent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        // 만일 내가 소유권을 가진 캐릭터라면 
        if(pv.IsMine)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if ((touch.phase == TouchPhase.Began) || (touch.phase == TouchPhase.Moved))
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 9999f, layerMaskGround))
                    {
                        agent.SetDestination(hit.point);
                    }
                }
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, myPos, Time.deltaTime);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 만일 데이터를 서버에 전송(PhotonView.IsMine == true)하는 상태라면
        if(stream.IsWriting)
        {
            // iterable 데이터를 보낸다 
            stream.SendNext(transform.position);
        }

        // 그렇지 않고 만일 데이터를 서버로부터 읽어오는 상태라면
        else if (stream.IsReading)
        {
            myPos = (Vector3)stream.ReceiveNext();
        }
    }
}
