using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;



public class P_ReadyClass : MonoBehaviour
{
    public Transform contents;
    public GameObject Prefab_StoryButton;

    public GameObject panel_CreateRoom;

    public Lessons lessons;

    void Start()
    {

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
                lessons = P_CreatorToolConnectMgr.Instance.ParseLessons(downloadHandler.text);

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
        if (lessons.lessonMaterials.Count >= 2)
        {
            // 생성용 버튼 생성
            GameObject btn_Story = Instantiate(Prefab_StoryButton, contents);



            for (int i = 0; i < lessons.lessonMaterials.Count; i++)
            {
                // 수업 자료 받아서 생성

                
            }
        }
        else
        {
            for (int i = 0; i < 3 - lessons.lessonMaterials.Count; i++)
            {
                // 생성용 버튼 생성
            }

            for (int i = 0; i < lessons.lessonMaterials.Count; i++)
            {
                // 수업 자료 받아서 생성


            }
        }
        
    }
}
