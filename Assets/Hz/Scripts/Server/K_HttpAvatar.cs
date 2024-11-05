﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class K_HttpAvatar : MonoBehaviourPun
{

    //// 비디오
    //public VideoPlayer vp;
    ////public VideoPlayer vp_Walk;
    //public VideoClip[] videoClips;
    //public RawImage img_VideoPlayer;
    //public RenderTexture[] renderChromaKey;

    // 아바타 보내기
    public RawImage rawImage;   
    public Button btn_CreateAvatar;
    public Button btn_DoneCreateAvatar;
    public GameObject PaintUI;

    // 아바타 이미지 받기
    public GameObject ChooseCharacterUI;
    //public Image avatarImage;
    public GameObject btn_ToMap;

    // 이미지 띄우는 화면
    //public GameObject[] buttons;
    // 버튼 어떤 게 눌렸나 받아오기
    //public int characterNum = 0;
    //public int avatarIndex;

    // URL
    public string uploadUrl = "http://125.132.216.190:8203/api/avatar/upload-img";
    //public string downloadUrl = "";
    private string avatarImgUrl;
    private List<string> animationUrls;

    // 그림 보내기 POST
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
                Debug.Log("응답 완료" + downloadHandler.text);

                // 서버에서 받은 JSON 응답 파싱하며 Url 설정
                UserAvatarData avatarData = JsonUtility.FromJson<UserAvatarData>(downloadHandler.text);
                avatarImgUrl = avatarData.avatarImg;
                // animations 필드에서 URL 추출
                //animationUrls = new List<string>();
                animationUrls = avatarData.animations.Select(anim => anim.animation).ToList();

                // 버튼 체인지
                btn_DoneCreateAvatar.gameObject.SetActive(true);

                // 업로드 완료되면 UI 활성화 관리
                PaintUI.SetActive(false);
                ChooseCharacterUI.SetActive(true);
                btn_CreateAvatar.gameObject.SetActive(false);
                btn_ToMap.SetActive(true);

                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

                // 동기화
                photonView.RPC(nameof(SyncAvatarData), RpcTarget.All, avatarData.userId, avatarData.lessonId, avatarImgUrl, animationUrls.ToArray(), actorNumber);
                print(actorNumber);
            }
        };

        yield return StartCoroutine(HttpManager.GetInstance().UploadFileByFormDataArt(info, pngData));
    }

    [PunRPC]
    void SyncAvatarData(int userId, int lessonId, string avatarImgUrl, string[] animationUrls, int actorNumber)
    {
        // 잠만
        //StartCoroutine(OnDownloadImage(userId, avatarImgUrl));

        photonView.RPC(nameof(OnDownloadImage), RpcTarget.All, userId, avatarImgUrl, actorNumber);

        for (int i = 0; i < animationUrls.Length; i++)
        {
            StartCoroutine(DownloadVideo(userId, animationUrls[i], "animation", actorNumber));
        }
    }

    // actorNum = chararcterNum으로 받아옴 
    [PunRPC]
    public IEnumerator OnDownloadImage(int userId, string imageUrl, int actorNum)
    {
        if (string.IsNullOrEmpty(imageUrl))
        {
            Debug.LogError("다운로드 URL이 설정되지 않았습니다.");
            yield break;
        }

        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Texture2D receivedTexture = DownloadHandlerTexture.GetContent(webRequest);

                // Texture2D를 Sprite로 변환하여 UI에 적용
                Sprite receivedSprite = Sprite.Create(
                    receivedTexture,
                    new Rect(0, 0, receivedTexture.width, receivedTexture.height),
                    new Vector2(0.5f, 0.5f)
                );
                // [PhotonNetwork.LocalPlayer.ActorNumber - 1]
                int characterNum = Y_BookController.Instance.allPlayers[actorNum - 1].GetComponent<Y_PlayerAvatarSetting>().avatarIndex;
                // 유저가 선택한 캐릭터 화면에 맞게 떠야함
                Y_BookController.Instance.buttons[characterNum].GetComponent<Image>().sprite = receivedSprite;

                print("charNum 체크 " + characterNum);

                Debug.Log("아바타 이미지가 UI에 성공적으로 적용되었습니다.");
            }
            else
            {
                Debug.LogError("이미지 다운로드 실패: " + webRequest.error);
            }
        }
    }

    // 애니메이션 다운로드 및 로컬 저장
    private IEnumerator DownloadVideo(int userId, string videoUrl, string fileName, int actorNumber)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(videoUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                byte[] videoData = webRequest.downloadHandler.data;

                print(fileName);

                // 애니메이션 데이터를 로컬 파일로 저장
                string filePath = Application.persistentDataPath + "/" + fileName + ".mp4";
                System.IO.File.WriteAllBytes(filePath, videoData);

                Debug.Log($"MP4 파일 다운로드 및 저장 성공: {filePath}");

                string videoPathWithProtocol = "file://" + filePath;

                // RPC 비디오 파일 경로 동기화
                //photonView.RPC(nameof(OnDownloadImage), RpcTarget.All, userId, avatarImgUrl, actorNumber);
                photonView.RPC(nameof(ApplayVideoToPlayer), RpcTarget.All, videoPathWithProtocol, actorNumber);

            }
            else
            {
                Debug.LogError("MP4 다운로드 실패: " + webRequest.error);
            }
        }
    }

    [PunRPC]
    private void ApplayVideoToPlayer(string videoPath, int actorNumber)
    {
        Y_BookController.Instance.allPlayers[actorNumber - 1].GetComponent<K_AvatarVpSettings>().SetVideoPath(videoPath);
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
        public int lessonId;
        public string avatarImg;
        public List<AnimationData> animations;
    }

    // 아바타 애니메이션 데이터
    [System.Serializable]
    public struct AnimationData
    {
        public int animationId;
        public string animation;
    }

}