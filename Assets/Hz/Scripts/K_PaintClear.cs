using System.Collections;
using System.Collections.Generic;
using iTextSharp.text;
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
    Texture2D paintTexture;
    RectTransform paint_RT;

    // 기본 배경 색상(흰색)
    private Color defaultColor = Color.white; 
    // 그림을 그릴 텍스처
    
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
    }


    void Start()
    {
        // 처음엔 UI 꺼주기
        img_PaintClear.gameObject.SetActive(false);

        // 텍스쳐 설정
        if(paint.texture is Texture2D  texture)
        {
            paintTexture = texture;
        }       
        else
        {
            // 초기화
        }
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
        print("1");

        // K_Drawing의 static 텍스처를 초기화 (새로운 빈 텍스처로 설정)
        K_Drawing.pixel_Texture = new Texture2D((int)paint_RT.rect.width, (int)paint_RT.rect.height);
        
        // 텍스처를 흰색으로 초기화 (배경색으로 설정)
        Color[] clearColors = new Color[K_Drawing.pixel_Texture.width * K_Drawing.pixel_Texture.height];
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = defaultColor;  // 기본 배경 색 (흰색)
        }
        K_Drawing.pixel_Texture.SetPixels(clearColors);
        K_Drawing.pixel_Texture.Apply();  // 적용하여 텍스처 업데이트

        // paint의 RawImage에 텍스처 할당
        paint.texture = K_Drawing.pixel_Texture;

        // 다시 그릴 수 있도록 K_Drawing의 StartDrawing 메서드 호출
        FindObjectOfType<K_Drawing>().StartDrawing();

        print("2");
    }



    public void ClearPaintNo()
    {
        // 아니오 버튼 -> UI 없애기
        img_PaintClear.gameObject.SetActive(false);
}
    }

    
    
    
