using System;
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
    public GameObject PaintUI;

    // 아바타 이미지 받기
    public GameObject ChooseCharacterUI;
    public Image avatarImage;
    public GameObject btn_MakeAvatar;
    public GameObject btn_ToMap;

    // URL
    public string uploadUrl = "http://211.250.74.75:8899/api/avatar/upload-img";

    void Start()
    {
        //btn_CreateAvatar.onClick.AddListener(CreateAvatar);
    }

    public IEnumerator UploadTextureAsPng()
    {
        Texture2D textureToUpload = rawImage.texture as Texture2D;

        if (textureToUpload == null)
        {
            Debug.LogError("텍스처가 설정되지 않았습니다.");
            yield break;
        }

        byte[] pngData = textureToUpload.EncodeToPNG();

        HttpInfo info = new HttpInfo
        {
            url = uploadUrl,
            contentType = "multipart/form-data",
            body = "img",
            onComplete = (DownloadHandler downloadHandler) =>
            {

                // 서버 응답을 JSON으로 파싱하여 이미지 처리
                UserAvatarData avatarData = JsonUtility.FromJson<UserAvatarData>(downloadHandler.text);

                // Base64 인코딩된 avatarImg 데이터를 디코딩하여 Texture2D로 변환
                byte[] imageData = Convert.FromBase64String(avatarData.avatarImg);
                Texture2D receivedTexture = new Texture2D(2, 2);
                bool isLoaded = receivedTexture.LoadImage(imageData);

                if (isLoaded)
                {
                    // Texture2D를 Sprite로 변환하여 avatarImage에 적용
                    Sprite receivedSprite = Sprite.Create(
                        receivedTexture,
                        new Rect(0, 0, receivedTexture.width, receivedTexture.height),
                        new Vector2(0.5f, 0.5f)
                    );
                    avatarImage.sprite = receivedSprite;

                    Debug.Log("아바타 이미지가 성공적으로 UI에 적용되었습니다.");
                }
                else
                {
                    Debug.LogError("이미지 로딩 실패");
                }

                // UI 상태 변경
                PaintUI.SetActive(false);
                ChooseCharacterUI.SetActive(true);
                btn_MakeAvatar.SetActive(false);
                btn_ToMap.SetActive(true);
            }
        };

        yield return StartCoroutine(HttpManager.GetInstance().UploadFileByFormDataArt(info, pngData));



        //Texture2D textureToUpload = rawImage.texture as Texture2D;

        //if (textureToUpload == null)
        //{
        //    Debug.Log("텍스쳐 설정 안댐 ");

        //    yield break;
        //}

        //byte[] pngData = textureToUpload.EncodeToPNG();

        //HttpInfo info = new HttpInfo
        //{
        //    url = uploadUrl,
        //    contentType = "multipart/form-data",
        //    body = "img",
        //    onComplete = (DownloadHandler downloadHandler) =>
        //    {
        //        // 응답으로 받은 이미지 데이터를 Texture2D로 변환
        //        Texture2D receivedTexture = new Texture2D(2, 2);
        //        receivedTexture.LoadImage(downloadHandler.data);

        //        print(downloadHandler.data);

        //        // Texture2D를 Sprite로 변환하여 avatarImage에 적용
        //        Sprite receivedSprite = Sprite.Create(
        //            receivedTexture,
        //            new Rect(0, 0, receivedTexture.width, receivedTexture.height),
        //            new Vector2(0.5f, 0.5f)
        //        );
        //        avatarImage.sprite = receivedSprite;

        //        print("잘 되는거니..");

        //        // PaintUI 비활성화 및 ChooseCharacterUI 활성화
        //        PaintUI.SetActive(false);
        //        ChooseCharacterUI.SetActive(true);
        //        btn_MakeAvatar.SetActive(false);
        //        btn_ToMap.SetActive(true);

        //        Debug.Log("Received image data length: " + downloadHandler.data.Length);
        //        Debug.Log("아바타 이미지가 성공적으로 UI에 적용되었습니다.");
        //    }
        //};

        //yield return StartCoroutine(HttpManager.GetInstance().UploadFileByFormDataArt(info, pngData));
    }

    // 캐릭터 생성하기 버튼 누르면 서버에 전송
    public void CreateAvatar()
    {
        // 아바타 만든거 보내기 (POST)
        StartCoroutine(UploadTextureAsPng());
    }


    // 아바타 관련 데이터
    [System.Serializable]
    public struct UserAvatarData
    {
        public int userId;
        public string avatarImg;
    }

    // 아바타 관련 데이터
    [System.Serializable]
    public struct UserAnimationData
    {
        public string animation_idle;
        public string animation_walk;
    }


    [System.Serializable]
    public struct UserAvatarDataArray
    {
        public List<UserAvatarData> data;
    }

}