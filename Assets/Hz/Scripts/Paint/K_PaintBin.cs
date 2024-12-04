using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class K_PaintBin : MonoBehaviour
{
    // 그림을 그릴 페인트 
    [SerializeField]
    RawImage paint;

    // 텍스쳐
    //public Texture2D texture;

    //private Color targetColor;

    // 채울 색상 - 일단 blue?
    public static Color fill_Color = Color.blue;

    // 색상 일치 허용 오차
    [SerializeField]
    float colorTolerance = 0.1f;

    //public Button btn_PaintBin;

    void Start()
    {
        // 이벤트 등록
        //btn_PaintBin.onClick.AddListener(FloodFill(texture, startX, startY, fill_Color));
    }

    void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     RectTransform rectTransform = paint.rectTransform;
        //     Vector2 localPos;
        //     RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //         rectTransform,
        //         Input.mousePosition,
        //         null,
        //         out localPos
        //     );

        //     Texture2D texture = K_Drawing.pixel_Texture;
        //     int pixelX = Mathf.RoundToInt(((localPos.x / rectTransform.rect.width) + 0.5f) * texture.width);
        //     int pixelY = Mathf.RoundToInt(((localPos.y / rectTransform.rect.height) + 0.5f) * texture.height);


        //     // FloodFill 수행
        //     FloodFill(texture, pixelX, pixelY, fill_Color);
        // }
    }

    public void OnPaintBucketClick()
    {
        // 마우스 위치 가져오기
        Vector3 mousePos = Input.mousePosition;

        // 마우스 위치를 텍스처 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        paint.rectTransform,
        mousePos,
        null,
        out Vector2 localPoint
    );

        int pixelX = Mathf.RoundToInt(((localPoint.x / paint.rectTransform.rect.width) + 0.5f) * K_Drawing.pixel_Texture.width);
        int pixelY = Mathf.RoundToInt(((localPoint.y / paint.rectTransform.rect.height) + 0.5f) * K_Drawing.pixel_Texture.height);

        //Color targetColor = K_Drawing.pixel_Texture.GetPixel(pixelX, pixelY);

        // FloodFill 메서드 호출
        FloodFill(K_Drawing.pixel_Texture, pixelX, pixelY, fill_Color);
    }

    public void FloodFill(Texture2D texture, int startX, int startY, Color fillColor)
    {
        // 경계를 확인
        if (startX < 0 || startX >= texture.width || startY < 0 || startY >= texture.height)
            return;

        Color targetColor = texture.GetPixel(startX, startY);

        // 대상 색상이 이미 채우기 색상과 같다면 종료하기
        if (ColorsMatch(targetColor, fillColor)) return;


        // Queue
        Queue<Vector2Int> pixelsToCheck = new Queue<Vector2Int>();
        pixelsToCheck.Enqueue(new Vector2Int(startX, startY));

        // 무한루프 방지 빙믄힌 픽셀 추적
        bool[,] visited = new bool[texture.width, texture.height];

        while (pixelsToCheck.Count > 0)
        {
            Vector2Int currentPixel = pixelsToCheck.Dequeue();
            int x = currentPixel.x;
            int y = currentPixel.y;

            // 경계 벗어나거나 색상 일치하지 않을 시, 건너뜀
            if (x < 0 || x >= texture.width || y < 0 || y >= texture.height ||
                visited[x, y] || !ColorsMatch(texture.GetPixel(x, y), targetColor))
                continue;

            // 방문 표시 및 채우기
            visited[x, y] = true;
            texture.SetPixel(x, y, fillColor);

            // 인접한 픽셀들을 큐에 추가
            pixelsToCheck.Enqueue(new Vector2Int(x + 1, y));
            pixelsToCheck.Enqueue(new Vector2Int(x - 1, y));
            pixelsToCheck.Enqueue(new Vector2Int(x, y + 1));
            pixelsToCheck.Enqueue(new Vector2Int(x, y - 1));
        }

        // 텍스쳐 변경사항 적용
        texture.Apply();
    }

    private bool ColorsMatch(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) <= colorTolerance * 1.5f &&
               Mathf.Abs(a.g - b.g) <= colorTolerance * 1.5f &&
               Mathf.Abs(a.b - b.b) <= colorTolerance * 1.5f &&
               Mathf.Abs(a.a - b.a) <= colorTolerance * 1.5f;
    }






    // // GET
    // public void Fill()
    // {

    // }

    // // SET
    // public void FloodFill()
    // {

    // }


}
