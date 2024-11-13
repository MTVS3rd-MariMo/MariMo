using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public string[] participants;
    public OpenQuestions[] openQuestions;
    public HotSittings[] hotSittings;
    public string photoUrl;
    public Roles[] roles;
}

[Serializable]
public class OpenQuestions
{
    public string question;
    public ResultAnswer[] reultAnswers;
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



public class P_LibraryTeacher : MonoBehaviour
{
    public Transform contents;
    public GameObject Prefab_logTitle;

    public GameObject panel_log;
    public Button btn_CloseLog;

    public TeacherLibrary teacherLibrary;

    public AllData allData;

    void Start()
    {
        // 수업 기록 조회
        GetLogList();

        // 기록 조회 끝나면 실행
        
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
                btn_Album.GetComponent<Button>().onClick.AddListener(() => OnclickOpenButton(teacherLibrary.teacherResults[i].lessonId));

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
        };

        StartCoroutine(HttpManager.GetInstance().Get(info));
    }

    void ParseLogDetail(string jsonString)
    {
        try
        {
            allData = JsonUtility.FromJson<AllData>(jsonString);

            DebugingLog(allData);
        }
        catch (Exception e)
        {
            Debug.LogError($"결과 데이터 파싱 실패: {e.Message}");
            throw;
        }
    }

    void DebugingLog(AllData allData)
    {
        if (allData == null)
        {
            Debug.LogError("allData가 null입니다. 데이터 로드 실패.");
            return;
        }

        Debug.Log("=== AllData 정보 출력 시작 ===");

        // 기본 정보
        Debug.Log("책 제목: " + allData.bookTitle);
        Debug.Log("생성 날짜: " + allData.createdAt);
        Debug.Log("사진 URL: " + allData.photoUrl);

        // 참가자 정보
        if (allData.participants != null && allData.participants.Length > 0)
        {
            Debug.Log("참가자 목록:");
            foreach (string participant in allData.participants)
            {
                Debug.Log("- " + participant);
            }
        }
        else
        {
            Debug.Log("참가자 정보가 없습니다.");
        }

        // OpenQuestions 정보
        if (allData.openQuestions != null && allData.openQuestions.Length > 0)
        {
            Debug.Log("OpenQuestions 목록:");
            foreach (var question in allData.openQuestions)
            {
                Debug.Log("질문: " + question.question);
                if (question.reultAnswers != null && question.reultAnswers.Length > 0)
                {
                    foreach (var answer in question.reultAnswers)
                    {
                        Debug.Log(" - 사용자: " + answer.userName + ", 답변: " + answer.answer);
                    }
                }
                else
                {
                    Debug.Log(" - 답변이 없습니다.");
                }
            }
        }
        else
        {
            Debug.Log("OpenQuestions 정보가 없습니다.");
        }

        // HotSittings 정보
        if (allData.hotSittings != null && allData.hotSittings.Length > 0)
        {
            Debug.Log("HotSittings 목록:");
            foreach (var sitting in allData.hotSittings)
            {
                Debug.Log("자기소개: " + sitting.selfIntroduce);
                if (sitting.questionAnswers != null && sitting.questionAnswers.Length > 0)
                {
                    foreach (var answer in sitting.questionAnswers)
                    {
                        Debug.Log(" - 질문 답변: " + answer);
                    }
                }
                else
                {
                    Debug.Log(" - 질문 답변이 없습니다.");
                }
            }
        }
        else
        {
            Debug.Log("HotSittings 정보가 없습니다.");
        }

        // Roles 정보
        if (allData.roles != null && allData.roles.Length > 0)
        {
            Debug.Log("Roles 목록:");
            foreach (var role in allData.roles)
            {
                Debug.Log("사용자 이름: " + role.userName);
                Debug.Log("캐릭터: " + role.character);
                Debug.Log("아바타 URL: " + role.avatarUrl);
            }
        }
        else
        {
            Debug.Log("Roles 정보가 없습니다.");
        }

        Debug.Log("=== AllData 정보 출력 종료 ===");
    }

    void SetLogDetail(AllData allData)
    {

    }
}
