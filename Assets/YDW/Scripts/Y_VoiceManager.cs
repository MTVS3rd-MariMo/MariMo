using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;
using Photon.Pun;
using Photon.Voice.PUN;
using UnityEngine.UI;
using System.IO;
using System;

public class Y_VoiceManager : MonoBehaviour
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

        if (Input.GetKeyDown(KeyCode.N))
        {
            StartRecording(1, 5);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            StopRecording(1, "test");
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

    public void StartRecording(int actorNumber, int recordingLength)
    {
        if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            //PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled = true;
            currentRecording = Microphone.Start(null, false, recordingLength, recordingFrequency);
            Debug.Log($"녹음 시작됨: {actorNumber}");
        }
    }

    public void StopRecording(int actorNumber, string filename)
    {
        if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            //PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled = false;

            if (Microphone.IsRecording(null))
            {
                Microphone.End(null);
            }

            if (currentRecording != null)
            {
                voiceData[actorNumber] = currentRecording;
                Debug.Log($"녹음 Dictionary 에 저장됨: {actorNumber}");
                //if(PhotonNetwork.IsMasterClient)
                //{
                    SaveAsWav(currentRecording, "C:\\Users\\Admin\\OneDrive\\문서\\FinalProject\\HotSeatingAudio\\" + filename + ".wav");
                //}
                Debug.Log($"Wav 파일로 저장됨: {actorNumber}");
            }
        }
    }

    public static void SaveAsWav(AudioClip clip, string filePath)
    {
        // AudioClip 데이터 추출
        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        byte[] wavData = ConvertToWav(samples, clip.channels, clip.frequency);

        // 파일로 저장
        File.WriteAllBytes(filePath, wavData);
        Debug.Log($"AudioClip saved as WAV at: {filePath}");
    }

    private static byte[] ConvertToWav(float[] samples, int channels, int frequency)
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
