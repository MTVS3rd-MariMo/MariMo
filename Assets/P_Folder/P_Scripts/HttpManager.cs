﻿using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;

[System.Serializable] // json 데이터를 받을 떄 중요
public struct PostInfo
{
    public int userId;
    public int id;
    public string title;
    public string body;
}

[System.Serializable]
public struct PostInfoArray
{
    public List<PostInfo> data;
}

public class HttpInfo
{
    public string url = "";

    // Body 데이터
    public string body = "";

    // contentType
    public string contentType = "";

    // 통신 성공 후 호출되는 함수 담을 변수
    public Action<DownloadHandler> onComplete;

}





public class User
{
    public string userId;
}

// 아바타 관련 데이터
[System.Serializable]
public struct UserAvatarData
{
    public int userId;
    public int lessonId;
    public string avatarImg;
    public List<AnimationData> animations;
}

// 아바타 애니메이션 데이터
[System.Serializable]
public struct AnimationData
{
    public int animationId;
    public string animation;
}


public class HttpManager : MonoBehaviour
{
    static HttpManager instance;

    public static HttpManager GetInstance()
    {
        if (instance == null)
        {
            GameObject go = new GameObject();
            go.name = "HttpManager";
            go.AddComponent<HttpManager>();
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            HttpInfo info = new HttpInfo();
            info.url = "https://jsonplaceholder.typicode.com/posts";

            StartCoroutine(Get(info));
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            HttpInfo info = new HttpInfo();
            info.url = "https://jsonplaceholder.typicode.com/albums";

            StartCoroutine(Get(info));
        }
    }

