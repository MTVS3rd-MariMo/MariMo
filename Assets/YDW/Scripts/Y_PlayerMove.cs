using Cinemachine;
using Photon.Pun;
using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Utilities;

public class Y_PlayerMove : MonoBehaviour, IPunObservable
{
    public float trackingSpeed = 10;
    public float speed;
    CharacterController cc;

    int layerMaskGround;

    public GameObject test;

    public PhotonView pv;
    Vector3 myPos;

    public bool movable = false;

    PhotonVoiceView voiceView;
    Y_PlayerVoice playerVoice;

    public bool isFive;

    public float moveDistance;
    Vector3 targetPosition;
    bool isMoving;

    public ParticleSystem footstepsSystem;
    public float footstepDelta = 1f; // 발자국 간 거리
    public float footstepGap = 0.5f; // 좌우 간격
    private int dir = 1; // 발자국 좌우 방향
    private Vector3 lastFootstepPosition;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        voiceView = GetComponent<PhotonVoiceView>();
        playerVoice = GetComponent<Y_PlayerVoice>();
    }

    // Start is called before the first frame update
    void Start()
    {
        speed = 10;
        //if (Application.platform == RuntimePlatform.Android) speed = 0.001f;
        lastFootstepPosition = transform.position; // 초기 위치
        layerMaskGround = LayerMask.GetMask("Ground");

        cc = GetComponent<CharacterController>();

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    void FixedUpdate()
    {
        if (pv.Owner.IsMasterClient && isFive) // pv.Owner.IsMasterClient
        {
            //Debug.LogError("playerAverage 널이니??? : " + (GetComponent<Y_SetCamera>().playerAverage == null)); -> False
            gameObject.transform.position = GetComponent<Y_SetCamera>().playerAverage.position;
        }

        if (pv.IsMine)
        {
            // 로컬 플레이어만 이동 처리
            if (movable)
            {
                Move();
            }
        }
        else
        {
            // 네트워크 위치 데이터 기반으로만 갱신
            transform.position = Vector3.Lerp(transform.position, myPos, Time.deltaTime * trackingSpeed);
        }

    }

    // 당겨지는 강도 
    float pulling = 100f;

    void Move()
    {
        //movable = true; ////////////////
        // 만일 내가 소유권을 가진 캐릭터라면 
        if(pv.IsMine)
        {
            if(Application.platform == RuntimePlatform.Android && !pv.Owner.IsMasterClient)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    //Debug.LogWarning("터치됐다~!");

                    if ((touch.phase == TouchPhase.Began) || (touch.phase == TouchPhase.Moved))
                    {
                        Ray ray = Camera.main.ScreenPointToRay(touch.position);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, 9999f, layerMaskGround))
                        {
                            //Debug.LogWarning("에이전트가 찾아간다");
                            //agent.SetDestination(hit.point);
                           
                            // 목표 위치 설정
                            targetPosition = hit.point;
                            targetPosition.y = 3f;
                            isMoving = true;

                            // 화면 밖으로 고정은 어차피 화면 밖은 터치할 수 없으니 생략
                            // 근데 플레이어 끌고 오기는 해야 할 듯.....


                        }
                    }
                }
                if (isMoving)
                {
                    MoveTowardsTarget();
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

                moveDistance = finalDir.sqrMagnitude;

                // finalDir 을 정규화 하자 (벡터의 크기를 1로 만든다)
                finalDir.Normalize();

                // 구해진 방향으로 계속 이동하고 싶다.
                // 오른쪽으로 이동하고 싶다.
                //transform.Translate(finalDir * speed * Time.deltaTime);
                // P = P0 + vt (이동공식)
                //transform.position += finalDir * speed * Time.deltaTime;
                cc.Move(finalDir * speed * Time.fixedDeltaTime);
            }

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
        else
        {
            transform.position = Vector3.Lerp(transform.position, myPos, Time.deltaTime * trackingSpeed);
        }
    }

    // 목표 지점으로 이동
    void MoveTowardsTarget()
    {
        // 목표 지점까지의 거리 계산
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // 방향 벡터 계산 및 이동
        //Vector3 direction = (targetPosition - transform.position).normalized;
        //transform.position += direction * speed * Time.deltaTime;
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;
        cc.Move(direction * speed * Time.fixedDeltaTime);

        moveDistance = direction.magnitude;

        // 이동 후 거리 계산
        float newDistanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // 목표 지점에 도달했거나 초과했는지 확인
        if (newDistanceToTarget <= 0.1f) // 임계값 조정 가능 // newDistanceToTarget <= 0.1f ||
        {
            transform.position = targetPosition; // 목표 지점에 고정
            isMoving = false;                    // 이동 중단
            moveDistance = 0;
            return;
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
            stream.SendNext(moveDistance);
        }

        // 그렇지 않고 만일 데이터를 서버로부터 읽어오는 상태라면
        else if (stream.IsReading)
        {
            myPos = (Vector3)stream.ReceiveNext();
            playerVoice.isTalking = (bool)stream.ReceiveNext();
            moveDistance = (float)stream.ReceiveNext();
        }
    }
}
