using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class K_AvatarVpSettings : MonoBehaviourPun
{
    // 상태전환
    private enum AnimState
    {
        Idle,
        Walk
    }
    // 기본 셋팅 Idle 상태
    private AnimState currState = AnimState.Idle;

    private string idleUrl;
    private string walkUrl;

    PhotonView pv;
    int index;
    string name;

    Y_BookController bookController;

    public int avatarIndex;

    public VideoPlayer vp;
    public RawImage rawImage;
    public RenderTexture[] renderTextures;
    public VideoClip[] videoClips;
    public Sprite[] images;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        bookController = GameObject.Find("BookCanvas").GetComponent<Y_BookController>();
        bookController.AddPlayer(pv);
        index = pv.Owner.ActorNumber - 1;
        name = pv.Owner.NickName;
        print(PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public void RPC_SelectCharNum(int characterIndex)
    {
        if(pv != null)
        {
            pv.RPC(nameof(SelectCharNum), RpcTarget.AllBuffered, characterIndex);
        }
        else
        {
            Debug.Log("rpc업슴");
        }
        
    }

    [PunRPC]
    void SelectCharNum(int characterIndex)
    {

        avatarIndex = characterIndex - 1;
        // 원래 videClips[avatarIndex]
        vp.clip = videoClips[avatarIndex - 1];

        // Debug
        if(avatarIndex < 0 || avatarIndex >= renderTextures.Length)
        {
            Debug.Log("Invalid avatarIndex : " + avatarIndex + ". Array length is " + renderTextures.Length);
            return;
        }

        if(rawImage == null || vp == null)
        {
            Debug.Log("로우이미지 vp 업슴");
            return;
        }

        if (renderTextures[avatarIndex] == null)
        {
            Debug.LogError("Render texture at index " + avatarIndex + " is null");
            return;
        }


        //print(avatarIndex);
        rawImage.texture = vp.targetTexture = renderTextures[avatarIndex];
        //print("renderTexture" + renderTextures[avatarIndex]);
    }

    // 서버에서 전달받은 비디오 URL 적용

    // MP4 다운로드 및 적용
    public void SetVideoPath(string videoPath, int actorNumber)
    {

        // ?????????????????
        //int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        int adjustActorNumber = actorNumber - 1;
        //print("알려줘" + adjustActorNumber);

        if (vp != null && adjustActorNumber >= 0 && adjustActorNumber <= 3)
        {
            vp.url = videoPath;
            print(videoPath);
            rawImage.texture = vp.targetTexture = renderTextures[adjustActorNumber ];
            //vp.clip = videoClips[actorNumber];
            vp.Play();
        }
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        vp.Play();
        Debug.Log("비디오가 성공적으로 재생되었습니다: " + vp.url);
    }

    public void SetAvatarImage(Texture2D texture)
    {
        if (rawImage != null && texture != null)
        {
            Sprite avatarSprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
            rawImage.texture = texture;
            Debug.Log("아바타 이미지가 성공적으로 적용되었습니다.");
        }
    }

    public void RPC_UpdatePhoto(int index)
    {
        pv.RPC(nameof(UpdatePhoto), RpcTarget.All, index);
    }

    [PunRPC]
    void UpdatePhoto(int index)
    {
        avatarIndex = index - 1;
        //print("avatarIndex from UpdatePhoto: " + avatarIndex);
        bookController.buttons[avatarIndex].GetComponent<Image>().sprite = images[avatarIndex];
    }
}
