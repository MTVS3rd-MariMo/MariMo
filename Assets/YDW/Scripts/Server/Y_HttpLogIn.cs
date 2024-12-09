using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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
    public string mainServer = "http://211.250.74.75:8202/";

    public bool isLoggedIn = false;

    public string userNickName;

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
        //mainServer = "http://3.36.39.119:80/";
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
    public GameObject signUpError;
    public GameObject signUpUI;
    public GameObject logInUI;

    // 서버 응답 데이터를 위한 클래스 정의
    [System.Serializable]
    public class ServerErrorResponse
    {
        public string message;
        public int status;
        public string error;
    }

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

                signUpUI.SetActive(false);
                logInUI.SetActive(true);
            }
            else
            {
                // 서버 응답 메시지 처리
                string responseText = webRequest.downloadHandler.text;
                var responseJson = JsonUtility.FromJson<ServerErrorResponse>(responseText);

                if (webRequest.responseCode == 500 && responseJson.message.Contains("유저가 이미 존재합니다.")) // && "유저가 이미 존재합니다" 일 때
                {
                    signUpError.SetActive(true);
                    yield return new WaitForSeconds(2f);
                    signUpError.SetActive(false);
                }
            }
        }
    }

    public Sprite[] logInErrorsSprite;
    public GameObject logInError;

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

                userNickName = registerData.name;

                Role userRole;
                Enum.TryParse(responseData.role, true, out userRole);
                isTeacher = userRole == Role.TEACHER;
                print(isTeacher);

                Y_SignUp.signUp.logInUI.SetActive(false);

                if (isTeacher)
                {
                    Y_SignUp.signUp.creatorUI.SetActive(true);
                    GameObject.Find("Canvas_CreatorTool").GetComponent<P_CreatorToolController>().titleNickname.text = userNickName;
                    img_background.SetActive(false);
                }
                else
                {
                    Y_SignUp.signUp.titleUI.SetActive(true);
                    Y_SignUp.signUp.titleNickname.text = userNickName;
                }
                //SceneManager.
                //(1);
            }
            else
            {
                if (webRequest.responseCode == 401)
                {
                    logInError.GetComponent<Image>().sprite = logInErrorsSprite[0];
                }
                else if ((webRequest.result == UnityWebRequest.Result.ConnectionError) || (webRequest.result == UnityWebRequest.Result.ProtocolError))
                {
                    logInError.GetComponent<Image>().sprite = logInErrorsSprite[0];
                }
                else
                {
                    logInError.GetComponent<Image>().sprite = logInErrorsSprite[0];
                }

                logInError.SetActive(true);
                yield return new WaitForSeconds(2f);
                logInError.SetActive(false);

            }
        }
    }


}
