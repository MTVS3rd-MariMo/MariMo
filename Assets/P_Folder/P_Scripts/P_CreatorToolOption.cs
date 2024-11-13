using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_CreatorToolOption : MonoBehaviour
{
    public GameObject panel_CreatorTool;

    public Button btn_LogOut;
    public Button btn_Close;

    void Start()
    {
        btn_LogOut.onClick.AddListener(OnclickLogOut);
        btn_Close.onClick.AddListener(OnclickClose);
    }

    void OnclickLogOut()
    {
        panel_CreatorTool.SetActive(false);
    }

    void OnclickClose()
    {
        this.gameObject.SetActive(false);
    }
}
