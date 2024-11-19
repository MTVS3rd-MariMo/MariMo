using Cinemachine;
using Org.BouncyCastle.Asn1.Crmf;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
//using UnityEngine.tvOS;

// 버튼들에 연결하는 식들
// UI 에서 참조하는 변수들

public class Y_BookController : MonoBehaviourPun
{
    #region Server
    // Server 
    K_HttpAvatar k_HttpAvatar;
    private string uploadUrl = "http://192.168.0.113:9988/api/avatar/upload-img";
    private string avatarImgUrl;
    private List<string> animationUrls;

    // 아바타 보내기
    private RawImage rawImage;
    private Button btn_CreateAvatar;
    //public GameObject PaintUI;

    // 아바타 이미지 받기
    //public GameObject ChooseCharacterUI;
    // 선택된 아바타 UI
    private Image avatarImage;
    private GameObject btn_MakeAvatar;
    private GameObject btn_ToMap;

    // VideoPlayer



    #endregion

    //public static Y_BookController Instance { get; private set; }

    public Camera mainCam;
    public CinemachineVirtualCamera virtualCam;

    public GameObject bookUI;
    public GameObject ChooseCharacterUI;
    public GameObject PaintUI;
    //public GameObject ChooseCompleteUI;
    public PhotonView pv;

    // 오브젝트 애니메이션
    public GameObject Ani_Object;

    #region Book
    public string text = "수남이는 청계천 세운상가 뒷길의 전기 용품 도매상의 꼬마 점원이다.\r\n수남이란 어엿한 이름이 있는데도 꼬마로 통한다. 열여섯 살이라지만 볼은 아직 어린아이처럼 토실하니 붉고, 눈 속이 깨끗하다. 숙성한 건 목소리뿐이다. 제법 굵고 부드러운 저음이다. 그 목소리가 전화선을 타면 점잖고 떨떠름한 늙은이 목소리로 들린다.";
    List<string> texts;

    public TMP_Text pageNoTxt;
    public int pageNo;

    public TMP_Text leftTxt;
    public TMP_Text rightTxt;

    public RectTransform leftTextBox;
    public RectTransform rightTextBox;

    public GameObject img_loading;
    public GameObject img_loadingbar;

    public GameObject titleText;
    public GameObject authorText;
    public GameObject titleImg;

    #endregion

    #region ChooseCharacter

    public GameObject img_charName1;
    public GameObject img_charName2;
    public GameObject img_charName3;
    public GameObject img_charName4;
    public GameObject btn_chooseChar;
    public GameObject btn_toMap;

    // 버튼 어떤 게 눌렸나 받아오기
    public int characterNum = 0;

    GameObject player;
    public int currentPlayerNum;
    public string playerName;   
    public Dictionary<int, string> playerNames = new Dictionary<int, string>(); // int, PhotonView 로 고치고

    public GameObject[] buttons;

    public Button[] buttonsToChange;
    public Sprite[] buttonSprites;

    #endregion


    private void Start()
    {
        mainCam = Camera.main;

        pv = GetComponent<PhotonView>();

        texts = new List<string>();
        pageNo = int.Parse(pageNoTxt.text);

        Y_SoundManager.instance.StopBgmSound();

        img_loading = GameObject.Find("Img_Loading");
        img_loadingbar = GameObject.Find("Img_LoadingBar");

        StartCoroutine(loadingCoroutine());

        //StartCoroutine(SplitTextIntoPages());
        //StartCoroutine(DisplayPage(pageNo));
    }

    IEnumerator loadingCoroutine()
    {
        float time = 0;
        //float fillAmount = img_loadingbar.GetComponent<Image>().fillAmount;
        while (time < 5f)
        {
            time += Time.deltaTime;
            img_loadingbar.GetComponent<Image>().fillAmount = Mathf.Lerp(img_loadingbar.GetComponent<Image>().fillAmount, 1, time/2);
            yield return null;
        }
    }

