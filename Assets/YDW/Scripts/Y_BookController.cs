using Cinemachine;
using Org.BouncyCastle.Asn1.Crmf;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
//using UnityEngine.tvOS;

// 버튼들에 연결하는 식들
// UI 에서 참조하는 변수들

public class Y_BookController : MonoBehaviour
{
    public static Y_BookController Instance { get; private set; }

    public Camera mainCam;
    public CinemachineVirtualCamera virtualCam;
    public Camera paintCam;

    public GameObject bookUI;
    public GameObject ChooseCharacterUI;
    public GameObject PaintUI;
    public GameObject ChooseCompleteUI;
    public PhotonView pv;

    #region Book
    string text = "옛날 옛날에 모두의 사랑을 받는 작고 귀여운 소녀가 있었습니다. 하지만 그 소녀를 가장 사랑하는 것은 그녀의 할머니였습니다. 할머니는 소녀에게 무엇을 줘야 할지 몰랐습니다. 한번은 할머니가 소녀에게 붉은 벨벳으로 만들어진 모자를 선물했습니다. 소녀에게 그 모자가 잘 어울렸고, 소녀가 그 모자가 아닌 다른 것은 쓰지 않으려고 했습니다. 그래서 그 소녀는 '빨간 모자'라고 불렸습니다. 어느 날, 소녀의 엄마가 그녀에게 말했습니다. \"빨간 모자, 여기로 와보렴. 여기 케이크 한 조각과 와인 한 병을 할머니에게 가져다 주렴. 할머니가 편찮으시니까 네가 가면 기뻐하실 거야. 더워지기 전에 출발하렴. 그리고 할머니 댁에 갈 때, 조심해서 가고 길에서 벗어나지 마렴. 그렇지 않으면 네가 넘어져서 병을 깨뜨릴 거야. 그러면 할머니는 아무것도 받지 못하신단다. 그리고 할머니 방에 가면, 먼저 인사하고 방 안 구석구석을 살펴보는 것을 잊지 말아라!\"\r\n\r\n\"엄마 말 잘 들을게요.\" 빨간 머리가 엄마에게 말했습니다. 하지만 할머니는 마을에서 30분 떨어진 거리에 있는 숲에 사셨습니다. 빨간 모자가 숲에 같을 때, 그녀는 늑대를 만났습니다. 하지만 빨간 모자는 늑대가 얼마나 나쁜 동물인지 몰랐고 늑대를 무서워하지 않았습니다. \"안녕, 빨간 모자!\" 늑대가 말했습니다. \"고마워, 늑대!\" – \" 빨간 모자야, 이렇게 일찍 어딜 가니?\" – \"할머니께\" – \"무엇을 가져가니?\" – \"케이크와 와인. 어제 우리가 편찮으신 할머니께 좋은 것을 드리고 기운 내게 해드리려고 케이크를 만들었어.\" – \"빨간 모자야, 너희 할머니는 어디 사시니?\" – \"여기서도 15분 더 숲 속으로 가야 해. 3개의 오크 나무 아래에 할머니 집이 있어. 그 아래에는 개암나무가 있어. 너도 거기를 알거야.\" 빨간 모자가 말했습니다. 늑대는 혼자 생각했습니다. '저 어리고 상냥한 한입거리가 노인보다 훨씬 맛있을거야. 둘 다 잡아먹으려면 빨리 꾀를 생각해내야 해.' 늑대가 빨간 모자 옆으로 가서 말했습니다. \"빨간 모자야, 여기 주변에 있는 예쁜 꽃들 좀 봐. 왜 주변을 둘러보지 않니? 내 생각엔, 너가 새들이 얼마나 아름답게 노래하고 있는지 듣지 않는 것 같아. 너는 학교 가는 것처럼 바쁘게 가는구나. 여기 숲을 둘러보는 건 정말 재미있어.\"\r\n\r\n빨간 모자가 눈을 뜨고 햇빛이 나무를 향해 춤추는 모습과 예쁜 꽃들을 둘러봤을 때, 그녀는 생각했습니다. '내가 할머니께 꽃다발을 가져다 드리면, 할머니가 기뻐하실거야. 어쨌든 지금은 아직 이르니까 난 제시간에 할머니 댁에 도착할거야.' 그래서 그녀는 길에서 벗어나 꽃을 찾아 다녔습니다. 그녀가 꽃 하나를 꺾을 때마다, 그녀는 조금만 더 가면 더 예쁜 꽃이 있을 거라고 생각했습니다. 그래서 계속해서 숲 속 깊이 들어갔습니다. 하지만 늑대는 바로 할머니 댁을 향해 갔고 문을 두드렸습니다. \"밖에 누구세요?\" – \"빨간 모자에요. 제가 케이크와 와인을 들고 왔어요. 문 열어 주세요!\" – \"그냥 손잡이를 누르면 된단다!\" 할머니가 말했습니다. \"나는 너무 아파서 일어날 수가 없단다.\" 늑대가 손잡이를 누르고 할머니 댁으로 들어갔습니다. 말도 없이 할머니 침대로 가서 할머니를 잡아먹었습니다. 그리고 늑대는 할머니의 옷을 입고, 두건을 쓰고 침대에 누워 커튼을 쳤습니다.\r\n\r\n하지만 빨간 모자는 계속 꽃을 찾아 다녔습니다. 그녀가 더 가져갈 수 없을 만큼 꽃을 모은 후에야, 할머니가 생각나서 할머니 댁으로 향하기 시작했습니다. 빨간 모자가 할머니 댁에 도착했을 때, 문이 열려 있어서 놀랐습니다. 할머니 방으로 들어갔을 때, 모든 것이 너무 이상해서 빨간 모자는 생각했습니다. '맙소사. 내가 왜 이렇게 무서워하지? 나는 할머니 댁에 있는 것을 좋아하잖아!' 빨간 모자가 말했습니다. \"안녕하세요.\" 하지만 대답은 듣지 못했습니다. 빨간 모자가 할머니 침대로 다가가서 커튼을 젖혔습니다. 거기에 할머니가 두건을 깊게 눌러쓰고 누워있었고 매우 이상해 보였습니다. \"아, 할머니, 귀가 정말 크시네요!\" – \"내가 너의 목소리를 더 잘 듣기 위해서란다!\" – \"아, 할머니, 눈이 정말 크시네요!\" – \"내가 너를 더 잘 보기 위해서란다!\" – \"아, 할머니, 손이 정말 크시네요!\" – \"내가 너를 더 잘 잡기 위해서란다!\" – \"하지만, 할머니, 무서울 만큼 큰 입을 가지셨네요!\" – \"내가 너를 더 잘 잡아먹기 위해서란다!\" 늑대가 그것을 말하자마자 침대에서 뛰어나와 그 불쌍한 빨간 모자를 잡아먹었습니다.\r\n\r\n늑대가 다 먹어 치웠을 때, 그는 다시 침대에 누워서 잠에 들었고 시끄럽게 코를 골기 시작했습니다. 한 사냥꾼이 할머니 댁을 지나가면서 생각했습니다. '할머니가 왜 이렇게 시끄럽게 코를 고시는 거지? 가서 할머니에게 문제가 있나 확인해봐야겠다.' 그래서 그는 할머니 방 안으로 들어갔습니다. 그가 침대로 왔을 때, 그는 늑대가 거기에 누워있는 것을 봤습니다. \"너를 여기서 보는구나\" 사냥꾼이 말했습니다. \"내가 너를 오랫동안 찾아다녔지.\" 이제 그는 늑대에게 총을 쏘려고 했습니다. 그런데 그때 그에게 무언가 떠올랐습니다. '늑대가 할머니를 잡아먹었지만, 할머니를 구할 수 있을지도 몰라. 총을 쏘면 안돼.\" 그 대신에 사냥꾼은 가위를 가져와서 자고 있는 늑대의 배를 가르기 시작했습니다. 잠시 후에 그는 빨간 모자를 발견했습니다. 그리고 늑대 배를 조금 더 자르자 빨간 모자가 나와서 말했습니다. \"저는 정말 무서웠어요. 늑대 배 안은 정말 어두웠어요!\" 그리고 할머니도 살아서 나왔고 가까스로 숨을 쉬었습니다.\r\n\r\n빨간 모자가 무거운 돌들을 가져와서 늑대의 배 안을 그 돌로 채웠습니다. 그래서 늑대가 깨어났을 때, 그가 도망가려고 해도 돌이 너무 무거워서 넘어져 죽을 것입니다. 그 세 명은 모두 행복했습니다. 사냥꾼은 늑대의 가죽을 가져갔습니다. 할머니는 빨간 모자가 가져 온 먹고 와인을 마시고 병에서 회복했습니다. 빨간 모자는 생각했습니다. '내가 살아있는 동안 엄마가 허락하지 않으시면 다시는 혼자 길에서 벗어나지 않을 거야.'";
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

