using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_Eraser : MonoBehaviour
{
    // PaintToolType 참조
    //public ToolType toolType = ToolType.Eraser;
    public Camera paintCamera;
    public RectTransform sketchBook;
    public float zPosition = -9f;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Erase();
        }
    }

    public void Erase()
    {
        
        if (RectTransformUtility.RectangleContainsScreenPoint(sketchBook, Input.mousePosition, paintCamera))
        {
            Ray ray = paintCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                LineRenderer line = hit.collider.gameObject.GetComponent<LineRenderer>();
                if (line != null)
                {
                    Destroy(hit.collider.gameObject);
                    print("라인 지워짐");
                }
                else
                {
                    print("충돌 감지 안댐");
                }
            }
            else
            {
                print("스케치북 바깥");
            }
        }

    }
}
