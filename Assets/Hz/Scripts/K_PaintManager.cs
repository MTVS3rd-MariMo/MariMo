using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class K_PaintManager : MonoBehaviour
{
    public K_DrawPen drawTool;
    public K_Eraser eraserTool;

    // 버튼
    public Button btn_Eraser;
    public Button btn_Create;
    // 색상 버튼
    public Button btn_Black;
    public Button btn_Red;
    public Button btn_Blue;
    public Button btn_Yellow;
    public Button btn_Green;

    // enum
    public ToolType toolType = ToolType.Pen;
    // 기본 색상은 검정
    private Color currcolor = Color.black;

    void Update()
    {
        if (toolType == ToolType.Pen && Input.GetMouseButton(0))
        {
            SetPenMode();
        }

        if (toolType == ToolType.Eraser && Input.GetMouseButton(1))
        {
            SetEraserMode();
        }
    }

    public void SetPenMode()
    {
        toolType = ToolType.Pen;
        drawTool.enabled = true;
        eraserTool.enabled = false;
    }

    public void SetEraserMode()
    {
        toolType = ToolType.Eraser;
        drawTool.enabled = false;
        eraserTool.enabled = true;
    }

    public void SetPenColor(Color color)
    {
        currcolor = color;
        if (drawTool != null)
        {
            drawTool.SetPenColor(currcolor);
        }
    }

    public void SetColorToBlack()
    {
        SetPenColor(Color.black);
    }

    public void SetColorToRed()
    {
        SetPenColor(Color.red);
    }

    public void SetColorToBlue()
    {
        SetPenColor(Color.blue);
    }

    public void SetColorToYellow()
    {
        SetPenColor(Color.yellow);
    }

    public void SetColorToGreen()
    {
        SetPenColor(Color.green);
    }


    public void CreateCharacter()
    {
        // 만들기 전 비활성
        // 버튼 누르면 캐릭터 생성
    }
}
