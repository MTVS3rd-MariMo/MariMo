using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;
using Photon.Pun;
using Photon.Voice.PUN;
using UnityEngine.UI;
using System.IO;
using System;
using Photon.Pun.Demo.PunBasics;

public class Y_VoiceManager : MonoBehaviourPun
{
    public static Y_VoiceManager Instance { get; private set; }

    public Recorder recorder;
    public Image voiceIcon;
    public Image noVoiceIcon;
    public bool isTalking = false;
    public bool hasChanged = false;

    private Dictionary<int, AudioClip> voiceData = new Dictionary<int, AudioClip>();
    private int recordingFrequency = 44100;
    //private int recordingLength = 15; // 최대 녹음 길이 (초 단위)
    private AudioClip currentRecording;

    public int actorNumber;
    PhotonVoiceView myVoiceView;

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
        actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        //// 현재 로컬 플레이어의 voiceView 찾기
        //PhotonVoiceView[] allVoiceViews = FindObjectsOfType<PhotonVoiceView>();

        //foreach (var voiceView in allVoiceViews)
        //{
        //    PhotonView photonVoiceView = voiceView.GetComponent<PhotonView>();

        //    if(photonVoiceView.)
        //    myVoiceView = 



        //    if (voiceView.RecorderInUse.TransmitEnabled)
        //    {
        //        noVoiceIcon.gameObject.SetActive(false);
        //        voiceIcon.gameObject.SetActive(true);
        //    }
        //    else
        //    {
        //        voiceIcon.gameObject.SetActive(false);
        //        noVoiceIcon.gameObject.SetActive(true);
        //    }
        //}
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

        // 음소거가 되어 있으면 빗금친 스피커 이미지로 바꾼다


        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    StartRecording(1, 5);
        //}

        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    StopRecording(1, "test");
        //}
    }



    public void StartRecording(int playerId, int recordingLength)
    {
        //print("!!!!!!!!! playerId " + playerId + ", actorNumber: " + PhotonNetwork.LocalPlayer.ActorNumber);

        if (PhotonNetwork.LocalPlayer.ActorNumber == playerId)
        {
            currentRecording = Microphone.Start(null, true, recordingLength, recordingFrequency);
            Debug.Log($"녹음 시작됨: {testInt}");
        }
    }

    int testInt = 0;

    public void StopRecording(int playerId, int selfIntNum)
    {
        if (Microphone.IsRecording(null) && PhotonNetwork.LocalPlayer.ActorNumber == playerId)
        {
            int recordingPosition = Microphone.GetPosition(null);
            Microphone.End(null);

            if (currentRecording != null && recordingPosition > 0)
            {
                // 녹음 데이터를 실제 녹음 길이만큼 잘라내기
                AudioClip trimmedRecording = TrimAudioClip(currentRecording, recordingPosition);

                /////////////// 딕셔너리에 추가 말고 바로 통신 해야 함 

                //voiceData[playerId] = trimmedRecording;

                //Debug.Log($"녹음 Dictionary 에 저장됨: {testInt}");

                //SendAsWav(trimmedRecording, selfIntNum);
                Debug.Log($"Wav 파일로 저장됨: {testInt}");
            }

            RPC_UpdateTestInt();
        }
    }

    public void RPC_UpdateTestInt()
    {
        photonView.RPC(nameof(updateTestInt), RpcTarget.All);
    }

    [PunRPC]
    public void updateTestInt()
    {
        testInt++;
        print("TestInt Update: " + testInt);
    }

    private AudioClip TrimAudioClip(AudioClip clip, int lengthSamples)
    {
        float[] samples = new float[lengthSamples];
        clip.GetData(samples, 0);

        AudioClip trimmedClip = AudioClip.Create(clip.name, lengthSamples, clip.channels, clip.frequency, false);
        trimmedClip.SetData(samples, 0);

        return trimmedClip;
    }

    public void SendAsWav(AudioClip clip, int selfIntNum) // string filePath
    {
        // AudioClip 데이터 추출
        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        byte[] wavData = ConvertToWav(samples, clip.channels, clip.frequency);

        // 통신
        StartCoroutine(Y_HttpHotSeat.GetInstance().SendInterviewFile(wavData, selfIntNum));

        // 파일로 저장 -> 나중에 저장 대신 통신으로 바꿔야 함
        //File.WriteAllBytes(filePath, wavData);
        //Debug.Log($"AudioClip saved as WAV at: {filePath}");
    }

    private byte[] ConvertToWav(float[] samples, int channels, int frequency)
    {
        MemoryStream stream = new MemoryStream();

        int byteRate = frequency * channels * 2; // 2 bytes per sample (16-bit audio)
        int dataSize = samples.Length * 2;

        // WAV 헤더 작성
        stream.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"), 0, 4);
        stream.Write(BitConverter.GetBytes(36 + dataSize), 0, 4);
        stream.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"), 0, 4);
        stream.Write(System.Text.Encoding.ASCII.GetBytes("fmt "), 0, 4);
        stream.Write(BitConverter.GetBytes(16), 0, 4);
        stream.Write(BitConverter.GetBytes((short)1), 0, 2);
        stream.Write(BitConverter.GetBytes((short)channels), 0, 2);
        stream.Write(BitConverter.GetBytes(frequency), 0, 4);
        stream.Write(BitConverter.GetBytes(byteRate), 0, 4);
        stream.Write(BitConverter.GetBytes((short)(channels * 2)), 0, 2);
        stream.Write(BitConverter.GetBytes((short)16), 0, 2);

        stream.Write(System.Text.Encoding.ASCII.GetBytes("data"), 0, 4);
        stream.Write(BitConverter.GetBytes(dataSize), 0, 4);

        // 오디오 샘플 데이터를 16-bit PCM으로 변환
        foreach (var sample in samples)
        {
            short intSample = (short)(Mathf.Clamp(sample, -1f, 1f) * short.MaxValue);
            stream.Write(BitConverter.GetBytes(intSample), 0, 2);
        }

        return stream.ToArray();
    }


}
