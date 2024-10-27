using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class K_QuizPhoton : MonoBehaviourPunCallbacks
{
    PhotonView pv;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // 선지 4개 중 정답은 1개
    // 4명 다 정답 위에 있을 경우, 정답 처리
    // 4명 다 정답 위에 있지 않을 경우, 오답 처리
}
