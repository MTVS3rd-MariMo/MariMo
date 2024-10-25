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
    ////////////////////////////////
    

    PhotonView pv;
    int index;
    string name;

    Y_BookController bookController;

    int avatarIndex;

    public VideoPlayer vp;
    public RawImage rawImage;
    public RenderTexture[] renderTextures;
    public VideoClip[] videoClips;
    //public GameObject[] buttons;
    public Sprite[] images;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        bookController = GameObject.Find("BookCanvas").GetComponent<Y_BookController>();
        bookController.AddPlayer(pv);
        index = pv.Owner.ActorNumber - 1;
        name = pv.Owner.NickName;

        
    }

    public void RPC_SelectChar(int characterIndex)
    {
        pv.RPC(nameof(SelectChar), RpcTarget.All, characterIndex);
    }

    [PunRPC]
    void SelectChar(int characterIndex)
    {
        avatarIndex = characterIndex - 1;

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
        bookController.buttons[avatarIndex].GetComponent<Image>().sprite = images[avatarIndex];
    }
}
