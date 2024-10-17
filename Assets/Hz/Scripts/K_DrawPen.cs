using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class K_DrawPen : MonoBehaviour
{
    // PaintToolType 참조
    //public ToolType toolType = ToolType.Pen;

    public LineRenderer line;
    public Material penMaterial;

    private bool isDrawing = false;

    public Camera uiCamera;
    public RectTransform rt;
    public RectTransform sktechBook;
    Vector3 prevPos;
    public float minDistance = 1f;
    public float zPosition = -9f;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            StartDrawing();
        }

        if (Input.GetMouseButton(0))
        {
            UpdateDrawing(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopDrawing();
        }
    }

    public void StartDrawing()
    {
        // 스케치북 안에서만 활성화되도록 -> 라인렌더러 생성 막기
        if(RectTransformUtility.RectangleContainsScreenPoint(sktechBook, Input.mousePosition, uiCamera))
        {
            isDrawing = true;
            line = new GameObject("K_Crayon").AddComponent<LineRenderer>();

            // 생성된 GameObject의 위치를 rt 위치로 설정 (또는 원하는 위치로 설정)
            line.transform.position = rt.transform.position;
            line.material = penMaterial;
            line.positionCount = 0;
            line.useWorldSpace = true;
            line.gameObject.layer = LayerMask.NameToLayer("UI");
            line.sortingLayerName = "UI";
            line.sortingOrder = 1;
            line.startWidth = 0.5f;
            line.endWidth = 0.5f;
        }
        
    }

    public void UpdateDrawing(Vector3 startPos)
    {
        if(isDrawing)
        {
            Vector2 mousePos;
            // 마우스 좌표 rt의 로컬로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, uiCamera, out mousePos);

            print("변환됨");

            // 변환된 마우스 좌표 월드 좌표로 변환
            Vector3 worldPos = rt.TransformPoint(mousePos);
            worldPos.z = zPosition;

            // 스케치북 안에 있을 시 그려지게
            if(RectTransformUtility.RectangleContainsScreenPoint(sktechBook, Input.mousePosition, uiCamera))
            {
                line.positionCount++;
                line.SetPosition(line.positionCount - 1, worldPos);
                // 이전 위치 갱신
                prevPos = worldPos;

                print("그려짐");
            }
            else
            {
                print("벗어남");

                // 해당영역 벗어나면 LineRenderer 생성안되게 함
                //StopDrawing();
            }
            
        }
    }

    public void StopDrawing()
    {
        isDrawing = false;
    }

}


