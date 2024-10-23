using Cinemachine;
using Photon.Pun;
using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Utilities;

public class Y_PlayerMove : MonoBehaviour, IPunObservable
{
    public float trackingSpeed = 3;
    public float speed;
    //CharacterController conn;

    public NavMeshAgent agent;
    int layerMaskGround;

    public GameObject test;

    PhotonView pv;
    Vector3 myPos;

    public bool movable = false;

    PhotonVoiceView voiceView;
    Y_PlayerVoice playerVoice;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        voiceView = GetComponent<PhotonVoiceView>();
        playerVoice = GetComponent<Y_PlayerVoice>();
    }

    // Start is called before the first frame update
    void Start()
    {
        speed = 20;
        //conn = GetComponent<CharacterController>();
        layerMaskGround = LayerMask.GetMask("Ground");
        agent.updateRotation = false;
    }

    void Update()
    {
        movable = true; //////////////
        if (movable)
        {
            Move();
        }
        else
        {
            // 친구들을 기다리고 있어요! UI
        }
    }

    // 당겨지는 강도 
    float pulling = 100f;

    void Move()
    {
        movable = true; ////////////////
        // 만일 내가 소유권을 가진 캐릭터라면 
        if(pv.IsMine)
        {
            if(Application.platform == RuntimePlatform.Android)
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
                // 사용자의 입력을 받아서
                // A : -1, D : 1, 누르지 않으면 0
                float h = Input.GetAxis("Horizontal");
                // S : -1, W : 1, 누르지 않으면 0
                float v = Input.GetAxis("Vertical");

                // 방향을 구하자.
                // 수평 방향
                Vector3 dirH = transform.right * h;
                // 수직 방향
                Vector3 dirV = transform.forward * v;
                // 최종적으로 이동해야 하는 방향
                Vector3 finalDir = dirH + dirV;
                // finalDir 을 정규화 하자 (벡터의 크기를 1로 만든다)
                finalDir.Normalize();

                // 구해진 방향으로 계속 이동하고 싶다.
                // 오른쪽으로 이동하고 싶다.
                //transform.Translate(finalDir * speed * Time.deltaTime);
                // P = P0 + vt (이동공식)
                transform.position += finalDir * speed * Time.deltaTime;

                // 화면 밖으로 나가지 못하게 고정
                Vector3 viewPortPoint = Camera.main.WorldToViewportPoint(transform.position);

                if (viewPortPoint.x < 0.1f)
                {
                    transform.position += Vector3.right * (0.1f - viewPortPoint.x) * pulling * Time.deltaTime;
                }

                if (viewPortPoint.x > 0.9f)
                {
                    transform.position += Vector3.right * (0.9f - viewPortPoint.x) * pulling * Time.deltaTime;
                }

                if (viewPortPoint.y < 0.1f)
                {
                    pulling = 50;
                    transform.position += Vector3.forward * (0.1f - viewPortPoint.y) * pulling * Time.deltaTime;
                }

                if (viewPortPoint.y > 0.7f)
                {
                    pulling = 200;
                    transform.position += Vector3.forward * (0.7f - viewPortPoint.y) * pulling * Time.deltaTime;
                }
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, myPos, Time.deltaTime * trackingSpeed);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 만일 데이터를 서버에 전송(PhotonView.IsMine == true)하는 상태라면
        if(stream.IsWriting)
        {
            // iterable 데이터를 보낸다 
            stream.SendNext(transform.position);
            stream.SendNext(voiceView.IsRecording);
        }

        // 그렇지 않고 만일 데이터를 서버로부터 읽어오는 상태라면
        else if (stream.IsReading)
        {
            myPos = (Vector3)stream.ReceiveNext();
            playerVoice.isTalking = (bool)stream.ReceiveNext();
        }
    }
}
