using Photon.Pun;
using Photon.Realtime;
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

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        bookController = GameObject.Find("BookCanvas").GetComponent<Y_BookController>();
        bookController.AddPlayer(pv);
        index = pv.Owner.ActorNumber - 1;
        name = pv.Owner.NickName;

        //SyncAllPlayers();
        //bookController.playerNames[index] = name;
    }

    //private void SyncAllPlayers()
    //{
    //    // 현재 룸의 모든 플레이어 정보 전송
    //    foreach (var player in PhotonNetwork.PlayerList)
    //    {
    //        int idx = player.ActorNumber - 1;
    //        string name = player.NickName;
    //        bookController.playerNames[idx] = name;
    //    }
    //}

    // Update is called once per frame
    void Update()
    {

    }

    public VideoPlayer vp;
    public RawImage rawImage;
    public RenderTexture[] renderTextures;
    public VideoClip[] videoClips;

    public void SelectChar(int characterIndex)
    {
        pv.RPC(nameof(RPC_SelectChar), RpcTarget.All, characterIndex);
    }

    [PunRPC]
    void RPC_SelectChar(int characterIndex)
    {
        avatarIndex = characterIndex - 1;

        // MP4 -> characterIndex
        vp.clip = videoClips[avatarIndex];
        // VideoRenderer
        rawImage.texture = vp.targetTexture = renderTextures[avatarIndex];
    }

}
