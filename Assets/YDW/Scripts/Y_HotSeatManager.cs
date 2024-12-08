using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Y_HotSeatManager : MonoBehaviourPun
{
    //public static Y_HotSeatManager Instance { get; private set; }

    float triggerNum = 0;
    bool act = false;
    List<GameObject> players = new List<GameObject>();
    public GameObject hotSeatCanvas;

    // 애니메이션 오브젝트
    public GameObject Ani_Object;
    public GameObject VirtualCamera;
    //public Image hotSeatUI;

    private void Awake()
    {
        //// Singleton 인스턴스 설정
        //if (Instance == null)
        //{
        //    Instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !act)
        {
            triggerNum++;
            //Debug.LogError("+++++++++ : " + triggerNum);

            players.Add(other.gameObject);

            if (triggerNum >= 4 && !act)
            {

                //hotSeatCanvas.SetActive(true);
                VirtualCamera.SetActive(true);
                StartCoroutine(SetPlayerPos());

                //if(photonView.IsMine)
                //{
                //    act = true;

                //    RPC_MoveControl(false);

                //    RPC_AniDelayStart();
                //}
            }
        }
    }

    public Transform[] hotSeatPos;

    IEnumerator SetPlayerPos()
    {
        bool[] playersInPosition = new bool[players.Count]; // 각 플레이어의 도달 상태를 추적
        int i = 0;

        while (true)
        {
            bool allPlayersInPosition = true;

            for (i = 0; i < players.Count; i++)
            {
                if (playersInPosition[i]) continue; // 이미 도달한 플레이어는 무시

                GameObject player = players[i];
                Vector3 playerStartPos = hotSeatPos[i].position;
                playerStartPos.y = player.transform.position.y;

                float distanceSqr = (player.transform.position - playerStartPos).sqrMagnitude; // 제곱 거리
                if (distanceSqr < 1f) // 0.1f^2 = 0.01
                {
                    player.transform.position = playerStartPos; // 정확히 위치 고정
                    playersInPosition[i] = true; // 도달 상태 업데이트
                }
                //if (Vector3.Distance(player.transform.position, playerStartPos) < 0.5f) // 도달 여부 확인
                //{
                //    player.transform.position = playerStartPos; // 정확히 위치 고정
                //    playersInPosition[i] = true; // 도달 상태 업데이트
                //}
                else
                {
                    allPlayersInPosition = false;
                    player.transform.position = Vector3.Lerp(player.transform.position, playerStartPos, 0.01f); // 부드럽게 이동
                }
            }

            if (allPlayersInPosition)
            {
                if (photonView.IsMine)
                {
                    act = true;

                    RPC_MoveControl(false);
                    RPC_AniDelayStart();
                }
                break; // 모든 플레이어가 위치에 도달하면 코루틴 종료
            }

            yield return null;
        }
    }

    void RPC_ActivateHotSeat()
    {
        photonView.RPC(nameof(ActivateHotSeat), RpcTarget.All);
    }

    [PunRPC]
    void ActivateHotSeat()
    {
        CanvasRenderer[] canvasRenderers = hotSeatCanvas.GetComponentsInChildren<CanvasRenderer>();
        //Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_3D_OBJECT_03);
        hotSeatCanvas.SetActive(true);
        StartCoroutine(IncreaseAlpha(canvasRenderers));
    }

    float canvasAlphaTime = 0;

    public IEnumerator IncreaseAlpha(CanvasRenderer[] canvasRenderers)
    {
        while (true)
        {
            canvasAlphaTime += Time.deltaTime;

            foreach (CanvasRenderer canvasRenderer in canvasRenderers)
            {
                Color originalColor = canvasRenderer.GetColor();
                originalColor.a = canvasAlphaTime;
                canvasRenderer.SetColor(originalColor);
            }

            if (canvasAlphaTime > 1)
            {
                canvasAlphaTime = 0;
                break;
            }

            yield return null;
        }
    }


    void RPC_AniDelayStart()
    {
        photonView.RPC(nameof (AniDelayStart), RpcTarget.All);
    }

    [PunRPC]
    public void AniDelayStart()
    {
        StartCoroutine(AniDelay());
    }

    public IEnumerator AniDelay()
    {
        //VirtualCamera.SetActive(true);

        Ani_Object.SetActive(true);
        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_3D_OBJECT_03);
        yield return new WaitForSeconds(2f);
        if (photonView.IsMine) RPC_ActivateHotSeat();
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !act)
        {
            triggerNum--;
            //Debug.LogError("-------- : " + triggerNum);

            players.Remove(other.gameObject);
        }
    }

    void RPC_MoveControl(bool canmove)
    {
        photonView.RPC(nameof(MoveControl), RpcTarget.All, canmove);
    }

    [PunRPC]
    public void MoveControl(bool canmove)
    {
        foreach (GameObject obj in players)
        {
            obj.GetComponent<Y_PlayerMove>().movable = canmove;

            obj.transform.position = new Vector3(obj.transform.position.x, 3, obj.transform.position.z);
        }
    }
}
