using Cinemachine;
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
    public GameObject VirtualCamera;

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

                if(photonView.IsMine)
                {
                    act = true;

                    RPC_MoveControl(false);

                    RPC_AniDelayStart();
                }
            }
        }
    }

    void RPC_ActivateHotSeat()
    {
        
        photonView.RPC(nameof(ActivateHotSeat), RpcTarget.All);
    }

    [PunRPC]
    void ActivateHotSeat()
    {
        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_3D_OBJECT_03);
        hotSeatCanvas.SetActive(true);
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
        VirtualCamera.SetActive(true);

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
        }
    }
}
