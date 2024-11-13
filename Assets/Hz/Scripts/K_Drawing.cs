using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class K_Drawing : MonoBehaviour
{
    [SerializeField]
    Image whiteBoard;

    [SerializeField]
    RawImage paint;
    RectTransform paint_RT;
    Color paint_Color;

    // '화이트 보드 전환'
    public static Color draw_Color = Color.black;

    public static Texture2D pixel_Texture;
    int pixel_Width, pixel_Height;

    private void Awake()
    {
        paint_RT = paint.GetComponent<RectTransform>();

        paint_Color = whiteBoard.color;

        pixel_Width = (int)paint_RT.rect.width;
        pixel_Height = (int)paint_RT.rect.height;


        pixel_Texture = new Texture2D(pixel_Width, pixel_Height);


        for (int height = 0; height < pixel_Height; height++)
        {
            for (int width = 0; width < pixel_Width; width++)
            {
                pixel_Texture.SetPixel(width, height, paint_Color);
            }
        }

        pixel_Texture.Apply();
        paint.texture = pixel_Texture;
        pen_Button = GetComponent<Button>();
    }

    public static bool pen_Active = false;
    public static Button pen_Button;

    public void ButtonOn()
    {
        pen_Active = true;
        K_Erasing.erase_Active = false;
        //print("눌리니?");

    }

    public void ButtonOff()
    {
        pen_Active = false;
        K_Erasing.erase_Active = true;
        print("그만 눌려라 ");
    }

    Vector2 lastPosition, currPosition = Vector2.zero;
    Vector2 mousePosition;

    void Update()
    {
        if (pen_Active)
        {
            //print("pen Activated!");
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
                Draw_Jum(mousePosition);
                //lastPosition = currPosition;
            }

            else if (Input.GetMouseButton(0))
            {
                Draw_Line(lastPosition, mousePosition);
                //lastPosition = currPosition;

            }

            pixel_Texture.Apply();
        }
    }

    void Draw_Jum(Vector2 mousePos)
    {
        int brush_Width = (int)mousePos.x;
        int brush_Height = (int)mousePos.y;

        //print(brush_Height);
        //print(brush_Width);

        for (int height_Plus = -3; height_Plus <= 3; height_Plus++)
        {
            for (int width_Plus = -3; width_Plus <= 3; width_Plus++)
            {
                if (brush_Height + height_Plus >= 0 && brush_Height + height_Plus < pixel_Height
                    && brush_Width + width_Plus >= 0 && brush_Width + width_Plus < pixel_Width)
                {
                    pixel_Texture.SetPixel(brush_Width + width_Plus, brush_Height + height_Plus, draw_Color);
                }
            }
        }

        lastPosition = mousePos;
        //pixel_Texture.Apply();
    }

    void Draw_Line(Vector2 lastPos, Vector2 currPos)
    {
        float distance = Vector2.Distance(currPos, lastPos);

        float interval = 1.0f / distance;

        for (float count = 0.1f; count <= distance; count++)
        {
            Vector2 Line = Vector2.Lerp(lastPos, currPos, interval * count);
            Draw_Jum(Line);
        }
    }
}
