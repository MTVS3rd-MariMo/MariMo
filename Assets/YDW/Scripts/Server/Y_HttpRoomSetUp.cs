using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class SendLessonId
{
    public int lessonId;
}

[Serializable]
public class RequestClassMaterial
{
    public int lessonMaterialId;
}

[System.Serializable]
public class UserIdList
{
    public List<int> userIds;
}

[Serializable]
public class ClassMaterial
{
    public string bookTitle;
    public string bookContents;
    public List<Quiz> quizzes;
    public List<OpenQuestion> openQuestions;
    public List<string> lessonRoles;
}

public class Y_HttpRoomSetUp : MonoBehaviour
{
    static Y_HttpRoomSetUp instance;

    public List<int> userList = new List<int>();

    public ClassMaterial realClassMaterial;


    public static Y_HttpRoomSetUp GetInstance()
    {
        if (instance == null)
        {
            GameObject go = new GameObject();
            go.name = "Y_HttpRoomSetUp";
            go.AddComponent<Y_HttpRoomSetUp>();
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

    // GET : 서버에게 데이터를 조회 요청
    public IEnumerator Get(HttpInfo info)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(info.url))
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            StartCoroutine(GetClassMaterial());
            //print("userID 프린트하기 : " + Y_HttpLogIn.GetInstance().userId);
        }
    }

    public string sendLessonIdUrl = "api/lesson/enter/";

    public IEnumerator SendLessonId()
    {
        SendLessonId sendLessonId = new SendLessonId
        {
            lessonId = 101
            // 더미!!!!!
        };

        // JSON 형식으로 변환
        string jsonBody = JsonUtility.ToJson(sendLessonId);

        // HttpInfo 설정
        HttpInfo info = new HttpInfo
        {
            url = Y_HttpLogIn.GetInstance().mainServer + sendLessonIdUrl + sendLessonId.lessonId.ToString(),
            body = jsonBody,
            contentType = "application/json",
            onComplete = (DownloadHandler downloadHandler) =>
            {
                Debug.Log("레슨 아이디 보내기 성공: " + downloadHandler.text);
            }
        };

        // Put 메서드 호출
        yield return StartCoroutine(Put(info));
    }

    public IEnumerator GetUserIdList()
    {
        SendLessonId sendLessonId = new SendLessonId
        {
            lessonId = 101 // 더미!!!!!
        };

        string jsonBody = JsonUtility.ToJson(sendLessonId);
        string requestUserListUrl = "api/lesson/participant/";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(Y_HttpLogIn.GetInstance().mainServer + requestUserListUrl + sendLessonId.lessonId.ToString()))
        {
            webRequest.downloadHandler = new DownloadHandlerBuffer(); // 서버가 다운로드 할 수 있는 공간 만듦

            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버 응답 처리
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("유저 아이디 리스트 받아오기 성공: " + webRequest.downloadHandler.text);

                // JSON 데이터 파싱
                UserIdList userIdList = JsonUtility.FromJson<UserIdList>(webRequest.downloadHandler.text);
                foreach (int userId in userIdList.userIds)
                {
                    Debug.Log("유저 아이디 찍어보기 : " + userId);
                }

                userList = userIdList.userIds;
            }
            else
            {
                Debug.LogError("유저 아이디 리스트 받아오기 실패: " + webRequest.error);
            }
        }

    }

    public IEnumerator GetClassMaterial()
    {
        SendLessonId sendLessonId = new SendLessonId
        {
            lessonId = 101 // 더미!!!!!
        };

        string jsonBody = JsonUtility.ToJson(sendLessonId);
        string requestUserListUrl = "api/lesson/";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(Y_HttpLogIn.GetInstance().mainServer + requestUserListUrl + sendLessonId.lessonId.ToString()))
        {
            webRequest.downloadHandler = new DownloadHandlerBuffer(); // 서버가 다운로드 할 수 있는 공간 만듦

            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버 응답 처리
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("수업자료 받아오기 성공: " + webRequest.downloadHandler.text);

                // JSON 데이터 파싱
                ClassMaterial classMaterial = JsonUtility.FromJson<ClassMaterial>(webRequest.downloadHandler.text);
                print("bookTitle : " + classMaterial.bookTitle);
                print("bookTContent : " + classMaterial.bookContents);

                foreach(Quiz quiz in classMaterial.quizzes)
                {
                    print("퀴즈 질문 : " + quiz.question);
                    print("퀴즈 답 : " + quiz.answer);
                    print("퀴즈 선지1 : " + quiz.choices1);
                    print("퀴즈 선지2 : " + quiz.choices2);
                    print("퀴즈 선지3 : " + quiz.choices3);
                    print("퀴즈 선지4 : " + quiz.choices4);
                }

                foreach(OpenQuestion openQuestion in classMaterial.openQuestions)
                {
                    print("열린 질문 : " + openQuestion.questionTitle);
                }

                foreach(string lessonRole in classMaterial.lessonRoles)
                {
                    print("레슨 롤 : " + lessonRole);
                }

                realClassMaterial = classMaterial;
            }
            else
            {
                Debug.LogError("수업자료 받아오기 실패: " + webRequest.error);
            }
        }
    }

}
