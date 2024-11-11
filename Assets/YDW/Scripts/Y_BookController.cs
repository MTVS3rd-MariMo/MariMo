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

    #region Book
    public string text = "수남이는 청계천 세운상가 뒷길의 전기 용품 도매상의 꼬마 점원이다.\r\n수남이란 어엿한 이름이 있는데도 꼬마로 통한다. 열여섯 살이라지만 볼은 아직 어린아이처럼 토실하니 붉고, 눈 속이 깨끗하다. 숙성한 건 목소리뿐이다. 제법 굵고 부드러운 저음이다. 그 목소리가 전화선을 타면 점잖고 떨떠름한 늙은이 목소리로 들린다.\r\n이 가게에는 변두리 전기 상회나 전공들로부터 걸려오는 전화가 잦다. 수남이가 받으면,\r\n\"주인 영감님이십니까?\"\r\n하고 깍듯이 존대를 해 온다.\r\n\"아, 아닙니다. 꼬맙니다.\"\r\n수남이는 제가 무슨 큰 실수나 저지른 것처럼 황공해 하며 볼까지 붉어진다.\r\n\"짜아식, 새벽부터 재수 없게 누굴 놀려. 너 이따 두고 보자.\"\r\n이런 호령이라도 들려오면 수남이는 우선 고개를 움츠려 알밤을 피하는 시늉부터 한다. 설마 전화통에서 알밤이 튀어나올 리는 없는데 말이다. 실수만 했다 하면 알밤 먹을 것을 예상하고 고개가 자라 모가지처럼 오그라드는 게 수남이가 이 곳 전기 상회에 취직하고 나서부터 얻은 조건 반사다.\r\n이 곳 단골 손님들은 우락부락한 전공들이 대부분이어서 성질들이 거칠고 급하다. 자기가 요구하는 것을 수남이가 빨리 알아듣고 척척 챙기지 못하고 조금만 어릿어릿하면 짜아식 하며 사정없이 밤송이 같은 머리에 알밤을 먹인다.\r\n수남이는 그 숱한 전기 용품 이름을 척척 알아들을 수 있을 만큼 일에 익숙해질 때까지 숱한 알밤을 먹었다.\r\n그런데 일에 익숙해진 후에도 수남이는 심심찮게 까닭도 없는 알밤을 얻어먹는다. 이 거친 사내들은 그런 짓궂은 방법으로 수남이를 귀여워하는 것이다. 예쁜 아이를 보면 물어뜯어 울려 놓고 마는 사람이 있듯이, 이 사내들은 그런 방법으로 수남이에게 애정 표시를 했다.\r\n\"짜아식, 잘 잤냐?\"\r\n\"짜아식, 요새 제법 컸단 말야. 장가들여야겠는데, 짜아식 좋아서…….\"\r\n그리곤 알밤이다. 주먹과 팔짓만 허풍스럽게 컸지, 아주 부드러운 알밤이다. 그러니까 수남이는 그만큼 인기 있는 점원인 셈이다.\r\n수남이는 단골 손님들에게만 인기가 있는 게 아니라, 주인 영감에게도 여간 잘 뵌 게 아니다. 누구든지 수남이에게 알밤을 먹이는 걸 들키기만 하면 단박 불호령이 내린다.\r\n\"왜 하필 남의 머리를 쥐어박어? 채 굳지도 않은 머리를. 그게 어떤 머린 줄이나 알고들 그래, 응? 공부 많이 해서 대학도 가고 박사도 될 머리란 말야. 임자들 같은 돌대가리가 아니란 말야.\"\r\n그러면 아무리 막돼먹은 손님이라도 선생님 꾸지람에 떠는 초등 학생처럼 풀이 죽어서 수남이에게 진심으로 미안해했다. 그리고는,\r\n\"꼬마야, 그럼 너 요새 어디 야학이라도 다니니?\"\r\n하며 은근히 부러워하는 눈치까지 보였다. 그러면 영감님은 딱하다는 듯이 혀를 차며,\r\n\"아니, 야학은 아무 때나 들어가나. 똥통 학교라면 또 몰라. 수남이는 내년 봄에 시험 봐서 들어가야 해. 야학이라도 일류로, 그래서 인석이 그저 틈만 있으면 책이라고. 허허…….\"\r\n수남이는 가슴이 크게 출렁거린다. 수남이는 한 번도 주인 영감님에게 하다 못 해 야학이라도 들어가 공부를 해 보고 싶단 말을 비친 적이 없다. 맨 손으로 어린 나이에 서울에 와서 거지도 안 되고 깡패도 안 되고 이런 어엿한 가게의 점원이 된 것만도 수남이로서는 눈부신 성공인데, 벼락맞을 노릇이지, 어떻게 감히 공부까지를 바라겠는가.\r\n그러면서도 자기 또래의 고등 학생만 보면 가슴이 짜릿짜릿하던 수남이다. 처음 전기 용품 취급이 서툴러 시험을 하다 툭하면 손 끝에 감전이 되어 짜릿하며 화들짝 놀랐던 것처럼, 고등 학교 교복은 수남이의 심장에 짜릿한 감전을 일으키며 가슴을 온통 마구 휘젓는 이상한 힘이 있었다.\r\n그런 수남이의 비밀을 주인 영감님은 알고 있었던 것이다. 수남이는 부끄럽고도 기뻤다.\r\n그래서 수남이는 \"내년 봄에 시험 봐서 들어가야 해. 야학이라도 일류로…….\" 할 때의 주인 영감님이 그렇게 좋을 수가 없다. 그 소리를 듣기 위해서라면 그까짓 알밤쯤 하루 골백번을 맞으면 대수랴 싶다. 그런 소리를 자기를 위해 해 주는 주인 영감님을 위해서라면 뼛골이 부러지게 일을 한들 눈꼽만큼도 억울할 것이 없을 것 같다. 월급은 좀 짜게 주지만, 그 감미로운 소리를 어찌 후한 월급에 비기겠는가.\r\n수남이의 하루는 눈코 뜰 새 없이 고단하지만 행복하다. 내년 봄 ― 내년 봄은 올 봄보다는 멀지만 오기는 올 것이다. ";
    List<string> texts;

    public TMP_Text pageNoTxt;
    public int pageNo;

    public TMP_Text leftTxt;
    public TMP_Text rightTxt;

    public RectTransform leftTextBox;
    public RectTransform rightTextBox;

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

        StartCoroutine(SplitTextIntoPages());
        //StartCoroutine(DisplayPage(pageNo));

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
    IEnumerator SplitTextIntoPages()
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

        DisplayPage(pageNo);
    }

    // 페이지에 띄우기
    void DisplayPage(int pageIndex)
    {
        //yield return new WaitForSeconds(3f);

        pageNoTxt.text = pageIndex.ToString();

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

    // 페이지 왼쪽 넘김
    public void left()
    {
        pageNo = Mathf.Max(1, --pageNo);
        DisplayPage(pageNo);
    }

    // 페이지 오른쪽 넘김
    public void right()
    {
        if (pageNo + 1 > (texts.Count + 1) / 2)
        {
            bookUI.SetActive(false);
            ChooseCharacterUI.SetActive(true);
        }
        else
        {
            pageNo = Mathf.Min((texts.Count + 1) / 2, ++pageNo);
            DisplayPage(pageNo);
        }  
    }

    #endregion

    #region ChooseCharacterUI

    // 다른 사람이 버튼 이미 눌렀으면 못 선택하게 나중에 처리할 것
    /////////////////////////////////////////////

    int clickSelectCnt = 0;

    public void Select(int num)
    {
        // 선생님이면 아무 일도 일어나지 않는다
        if (currentPlayerNum == -1) return;

        RPC_DeactivateAllNameUI(characterNum);
        characterNum = num;
        RPC_ActivateNameUI(characterNum, currentPlayerNum); // 5명으로 바꾸면서 currentPlayerNum + 1 로 바꿨었는데 이제 다시 currentPlayerNum 으로 바꿈??

        // 아바타 인덱스를 설정한다. 
        allPlayers[currentPlayerNum].GetComponent<Y_PlayerAvatarSetting>().RPC_SelectChar(characterNum);

        RPC_IncreaseClickSelectCount(); //도원
        //btn_toMap.GetComponent<Button>().interactable = true; // 도원

    }

    void RPC_IncreaseClickSelectCount()
    {
        photonView.RPC(nameof(IncreaseClickSelectCount), RpcTarget.All);
    }

    [PunRPC]
    void IncreaseClickSelectCount()
    {
        clickSelectCnt++;

        if(clickSelectCnt >= 4) btn_toMap.GetComponent<Button>().interactable = true;
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
        if(currentPlayerNum >= 0) allPlayers[currentPlayerNum].GetComponent<Y_PlayerAvatarSetting>().RPC_UpdatePhoto(characterNum);
        PaintUI.SetActive(false);
        ChooseCharacterUI.SetActive(true);
        for(int i = 0; i < buttons.Length; i++)
        {
            TMP_Text textComponent = buttons[i].GetComponentInChildren<TMP_Text>();
            Color color = textComponent.color;
            color.a = 0;
            textComponent.color = color;
        }

        btn_chooseChar.SetActive(false);
        btn_toMap.SetActive(true);
    }

    public void ToMap()
    {
        // 이 때 캐릭터 받아와서 내 플레이어에 동기화 시켜야 됨
        ////////////////////////

        // 맵 소개 UI 실행
        K_LobbyUiManager.instance.isAllArrived = true; 
        gameObject.SetActive(false);
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
