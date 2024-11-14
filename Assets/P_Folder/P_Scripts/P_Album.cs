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
    public GameObject panel_Photo;
    public Image Img_Album;
    public Button btn_ClosePhoto;

    public GameObject Prefab_Album;
    public Transform contents;

    public StudentResultsWrapper results = new StudentResultsWrapper();

    public ImageLoader imageLoader;

    void Start()
    {
        btn_ClosePhoto.onClick.AddListener(OnclickClose);

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
            ButtonSpawn();

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

    public void ButtonSpawn()
    {
        if (results.studentResults.Length < 3)
        {
            for (int i = 0; i < 3; i++ )
            {
                GameObject btn_Album = Instantiate(Prefab_Album, contents);

                if (i < results.studentResults.Length)
                {
                    btn_Album.GetComponent<P_AlbumInfo>().SetBookTitle(results.studentResults[i].bookTitle);
                    btn_Album.GetComponent<Button>().onClick.AddListener(() => Open_Album(results.studentResults[i].photo));
                }
            }
        }
        else
        {
            for (int i = 0; i < results.studentResults.Length; i++)
            {
                GameObject btn_Album = Instantiate(Prefab_Album, contents);

                btn_Album.GetComponent<P_AlbumInfo>().SetBookTitle(results.studentResults[i].bookTitle);
                btn_Album.GetComponent<Button>().onClick.AddListener(() => Open_Album(results.studentResults[i].photo));
            }
        }
    }

    void Open_Album(string url)
    {
        // url을 Img에 적용
        imageLoader.LoadImageFromUrl(url);

        panel_Photo.SetActive(true);
    }

    void OnclickClose()
    {
        panel_Photo.SetActive(false); 
    }
}
