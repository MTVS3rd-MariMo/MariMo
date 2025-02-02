﻿using Photon.Pun;
using Photon.Voice.PUN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class SelfIntroduce
{
    public int selfIntNum;
    public int lessonId;
    public string selfIntroduce;
}

[Serializable]
public class InterviewFile
{
    public int lessonId;
    public int selfIntNum;
    public string userName;
    public string character;
}

public class Y_HttpHotSeat : MonoBehaviour
{
    static Y_HttpHotSeat instance;

    public static Y_HttpHotSeat GetInstance()
    {
        if (instance == null)
        {
            GameObject go = new GameObject();
            go.name = "HttpHotSeat";
            go.AddComponent<Y_HttpHotSeat>();
        }

        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    Y_BookController bookController;

    private void Start()
    {
        bookController = GameObject.Find("BookCanvas").GetComponent<Y_BookController>();
    }

    public IEnumerator Post(HttpInfo info)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(info.url, info.body, info.contentType))
        {
            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info);
        }
    }

    public IEnumerator Put(HttpInfo info)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(info.url, "PUT"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(info.body);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("userId", Y_HttpLogIn.GetInstance().userId.ToString());

            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info);
        }
    }

    public IEnumerator UploadFileByFormDataWav(HttpInfo info, byte[] wavFile, int selfIntNum)
    {
        // data 를 MultipartForm 으로 셋팅
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("lessonId", Y_HttpRoomSetUp.GetInstance().userlessonId.ToString()));
        formData.Add(new MultipartFormDataSection("userName", GetUserNickName())); // 
        formData.Add(new MultipartFormDataSection("character", GetCharacterName())); // 
        formData.Add(new MultipartFormDataSection("selfIntNum", selfIntNum.ToString())); //GameObject.Find("HotSeatCanvas").GetComponent<Y_HotSeatController>().selfIntNum.ToString()

        formData.Add(new MultipartFormFileSection("wavFile", wavFile, "interview.wav", "audio/wav"));

        using (UnityWebRequest webRequest = UnityWebRequest.Post(info.url, formData))
        {
            webRequest.SetRequestHeader("userId", Y_HttpLogIn.GetInstance().userId.ToString());

            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 요청 결과 상태 확인
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("성공적으로 WAV 파일 업로드");
                Debug.Log("서버 응답: " + webRequest.downloadHandler.text);
            }
            else if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("연결 오류 발생: " + webRequest.error);
            }
            else if (webRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError("데이터 처리 오류 발생: " + webRequest.error);
            }
            else if (webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("프로토콜 오류 발생: " + webRequest.error);
            }
            else
            {
                Debug.LogError("알 수 없는 오류 발생: " + webRequest.error);
            }

            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info);
        }
    }

    void DoneRequest(UnityWebRequest webRequest, HttpInfo info)
    {
        // 만약에 결과가 정상이라면
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            // 응답 온 데이터를 요청한 클래스로 보내자
            if (info.onComplete != null)
            {
                info.onComplete(webRequest.downloadHandler);
            }
        }
        // 그렇지 않다면 (Error 라면)
        else
        {
            // Error 의 이유를 출력
            Debug.LogError("Net Error : " + webRequest.error);
        }
    }


    // 핫시팅 자기소개 전송
    public string sendSelfIntroduceUrl = "api/hot-sitting/self-introduce";

    public IEnumerator SendSelfIntroduce(int i)
    {
        SelfIntroduce selfIntroduce = new SelfIntroduce
        {
            //userId = Int32.Parse(Y_HttpLogIn.GetInstance().userId),
            selfIntNum = i,
            lessonId = Y_HttpRoomSetUp.GetInstance().userlessonId,
            selfIntroduce = GetSelfIntroduce()
        };

        // JSON 형식으로 변환
        string jsonBody = JsonUtility.ToJson(selfIntroduce);

        // HttpInfo 설정
        HttpInfo info = new HttpInfo
        {
            url = Y_HttpLogIn.GetInstance().mainServer + sendSelfIntroduceUrl,
            body = jsonBody,
            contentType = "application/json",
            onComplete = (DownloadHandler downloadHandler) =>
            {
                Debug.Log("자기소개 보내기 성공: " + downloadHandler.text);
            }
        };

        // Put 메서드 호출
        yield return StartCoroutine(Put(info));
    }

    public void StartSendIntCoroutine(int i)
    {
        StartCoroutine(SendSelfIntroduce(i));
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha8))
        //{
        //    StartCoroutine(SendSelfIntroduce(1));
        //}

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            print("7번 눌렀다");
            //PrintAvailableMicrophones();
            StartCoroutine(voiceTestCoroutine());
        }
    }

    void PrintAvailableMicrophones()
    {
        string[] devices = Microphone.devices;
        if (devices.Length == 0)
        {
            Debug.LogError("사용 가능한 마이크 장치가 없습니다.");
        }
        else
        {
            Debug.Log($"마이크 장치 목록 ({devices.Length}개):");
            for (int i = 0; i < devices.Length; i++)
            {
                Debug.Log($"[{i}] {devices[i]}");
            }
        }
    }

    IEnumerator voiceTestCoroutine()
    {
        print("코루틴 실행");
        PrintAvailableMicrophones();
        Y_VoiceManager.Instance.StartRecording(5, 100);
        yield return new WaitForSeconds(10f);
        Y_VoiceManager.Instance.StopRecording(5, 1);
    }

    string mySelfIntroduce;

    string GetSelfIntroduce()
    {
        mySelfIntroduce = GameObject.Find("HotSeatCanvas").GetComponent<Y_HotSeatController>().selfIntroduceInput.text;
        print("자기소개예요 : " + mySelfIntroduce);
        return mySelfIntroduce;
    }

    // 핫시팅 음성파일 전송
    private string sendInterviewWAV = "api/hot-sitting/wav-file";

    public IEnumerator SendInterviewFile(byte[] record, int selfIntNum)
    {
        //string jsonBody = JsonUtility.ToJson(interviewFile);

        // HttpInfo 설정
        HttpInfo info = new HttpInfo
        {
            url = Y_HttpLogIn.GetInstance().mainServer + sendInterviewWAV,
            body = "wavFile",
            contentType = "multipart/form-data",
            onComplete = (DownloadHandler downloadHandler) =>
            {
                Debug.Log("WAV 파일 보내기 성공: " + downloadHandler.text);
            }
        };

        yield return StartCoroutine(UploadFileByFormDataWav(info, record, selfIntNum));

    }

    string GetUserNickName()
    {
        print("유저 닉네임입니다 : " + bookController.myAvatar.pv.Owner.NickName);
        return bookController.myAvatar.pv.Owner.NickName;
    }

    string GetCharacterName()
    {
        //print("캐릭터 이름입니다 : " + Y_HotSeatController.Instance.characterNames[bookController.characterNum - 1].text);
        return GameObject.Find("HotSeatCanvas").GetComponent<Y_HotSeatController>().characterNames[bookController.characterNum - 1].text;
    }
}
