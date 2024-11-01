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
    // 기본 색상은 검정
    private Color currColor = Color.black;
    // 현재 머터리얼
    private Material currPenMaterial;
    // 이전에 그린 선 저장 리스트
    private List<LineRenderer> drawLines = new List<LineRenderer>();

    private bool isDrawing = false;

    public Camera uiCamera;
    public RectTransform rt;
    public RectTransform sktechBook;
    Vector3 prevPos;
    public float minDistance = 1f;
    public float zPosition = -9f;


    private void Start()
    {
        //uiCamera.cullingMask = 1 << 0;
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
        if (RectTransformUtility.RectangleContainsScreenPoint(sktechBook, Input.mousePosition, uiCamera))
        {
            isDrawing = true;
            line = new GameObject("K_Crayon").AddComponent<LineRenderer>();
            // 생성된 GameObject의 위치를 rt 위치로 설정 (또는 원하는 위치로 설정)
            line.transform.position = rt.transform.position;
            //line.material = penMaterial;

            // 현재 선 리스트에 추가
            drawLines.Add(line);

            // 추가
            currPenMaterial = new Material(Shader.Find("Sprites/Default"));
            currPenMaterial.color = currColor;

            line.material = currPenMaterial;
            line.positionCount = 0;
            line.useWorldSpace = true;
            line.gameObject.layer = LayerMask.NameToLayer("UI");
            line.sortingLayerName = "UI";
            line.sortingOrder = 1;
            line.startWidth = 0.3f;
            line.endWidth = 0.3f;
            line.gameObject.AddComponent<BoxCollider>();

            // 색상 설정
            line.startColor = currColor;
            line.endColor = currColor;
        }
    }

    public void UpdateDrawing(Vector3 startPos)
    {
        if (isDrawing)
        {
            Vector2 mousePos;
            // 마우스 좌표 rt의 로컬로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, Camera.main, out mousePos);

            print("변환됨");

            // 변환된 마우스 좌표 월드 좌표로 변환
            Vector3 worldPos = rt.TransformPoint(mousePos);
            worldPos.z = zPosition;

            // 스케치북 안에 있을 시 그려지게
            if (RectTransformUtility.RectangleContainsScreenPoint(sktechBook, Input.mousePosition, Camera.main))
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

    // 색상 변경
    public void SetPenColor(Color color)
    {
        currColor = color;
        //penMaterial.color = currColor;
        // 현재 그리고 있는 선에만 적용 추가
        if(isDrawing && line != null)
        {
            line.startColor = currColor;
            line.endColor = currColor;
            if(currPenMaterial != null)
            {
                currPenMaterial.color = currColor;
            }
        }
    }

}