    #endregion

    #region 캐릭터 선택 후

    ////////////////////////////////
    //// 각 플레이어에 할당할 MP4 파일 경로 배열
    //public string[] videoFilePaths = new string[]
    //{
    //    @"C:\Users\Admin\MariMo\Assets\YDW\VideoPlayer\garlic.mp4",
    //    @"C:\Users\Admin\MariMo\Assets\YDW\VideoPlayer\pngman_center.mp4",
    //    @"C:\Users\Admin\MariMo\Assets\YDW\VideoPlayer\pngman_animation.mp4",
    //    @"C:\Users\Admin\MariMo\Assets\YDW\VideoPlayer\pngman_dab.mp4"
    //};

    #endregion

    private void Awake()
    {
        // Singleton 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        mainCam = Camera.main;

        pv = GetComponent<PhotonView>();

        texts = new List<string>();
        pageNo = int.Parse(pageNoTxt.text);

        SplitTextIntoPages();
        DisplayPage(pageNo);
    }

    public bool isSync = true;

    private void Update()
    {

        if (PhotonNetwork.IsMasterClient && isSync) //&& !(PhotonNetwork.PlayerList.Length == 2)
        {
            SyncAllPlayers();
        }

        if (PhotonNetwork.PlayerList.Length == 4) //////////////
        {
            isSync = false;
        }
    }

