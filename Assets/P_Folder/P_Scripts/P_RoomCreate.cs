using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class P_RoomCreate : MonoBehaviour
{
    public TMP_InputField RoomName;
    public TMP_InputField Class;
    public TMP_InputField PlayerNum;
    public Button btn_RoomCreate;
    public Sprite[] sprites;
    private int players;

    [SerializeField]
    private TMP_Dropdown dropdown;
    private string[] books = new string[3];


    void Start()
    {
        
        btn_RoomCreate.onClick.AddListener(OnclickRoomCreate);
    }

    void Update()
    {
        // 빈 칸이 없는지 체크
        if (RoomName.text != null && Class.text != null && PlayerNum.text != null)
        {
            btn_RoomCreate.image.sprite = sprites[1];   
            btn_RoomCreate.interactable = true;
        }
        else
        {
            btn_RoomCreate.image.sprite = sprites[0];
            btn_RoomCreate.interactable = false;
        }
    }

    public void DropDownUpdate()
    {
        dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();

        for (int i = 0; i < P_CreatorToolConnectMgr.Instance.lessons.lessonMaterials.Count; i++)
        {
            books[i] = P_CreatorToolConnectMgr.Instance.lessons.lessonMaterials[i].bookTitle;
        }
        Debug.Log(books);

        foreach (string str in books)
        {
            optionDatas.Add(new TMP_Dropdown.OptionData(str));
        }

        dropdown.AddOptions(optionDatas);

        dropdown.value = 0;
    }

    void OnclickRoomCreate()
    {
        players = Convert.ToInt32(PlayerNum.text);

        if (players == 0)
            return;
        else if (dropdown.itemText == null)
            return;

        GetLessonId();
    }

    public void GetLessonId()
    {
        HttpInfo info = new HttpInfo();
        info.url = P_CreatorToolConnectMgr.Instance.url_Front + "api/lesson/lesson-material";
        info.body = JsonUtility.ToJson(P_CreatorToolConnectMgr.Instance.lessons.lessonMaterials[dropdown.value].lessonMaterialId);
        info.contentType = "application/json";
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            print(downloadHandler.text);

            // 받은 데이터 파싱할 함수 호출
            int lessenId = JsonUtility.FromJson<int>(downloadHandler.text);
            // 포톤 방 생성 ( 파싱 완료 후 )

            // 그림그리기 요청

        };

        StartCoroutine(HttpManager.GetInstance().PostRoom(info));
    }

    public void DrawPicture()
    {
        HttpInfo info = new HttpInfo();
        info.url = P_CreatorToolConnectMgr.Instance.url_Front + "";
        info.onComplete = (DownloadHandler downloadHandler) =>
        {

        };

        StartCoroutine(HttpManager.GetInstance().Post(info));
    }
}
