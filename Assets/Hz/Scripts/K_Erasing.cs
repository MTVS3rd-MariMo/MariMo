using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class K_Erasing : MonoBehaviour
{
    [SerializeField]
    Image whiteBoard;

    [SerializeField]
    RawImage paint;
    RectTransform paint_RT;

    int pixel_Width, pixel_Height;

    Color erase_Color;

    //// '화이트 보드 전환'
    //public static Color draw_Color = Color.white;

    //public static Texture2D pixel_Texture;

    private void Awake()
    {
        paint_RT = paint.GetComponent<RectTransform>();

        pixel_Width = (int)paint_RT.rect.width;
        pixel_Height = (int)paint_RT.rect.height;

        erase_Color = whiteBoard.color;
        erase_Button = GetComponent<Button>();
    }

    public static bool erase_Active = false;
    public static Button erase_Button;


    public void ButtonOn()
    {
        erase_Active = true;

        K_Drawing.pen_Active = false;
        print("지우개 지워");
    }

    public void ButtonOff()
    {
        erase_Active = false;

        K_Drawing.pen_Active = true;
        print("다시 그릴꺼야");
    }

    Vector2 lastPosition, currPosition = Vector2.zero;
    Vector2 mousePosition;

    void Update()
    {
        if (erase_Active)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(paint_RT, Input.mousePosition, null, out currPosition);
            //currPosition.x = Mathf.Clamp((int)currPosition.x, 0, pixel_Width - 1);
            //currPosition.y = Mathf.Clamp((int)currPosition.y, 0, pixel_Height - 1);

            mousePosition = new Vector2
                (
                    (currPosition.x / pixel_Width + 0.5f) * pixel_Width,
                    (currPosition.y / pixel_Height + 0.5f) * pixel_Height

                );

            if (Input.GetMouseButtonDown(0))
            {
                Erase_Jum(mousePosition);
                //lastPosition = currPosition;
            }

            else if (Input.GetMouseButton(0))
            {
                Erase_Line(lastPosition, mousePosition);
                //lastPosition = currPosition;

            }

            K_Drawing.pixel_Texture.Apply();
        }
    }

    void Erase_Jum(Vector2 mousePos)
    {
        int brush_Width = (int)mousePos.x;
        int brush_Height = (int)mousePos.y;

        //print(brush_Height);
        //print(brush_Width);

        for (int height_Plus = -6; height_Plus <= 6; height_Plus++)
        {
            for (int width_Plus = -6; width_Plus <= 6; width_Plus++)
            {
                if (brush_Height + height_Plus >= 0 && brush_Height + height_Plus < pixel_Height
                    && brush_Width + width_Plus >= 0 && brush_Width + width_Plus < pixel_Width)
                {
                    K_Drawing.pixel_Texture.SetPixel(brush_Width + width_Plus, brush_Height + height_Plus, erase_Color);
                }
            }
        }

        lastPosition = mousePos;

    }

    void Erase_Line(Vector2 lastPos, Vector2 currPos)
    {
        float distance = Vector2.Distance(currPos, lastPos);

        float interval = 1.0f / distance;

        for (float count = 0.1f; count <= distance; count++)
        {
            Vector2 Line = Vector2.Lerp(lastPos, currPos, interval * count);
            Erase_Jum(Line);
        }
    }
}
