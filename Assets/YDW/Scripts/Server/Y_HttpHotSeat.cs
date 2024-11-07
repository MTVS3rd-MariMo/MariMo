using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class SelfIntroduce
{
    public int userId;
    public int lessonId;
    public string selfIntroduce;
}

[Serializable]
public class InterviewFile
{
    public int lessonId;
    public string userName;
    public string character;
    public byte[] wavFile;
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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
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
    public string sendSelfIntroduceUrl = "api/lesson-result/hot-sitting/self-inroduce";

    public IEnumerator SendSelfIntroduce(string content)
    {
        SelfIntroduce selfIntroduce = new SelfIntroduce
        {
            userId = Int32.Parse(Y_HttpLogIn.GetInstance().userId),
            lessonId = 101, // 더미!!!!!
            selfIntroduce = "안녕하세요"
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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha8))
        {
            GetSelfIntroduce();
        }
    }

    string mySelfIntroduce;

    void GetSelfIntroduce()
    {
        mySelfIntroduce = Y_HotSeatController.Instance.selfIntroduceInput.text;
        print("자기소개예요 : " + mySelfIntroduce);
    }

    // 핫시팅 음성파일 전송
    public string sendInterviewWAV = "api/lesson-result/hot-sitting/record";

    //public IEnumerator SendInterviewFile(byte[] record)
    //{
    //    InterviewFile interviewFile = new InterviewFile
    //    {
    //        lessonId = 101,
    //        userName = "정현민",
    //        character = "바다거북",
    //        wavFile = new byte[]
    //    };
    //}

}
