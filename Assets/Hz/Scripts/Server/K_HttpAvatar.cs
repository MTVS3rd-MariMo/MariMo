using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class K_HttpAvatar : MonoBehaviour
{
    public RawImage rawImage;
    public string uploadUrl = "https://localhost:8080/api/avatar/upload-img";

    public Button btn_CreateAvatar;

    void Start()
    {
        btn_CreateAvatar.onClick.AddListener(CreateAvatar);
    }

    public IEnumerator UploadTextureAsPng()
    {
        Texture2D textureToUpload = rawImage.texture as Texture2D;

        if(textureToUpload == null)
        {
            Debug.Log("텍스쳐 설정 안댐 ");

            yield break;
        }

        byte[] pngData = textureToUpload.EncodeToPNG();
        string base64PngData = System.Convert.ToBase64String(pngData);

        HttpInfo info = new HttpInfo
        {
            url = uploadUrl,
            body = base64PngData,
            contentType = "multipart/form-data",
            onComplete = (DownloadHandler downloadHandler) =>
            {
                // 응답 받아
                Debug.Log("Upload 완료 : " + downloadHandler.text);
            }
        };

        // 요청 보내고 
        // 서버에 PNG 이미지 전송
        //StartCoroutine(HttpManager.GetInstance().UploadFileByByte(info));

        // 디버그 체크
        Debug.Log("Attempting to upload to URL: " + info.url); 
        yield return StartCoroutine(HttpManager.GetInstance().UploadFileByFormData(info));
    }

    // 캐릭터 생성하기 버튼 누르면 서버에 전송
    public void CreateAvatar()
    {
        // 아바타 만든거 보내기 (POST)
        StartCoroutine(UploadTextureAsPng());
    }
}