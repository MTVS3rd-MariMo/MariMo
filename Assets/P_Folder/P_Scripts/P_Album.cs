using Photon.Pun.UtilityScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


// 학생 기록조회용
[Serializable]
public class StudentResultsWrapper
{
    public StudentResult[] studentResults;
}

[Serializable]
public class StudentResult
{
    public string bookTitle;
    public string photo;
}


public class P_Album : MonoBehaviour
{
    public StudentResultsWrapper results = new StudentResultsWrapper();

    public Button[] buttons;
    public TMP_Text[] texts;

    void Start()
    {
        GetResultsList();
    }

    void Update()
    {
        
    }


    void GetResultsList()
    {
        HttpInfo info = new HttpInfo();
        info.url = P_CreatorToolConnectMgr.Instance.url_Front + "/api/result/student";
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            print(downloadHandler.text);

            // 파싱 함수 호출
            ParseResults(downloadHandler.text);


            // 파싱 끝나고 책제목 적용
            ButtonSet();

        };

        StartCoroutine(HttpManager.GetInstance().Get(info));
    }



    public void ParseResults(string jsonString)
    {
        try
        {
            results = JsonUtility.FromJson<StudentResultsWrapper>(jsonString);

            Debug.Log("results.studentResults.Length :" + results.studentResults.Length);
            Debug.Log("results.studentResults[0] :" + results.studentResults[0]);
            Debug.Log("results.studentResults[0].bookTitle / results.studentResults[0].url :" + results.studentResults[0].bookTitle + " / " + results.studentResults[0].photo);

        }
        catch (Exception e)
        {
            Debug.LogError($"결과 데이터 파싱 실패: {e.Message}");
            throw;
        }
    }

    public void ButtonSet()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            texts[i].text = "+";
            buttons[i].onClick.RemoveAllListeners();

            if (i < results.studentResults.Length)
            {
                texts[i].text = results.studentResults[i].bookTitle;
                //buttons[i].onClick.AddListener(OnclickStory);
            }
        }
    }

    
}
