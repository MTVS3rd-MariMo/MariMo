using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class K_HttpAvatar : MonoBehaviour
{
    public RawImage rawImage;
    public string uploadUrl = "http://172.30.1.44:8080/api/avatar/upload-img";
    public Button btn_CreateAvatar;

    void Start()
    {
        btn_CreateAvatar.onClick.AddListener(CreateAvatar);
    }

    public IEnumerator UploadTextureAsPng()
    {
        Texture2D textureToUpload = rawImage.texture as Texture2D;

        if (textureToUpload == null)
        {
            Debug.Log("텍스쳐 설정 안댐 ");

            yield break;
        }

        byte[] pngData = textureToUpload.EncodeToPNG();
        //string img = System.Convert.ToBase64String(pngData);

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
        yield return StartCoroutine(HttpManager.GetInstance().UploadFileByFormData(info, pngData));
    }

    // 캐릭터 생성하기 버튼 누르면 서버에 전송
    public void CreateAvatar()
    {
        // 아바타 만든거 보내기 (POST)
        StartCoroutine(UploadTextureAsPng());
    }

    public IEnumerator TestConnection()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("http://localhost:8080/api/avatar/upload-img"))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                Debug.Log("Received: " + webRequest.downloadHandler.text);
            }
        }
    }
}