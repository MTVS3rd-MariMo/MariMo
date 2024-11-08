using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class P_CreatorToolController : MonoBehaviour
{
    public GameObject panel_SelectStory;
    public GameObject panel_CreateRoom;
    public GameObject panel_Library;
    public GameObject panel_NewStory;
    public GameObject panel_FileViewer;
    public GameObject panel_Making;
    public GameObject panel_SelectQuiz;
    public GameObject panel_MakingQuiz;
    public GameObject panel_MakingAsk;

    public Button btn_SelectStory;
    public Sprite[] sp_SelectStory;
    public Button btn_CreateRoom;
    public Sprite[] sp_CreateRoom;
    public Button btn_Library;
    public Sprite[] sp_Library;
    public TMP_Text[] text_Storys;
    public Button[] btn_Story;
    public Button btn_Back_NewStory;
    public Button btn_SerchPDF;
    public Button btn_Checking;
    public Button btn_Back_SerchPDF;
    public Button btn_SendPDF;
    public Button btn_SelectQuiz;



    void Start()
    {
        P_CreatorToolConnectMgr.Instance.OnDataParsed += SendFinish;

        btn_SelectStory.onClick.AddListener(OnclickSelectStory);
        btn_CreateRoom.onClick.AddListener(OnclickCreateRoom);
        btn_Library.onClick.AddListener(OnclickLibrary);
        btn_Back_NewStory.onClick.AddListener(OnclickBack_NewStory);
        btn_SerchPDF.onClick.AddListener(OnclickSerchPDF);
        btn_SendPDF.onClick.AddListener(OnclickSend);
        btn_CreateRoom.onClick.AddListener(OnclickCreateRoom);
        btn_Checking.onClick.AddListener(OnclickChecking);
        btn_SelectQuiz.onClick.AddListener(OnclickSelectComplete);


        btn_SelectStory.image.sprite = sp_SelectStory[1];

        LessonUpdate();
    }

    private void Update()
    {
        if (P_CreatorToolConnectMgr.Instance.pdfPath != null)
            btn_SendPDF.interactable = true;
        else
            btn_SendPDF.interactable = false;
    }

    public void OnclickSelectStory()
    {
        // 이전 작업들 끄기
        panel_MakingAsk.SetActive(false);
        panel_MakingQuiz.SetActive(false);
        panel_Making.SetActive(false);
        panel_FileViewer.SetActive(false);
        panel_NewStory.SetActive(false);
        panel_Making.SetActive(false);
        panel_MakingAsk.SetActive(false);
        panel_MakingQuiz.SetActive(false);

        panel_SelectStory.SetActive(true);
        btn_SelectStory.image.sprite = sp_SelectStory[1];
        panel_CreateRoom.SetActive(false);
        btn_CreateRoom.image.sprite = sp_CreateRoom[0];
        panel_Library.SetActive(false);
        btn_Library.image.sprite = sp_Library[0];

        LessonUpdate();
    }

    public void OnclickCreateRoom()
    {
        // 이전 작업들 끄기
        panel_Making.SetActive(false);
        panel_FileViewer.SetActive(false);
        panel_NewStory.SetActive(false);

        panel_SelectStory.SetActive(false);
        btn_SelectStory.image.sprite = sp_SelectStory[0];
        panel_CreateRoom.SetActive(true);
        btn_CreateRoom.image.sprite = sp_CreateRoom[1];
        panel_Library.SetActive(false);
        btn_Library.image.sprite = sp_Library[0];
    }

    public void OnclickLibrary()
    {
        // 이전 작업들 끄기
        panel_Making.SetActive(false);
        panel_FileViewer.SetActive(false);
        panel_NewStory.SetActive(false);

        panel_SelectStory.SetActive(false);
        btn_SelectStory.image.sprite = sp_SelectStory[0];
        panel_CreateRoom.SetActive(false);
        btn_CreateRoom.image.sprite = sp_CreateRoom[0];
        panel_Library.SetActive(true);
        btn_Library.image.sprite = sp_Library[1];
    }

    public void OnclickStory()
    {
        panel_NewStory.SetActive(true);
    }

    public void OnclickBack_NewStory()
    {
        panel_NewStory.SetActive(false);
    }

    public void OnclickSerchPDF()
    {
        panel_FileViewer.SetActive(true);
    }

    public void OnclickBack_SerchPDF()
    {
        panel_FileViewer.SetActive(false);
    }

    public void OnclickChecking()
    {
        panel_SelectQuiz.SetActive(true);

        panel_SelectQuiz.GetComponent<P_QuizSelect>().QuizSetting();
    }

    public void OnclickSend()
    {
        ////// 더미데이터용
        //P_CreatorToolConnectMgr.Instance.quizData = P_CreatorToolConnectMgr.Instance.dummydata;

        //btn_SendPDF.interactable = false;

        //panel_Making.SetActive(true);
        /////////


        print(P_CreatorToolConnectMgr.Instance.pdfPath);

        HttpInfo info = new HttpInfo();
        // api/lesson-material/upload-pdf 앤드포인트..?
        info.url = P_CreatorToolConnectMgr.Instance.url_Front + "/api/lesson-material/upload-pdf";
        info.body = P_CreatorToolConnectMgr.Instance.pdfPath;
        info.contentType = "multipart/form-data";
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            try
            {
                // QuizManager의 ParseQuizData 메서드 호출하여 데이터 파싱
                P_CreatorToolConnectMgr.Instance.ParseQuizData(downloadHandler.text);

                // 데이터 로드 완료 후 처리할 작업이 있다면 여기에 추가
                Debug.Log("퀴즈 데이터 로드 완료!");
                Debug.Log($"로드된 퀴즈 개수: {P_CreatorToolConnectMgr.Instance.GetQuizCount()}");
                Debug.Log($"로드된 주관식 문제 개수: {P_CreatorToolConnectMgr.Instance.GetOpenQuestionCount()}");


            }
            catch (Exception e)
            {
                Debug.LogError($"JSON 파싱 중 에러 발생: {e.Message}");
            }
        };

        btn_SendPDF.interactable = false;

        StartCoroutine(HttpManager.GetInstance().UploadFileByFormDataPDF(info));

    }

    // 데이터 로드 완료시 실행
    private void SendFinish()
    {
        
        panel_Making.SetActive(true);
    }
     

    public void OnFinishMaking(QuizData quizData)
    {
        HttpInfo info = new HttpInfo();
        info.url = P_CreatorToolConnectMgr.Instance.url_Front + "/api/lesson-material";
        info.body = JsonUtility.ToJson(quizData);
        info.contentType = "application/json";
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            // 완료시 실행
            LessonUpdate();
        };

        StartCoroutine(HttpManager.GetInstance().Put(info));

    }

    // 선택한 퀴즈 데이터 전송
    public void OnclickSelectComplete()
    {
        // 선택한 퀴즈가 아닌건 삭제
        P_CreatorToolConnectMgr.Instance.DeleteQuiz(P_CreatorToolConnectMgr.Instance.selectedID);

        panel_MakingQuiz.SetActive(true);

        // 디버깅
        Debug.Log(P_CreatorToolConnectMgr.Instance.GetQuiz(0).question);

        panel_MakingQuiz.GetComponent<P_MakingQuiz>().OpenQuizSet(0);

        panel_SelectQuiz.SetActive(false);
    }


    public void LessonUpdate()
    {
        HttpInfo info = new HttpInfo();
        info.url = P_CreatorToolConnectMgr.Instance.url_Front + "/api/lesson-material";
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            print(downloadHandler.text);

            try
            {
                // quizmanager의 parsequizdata 메서드 호출하여 데이터 파싱
                P_CreatorToolConnectMgr.Instance.ParseLessons(downloadHandler.text);

                // 데이터 로드 완료 후 처리할 작업이 있다면 여기에 추가
                Debug.Log("수업자료 갯수");
                Debug.Log(P_CreatorToolConnectMgr.Instance.lessons.lessonMaterials.Count);
                Debug.Log("수업자료 이름들");
                for(int i = 0; i < P_CreatorToolConnectMgr.Instance.lessons.lessonMaterials.Count; i++)
                {
                    Debug.Log(P_CreatorToolConnectMgr.Instance.lessons.lessonMaterials[i].bookTitle);
                }

                StoryButtonSet();

                panel_CreateRoom.GetComponent<P_RoomCreate>().DropDownUpdate();
            }
            catch (Exception e)
            {
                Debug.LogError($"json 파싱 중 에러 발생: {e.Message}");
            }
        };

        StartCoroutine(HttpManager.GetInstance().GetLesson(info));
    }

    void StoryButtonSet()
    {
        
        for (int i = 0; i < text_Storys.Length; i++)
        {
            text_Storys[i].text = "+";
            btn_Story[i].onClick.AddListener(OnclickStory);

            if (i < P_CreatorToolConnectMgr.Instance.lessons.lessonMaterials.Count)
            {
                text_Storys[i].text = P_CreatorToolConnectMgr.Instance.lessons.lessonMaterials[i].bookTitle;
                btn_Story[i].onClick.RemoveAllListeners();
            }
        }
    }
}
