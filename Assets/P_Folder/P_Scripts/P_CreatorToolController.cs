using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;

public class P_CreatorToolController : MonoBehaviour
{
    public GameObject panel_SelectStory;
    public GameObject panel_CreateRoom;
    public GameObject panel_Library;
    public GameObject panel_NewStory;
    public GameObject panel_SelectPDF;
    public GameObject panel_FileViewer;
    public GameObject panel_Making;
    public GameObject panel_SelectQuiz;
    public GameObject panel_MakingQuiz;
    public GameObject panel_MakingAsk;
    public GameObject panel_Option;

    public Button btn_SelectStory;
    public Button btn_CreateRoom;
    public Button btn_Library;
    public Button[] btn_Story;
    public Button[] btn_DeleteStory;
    public Button btn_Back_NewStory;
    public Button btn_NamingDone;
    public Button btn_Back_SelectPDF;
    public Button btn_SerchPDF;
    public Button btn_Checking;
    public Button btn_Back_SerchPDF;
    public Button btn_SendPDF;
    public Button btn_SelectQuiz;
    public Button btn_Option;


    public TMP_Text titleNickname;

    public Sprite[] sp_SelectStory;
    public Sprite[] sp_CreateRoom;
    public Sprite[] sp_Library;

    public TMP_Text[] text_Storys;

    public TMP_InputField text_bookName;
    public TMP_InputField text_bookWriter;

    bool isSend = false;

    // 인풋필드 사이즈 조정
    public Vector2 expandedSize = new Vector2(1200, 200); // 확장된 크기
    public Vector2 expandedPos = new Vector2(-345, 180); // 확장됐을 때 위치
    private Vector2 originalSize;
    private Vector2 originalPosition;

    void Start()
    {
        P_CreatorToolConnectMgr.Instance.OnDataParsed += SendFinish;

        btn_SelectStory.onClick.AddListener(OnclickSelectStory);
        btn_CreateRoom.onClick.AddListener(OnclickCreateRoom);
        btn_Library.onClick.AddListener(OnclickLibrary);
        btn_NamingDone.onClick.AddListener(OnclickNamingDone);
        btn_Back_NewStory.onClick.AddListener(OnclickBack_NewStory);
        btn_Back_SelectPDF.onClick.AddListener(OnclickBack_SelectPDF);
        btn_SerchPDF.onClick.AddListener(OnclickSerchPDF);
        btn_SendPDF.onClick.AddListener(OnclickSend);
        btn_CreateRoom.onClick.AddListener(OnclickCreateRoom);
        btn_Checking.onClick.AddListener(OnclickChecking);
        btn_SelectQuiz.onClick.AddListener(OnclickSelectComplete);
        btn_Option.onClick.AddListener(OnclickOption);


        btn_SelectStory.image.sprite = sp_SelectStory[1];

        // InputField 이벤트 연결
        AddInputFieldEventTriggers(text_bookName);
        AddInputFieldEventTriggers(text_bookWriter);

        LessonUpdate();
    }

    private void Update()
    {
        if (P_CreatorToolConnectMgr.Instance.pdfPath != null && !isSend)
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
        panel_SelectPDF.SetActive(false);
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
        panel_SelectPDF.SetActive(false);

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
        panel_SelectPDF.SetActive(false);

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

    public void OnclickNamingDone()
    {
        panel_SelectPDF.SetActive(true);
    }

    public void OnclickBack_SelectPDF()
    {
        panel_SelectPDF.SetActive(false);
    }

    public void OnclickSerchPDF()
    {
        panel_FileViewer.SetActive(true);
    }

    public void OnclickBack_NewStory()
    {
        panel_NewStory.SetActive(false);
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

    public void OnclickOption()
    {
        panel_Option.SetActive(true);
    }

    public void OnclickSend()
    {
        ////// 더미데이터용
        //P_CreatorToolConnectMgr.Instance.quizData = P_CreatorToolConnectMgr.Instance.dummydata;

        //btn_SendPDF.interactable = false;

        //panel_Making.SetActive(true);
        /////////
        isSend = true;

        print(P_CreatorToolConnectMgr.Instance.pdfPath);

        HttpInfo info = new HttpInfo();
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

        StartCoroutine(HttpManager.GetInstance().UploadFileByFormDataPDF(info, text_bookName.text, text_bookWriter.text));

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

            P_CreatorToolConnectMgr.Instance.InitializeConnectMgr();

            // 그림그리기 요청
            MakingBackGround(quizData);
        };

        StartCoroutine(HttpManager.GetInstance().Put(info));

    }

    public void MakingBackGround(QuizData quizData)
    {
        HttpInfo info = new HttpInfo();
        info.url = P_CreatorToolConnectMgr.Instance.url_Front + "/api/photo/background/" + quizData.lessonMaterialId;
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            Debug.Log("그림그리기 요청 완료");
        };
        StartCoroutine(HttpManager.GetInstance().PostBackGround(info));
    }

