using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;
using Photon.Pun;
using UnityEngine.UI;

public class Y_VoiceManager : MonoBehaviour
{
    public static Y_VoiceManager Instance { get; private set; }

    public Recorder recorder;
    public Image voiceIcon;
    public Image noVoiceIcon;
    public bool isTalking = false;
    public bool hasChanged = false;

    private void Awake()
    {
        // Singleton 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
                noVoiceIcon.gameObject.SetActive(false);
                voiceIcon.gameObject.SetActive(true);
            }
            else
            {
                voiceIcon.gameObject.SetActive(false);
                noVoiceIcon.gameObject.SetActive(true);
            }
        }

        //if(isTalking)
        //{
        //    recorder.TransmitEnabled = true;
        //    noVoiceIcon.gameObject.SetActive(false);
        //    voiceIcon.gameObject.SetActive(true);
        //    isTalking = false;
        //}
        //else if(!hasChanged)
        //{
        //    recorder.TransmitEnabled = false;
        //    voiceIcon.gameObject.SetActive(false);
        //    noVoiceIcon.gameObject.SetActive(true);
        //}
    }


}
