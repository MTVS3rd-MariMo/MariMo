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

    [SerializeField]
    private TMP_Dropdown dropdown;
    private string[] books = new string[3];

    private void Awake()
    {
            

        
    }


    void Start()
    {
        
        btn_RoomCreate.onClick.AddListener(OnclickRoomCreate);
    }

    void Update()
    {
        // 빈 칸이 없는지 체크
        if (RoomName.text != null && Class.text != null && 0 < Convert.ToInt32(PlayerNum.text) )
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
        HttpInfo info = new HttpInfo();
        info.url = "http://mtvs.helloworldlabs.kr:7771/api/json";
        info.body = JsonUtility.ToJson(P_CreatorToolConnectMgr.Instance.lessons.lessonMaterials[dropdown.value].lessonMaterialId);
        info.contentType = "application/json";
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            print(downloadHandler.text);
        };

        StartCoroutine(HttpManager.GetInstance().Post(info));
    }
}
