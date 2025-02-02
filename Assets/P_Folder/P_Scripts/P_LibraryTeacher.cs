﻿using iTextSharp.text.pdf;
using iTextSharp.text;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


// 수업 기록 리스트 조회용
[Serializable]
public class TeacherLibrary
{
    public Results[] teacherResults;
}

[Serializable]
public class Results
{
    public string bookTitle;
    public string[] participantList;
    public string createdAt;
    public int lessonId;
}


// 수업 기록 디테일 조회용
[Serializable]
public class AllData
{
    public string bookTitle;
    public string createdAt;
    public OpenQuestions[] openQuestions;
    public HotSittings[] hotSittings;
    public Roles[] roles;
}

[Serializable]
public class OpenQuestions
{
    public string question;
    public ResultAnswer[] resultAnswers;
}

[Serializable]
public class ResultAnswer
{
    public string userName;
    public string answer;
}

[Serializable]
public class HotSittings
{
    public string userName;
    public string character;
    public string selfIntroduce;
    public string[] questionAnswers;
}

[Serializable]
public class Roles
{
    public string userName;
    public string character;
    public string avatarUrl;
}


public class PDFCreator
{
    public void CreatePDF(string textContent, string filePath)
    {
        // PDF 문서 생성
        Document pdfDoc = new Document();

        try
        {


            // PDF 파일 작성
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                string fontPath = Path.Combine(Application.streamingAssetsPath, "MALGUN.ttf");
                BaseFont koreanFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                iTextSharp.text.Font font = new iTextSharp.text.Font(koreanFont, 12, iTextSharp.text.Font.NORMAL);


                // 텍스트 추가
                pdfDoc.Add(new Paragraph(textContent, font));

                pdfDoc.Close();
            }

            Debug.Log($"PDF 파일 생성 완료: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"PDF 생성 중 오류 발생: {e.Message}");
        }
    }
}


public class P_LibraryTeacher : MonoBehaviour
{
    public Transform contents;
    public GameObject Prefab_logTitle;

    public GameObject panel_log;
    public TMP_Text text_Log;
    public Button btn_CloseLog;
    public Button btn_MakePDF;

    public TeacherLibrary teacherLibrary;

    public AllData allData;

    void Start()
    {
        // 수업 기록 조회
        GetLogList();

        btn_CloseLog.onClick.AddListener(OnclickCloseLog);
        btn_MakePDF.onClick.AddListener(OnclickMakePDF);
    }

    void GetLogList()
    {
        HttpInfo info = new HttpInfo();
        info.url = P_CreatorToolConnectMgr.Instance.url_Front + "/api/result/teacher";
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            print(downloadHandler.text);

            // 파싱 함수 호출
            ParseLogList(downloadHandler.text);


            // 파싱 끝나고 책제목 적용
            BtnSetting();
        };