    public bool isSync = true;

    private void Update()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 2 && isSync) // 첫번째 조건이 PhotonNetwork.IsMasterClient 조건이었음
        {
            SyncAllPlayerNames();
            List<int> keys = new List<int>(allPlayers.Keys);
        }

        if (isSync) SyncAllPlayers();

        if (PhotonNetwork.PlayerList.Length == 5 && isSync) //////////////
        {
            StartCoroutine(WaitUntilSync());
        }
    }

    IEnumerator WaitUntilSync()
    {
        yield return new WaitForSeconds(1f);
        //foreach(KeyValuePair<int, PhotonView> kv in allPlayers)
        //{
        //    Debug.LogError("allPlayers : " + kv.Key);
        //}
        isSync =  false;
    }


    // 플레이어가 참여할 때 호출
    public void RPC_AddPlayerNames(int playerIndex, string nickName)
    {
        if (!playerNames.ContainsKey(playerIndex))
        {
            //마스터 클라이언트인 경우에만 전체 동기화 실행
            //if (PhotonNetwork.IsMasterClient) ///// 액터 넘버 2일 경우에? 그리고 마스터 클라이언트일 경우에는 그냥 리턴해야
            //{
            //    SyncAllPlayers();
            //}
            if (PhotonNetwork.LocalPlayer.ActorNumber >= 2)
            {
                SyncAllPlayerNames();
            }
            else if (PhotonNetwork.IsMasterClient) ///// 액터 넘버 2일 경우에? 그리고 마스터 클라이언트일 경우에는 그냥 리턴해야
            {
                return;
            }
            else
            {
                // 마스터가 아닌 경우 자신의 정보만 전송
                pv.RPC(nameof(AddPlayerNames), RpcTarget.All, playerIndex, nickName);
            }
        }
    }

    // 전체 플레이어 동기화
    public void SyncAllPlayerNames()
    {
        // 현재 룸의 모든 플레이어 정보 전송
        foreach (var player in PhotonNetwork.PlayerList)
        {
            int idx = player.ActorNumber - 1;
            string name = player.NickName;
            if(idx > 0) pv.RPC(nameof(AddPlayerNames), RpcTarget.All, idx - 1, name);
        }
    }

    [PunRPC]
    void AddPlayerNames(int playerIndex, string nickName)
    {
        if(playerIndex >= 0) playerNames[playerIndex] = nickName;
    }

    #region BookUI

    // 텍스트 쪼개기
    public IEnumerator SplitTextIntoPages()
    {
        yield return new WaitForSeconds(2f);

        GameObject.Find("MapSetUpManager").GetComponent<Y_MapSetUp>().ApplyClassMaterial();

        string[] words = text.Split(' ');
        string currentText = "";

        foreach (string word in words)
        {
            string tempText = currentText + (currentText.Length > 0 ? " " : "") + word;

            // 왼쪽 텍스트 박스에 맞는지 확인
            leftTxt.text = tempText;
            Vector2 leftSize = leftTxt.GetPreferredValues(leftTextBox.rect.size.x, leftTextBox.rect.size.y);

            if (leftSize.y > leftTextBox.rect.size.y)
            {
                texts.Add(currentText);
                currentText = word;
            }
            else
            {
                currentText = tempText;
            }
        }

        if (!string.IsNullOrEmpty(currentText))
        {
            texts.Add(currentText);
        }

        leftTxt.text = "";
        DisplayPage(0);
    }

    // 페이지에 띄우기
    void DisplayPage(int pageIndex)
    {
        //yield return new WaitForSeconds(3f);

        pageNoTxt.text = pageIndex.ToString();

        if(pageIndex > 0)
        {
            titleText.SetActive(false);
            authorText.SetActive(false);
            titleImg.SetActive(true);

            leftTxt.text = texts[pageNo * 2 - 2];
            if (pageNo * 2 - 1 < texts.Count)
            {
                rightTxt.text = texts[pageNo * 2 - 1];
            }
            else if (pageNo * 2 > texts.Count)
            {
                rightTxt.text = "";
            }
        }
    }

    // 페이지 왼쪽 넘김
    public void left()
    {
        pageNo = Mathf.Max(0, --pageNo);
        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_BUTTON);
        DisplayPage(pageNo);
    }

    // 페이지 오른쪽 넘김
    public void right()
    {
        if (pageNo + 1 > (texts.Count + 1) / 2)
        {
            bookUI.SetActive(false);
            Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_BUTTON);
            ChooseCharacterUI.SetActive(true);
        }
        else
        {
            pageNo = Mathf.Min((texts.Count + 1) / 2, ++pageNo);
            Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_BUTTON);
            DisplayPage(pageNo);
        }  
    }

    #endregion

    #region ChooseCharacterUI

    // 다른 사람이 버튼 이미 눌렀으면 못 선택하게 나중에 처리할 것
    /////////////////////////////////////////////

    int clickSelectCnt = 0;
    int clickPaintCnt = 0;

    public void Select(int num)
    {
        // 선생님이면 아무 일도 일어나지 않는다
        if (currentPlayerNum == -1) return;

        // 이미 선택된 아바타 번호라면 아무 일도 하지 않음
        if (IsCharacterSelected(num)) return;

        // 자신이 이미 선택한 번호를 다시 클릭했을 경우 선택 해제
        if (characterNum == num)
        {
            RPC_DeactivateAllNameUI(characterNum);
            RPC_DecreaseCnt();
            characterNum = 0; // 초기화 (아바타 선택 해제)
            allPlayers[currentPlayerNum].GetComponent<Y_PlayerAvatarSetting>().RPC_SelectChar(characterNum);
            Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_BUTTON);

            RPC_InteractableFalse();

            return;
        }

        if(characterNum == 0) RPC_IncreaseCnt();

        RPC_DeactivateAllNameUI(characterNum);
        characterNum = num;
        RPC_ActivateNameUI(characterNum, currentPlayerNum); // 5명으로 바꾸면서 currentPlayerNum + 1 로 바꿨었는데 이제 다시 currentPlayerNum 으로 바꿈??

        // 아바타 인덱스를 설정한다. 
        allPlayers[currentPlayerNum].GetComponent<Y_PlayerAvatarSetting>().RPC_SelectChar(characterNum);

        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_BUTTON);
    }

    // 다른 플레이어가 이미 선택한 캐릭터 번호인지 확인
    private bool IsCharacterSelected(int num)
    {
        foreach (var player in allPlayers)
        {
            int actorNumber = player.Key;
            // 현재 플레이어는 제외
            if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber - 2) continue;

            var avatarSetting = player.Value.GetComponent<Y_PlayerAvatarSetting>();
            if (avatarSetting != null && (avatarSetting.avatarIndex + 1) == num)
            {
                return true; // 다른 플레이어가 이미 선택한 번호임
            }
        }
        return false;
    }

    void RPC_InteractableFalse()
    {
        photonView.RPC(nameof(InteractableFalse), RpcTarget.All);
    }

    [PunRPC]
    void InteractableFalse()
    {
        // 만약 캐릭터 선택하기가 활성화되어 있으면 다시 비활성화
        btn_chooseChar.GetComponent<Image>().sprite = buttonSprites[7];
        btn_chooseChar.GetComponent<Button>().interactable = false;
    }

    public void RPC_DecreaseCnt()
    {
        photonView.RPC(nameof(DecreaseCnt), RpcTarget.All);
    }

    [PunRPC]
    void DecreaseCnt()
    {
        clickSelectCnt--;
    }

    public void RPC_IncreaseCnt()
    {
        photonView.RPC(nameof(IncreaseCnt), RpcTarget.All);
    }

    [PunRPC]
    void IncreaseCnt()
    {
        clickSelectCnt++;

        if(clickSelectCnt >= 4)
        {
            btn_chooseChar.GetComponent<Image>().sprite = buttonSprites[6];
            btn_chooseChar.GetComponent<Button>().interactable = true;
        }
    }

    public void RPC_IncreaseClickSelectCount()
    {
        photonView.RPC(nameof(IncreaseClickSelectCount), RpcTarget.All);
    }

    [PunRPC]
    void IncreaseClickSelectCount()
    {
        if (PhotonNetwork.IsMasterClient) return;

        clickPaintCnt++;
        Debug.LogError(clickPaintCnt);

        if(clickPaintCnt >= 4)
        {
            btn_toMap.GetComponent<Image>().sprite = buttonSprites[5];
            btn_toMap.GetComponent<Button>().interactable = true;
        }    
    }
   
     public void RPC_ActivateNameUI(int characterIndex, int playerIndex)
     {  
        pv.RPC(nameof(ActivateNameUI), RpcTarget.All, characterIndex, playerIndex);
     }

    [PunRPC]
    void ActivateNameUI(int characterIndex, int playerIndex)
    {
        string playerName = playerNames.ContainsKey(playerIndex) ? playerNames[playerIndex] : "Not Found";

        switch (characterIndex)
        {
            case 1:
                img_charName1.SetActive(true);
                img_charName1.GetComponentInChildren<TMP_Text>().text = playerName;
                break;
            case 2:
                img_charName2.SetActive(true);
                img_charName2.GetComponentInChildren<TMP_Text>().text = playerName;
                break;
            case 3:
                img_charName3.SetActive(true);
                img_charName3.GetComponentInChildren<TMP_Text>().text = playerName;
                break;
            case 4:
                img_charName4.SetActive(true);
                img_charName4.GetComponentInChildren<TMP_Text>().text = playerName;
                break;
        }
    }

    public void RPC_DeactivateAllNameUI(int characterIndex)
    {
        pv.RPC("DeactivateAllNameUI", RpcTarget.All, characterIndex);
    }

    [PunRPC]
    void DeactivateAllNameUI(int characterIndex)
    {
        switch (characterIndex)
        {
            case 1:
                img_charName1.SetActive(false);
                break;
            case 2:
                img_charName2.SetActive(false);
                break;
            case 3:
                img_charName3.SetActive(false);
                break;
            case 4:
                img_charName4.SetActive(false);
                break;
        }       
    }

    public void FinishSelect()
    {
        // 나중에 백엔드에게 어떤 캐릭터 골랐는지 보내주기
        // CharacterNum 을 넘겨줘야 할듯....
        //////////////////////////////

        ChooseCharacterUI.SetActive(false);
        //mainCam.gameObject.SetActive(false);
        //virtualCam.gameObject.SetActive(false);
        //paintCam.gameObject.SetActive(true);
        
        PaintUI.SetActive(true);

        /////////22222222
        // currentPlayerNum 에 따라 RenderTexture, characterNum에 따라 MP4 파일을 설정
        //SetCharacterVideo(currentPlayerNum, characterNum);

        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_BUTTON);

    }

    // 비디오 다운로드 후 설정
    private IEnumerator DownloadAndSetVideo(string videoUrl, VideoPlayer videoPlayer)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(videoUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                byte[] videoData = webRequest.downloadHandler.data;
                string filePath = Path.Combine(Application.persistentDataPath, "character_animation.mp4");
                File.WriteAllBytes(filePath, videoData);

                videoPlayer.url = filePath;
                videoPlayer.Prepare();
                videoPlayer.Play();

                Debug.Log($"애니메이션 다운로드 및 설정 완료: {filePath}");
            }
            else
            {
                Debug.LogError("애니메이션 다운로드 실패: " + webRequest.error);
            }
        }
    }

    public void PaintToComplete()
    {
        // 이 때 AI 한테 이미지 보내주는 거 나중에 구현할 것
        ////////////////////////////
        ///

        // 각 플레이어마다 루프 돌면서 이미지 넣어줌

        //mainCam.gameObject.SetActive(true);
        //virtualCam.gameObject.SetActive(true);
        //paintCam.gameObject.SetActive(false);
        //if(currentPlayerNum >= 0) allPlayers[currentPlayerNum].GetComponent<Y_PlayerAvatarSetting>().RPC_UpdatePhoto(characterNum);
        //PaintUI.SetActive(false);
        //ChooseCharacterUI.SetActive(true);
        for(int i = 0; i < buttons.Length; i++)
        {
            TMP_Text textComponent = buttons[i].GetComponentInChildren<TMP_Text>();
            Color color = textComponent.color;
            color.a = 0;
            textComponent.color = color;
        }

        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_BUTTON);
        Y_SoundManager.instance.PlayBgmSound(Y_SoundManager.EBgmType.BGM_MAIN);

        btn_chooseChar.SetActive(false);
        btn_toMap.SetActive(true);
    }

    public void ToMap()
    {
        // 이 때 캐릭터 받아와서 내 플레이어에 동기화 시켜야 됨
        ////////////////////////
        if(!PhotonNetwork.IsMasterClient)
        {
            RPC_IncreaseMapCount(characterNum);
        }
        btn_toMap.GetComponent<Image>().sprite = buttonSprites[4];
        btn_toMap.GetComponent<Button>().interactable = false;
    }

    int mapCnt = 0;
    public GameObject[] isReadyImgs;

    public void RPC_IncreaseMapCount(int characterNum)
    {
        photonView.RPC(nameof(IncreaseMapCount), RpcTarget.All, characterNum);
    }

    [PunRPC]
    void IncreaseMapCount(int characterNum)
    {
        mapCnt++;
        isReadyImgs[characterNum - 1].SetActive(true);

        if (mapCnt >= 4)
        {
            // 맵 소개 UI 실행
            K_LobbyUiManager.instance.isAllArrived = true;
            Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_BUTTON);
            Ani_Object.SetActive(true);
            Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_3D_OBJECT_01);
            gameObject.SetActive(false);
        }
    }

    #endregion

    public Y_PlayerAvatarSetting myAvatar;
    public Dictionary<int, PhotonView> allPlayers = new Dictionary<int, PhotonView>(); // MyAvatar 로 바꾸기
    public void AddAllPlayer(PhotonView pv)
    {
        // 원래는 -1 이었는데 -2로?
        if (pv.Owner.ActorNumber - 2 >= 0)
        {
            allPlayers[pv.Owner.ActorNumber - 2] = pv;
        }

        if(pv.IsMine)
        {
            myAvatar = pv.GetComponent<Y_PlayerAvatarSetting>();
        }
    }

    //public void RPC_SyncAllPlayers(List<int> keys)
    //{
    //    photonView.RPC(nameof(SyncAllPlayers), RpcTarget.All, keys);
    //}

    public void SyncAllPlayers()
    {
        // Key 리스트를 따로 저장한 후 수정

        for (int i = 0; i < 4; i++)
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if(player.ActorNumber - 2 == i)
                {
                    PhotonView playerPV = GetPhotonViewByActorNumber(player.ActorNumber);
                    allPlayers[i] = playerPV;
                }
            }
            //allPlayers[key] = allPlayers[key];
        }
    }

    // ActorNumber로 PhotonView 찾기
    private PhotonView GetPhotonViewByActorNumber(int actorNumber)
    {
        foreach (var view in FindObjectsOfType<Y_PlayerMove>())
        {
            if (view.pv.Owner != null && view.pv.Owner.ActorNumber == actorNumber)
            {
                return view.pv;
            }
        }
        return null;
    }

}
