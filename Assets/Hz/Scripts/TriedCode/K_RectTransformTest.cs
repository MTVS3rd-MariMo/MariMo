using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_RectTransformTest : MonoBehaviour
{
    public RectTransform rt;
    public LineRenderer line;
    public float zPosition = -9f;
    public Camera uiCamera;

    // 마지막 점 - 시작 지점, 현재 지점, 마지막 지점으로 나눠야함
    private Vector3 lastPosition;

    // 그리고 있는지
    private bool isDrawing = false;
    // 새로운 선인지
    private bool isNewLine = true;

    public float minDistance = 1f;

    void Update()
    {
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, uiCamera, out mousePos);

        Vector3 worldPos = rt.TransformPoint(mousePos);
        worldPos.z = zPosition;

        // 0
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log($"localPoint : {mousePos}");

            //DrawLine(new Vector3(worldPos.x, worldPos.y, zPosition));

            DrawStart(worldPos);
        }
        // 1
        else if (Input.GetMouseButton(0))
        {
            DrawKeep(worldPos);
        }

        // 2
        else if (Input.GetMouseButtonUp(0))
        {
            DrawEnd();
        }
    }


    // 그림 그리기 시작
    void DrawStart(Vector3 startPos)
    {
        isDrawing = true;

        if(isNewLine)
        {
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, startPos);
        }
        
        //lastPosition = startPos;
        isNewLine = false;
    }

    // 계속 그리는 중
    void DrawKeep(Vector3 currPos)
    {
        // 그리고 있으면 리턴
        if (!isDrawing) return;

        // 마지막 지점이랑 현재 지점의 간격
        float distance = Vector3.Distance(lastPosition, currPos);
        // 간격이 최소 보간 거리보다 작아지면 리턴
        if (distance < minDistance) return;

        line.positionCount++;
        line.SetPosition(line.positionCount - 1, currPos);

        lastPosition = currPos;
    }


    void DrawEnd()
    {
        isDrawing = false;
        isNewLine = true;

        // 마지막 그리기 지점 저장
        //lastPosition = line.GetPosition(line.positionCount - 1);
    }

    // 그리기 초기화
    public void DrawClean()
    {
        line.positionCount = 0;
        isDrawing = false;
        isNewLine = true;
    }
}
