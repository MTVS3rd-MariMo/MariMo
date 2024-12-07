using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class K_ChangeDrawColor : MonoBehaviour
{
    [SerializeField]
    Button btn_ChangeColor;

    [SerializeField]
    Color penColor;

    void Start()
    {
        // 이벤트 등록
        btn_ChangeColor.onClick.AddListener(ChangePenColor);
    }


    void ChangePenColor()
    {
        K_Drawing.draw_Color = penColor;
        K_PaintBin.fill_Color = penColor;
    }

    void ChangeBinColor()
    {
        // 페인트통 컬러
        K_PaintBin.fill_Color = penColor;
    }
}
