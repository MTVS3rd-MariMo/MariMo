﻿using Photon.Pun;
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
                //hotSeatCanvas.SetActive(true);

                MoveControl(false);

                StartCoroutine(AniDelay());
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
        //Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_3D_OBJECT_03);
        hotSeatCanvas.SetActive(true);
    }

    IEnumerator AniDelay()
    {
        Ani_Object.SetActive(true);
        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_3D_OBJECT_03);
        yield return new WaitForSeconds(1.5f);
        RPC_ActivateHotSeat();
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
