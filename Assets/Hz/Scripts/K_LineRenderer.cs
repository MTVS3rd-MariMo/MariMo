using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class K_LineRenderer : MonoBehaviour
{
    public RectTransform paint_Rt;
    RawImage paint_BG;

    private LineRenderer line;
    private Vector3 prevPos;

    [SerializeField]
    private float minDistance = 0.1f;

    private Camera mainCamera;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;

        mainCamera = Camera.main;
        prevPos = transform.position;

    }

    void Update()
    {
        // 마우스로 먼저
        if(Input.GetMouseButton(0))
        {
            // 마우스 스크린 좌표를 캔버스 로컬 좌표로 변환
            Vector3 currPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.nearClipPlane));           
            //currPos.z = -(float)9.1; //mainCamera.nearClipPlane;
            currPos.z = -9;

            if(Vector3.Distance(currPos, prevPos) > minDistance)
            {
                print("제발그려져");
                line.positionCount++;
                line.SetPosition(line.positionCount - 1, currPos);
                prevPos = currPos;
            }
            else
            {
                prevPos = Vector3.zero;
            }
        }
    }
}
