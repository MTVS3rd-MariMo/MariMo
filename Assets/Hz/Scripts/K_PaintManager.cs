using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class K_PaintManager : MonoBehaviour
{
    private K_DrawPen drawTool;
    private K_Eraser eraserTool;

    // 버튼
    //public Button btn_Color;
    public Button btn_Eraser;
    public Button btn_Create;

    // enum
    public ToolType toolType = ToolType.Pen;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            SetPenMode();
        }

        if (Input.GetMouseButton(1))
        {
            SetEraserMode();
        }
    }

    public void SetPenMode()
    {
        toolType = ToolType.Pen;
    }

    public void SetEraserMode()
    {
        toolType = ToolType.Eraser;
        
    }

    public void CreateCharacter()
    {
        // 만들기 전 비활성
        // 버튼 누르면 캐릭터 생성
    }
}
