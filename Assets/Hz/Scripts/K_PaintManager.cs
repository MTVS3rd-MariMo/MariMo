using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_PaintManager : MonoBehaviour
{
    public K_DrawPen drawTool;
    public K_Eraser eraserTool;

    // enum
    public ToolType toolType = ToolType.Pen;

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            SetPenMode();
        }

        if(Input.GetMouseButton(1))
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
}