    // 선택한 퀴즈 데이터 전송
    public void OnclickSelectComplete()
    {
        // 선택한 퀴즈가 아닌건 삭제
        P_CreatorToolConnectMgr.Instance.DeleteQuiz(P_CreatorToolConnectMgr.Instance.selectedID);

        panel_MakingQuiz.SetActive(true);


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
            btn_Story[i].onClick.RemoveAllListeners();

            text_Storys[i].text = "+";
            btn_Story[i].onClick.AddListener(OnclickStory);
            btn_DeleteStory[i].onClick.RemoveAllListeners();

            if (i < P_CreatorToolConnectMgr.Instance.lessons.lessonMaterials.Count)
            {
                text_Storys[i].text = P_CreatorToolConnectMgr.Instance.lessons.lessonMaterials[i].bookTitle;

                switch (i)
                {
                    case 0:
                        btn_Story[0].onClick.AddListener(() => OnclickFixStory(0));
                        btn_DeleteStory[0].onClick.AddListener(() => OnclickDeleteStory(0));
                        break;
                    case 1:
                        btn_Story[1].onClick.AddListener(() => OnclickFixStory(1));
                        btn_DeleteStory[1].onClick.AddListener(() => OnclickDeleteStory(1));
                        break;
                    case 2:
                        btn_Story[2].onClick.AddListener(() => OnclickFixStory(2));
                        btn_DeleteStory[2].onClick.AddListener(() => OnclickDeleteStory(2));
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void OnclickDeleteStory(int index)
    {
        HttpInfo info = new HttpInfo();
        info.url = P_CreatorToolConnectMgr.Instance.url_Front + "/api/lesson-material/" + P_CreatorToolConnectMgr.Instance.lessons.lessonMaterials[index].lessonMaterialId;
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            Debug.Log("수업자료 삭제 요청 완료");

            LessonUpdate();
        };

        StartCoroutine(HttpManager.GetInstance().Delete(info));

    }

    void OnclickFixStory(int index)
    {
        HttpInfo info = new HttpInfo();
        info.url = P_CreatorToolConnectMgr.Instance.url_Front + "/api/lesson-material/detail/" + P_CreatorToolConnectMgr.Instance.lessons.lessonMaterials[index].lessonMaterialId;
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

                // 바로 퀴즈 수정 창으로 이동
                panel_SelectPDF.SetActive(true);
                panel_MakingQuiz.SetActive(true);
                panel_MakingQuiz.GetComponent<P_MakingQuiz>().OpenQuizSet(0);
            }
            catch (Exception e)
            {
                Debug.LogError($"JSON 파싱 중 에러 발생: {e.Message}");
            }
        };


        StartCoroutine(HttpManager.GetInstance().Get(info));
    }

    // 이벤트 트리거 추가 함수 ( 공부 필요 )
    void AddInputFieldEventTriggers(TMP_InputField inputField)
    {
        EventTrigger trigger = inputField.gameObject.AddComponent<EventTrigger>();

        // OnSelect 이벤트 추가
        EventTrigger.Entry selectEntry = new EventTrigger.Entry { eventID = EventTriggerType.Select };
        selectEntry.callback.AddListener((eventData) =>
            OnSelectInputField(inputField.GetComponent<RectTransform>())
        );
        trigger.triggers.Add(selectEntry);

        // OnDeselect 이벤트 추가
        EventTrigger.Entry deselectEntry = new EventTrigger.Entry { eventID = EventTriggerType.Deselect };
        deselectEntry.callback.AddListener((eventData) =>
            OnDeselectInputField(inputField.GetComponent<RectTransform>())
        );
        trigger.triggers.Add(deselectEntry);
    }

    // InputField가 선택되었을 때 호출
    public void OnSelectInputField(RectTransform inputFieldRect)
    {
        // 선택된 InputField의 원래 크기와 위치를 저장
        originalSize = inputFieldRect.sizeDelta;
        originalPosition = inputFieldRect.gameObject.transform.localPosition;

        // 크기와 위치를 확장
        inputFieldRect.sizeDelta = expandedSize;
        inputFieldRect.gameObject.transform.localPosition = expandedPos;
    }

    // InputField가 선택 해제되었을 때 호출
    public void OnDeselectInputField(RectTransform inputFieldRect)
    {
        // 원래 크기와 위치로 복원
        inputFieldRect.sizeDelta = originalSize;
        inputFieldRect.gameObject.transform.localPosition = originalPosition;
    }
}
