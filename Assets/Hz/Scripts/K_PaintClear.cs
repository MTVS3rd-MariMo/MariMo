using System.Collections;
using System.Collections.Generic;
using iTextSharp.text;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class K_PaintClear : MonoBehaviour
{
    // 화이트 보드 가져올것
    [SerializeField]
    Image whiteBoard;
    // 초기화 할 빈 텍스처
    [SerializeField]
    RawImage paint;
    RectTransform paint_RT;

    // 기본 배경 색상(흰색)
    private Color defaultColor = Color.white; 
    // 그림을 그릴 텍스처
    private Texture2D paintTexture;
    // 초기화 텍스쳐
    private Texture2D blankTexture;
    int pixel_Width, pixel_Height;

    // 그림 지울꺼냐는 안내 UI
    public GameObject img_PaintClear;


    void Awake()
    {
        // RawImage에서 텍스처 가져오기
        paintTexture = paint.texture as Texture2D;

        // Paint의 RectTransform 가져오기
        paint_RT = paint.GetComponent<RectTransform>();

        // Paint의 텍스처 초기화
        int width = Mathf.RoundToInt(paint_RT.rect.width);
        int height = Mathf.RoundToInt(paint_RT.rect.height);

        blankTexture = new Texture2D(pixel_Width, pixel_Height, TextureFormat.RGBA32, false);
        //ResetTexture(); // 빈 텍스처로 초기화
        //paint.texture = blankTexture; // Paint의 텍스처로 설정
    }


    void Start()
    {
        // 처음엔 UI 꺼주기
        img_PaintClear.gameObject.SetActive(false);       
    }


    public void DisPlayOnUI()
    {
        // 버튼 누르면 UI 띄울거임 
        // 휴지통 버튼 누르면 UI 뜨게
        img_PaintClear.gameObject.SetActive(true);
    }

    public void ClearPaintYes()
    {
        // 초기화 함수 호출
        ResetTexture();

        // 팝업닫기
        if(img_PaintClear != null)
        {
            img_PaintClear.SetActive(false);
            print("초기화 됨?");
        }        
    }

    // 기본 빈 텍스처 초기화
    void ResetTexture()
    {
        // 기존 텍스처의 크기를 가져오거나 기본 크기 지정
        int width = paint.texture != null ? paint.texture.width : 512;
        int height = paint.texture != null ? paint.texture.height : 512;

        // 빈 텍스처 생성
        blankTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // 텍스처에 기본 색상 채우기
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = defaultColor;
        }
        blankTexture.SetPixels(pixels);
        blankTexture.Apply();

        // RawImage에 새 텍스처 적용
        paint.texture = blankTexture;
    }



    public void ClearPaintNo()
    {
        // 아니오 버튼 -> UI 없애기
        img_PaintClear.gameObject.SetActive(false);
}
    }

    
    
    
