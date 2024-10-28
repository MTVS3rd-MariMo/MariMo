using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Y_PlayerAvatarSetting : MonoBehaviour
{
    ///////////////////////////////   

    public PhotonView pv;
    int index;
    string name;

    Y_BookController bookController;

    public int avatarIndex;
    public string mySelfIntroduce;

    public VideoPlayer vp;
    public RawImage rawImage;
    public RenderTexture[] renderTextures;
    public VideoClip[] videoClips;
    public Sprite[] images;
    public string[] selfIntroduces = new string[4];

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        bookController = GameObject.Find("BookCanvas").GetComponent<Y_BookController>();
        bookController.AddPlayer(pv);
        index = pv.Owner.ActorNumber - 1;
        name = pv.Owner.NickName;
        print(PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public void RPC_SelectChar(int characterIndex)
    {
        pv.RPC(nameof(SelectChar), RpcTarget.All, characterIndex);
    }

    [PunRPC]
    void SelectChar(int characterIndex)
    {
        avatarIndex = characterIndex - 1;
        //print("avatarIndex from SelectChar: " + avatarIndex);

        // MP4 -> characterIndex
        vp.clip = videoClips[avatarIndex];
        // VideoRenderer
        rawImage.texture = vp.targetTexture = renderTextures[avatarIndex];
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
