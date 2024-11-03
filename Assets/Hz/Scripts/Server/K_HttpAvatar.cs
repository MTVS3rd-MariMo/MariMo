using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class K_HttpAvatar : MonoBehaviour
{
    // 아바타 보내기
    public RawImage rawImage;   
    public Button btn_CreateAvatar;

    // 아바타 이미지 받기
    public Image avatarImage;


    public string uploadUrl = "http://172.30.1.44:8080/api/avatar/upload-img";
    public string downloadUrl = "";

    void Start()
    {
        btn_CreateAvatar.onClick.AddListener(CreateAvatar);
    }
    
    // 서버에 아바타 보내기
    public IEnumerator UploadTextureAsPng()
    {
        Texture2D textureToUpload = rawImage.texture as Texture2D;

        if (textureToUpload == null)
        {
            Debug.Log("텍스쳐 설정 안댐 ");

            yield break;
        }

        byte[] pngData = textureToUpload.EncodeToPNG();

        HttpInfo info = new HttpInfo
        {
            url = uploadUrl,
            body = "img",
            contentType = "multipart/form-data",
            onComplete = (DownloadHandler downloadHandler) =>
            {
                // 응답 받아
                Debug.Log("Upload 완료 : " + downloadHandler.text);
            }
        };

        // 디버그 체크
        Debug.Log("Attempting to upload to URL: " + info.url);
        Debug.Log("Coroutine Started");

        Debug.Log("이미지 데이터 길이 " + pngData.Length);
        yield return StartCoroutine(HttpManager.GetInstance().UploadFileByFormDataArt(info, pngData));
    }

    // 캐릭터 생성하기 버튼 누르면 서버에 전송
    public void CreateAvatar()
    {
        // 아바타 만든거 보내기 (POST)
        StartCoroutine(UploadTextureAsPng());
    }

    // 서버에서 아바타 받아오기
    public IEnumerator OnDownloadMP4()
    {
        HttpInfo info = new HttpInfo
        {
            url = downloadUrl,
            onComplete = (DownloadHandler downloadHandler) =>
            {
                Texture2D receivedTexture = new Texture2D(2, 2);
                receivedTexture.LoadImage(downloadHandler.data);
                rawImage.texture = receivedTexture;

                Sprite receivedSprite = Sprite.Create(
                    receivedTexture,
                    new Rect(0, 0, receivedTexture.width, receivedTexture.height),
                    new Vector2(0.5f, 0.5f)
                    );
                avatarImage.sprite = receivedSprite;

                Debug.Log("아바타 이미지 다운로드 댐");

                // 응답 받아
                //Debug.Log("Download 완료 : " + downloadHandler.text);
            }
        };
        
        yield return StartCoroutine(HttpManager.GetInstance().Get(info));

    }

    public void DownloadAvatar()
    {
        StartCoroutine(OnDownloadMP4());
    }
}