        StartCoroutine(HttpManager.GetInstance().Get(info));
    }


    void ParseLogList(string jsonString)
    {
        try
        {
            teacherLibrary = JsonUtility.FromJson<TeacherLibrary>(jsonString);
            Debug.Log("파싱 완료 : " + teacherLibrary.teacherResults.Length);
            Debug.Log("책 제목 : " + teacherLibrary.teacherResults[0].bookTitle);
            Debug.Log("참가자 : " + teacherLibrary.teacherResults[0].participantList);
            Debug.Log("생성 날짜 : " + teacherLibrary.teacherResults[0].createdAt);
            Debug.Log("레슨 아이디 : " + teacherLibrary.teacherResults[0].lessonId);
        }
        catch (Exception e)
        {
            Debug.LogError($"결과 데이터 파싱 실패: {e.Message}");
            throw;
        }
    }


    void BtnSetting()
    {
        if (teacherLibrary.teacherResults.Length < 3)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject btn_Album = Instantiate(Prefab_logTitle, contents);
                btn_Album.GetComponent<P_LogInfo>().SetBook("");
                btn_Album.GetComponent<P_LogInfo>().SetDate("");

                if (i < teacherLibrary.teacherResults.Length)
                {
                    btn_Album.GetComponent<P_LogInfo>().SetBook(teacherLibrary.teacherResults[i].bookTitle);
                    btn_Album.GetComponent<P_LogInfo>().SetPlayers(teacherLibrary.teacherResults[i].participantList);
                    btn_Album.GetComponent<P_LogInfo>().SetDate(teacherLibrary.teacherResults[i].createdAt);
                    btn_Album.GetComponent<P_LogInfo>().SetLessonId(teacherLibrary.teacherResults[i].lessonId);
                    btn_Album.GetComponent<Button>().onClick.AddListener(() => OnclickOpenButton(btn_Album.GetComponent<P_LogInfo>().GetLessonId()));
                }
            }
        }
        else
        {
            for (int i = 0; i < teacherLibrary.teacherResults.Length; i++)
            {
                GameObject btn_Album = Instantiate(Prefab_logTitle, contents);

                btn_Album.GetComponent<P_LogInfo>().SetBook(teacherLibrary.teacherResults[i].bookTitle);
                btn_Album.GetComponent<P_LogInfo>().SetPlayers(teacherLibrary.teacherResults[i].participantList);
                btn_Album.GetComponent<P_LogInfo>().SetDate(teacherLibrary.teacherResults[i].createdAt);
                btn_Album.GetComponent<P_LogInfo>().SetLessonId(teacherLibrary.teacherResults[i].lessonId);
                btn_Album.GetComponent<Button>().onClick.AddListener(() => OnclickOpenButton(btn_Album.GetComponent<P_LogInfo>().GetLessonId()));

            }
        }
    }


    void OnclickOpenButton(int lessonId)
    {
        // 기록 상세 조회
        HttpInfo info = new HttpInfo();
        info.url = P_CreatorToolConnectMgr.Instance.url_Front + "/api/result/teacher/" + lessonId;
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            print(downloadHandler.text);

            // 파싱 함수 호출
            ParseLogDetail(downloadHandler.text);


            // 파싱 끝나고 데이터 입력
            SetLogDetail(allData);

            panel_log.SetActive(true);
        };

        StartCoroutine(HttpManager.GetInstance().Get(info));
    }

    void ParseLogDetail(string jsonString)
    {
        try
        {
            allData = JsonUtility.FromJson<AllData>(jsonString);
        }
        catch (Exception e)
        {
            Debug.LogError($"결과 데이터 파싱 실패: {e.Message}");
            throw;
        }
    }


    void SetLogDetail(AllData allData)
    {
        if (allData == null)
        {
            Debug.LogError("allData가 null입니다. 데이터 로드 실패.");
            return;
        }


        // 기본 정보
        text_Log.text = ("책 제목: " + allData.bookTitle);
        text_Log.text += "\n" + ("생성 날짜: " + allData.createdAt);


        // Roles 정보
        if (allData.roles != null && allData.roles.Length > 0)
        {
            text_Log.text += "\n\n" + ("Roles 목록:");
            foreach (var role in allData.roles)
            {
                text_Log.text += "\n\n" + ("사용자 이름: " + role.userName);
                text_Log.text += "\n" + ("캐릭터: " + role.character);
            }
        }
        else
        {
            text_Log.text += "\n\n" + ("Roles 정보가 없습니다.");
        }

        // OpenQuestions 정보
        if (allData.openQuestions != null && allData.openQuestions.Length > 0)
        {
            text_Log.text += "\n\n" + ("OpenQuestions 목록:");
            foreach (var question in allData.openQuestions)
            {
                text_Log.text += "\n\n" + ("질문: " + question.question);
                if (question.resultAnswers != null && question.resultAnswers.Length > 0)
                {
                    foreach (var answer in question.resultAnswers)
                    {
                        text_Log.text += "\n\n" + (" - 작성자: " + answer.userName + ", \n답변: " + answer.answer);
                    }
                }
                else
                {
                    text_Log.text += "\n\n" + (" - 답변이 없습니다.");
                }
            }
        }
        else
        {
            text_Log.text += "\n\n" + ("OpenQuestions 정보가 없습니다.");
        }

        // HotSittings 정보
        if (allData.hotSittings != null && allData.hotSittings.Length > 0)
        {
            int k = 0;

            text_Log.text += "\n\n" + ("HotSittings 목록:");
            foreach (var sitting in allData.hotSittings)
            {
                text_Log.text += "\n\n" + ("자기소개: " + sitting.selfIntroduce);
                if (sitting.questionAnswers != null && sitting.questionAnswers.Length > 0)
                {
                    foreach (var answer in sitting.questionAnswers)
                    {
                        if (k % 2 == 0)
                        {
                            text_Log.text += "\n\n" + (" - 질문\n " + answer);
                        }
                        else
                        {
                            text_Log.text += "\n\n" + (" - 답변\n " + answer);
                        }
                        k++;
                    }
                }
                else
                {
                    text_Log.text += "\n\n" + (" - 질문 / 답변이 없습니다.");
                }
            }
        }
        else
        {
            text_Log.text += "\n\n" + ("HotSittings 정보가 없습니다.");
        }

        
    }

    void OnclickCloseLog()
    {
        panel_log.SetActive(false);
    }

    

    public void OnclickMakePDF()
    {
        string content = text_Log.text;
        string filePath = Application.persistentDataPath + "/GeneratedFile.pdf";

        PDFCreator pdfCreator = new PDFCreator();
        pdfCreator.CreatePDF(content, filePath);
    }
}