    // 플레이어가 참여할 때 호출
    public void RPC_AddPlayer(int playerIndex, string nickName)
    {
        if (!playerNames.ContainsKey(playerIndex))
        {
            // 마스터 클라이언트인 경우에만 전체 동기화 실행
            if (PhotonNetwork.IsMasterClient)
            {
                SyncAllPlayers();
            }
            else
            {
                // 마스터가 아닌 경우 자신의 정보만 전송
                pv.RPC("AddPlayer", RpcTarget.All, playerIndex, nickName);
            }
        }
    }

    // 전체 플레이어 동기화
    public void SyncAllPlayers()
    {
        // 현재 룸의 모든 플레이어 정보 전송
        foreach (var player in PhotonNetwork.PlayerList)
        {
            int idx = player.ActorNumber - 1;
            string name = player.NickName;
            pv.RPC("AddPlayer", RpcTarget.All, idx, name);
        }
    }

    [PunRPC]
    void AddPlayer(int playerIndex, string nickName)
    {
        playerNames[playerIndex] = nickName;
    }

    #region BookUI

    // 텍스트 쪼개기
    void SplitTextIntoPages()
    {
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
    }

    // 페이지에 띄우기
    void DisplayPage(int pageIndex)
    {
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

    public void Select(int num)
    {
        RPC_DeactivateAllNameUI(characterNum);
        characterNum = num;
        RPC_ActivateNameUI(characterNum, currentPlayerNum);

        allPlayers[currentPlayerNum].GetComponent<Y_PlayerAvatarSetting>().RPC_SelectChar(characterNum);
    }
   
     public void RPC_ActivateNameUI(int characterIndex, int playerIndex)
     {  
        pv.RPC("ActivateNameUI", RpcTarget.All, characterIndex, playerIndex);
     }

    [PunRPC]
    void ActivateNameUI(int characterIndex, int playerIndex)
    {
        string playerName = playerNames.ContainsKey(playerIndex) ? playerNames[playerIndex] : "Not Found";

        switch (characterIndex)
        {
            case 1:
                img_charName1.SetActive(true);
                img_charName1.GetComponentInChildren<TMP_Text>().text = playerNames[playerIndex];
                break;
            case 2:
                img_charName2.SetActive(true);
                img_charName2.GetComponentInChildren<TMP_Text>().text = playerNames[playerIndex];
                break;
            case 3:
                img_charName3.SetActive(true);
                img_charName3.GetComponentInChildren<TMP_Text>().text = playerNames[playerIndex];
                break;
            case 4:
                img_charName4.SetActive(true);
                img_charName4.GetComponentInChildren<TMP_Text>().text = playerNames[playerIndex];
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

        allPlayers[currentPlayerNum].GetComponent<Y_PlayerAvatarSetting>().RPC_UpdatePhoto(characterNum); /////////22222222
        // currentPlayerNum 에 따라 RenderTexture, characterNum에 따라 MP4 파일을 설정
        //SetCharacterVideo(currentPlayerNum, characterNum);
    }

    //private void SetCharacterVideo(int currentPlayerNum, int characterNum)
    //{
    //    foreach(KeyValuePair<int, string> items in playerNames)
    //    {
            
    //    }
    //}

    public void PaintToComplete()
    {
        // 이 때 AI 한테 이미지 보내주는 거 나중에 구현할 것
        ////////////////////////////
        ///

        // 각 플레이어마다 루프 돌면서 이미지 넣어줌

        //mainCam.gameObject.SetActive(true);
        //virtualCam.gameObject.SetActive(true);
        //paintCam.gameObject.SetActive(false);

        PaintUI.SetActive(false);
        ChooseCharacterUI.SetActive(true);
        btn_chooseChar.SetActive(false);
        btn_toMap.SetActive(true);
    }

    public void ToMap()
    {
        // 이 때 캐릭터 받아와서 내 플레이어에 동기화 시켜야 됨
        ////////////////////////

        // 맵 소개 UI 실행
        //K_LobbyUiManager.instance.isAllArrived = true; /////////////////// !!!!!!!!!!!!!!!!
        gameObject.SetActive(false);
    }

    #endregion

    public Y_PlayerAvatarSetting myAvatar;
    public Dictionary<int, PhotonView> allPlayers = new Dictionary<int, PhotonView>(); // MyAvatar 로 바꾸기
    public void AddPlayer(PhotonView pv)
    {
        allPlayers[pv.Owner.ActorNumber - 1] = pv;

        if(pv.IsMine)
        {
            myAvatar = pv.GetComponent<Y_PlayerAvatarSetting>();
            print("avatarIndex from AddPlayer: " + myAvatar.avatarIndex);
        }
    }

}
