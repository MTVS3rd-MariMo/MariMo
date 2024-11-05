using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Y_HttpLogIn : MonoBehaviour
{
    static Y_HttpLogIn instance;
    public string userId;

    public static Y_HttpLogIn GetInstance()
    {
        if (instance == null)
        {
            GameObject go = new GameObject();
            go.name = "HttpLogIn";
            go.AddComponent<Y_HttpLogIn>();
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

    public string RegisterUrl = "http://125.132.216.190:8202/api/user/signup"; ///////////////////

    public IEnumerator SignUpCoroutine(string username, string password, string school, int grade, int className, int studentNumber, bool isTeacher)
    {
        Role role;

        if (isTeacher)
        {
            role = Role.TEACHER;
        }
        else
        {
            role = Role.STUDENT;
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
        using (UnityWebRequest webRequest = new UnityWebRequest(RegisterUrl, "POST"))
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

    public string logInUrl = "http://125.132.216.190:8202/api/user/login";


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
        using (UnityWebRequest webRequest = new UnityWebRequest(logInUrl, "POST"))
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
                Debug.Log("로그인 성공: " + webRequest.downloadHandler.text);
                string jsonRaw = webRequest.downloadHandler.text;
                print(jsonRaw);
                userId = jsonRaw;
                print(userId);

                Y_SignUp.signUp.logInUI.SetActive(false);
                Y_SignUp.signUp.titleUI.SetActive(true);
                //SceneManager.LoadScene(1);
            }
            else
            {
                Debug.LogError("로그인 실패: " + webRequest.error);

            }
        }
    }
}
