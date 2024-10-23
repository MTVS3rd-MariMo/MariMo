using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.PUN;
using UnityEngine.UI;
using Photon.Pun;

public class Y_PlayerVoice : MonoBehaviour
{
    public RawImage voiceIcon;
    PhotonVoiceView voiceView;
    PhotonView pv;
    public bool isTalking = false;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        voiceView = GetComponent<PhotonVoiceView>();
    }

    // Update is called once per frame
    void Update()
    {
        if(pv.IsMine)
        {
            // 현재 말을 하고 있다면 보이스 아이콘을 활성화한다
            voiceIcon.gameObject.SetActive(voiceView.IsRecording);
        }
        else
        {
            voiceIcon.gameObject.SetActive(isTalking);
        }
        
    }
}
