﻿using System;
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
    public GameObject panel_MakingQuiz;
    public GameObject panel_MakingAsk;

    public Button btn_SelectStory;
    public Button btn_CreateRoom;
    public Button btn_Library;
    public Button btn_Story1;
    public Button btn_Story2;
    public Button btn_Story3;
    public Button btn_Back_NewStory;
    public Button btn_SerchPDF;
    public Button btn_Back_SerchPDF;
    public Button btn_SendPDF;

    public TMP_Dropdown dropdown;

    string url_Front = "http://192.168.0.44:8899";


    void Start()
    {
        btn_SelectStory.onClick.AddListener(OnclickSelectStory);
        btn_CreateRoom.onClick.AddListener(OnclickCreateRoom);
        btn_Library.onClick.AddListener(OnclickLibrary);
        btn_Story1.onClick.AddListener(OnclickStory1);
        btn_Story2.onClick.AddListener(OnclickStory2);
        btn_Story3.onClick.AddListener(OnclickStory3);
        btn_Back_NewStory.onClick.AddListener(OnclickBack_NewStory);
        btn_SerchPDF.onClick.AddListener(OnclickSerchPDF);
        btn_SendPDF.onClick.AddListener(OnclickSend);
        btn_CreateRoom.onClick.AddListener(OnclickCreateRoom);



        BtnColor(Color.blue, btn_SelectStory);
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

        panel_SelectStory.SetActive(true);
        BtnColor(Color.blue, btn_SelectStory);
        panel_CreateRoom.SetActive(false);
        BtnColor(Color.white, btn_CreateRoom);
        panel_Library.SetActive(false);
        BtnColor(Color.white, btn_Library);
    }

    public void OnclickCreateRoom()
    {
        // 이전 작업들 끄기
        panel_Making.SetActive(false);
        panel_FileViewer.SetActive(false);
        panel_NewStory.SetActive(false);

        panel_SelectStory.SetActive(false);
        BtnColor(Color.white, btn_SelectStory);
        panel_CreateRoom.SetActive(true);
        BtnColor(Color.blue, btn_CreateRoom);
        panel_Library.SetActive(false);
        BtnColor(Color.white, btn_Library);
    }

    public void OnclickLibrary()
    {
        // 이전 작업들 끄기
        panel_Making.SetActive(false);
        panel_FileViewer.SetActive(false);
        panel_NewStory.SetActive(false);

        panel_SelectStory.SetActive(false);
        BtnColor(Color.white, btn_SelectStory);
        panel_CreateRoom.SetActive(false);
        BtnColor(Color.white, btn_CreateRoom);
        panel_Library.SetActive(true);
        BtnColor(Color.blue, btn_Library);
    }

    public void OnclickStory1()
    {
        panel_NewStory.SetActive(true);
    }
    public void OnclickStory2()
    {
        panel_NewStory.SetActive(true);
    }
    public void OnclickStory3()
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

    public void OnclickSend()
    {
        HttpInfo info = new HttpInfo();
        // api/lesson-material/upload-pdf 앤드포인트..?
        info.url = url_Front + "/api/lesson-material/upload-pdf";
        info.body = P_CreatorToolConnectMgr.Instance.pdfPath;
        info.contentType = "multipart/form-data";
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            try
            {
                // 응답 데이터 로그 출력 (디버그용)
                Debug.Log("서버 응답: " + downloadHandler.text);

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

        StartCoroutine(HttpManager.GetInstance().UploadFileByFormDataPDF(info));

        // 생성중 UI 뜨고 생성 완료시 이동
        panel_Making.SetActive(true);
    }





    void BtnColor(Color color, Button button)
    {

        ColorBlock colorBlock = button.colors;

        colorBlock.normalColor = color;
        colorBlock.highlightedColor = color;
        colorBlock.selectedColor = color;

        button.colors = colorBlock;

    }
}
