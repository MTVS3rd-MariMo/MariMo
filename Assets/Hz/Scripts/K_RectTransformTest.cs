using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_RectTransformTest : MonoBehaviour
{
    public RectTransform rt;
    //public Camera uiCamera;
    public LineRenderer line;
    public float zPosition = -9f;
    public Camera uiCamera;

    void Update()
    {
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, uiCamera, out mousePos);

        Vector3 worldPos = rt.TransformPoint(mousePos);

        if (Input.GetMouseButton(0))
        {
            Debug.Log($"localPoint : {mousePos}");      
            DrawLine(new Vector3(worldPos.x, worldPos.y, zPosition));
        }
    }

    public void DrawLine(Vector3 point)
    {
        line.positionCount++;
        line.SetPosition(line.positionCount - 1, point);
    }
}
