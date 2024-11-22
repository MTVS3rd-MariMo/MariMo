using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class SignUpData
{
    public Role role;
    public string school;
    public int grade;
    public int classRoom;
    public int studentNumber;
    public string name;
    public string password;
}

[System.Serializable]
public class ResponseData
{
    public string userId;
    public string role;
}

public enum Role
{
    TEACHER,
    STUDENT
}

public class Y_HttpLogIn : MonoBehaviour
{
    static Y_HttpLogIn instance;
    public string userId;
    public string mainServer;

    public bool isLoggedIn = false;

    public static Y_HttpLogIn GetInstance()
    {
        if (instance == null)
        {
            GameObject go = new GameObject("HttpLogIn");
            instance = go.AddComponent<Y_HttpLogIn>();
        }

        return instance;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        mainServer = "http://211.250.74.75:8202/";
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 서버에게 내가 보내는 데이터를 생성해줘
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

    public string RegisterUrl = "api/user/signup"; ///////////////////

    public IEnumerator SignUpCoroutine(string username, string password, string school, int grade, int className, int studentNumber, bool isTeacher)
    {
        Role role = Role.STUDENT;

        if (isTeacher)
        {
            role = Role.TEACHER;
        }

        SignUpData registerData = new SignUpData
        {
            role = role,
            school = school,
            grade = grade,
            classRoom = className,
            studentNumber = studentNumber,
            name = username,
            password = password
        };

        string jsonBody = JsonUtility.ToJson(registerData);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);

        // 서버 요청 설정
        using (UnityWebRequest webRequest = new UnityWebRequest(mainServer + RegisterUrl, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend); // 로우 데이터 업로드
            webRequest.downloadHandler = new DownloadHandlerBuffer(); // 서버가 다운로드 할 수 있는 공간 만듦
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버 응답 처리
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("회원가입 성공: " + webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError("회원가입 실패: " + webRequest.error);
            }
        }
    }

    public string logInUrl = "api/user/login";
    public bool isTeacher;
    public GameObject img_background;
    public IEnumerator LogInCoroutine(string username, string password)
    {

        SignUpData registerData = new SignUpData
        {
            name = username,
            password = password
        };

        string jsonBody = JsonUtility.ToJson(registerData);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);

        // 서버 요청 설정
        using (UnityWebRequest webRequest = new UnityWebRequest(mainServer + logInUrl, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend); // 로우 데이터 업로드
            webRequest.downloadHandler = new DownloadHandlerBuffer(); // 서버가 다운로드 할 수 있는 공간 만듦
            webRequest.SetRequestHeader("Content-Type", "application/json");
            //webRequest.SetRequestHeader("userId", userId);

            // 서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            // 서버 응답 처리
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                isLoggedIn = true;
                //Debug.Log("로그인 성공: " + webRequest.downloadHandler.text);
                ResponseData responseData = JsonUtility.FromJson<ResponseData>(webRequest.downloadHandler.text);
                userId = responseData.userId;
                print(userId);

                Role userRole;
                Enum.TryParse(responseData.role, true, out userRole);
                isTeacher = userRole == Role.TEACHER;
                print(isTeacher);

                /////////// HZ
                //// 로그인 UI 꺼주기
                //Y_SignUp.signUp.logInUI.SetActive(false);

                ///// HZ
                //ReturnLobby();
               
                //// 선생님이라면
                //if (isTeacher)
                //{
                //    Y_SignUp.signUp.creatorUI.SetActive(true);
                //    GameObject.Find("Canvas_CreatorTool").GetComponent<P_CreatorToolController>().titleNickname.text = registerData.name;
                //    img_background.SetActive(false);
                //}
                //// 학생이라면
                //else
                //{
                //    Y_SignUp.signUp.titleUI.SetActive(true);
                //    Y_SignUp.signUp.titleNickname.text = registerData.name;
                //}
                //SceneManager.
                //(1);
            }
            else
            {
                Debug.LogError("로그인 실패: " + webRequest.error);

            }
        }
    }


    public void ReturnLobby()
    {
        Y_SignUp.signUp.logInUI.SetActive(false);

        if (isTeacher)
        {
            Y_SignUp.signUp.creatorUI.SetActive(true);
            img_background.SetActive(false);
        }
        else
        {
            Y_SignUp.signUp.titleUI.SetActive(true);
        }
    }
}
