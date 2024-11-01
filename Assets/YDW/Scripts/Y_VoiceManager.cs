using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;
using Photon.Pun;
using UnityEngine.UI;

public class Y_VoiceManager : MonoBehaviour
{
    Recorder recorder;
    public Image voiceIcon;

    // Start is called before the first frame update
    void Start()
    {
        recorder = GetComponent<Recorder>();
    }

    // Update is called once per frame
    void Update()
    {
        // 만일, M 키를 누르면 음소거한다
        if(Input.GetKeyDown(KeyCode.M))
        {
            recorder.TransmitEnabled = !recorder.TransmitEnabled;
            if(recorder.TransmitEnabled)
            {
                voiceIcon.gameObject.SetActive(true);
            }
            else
            {
                voiceIcon.gameObject.SetActive(false);
            }
        }
    }
}
