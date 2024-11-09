using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class P_LibraryTeacher : MonoBehaviour
{
    public Button[] buttons;
    public TMP_Text[] texts;

    void Start()
    {
        // 수업 기록 조회

        // 기록 조회 끝나면 실행
        BtnSetting();
    }


    void BtnSetting()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.AddListener(OnclickEmptyButton);
            texts[i].text = "수업기록이 없습니다";
        }
    }

    void OnclickEmptyButton()
    {
        return;
    }

    void OnclickFilledButton()
    {
        // 기록 상세 조회
    }
}