    // GET : 서버에게 데이터를 조회 요청
    public IEnumerator Get(HttpInfo info)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(info.url))
        {
            webRequest.SetRequestHeader("userId", Y_HttpLogIn.GetInstance().userId.ToString());
            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info);
        }
    }

    // Lessons 받는 용도
    public IEnumerator GetLesson(HttpInfo info)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(info.url))
        {
            webRequest.SetRequestHeader("userId", Y_HttpLogIn.GetInstance().userId.ToString());
            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info);
        }
    }


    // 서버에게 내가 보내는 데이터를 생성해줘
    public IEnumerator Post(HttpInfo info)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(info.url, info.body, info.contentType))
        {
            webRequest.SetRequestHeader("userId", Y_HttpLogIn.GetInstance().userId.ToString());
            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();


            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info);
        }
    }

    public IEnumerator PostBackGround(HttpInfo info)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(info.url, info.body, info.contentType))
        {
            webRequest.SetRequestHeader("userId", Y_HttpLogIn.GetInstance().userId.ToString());
            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();


            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info);
        }
    }

    public IEnumerator PostRoom(HttpInfo info)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(info.url, info.body, info.contentType))
        {
            webRequest.SetRequestHeader("userId", Y_HttpLogIn.GetInstance().userId.ToString());

            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();


            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info);
        }
    }

    // 수정용
    public IEnumerator Put(HttpInfo info)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(info.url,"PUT"))
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

    // 열린질문용
    public IEnumerator PutOpenQ(HttpInfo info, string userId )
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(info.url, "PUT"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(info.body);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", info.contentType);
            webRequest.SetRequestHeader("userId", userId);

            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info);
        }
    }

    public IEnumerator PutPicture(HttpInfo info)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("img", System.IO.File.ReadAllBytes(info.body), System.IO.Path.GetFileName(info.body), "image/png");

        using (UnityWebRequest webRequest = UnityWebRequest.Post(info.url, form))
        {
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            webRequest.SetRequestHeader("userId", Y_HttpLogIn.GetInstance().userId.ToString());
            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info);
        }
    }

    public IEnumerator PutUser(HttpInfo info, string userId)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(info.url, "PUT"))
        {
            webRequest.SetRequestHeader("userId", userId);

            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info);
        }
    }

    // 파일 업로드 (form-data)
    public IEnumerator UploadFileByFormData(HttpInfo info)
    {
        // info.data 에는 파일의 위치
        // info.data 에 있는 파일을 byte 배열로 읽어오자
        byte[] data = File.ReadAllBytes(info.body);

        // data 를 MultipartForm 으로 셋팅
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //formData.Add(new MultipartFormFileSection("file", data, "image.jpg", info.contentType));
        formData.Add(new MultipartFormFileSection("file", System.Convert.FromBase64String(info.body), "image.png", "image/png"));


        using (UnityWebRequest webRequest = UnityWebRequest.Post(info.url, formData))
        {
            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info);

        }
    }

    public IEnumerator UploadFileByFormDataPDF(HttpInfo info, string bookTitle, string author)
    {
        // info.data 에는 파일의 위치
        // info.data 에 있는 파일을 byte 배열로 읽어오자
        byte[] data = File.ReadAllBytes(info.body);

        string fileName = Path.GetFileName(info.body);

        // data 를 MultipartForm 으로 셋팅
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormFileSection("pdf", data, fileName, "application/pdf"));
        formData.Add(new MultipartFormDataSection("bookTitle", bookTitle));
        formData.Add(new MultipartFormDataSection("author", author));

        using (UnityWebRequest webRequest = UnityWebRequest.Post(info.url, formData))
        {
            //webRequest.SetRequestHeader("Content-Type", "multipart/form-data");
            webRequest.SetRequestHeader("userId", Y_HttpLogIn.GetInstance().userId.ToString());

            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 요청 결과 상태 확인
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("성공적으로 pdf 업로드");
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

            if (webRequest.result == UnityWebRequest.Result.Success)
            {

            }
        }
    }



    // 그림판 보내기 
    public IEnumerator UploadFileByFormDataArt(HttpInfo info, byte[] imgData, int userId, int lessonId)
    {
        // info.data 에는 파일의 위치
        // info.data 에 있는 파일을 byte 배열로 읽어오자
        //byte[] data = System.Convert.FromBase64String(info.body);

        // data 를 MultipartForm 으로 셋팅
        WWWForm form = new WWWForm();
        form.AddField("body", info.body);
        form.AddField("lessonId", lessonId.ToString());
        form.AddBinaryData("img", imgData, "img.png", "image/png");

        print("요청 체크1");

        using (UnityWebRequest webRequest = UnityWebRequest.Post(info.url, form))
        {
            print("요청 체크2");

            //webRequest.SetRequestHeader("multipart/form-data");

            webRequest.SetRequestHeader("userId", Y_HttpLogIn.GetInstance().userId.ToString());
            webRequest.SetRequestHeader("lessonId", Y_HttpRoomSetUp.GetInstance().userlessonId.ToString());

            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 요청 결과 상태 확인
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("성공적으로 이미지 업로드");
                Debug.Log("서버 응답: " + webRequest.downloadHandler.text);
            }
            // else if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            // {
            //     Debug.LogError("연결 오류 발생: " + webRequest.error);
            // }
            // else if (webRequest.result == UnityWebRequest.Result.DataProcessingError)
            // {
            //     Debug.LogError("데이터 처리 오류 발생: " + webRequest.error);
            // }
            // else if (webRequest.result == UnityWebRequest.Result.ProtocolError)
            // {
            //     Debug.LogError("프로토콜 오류 발생: " + webRequest.error);
            // }
            // else
            // {
            //     Debug.LogError("알 수 없는 오류 발생: " + webRequest.error);
            // }


            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info);
        }
    }

    // 파일 업로드
    public IEnumerator UploadFileByByte(HttpInfo info)
    {
        // info.data 에는 파일의 위치
        // info.data 에 있는 파일을 byte 배열로 읽어오자
        byte[] data = File.ReadAllBytes(info.body);


        using (UnityWebRequest webRequest = new UnityWebRequest(info.url, "POST"))
        {
            // 업로드 하는 데이터
            webRequest.uploadHandler = new UploadHandlerRaw(data);
            webRequest.uploadHandler.contentType = info.contentType;

            // 응답 받는 데이터 공간
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            webRequest.SetRequestHeader("userId", Y_HttpLogIn.GetInstance().userId.ToString());
            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info);

        }
    }

    public IEnumerator DownloadSprite(HttpInfo info)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(info.url))
        {
            webRequest.SetRequestHeader("userId", Y_HttpLogIn.GetInstance().userId.ToString());

            yield return webRequest.SendWebRequest();

            DoneRequest(webRequest, info);
        }
    }

    public IEnumerator DownloadAudio(HttpInfo info)
    {
        using (UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip(info.url, AudioType.WAV))
        {
            yield return webRequest.SendWebRequest();

            //DownloadHandlerAudioClip handler = webRequest.downloadHandler as DownloadHandlerAudioClip;
            //handler.audioClip 을 Audiosource 에 셋팅하고 플레이!

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
            // Erroe 의 이유를 출력
            Debug.LogError("Net Error : " + webRequest.error);
        }
    }

    public IEnumerator Delete(HttpInfo info)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Delete(info.url))
        {
            webRequest.SetRequestHeader("userId", Y_HttpLogIn.GetInstance().userId.ToString());
            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();


            // 서버에게 응답이 왔다.
            DoneRequest(webRequest, info);
        }
    }
}
