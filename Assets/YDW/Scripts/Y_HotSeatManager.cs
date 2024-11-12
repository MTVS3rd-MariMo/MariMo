using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_HotSeatManager : MonoBehaviourPun
{
    //public static Y_HotSeatManager Instance { get; private set; }

    float triggerNum = 0;
    bool act = false;
    List<GameObject> players = new List<GameObject>();
    public GameObject hotSeatCanvas;

    // 애니메이션 오브젝트
    public GameObject Ani_Object;
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
            print("+++++++++ : " + triggerNum);

            players.Add(other.gameObject);

            if (triggerNum >= 4 && !act)
            {
                act = true;

                if (photonView.IsMine) RPC_ActivateHotSeat();
                //hotSeatCanvas.SetActive(true);

                MoveControl(false);

                Ani_Object.SetActive(true);
            }
        }
    }

    void RPC_ActivateHotSeat()
    {
        Debug.LogError("RPC Hot Seat 했다");
        photonView.RPC(nameof(ActivateHotSeat), RpcTarget.All);
    }

    [PunRPC]
    void ActivateHotSeat()
    {
        //Debug.LogError("Activate Hot Seat 했다");
        hotSeatCanvas.SetActive(true);
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !act)
        {
            triggerNum--;
            print("-------- : " + triggerNum);

            players.Remove(other.gameObject);
        }
    }

    public void MoveControl(bool canmove)
    {
        foreach (GameObject obj in players)
        {
            obj.GetComponent<Y_PlayerMove>().movable = canmove;
        }
    }
}
