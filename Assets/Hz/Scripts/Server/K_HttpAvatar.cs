using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class K_HttpAvatar : MonoBehaviourPun
{

    // 아바타 보내기
    public RawImage rawImage;
    public Button btn_CreateAvatar;
    public Button btn_DoneCreateAvatar;
    public GameObject PaintUI;

    // 아바타 이미지 받기
    public GameObject ChooseCharacterUI;
    //public Image avatarImage;
    public GameObject btn_ToMap;

    // URL
    public string uploadUrl = "http://52.78.164.236/api/avatar/upload-img";
    private string avatarImgUrl;
    private List<string> animationUrls;

    // 다른 유저 조회 URL
    private string otherUserUrl = "http://52.78.164.236/api/avatar/upload-img";

    public GameObject bookCanvas;

    int userId;
    //int userIds = Y_HttpRoomSetUp.GetInstance().userList;
    int lessonId;

    Y_BookController bookController;

    private void Start()
    {
        userId = Convert.ToInt32(Y_HttpLogIn.GetInstance().userId);
        lessonId = Y_HttpRoomSetUp.GetInstance().userlessonId;


        btn_CreateAvatar.onClick.AddListener(() => CreateAvatar());
        //btn_CreateAvatar.onClick.AddListener(() => StartCoroutine(CreateAndFetchOtherAvatars(userId, lessonId)));

        bookController = GameObject.Find("BookCanvas").GetComponent<Y_BookController>();
    }


    // 그림 보내기 POST
    public string[] testUrl;
    public IEnumerator UploadTextureAsPng(int userId, int lessonId)
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
                // 이미지
                avatarImgUrl = avatarData.avatarImg;
                // 동영상
                animationUrls = avatarData.animations.Select(anim => anim.animation).ToList();

                // 업로드 완료되면 UI 활성화 관리
                PaintUI.SetActive(false);
                ChooseCharacterUI.SetActive(true);
                btn_ToMap.SetActive(true);

                // 수정
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber - 1;

                // 동기화
                Debug.LogWarning("유저 아이디 머야 : " + avatarData.userId + ", " + userId);
                photonView.RPC(nameof(SyncAvatarData), RpcTarget.All, avatarData.userId, avatarData.lessonId, avatarImgUrl, animationUrls.ToArray(), actorNumber);
                print(actorNumber);
            }
        };

        yield return StartCoroutine(HttpManager.GetInstance().UploadFileByFormDataArt(info, pngData, userId, lessonId));
    }

    [PunRPC]
    void SyncAvatarData(int userId, int lessonId, string avatarImgUrl, string[] animationUrls, int actorNumber)
    {
        // 이미지 다운로드 받아오기
        StartCoroutine(OnDownloadImage(userId, avatarImgUrl, actorNumber));


        // 애니메이션 다운로드 받아오기
        for (int i = 0; i < animationUrls.Length; i++)
        {
            StartCoroutine(DownloadVideo(userId, animationUrls[i], $"animation_{i}", actorNumber));
        }

    }

    // allPlayers 상태 디버깅 출력 메서드
    private void DebugAllPlayers()
    {
        Debug.Log("allPlayers 상태:");
        foreach (var player in bookController.allPlayers)
        {
            Debug.Log($"Key: {player.Key}, Value: {player.Value.Owner.NickName}");
        }
    }

    int characterNum = 0;

    // actorNum = chararcterNum으로 받아옴 
    [PunRPC]
    public IEnumerator OnDownloadImage(int userId, string imageUrl, int actorNum)
    {

        DebugAllPlayers();


        //if( PhotonNetwork.LocalPlayer.ActorNumber == 1)
        //{
        //    Debug.Log("연동 제외");
        //    yield break;
        //}

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
                if (actorNum != 0)
                {
                    characterNum = bookController.allPlayers[actorNum - 1].GetComponent<Y_PlayerAvatarSetting>().avatarIndex;
                    // 유저가 선택한 캐릭터 화면에 맞게 떠야함
                    bookController.buttons[characterNum].GetComponent<Image>().sprite = receivedSprite;

                    Debug.LogWarning("charNum 체크 " + characterNum);

                    Debug.Log("아바타 이미지가 UI에 성공적으로 적용되었습니다.");

                }


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
        //if (fileName.Equals("animation_1"))
        //{
        //    yield return new WaitForSeconds(3f);
        //}

        //string otherUserUrl = "http://211.250.74.75:8202/api/avatar/participant";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(videoUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                byte[] videoData = webRequest.downloadHandler.data;

                print(fileName);

                // 고유한 파일 이름을 생성하여 저장 경로 설정
                string uniqueFileName = $"{fileName}_{actorNumber}.mp4";
                string filePath = Application.persistentDataPath + "/" + actorNumber;// + "/" +  uniqueFileName;
                if (Directory.Exists(filePath) == false)
                {
                    Directory.CreateDirectory(filePath);
                }

                filePath += "/" + uniqueFileName;

                //System.IO.File.WriteAllBytes(filePath, videoData);

                //yield return new WaitUntil(() =>
                //{
                //    FileInfo a = new FileInfo(filePath);
                //    if (a.Length == 0)
                //    {
                //        print("변환중");
                //        return false;
                //    }
                //    else
                //    {
                //        print("변환완료");

                //        return true;
                //    }

                //}
                //);

                yield return new WaitForSeconds(0.1f);

                FileInfo a = new FileInfo(filePath);


                if (a == null)
                {
                    System.IO.File.WriteAllBytes(filePath, videoData);
                }
                else
                {
                    if(FileOpenCheck(filePath))
                    {
                        System.IO.File.WriteAllBytes(filePath, videoData);
                    }
                    else
                    {
                        yield return new WaitUntil(() => FileOpenCheck(filePath));
                    }

                }


                Debug.Log($"MP4 파일 다운로드 및 저장 성공: {filePath}");

                // 로컬 파일을 위한 파일 프로토콜 추가
                string videoPathWithProtocol = "file:///" + filePath.Replace("\\", "/");
                Debug.Log("videoPath : 들어왔니? " + videoPathWithProtocol);
                // K_AvatarVpSettings에서 상태별 비디오 경로 설정
                //Debug.LogError("Null 인가? 아닌가? : " + (bookController.allPlayers[actorNumber - 1] == null) + " ActorNum : " + bookController.allPlayers[actorNumber - 1].Owner.ActorNumber);
                K_AvatarVpSettings avatarSettings = null;
                if (actorNumber > 0)
                {
                    avatarSettings = bookController.allPlayers[actorNumber - 1].GetComponent<K_AvatarVpSettings>();
                }
                print("avatarSettings 들어왔니? : " + avatarSettings);

                // 파일 이름이 "animation_0"일 경우 idle 경로 설정, "animation_1"일 경우 walk 경로 설정
                if (fileName.Equals("animation_0"))
                {
                    print("animation_0 니? 웅");
                    if (avatarSettings != null) avatarSettings.SetVideoPath(videoPathWithProtocol, null, actorNumber);
                }
                else if (fileName.Equals("animation_1"))
                {
                    print("animation_1 니? 웅");
                    if (avatarSettings != null)
                    {
                        avatarSettings.SetVideoPath(null, videoPathWithProtocol, actorNumber);
                        int actorNum = PhotonNetwork.LocalPlayer.ActorNumber - 1;
                    }
                }



                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // 애니메이션 데이터를 로컬 파일로 저장
                // 모바일 -> persistnetDataPath 무조건 사용
                // 다른 컴퓨터 환경에서 테스트해보기
                //string filePath;
                ////#if UNITY_EDITOR
                //filePath = Application.persistentDataPath + "/" + PhotonNetwork.LocalPlayer.ActorNumber + "/" + fileName + actorNumber + ".mp4";
                ////#else
                ////filePath = Application.persistentDataPath + "/" + fileName + actorNumber + ".mp4";
                ////#endif
                //System.IO.File.WriteAllBytes(filePath, videoData);

                //Debug.Log($"MP4 파일 다운로드 및 저장 성공: {filePath}");

                //string videoPathWithProtocol = " " + filePath;

                //// RPC 비디오 파일 경로 동기화
                ////photonView.RPC(nameof(OnDownloadImage), RpcTarget.All, userId, avatarImgUrl, actorNumber);
                ////photonView.RPC(nameof(ApplayVideoToPlayer), RpcTarget.All, videoPathWithProtocol, actorNumber);

                //// 비디오 경로 설정해주기 -> 이거 일단 주석
                ////Y_BookController.Instance.allPlayers[actorNumber - 1].GetComponent<K_AvatarVpSettings>().SetVideoPath(videoPathWithProtocol, actorNumber);

                //// 백엔드에서 애니메이션 받아올때 animation0, animation1 이렇게줌 
                //if (fileName.Equals($"animation_0"))
                //{
                //    Y_BookController.Instance.allPlayers[actorNumber - 1].GetComponent<K_AvatarVpSettings>().SetVideoPath(videoPathWithProtocol, actorNumber);
                //}
                //else if (fileName.Equals($"animation_1"))
                //{
                //    Y_BookController.Instance.allPlayers[actorNumber - 1].GetComponent<K_AvatarVpSettings>().SetVideoPath(videoPathWithProtocol, actorNumber);
                //}
            }
            else
            {
                Debug.LogError("MP4 다운로드 실패: " + webRequest.error);
            }
        }
    }

    

    public bool FileOpenCheck(string path)
    {
        FileStream stream = null;

        try
        {
            stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }
        catch (IOException e)
        {
            return false;
        }
        finally
        {
            if (stream != null)
            {
                stream.Close();

            }
        }

        return true;
    }

    void RPC_AddUrls(int actorNum, string videoPathWithProtocol)
    {
        photonView.RPC(nameof(AddUrls), RpcTarget.All, actorNum, videoPathWithProtocol);
    }

    [PunRPC]
    void AddUrls(int actorNum, string videoPathWithProtocol)
    {
        print(videoPathWithProtocol);
        print(videoPathWithProtocol.ElementAt(videoPathWithProtocol.Length - 5).ToString());

        int urlNum = (int.Parse(videoPathWithProtocol.ElementAt(videoPathWithProtocol.Length - 5).ToString()));

        print(urlNum);

        Y_GameManager.instance.urls[urlNum - 1] = videoPathWithProtocol;

    }

    // 아바타 생성, 다른 유저 데이터 가져오기
    private IEnumerator CreateAndFetchOtherAvatars(int userId, int lessonId)
    {
        print("호출 1");
        // 내 아바타랑 이미지 -> 서버에 업로드
        yield return StartCoroutine(UploadTextureAsPng(userId, lessonId));

        // 서버 응답에서 받은 url 통해서 ui에 띄울거임
        //yield return StartCoroutine(OnDownloadImage(userId, avatarImgUrl, PhotonNetwork.LocalPlayer.ActorNumber));

        print("다른 유저 데이터 가져오기");

        //업로드 완료 시, 다른 유저들의 아바타 데이터를 가져옴
        List<int> otherUserIds = GetOtherUserIds();
        // 디버그
        Debug.Log("다른 유저 ID 리스트: " + string.Join(", ", otherUserIds));

        if (otherUserIds.Count > 0)
        {
            yield return StartCoroutine(GetAvatarData(lessonId, otherUserIds));
        }
        else
        {
            Debug.Log("다른 유저 없음: 아바타 데이터를 가져올 유저가 없습니다.");
        }
    }

    // 아바타 정보 요청 (GET)
    public IEnumerator GetAvatarData(int lessonId, List<int> userIds)
    {
        foreach (var userId in userIds)
        {
            // 동적으로 URL 생성
            string specificUserUrl = $"http://211.250.74.75:8202/api/avatar/participant/{Y_HttpRoomSetUp.GetInstance().userlessonId}/{Convert.ToInt32(Y_HttpLogIn.GetInstance().userId)}";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(specificUserUrl))
            {
                // 헤더 검증
                webRequest.SetRequestHeader("userId", Y_HttpLogIn.GetInstance().userId.ToString());

                print("서버에게 GET 요청 갔는지" + specificUserUrl);

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"GET 응답 받음: {webRequest.downloadHandler.text}");

                    ////////////////// HZ
                    //// JSON 데이터 파싱
                    //UserAvatarData avatarData = JsonUtility.FromJson<UserAvatarData>(webRequest.downloadHandler.text);

                    //// 이미지, 애니메이션 다운로드
                    //StartCoroutine(OnDownloadImage(avatarData.userId, avatarData.avatarImg, PhotonNetwork.LocalPlayer.ActorNumber));

                    //for (int i = 0; i < avatarData.animations.Count; i++)
                    //{
                    //    StartCoroutine(DownloadVideo(avatarData.userId, avatarData.animations[i].animation, $"animation_{avatarData.userId}_{i}", PhotonNetwork.LocalPlayer.ActorNumber));

                    //}

                    Debug.Log("다른 유저 데이터 가져옴");
                }
                else
                {
                    Debug.LogError("다른 유저 데이터 가져오기 실패" + webRequest.error);
                }
            }
        }
    }


    private List<int> GetOtherUserIds()
    {
        List<int> otherUserIds = new List<int>();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            // ActorNum 1은 제외 (선생님)
            if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber && player.ActorNumber != 1)
            {
                otherUserIds.Add(player.ActorNumber);
            }

            print(player.ActorNumber);
        }
        return otherUserIds;
    }

    //[PunRPC]
    //private void ApplayVideoToPlayer(string videoPath, int actorNumber)
    //{
    //    Y_BookController.Instance.allPlayers[actorNumber - 1].GetComponent<K_AvatarVpSettings>().SetVideoPath(videoPath, actorNumber);
    //}

    // 캐릭터 생성하기 버튼 누르면 서버에 전송
    public void CreateAvatar()
    {
        Debug.Log("createAvatar 호출됨");

        // UI 변경 -> 버튼 OFF로
        btn_CreateAvatar.gameObject.SetActive(false);


        // 생성중 UI 처리
        K_PaintController paintController = PaintUI.GetComponent<K_PaintController>();
        StartCoroutine(paintController.AvatarLoading());

        if (!PhotonNetwork.IsMasterClient) bookController.RPC_IncreaseClickSelectCount();

        // 내 아바타 보내고, 다른 유저의 데이터도 가져올꺼임
        StartCoroutine(CreateAndFetchOtherAvatars(Convert.ToInt32(Y_HttpLogIn.GetInstance().userId), Y_HttpRoomSetUp.GetInstance().userlessonId));

    }



    [System.Serializable]
    private class AvatarDataListWrapper
    {
        public List<UserAvatarData> items;
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