using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
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
    private AnimState currState = AnimState.Walk;

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
    public NavMeshAgent agent;
    int layerMaskGround;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        bookController = GameObject.Find("BookCanvas").GetComponent<Y_BookController>();
        bookController.AddAllPlayer(pv);
        index = pv.Owner.ActorNumber - 1;
        name = pv.Owner.NickName;
        y_PlayerMove = GetComponent<Y_PlayerMove>();
        layerMaskGround = LayerMask.GetMask("Ground");
    }

    private void Update()
    {
        bool isMove = y_PlayerMove.moveDistance > 0.1f;

        //// W, A, S, D 중 하나라도 눌려 있으면 walk 상태로 설정
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            // Walk 상태
            SetWalkingState(isMove);
        }       
        else
        {
            // Idle 상태
            SetWalkingState(isMove);
        }

        // 모바일 빌드용
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
                        // Walk 상태
                        SetWalkingState(isMove);
                    }
                }
            }
            else
            {
                SetWalkingState(isMove);
            }
        }

    }

    public void RPC_SelectCharNum(int characterIndex)
    {
        if(pv != null)
        {
            pv.RPC(nameof(SelectCharNum), RpcTarget.AllBuffered, characterIndex);
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
            return;
        }

        if(rawImage == null || vp == null)
        {
            return;
        }

        if (renderTextures[avatarIndex] == null)
        {
            return;
        }
        
        rawImage.texture = vp.targetTexture = renderTextures[avatarIndex];
    }

    // 서버에서 전달받은 비디오 URL 적용

    // MP4 다운로드 및 적용
    public void SetVideoPath(/*string videoPath*/ string idlePath, string walkPath, int actorNumber)
    {
        int adjustActorNumber = actorNumber - 1;

        print("adjustActorNum 뭐니? " + adjustActorNumber);

        if (vp != null && adjustActorNumber >= 0 && adjustActorNumber <= 3)
        {
            vp.targetTexture = renderTextures[adjustActorNumber];
            rawImage.material = new Material(rawImage.material);
            rawImage.material.mainTexture = vp.targetTexture;
            
            if (idlePath != null)
            {
                idleUrl = idlePath;
                PlayCurrAnim();
            }

            if(walkPath != null)
            {
                walkUrl = walkPath;
            }
        }
    }

    public void SetWalkingState(bool walking)
    {
        isWalking = walking;
        if(walking && currState == AnimState.Idle)
        {
            vp.url = walkUrl;
            vp.Play();
            currState = isWalking ? AnimState.Walk : AnimState.Idle;
        }
        else if(!walking && currState == AnimState.Walk)
        {
            vp.url = idleUrl;
            currState = isWalking ? AnimState.Walk : AnimState.Idle;
            vp.Play();

        }
    }

    public void PlayCurrAnim()
    {
        vp.url = idleUrl;
        vp.Play();
    }



    private void OnVideoPrepared(VideoPlayer source)
    {
        vp.Play();
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
        bookController.buttons[avatarIndex].GetComponent<Image>().sprite = images[avatarIndex];
    }
}
