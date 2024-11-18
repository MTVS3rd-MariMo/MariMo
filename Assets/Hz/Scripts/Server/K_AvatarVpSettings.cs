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

    // 걷냐
    private bool isWalking = false;

    private Y_PlayerMove y_PlayerMove;
    int layerMaskGround;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        bookController = GameObject.Find("BookCanvas").GetComponent<Y_BookController>();
        bookController.AddAllPlayer(pv);
        index = pv.Owner.ActorNumber - 1;
        name = pv.Owner.NickName;
        //print(PhotonNetwork.LocalPlayer.ActorNumber);
        y_PlayerMove = GetComponent<Y_PlayerMove>();
        layerMaskGround = LayerMask.GetMask("Ground");

    }

    private void Update()
    {
        //// W, A, S, D 중 하나라도 눌려 있으면 walk 상태로 설정
        //if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        //{
        //    // Walk 상태
        //    SetWalkingState(true);
        //}
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if ((touch.phase == TouchPhase.Began) || (touch.phase == TouchPhase.Moved))
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 9999f, layerMaskGround))
                    {
                        y_PlayerMove.agent.SetDestination(hit.point);
                    }
                }
            }
        }
        else
        {
            // Idle 상태
            SetWalkingState(false);
        }
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
    public void SetVideoPath(/*string videoPath*/ string idlePath, string walkPath, int actorNumber)
    {
        int adjustActorNumber = actorNumber - 1;

        print("adjustActorNum 뭐니? " + adjustActorNumber);

        if (vp != null && adjustActorNumber >= 0 && adjustActorNumber <= 3)
        {
            if (idlePath != null)
            {
                idleUrl = idlePath;
                print("idleUrl 은 머야 " + idleUrl);
            }

            if(walkPath != null)
            {
                walkUrl = walkPath;
                print("walkUrl 은 머야 " + walkUrl);
            }

            // 연산자 사용해보기
            //print(videoPath);
            vp.targetTexture = renderTextures[adjustActorNumber];
            rawImage.material = new Material(rawImage.material);
            rawImage.material.mainTexture = vp.targetTexture;
            vp.prepareCompleted += OnVideoPrepared;
            print("준비됐나요? ");
            //PlayCurrAnim();
        }



        //// ?????????????????
        ////int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        //int adjustActorNumber = actorNumber - 1;
        ////print("알려줘" + adjustActorNumber);

        //if (vp != null && adjustActorNumber >= 0 && adjustActorNumber <= 3)
        //{
        //    // 연산자 사용해보기
        //    //print(videoPath);
        //    rawImage.texture = vp.targetTexture = renderTextures[adjustActorNumber];
        //    vp.Play();
        //}
    }

    public void SetWalkingState(bool walking)
    {
        isWalking = walking;
        if(walking && currState == AnimState.Idle)
        {
            vp.url = walkUrl;
            vp.Play();
            currState = isWalking ? AnimState.Walk : AnimState.Idle;
            //print("걷기로 전환");
        }
        else if(!walking && currState == AnimState.Walk)
        {
            vp.url = idleUrl;
            currState = isWalking ? AnimState.Walk : AnimState.Idle;
            vp.Play();
            //print("대기로 전환");

        }

        //PlayCurrAnim();

        //print("SetWalkingState?");
    }

    public void PlayCurrAnim()
    {
        vp.url = currState == AnimState.Idle ? idleUrl : walkUrl;
        //vp.Prepare();

        //print("PlayAnim?");
    }



    private void OnVideoPrepared(VideoPlayer source)
    {
        vp.Play();
        //Debug.Log("비디오가 성공적으로 재생되었습니다: " + vp.url);
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
