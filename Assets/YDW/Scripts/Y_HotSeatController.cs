﻿using iTextSharp.text.html.simpleparser;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Rendering.LookDev;
using UnityEngine.UI;
using UnityEngine.Video;

public class Y_HotSeatController : MonoBehaviourPun
{
    public static Y_HotSeatController Instance { get; private set; }

    public GameObject guide;
    public GameObject selfIntroduce;
    public GameObject panel_waiting;
    public GameObject stage;

    Y_PlayerAvatarSetting myAvatarSetting;

    // selfIntroduce;
    public TMP_InputField selfIntroduceInput;
    public TMP_Text Txt_TitleText;
    public Image myAvatarImage;
    public RectTransform inputFieldRect;
    public Vector2 expandedSize = new Vector2(1200, 200); // 확장된 크기
    public Vector2 expandedPos = new Vector2(-345, 180); // 확장됐을 때 위치
    private Vector2 originalSize;
    private Vector2 originalPosition;
    private TouchScreenKeyboard keyboard;
    //public Dictionary<int, PhotonView> allPlayers;

    // stage
    int selfInt_count = 0;
    public List<Image> images = new List<Image>();
    public List<GameObject> players = new List<GameObject>();
    public Dictionary<int, string> selfIntroduces = new Dictionary<int, string>();
    public Dictionary<int, PhotonView> shuffledAllPlayers = new Dictionary<int, PhotonView>();
    public TMP_Text[] characterNames;
    public GameObject stageImg;
    public GameObject speechGuide;
    Color originalColor;
    int testNum = 0;
    public Vector2 playerPos;
    Vector2 stagePos;
    public Image[] stageScriptImgs;
    public TMP_Text[] stageScriptTxts;


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

    void Start()
    {
        originalSize = inputFieldRect.sizeDelta;
        originalPosition = inputFieldRect.gameObject.transform.localPosition;
        StartCoroutine(Deactivate(guide));

        originalColor = images[0].color;
        stagePos = new Vector2(stageImg.transform.position.x, stageImg.transform.position.y);
        playerPos = players[0].transform.position;

        myAvatarSetting = Y_BookController.Instance.myAvatar;
        Txt_TitleText.text = myAvatarSetting.pv.Owner.NickName;
        myAvatarImage.sprite = myAvatarSetting.images[myAvatarSetting.avatarIndex];

    }

    public IEnumerator Deactivate(GameObject gm)
    {
        yield return new WaitForSeconds(2);
        gm.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1) && testNum < players.Count)
        {
            testNum++;
            StartSpeech(testNum); ////////////////////222222222222233333333333333
        }  
    }

    #region SelfIntroduce

    // InputField가 선택되었을 때 호출
    public void OnSelect(BaseEventData eventData)
    {
        inputFieldRect.sizeDelta = expandedSize;
        inputFieldRect.gameObject.transform.localPosition = expandedPos;

        // 터치 키보드 호출 (모바일에서만 동작)
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    // InputField가 선택 해제되었을 때 호출
    public void OnDeselect(BaseEventData eventData)
    {
        inputFieldRect.sizeDelta = originalSize;
        inputFieldRect.gameObject.transform.localPosition = originalPosition;

        // 터치 키보드 닫기
        if (keyboard != null && keyboard.active)
        {
            keyboard.active = false;
        }
    }

    public TMP_Text[] characterNameTags;
    public TMP_Text[] playerNameTags;
    public RawImage[] rawImages;
    //public string[] selfIntroduces = new string[4];

    // 제출하기 버튼
    public void Submit()
    {
        selfIntroduce.SetActive(false);
        panel_waiting.SetActive(true);

        Shuffle();

        // 작성한 순서대로 추가됨
        RPC_AddSelfIntroduce(PhotonNetwork.LocalPlayer.ActorNumber, selfIntroduceInput.text);

        RPC_AllReady();
    }

    void RPC_AddSelfIntroduce(int actorNumber, string selfIntroduce)
    {
        photonView.RPC(nameof(AddSelfIntroduce), RpcTarget.All, actorNumber, selfIntroduce);
    }

    // 학생이 쓴 자기소개를 리스트에 넣어줌
    [PunRPC]
    void AddSelfIntroduce(int actorNumber, string selfIntroduce)
    {
        // actorNumber를 Key, selfIntroduce 를 Value 로 딕셔너리에 추가
        selfIntroduces.Add(actorNumber, selfIntroduce); 
        //for(int i = 0; i < selfIntroduces.Count; i++)
        //{
        //    if (selfIntroduces[i] != null)
        //    print("selfIntroduces : " + i + " " + selfIntroduces[i]);
        //}
        //selfIntroduces[avatarIndex] = selfIntroduce
        //Input.text; // 내 아바타의 인덱스 위치대로 추가
    }

    void RPC_AllReady()
    {
        photonView.RPC(nameof(AllReady), RpcTarget.All);
    }

    [PunRPC]
    public void AllReady()
    {
        selfInt_count++;

        if (selfInt_count >= 4)
        {
            for (int i = 1; i <= selfIntroduces.Count; i++)
            {
                if (selfIntroduces[i] != null)
                    print("selfIntroduces : " + i + " " + selfIntroduces[i]);
            }
            for (int j = 0; j < playerNums.Count; j++)
            {
                print("playerNums : " + j + " " + playerNums[j]);
            }
            panel_waiting.SetActive(false);
            stage.SetActive(true);
            MatchNameTags();
            MatchPlayerPos();
            MatchSelfIntroduce();
            StartSpeech(0); ////////////////////22222222222222222
        }
    }

    #endregion

    #region Stage

    public GameObject spotlight;
    public GameObject stageScript;
    List<int> playerNums = new List<int>();
    int[] playerNumsArray;

    // 셔플 돌린 후 싱크 맞춤
    public void Shuffle()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            playerNums = ShuffleList(Y_BookController.Instance.allPlayers);
            RPC_SyncPlayerNums(playerNums.ToArray());
        }
    }

    // Stage 단계의 상단 이름표 부분 구현 : 랜덤으로 순서 섞어서 보여주기
    void MatchNameTags()
    {
        // playerNums 순서대로 이름표 안의 텍스트 배치
        for (int i = 0; i < characterNameTags.Length; i++)
        {
            string name = Y_BookController.Instance.allPlayers[playerNums[i]].Owner.NickName;
            int avatarIndex = Y_BookController.Instance.allPlayers[playerNums[i]].GetComponent<Y_PlayerAvatarSetting>().avatarIndex;
            characterNameTags[i].text = characterNames[avatarIndex].text; // 캐릭터 이름
            playerNameTags[i].text = name; // 플레이어 이름
        }
    }

    // Stage 단계의 플레이어 이미지를 상단 이름표의 순서에 맞춰 배치
    void MatchPlayerPos()
    {
        // playerNums 순서대로 캐릭터 MP4 순서대로 배치
        for (int i = 0; i < rawImages.Length; i++)
        {
            int avatarIndex = Y_BookController.Instance.allPlayers[playerNums[i]].GetComponent<Y_PlayerAvatarSetting>().avatarIndex;
            rawImages[i].GetComponentInChildren<VideoPlayer>().clip = myAvatarSetting.videoClips[avatarIndex];
        }
    }

    // 각 플레이어가 쓴 자기소개를 순서에 따라 넣어놓기
    void MatchSelfIntroduce()
    {
        for (int i = 0; i < playerNums.Count; i++)
        {
            print("????????? i : " + i);
            int playerNum = playerNums[i];
            print("????????? playerNum" + playerNum);
            if (selfIntroduces.ContainsKey(playerNum))
            {
                stageScriptTxts[i].text = selfIntroduces[playerNum];
                print("???????????? stageScriptTxt : " + stageScriptTxts[i].text);
            }
        }
    }

    List<int> ShuffleList(Dictionary<int, PhotonView> playerDict)
    {
        foreach (int key in playerDict.Keys)
        {
            playerNums.Add(key);
        }

        int n = playerNums.Count;

        for(int i = 0; i < n; i++)
        {
            int j = Random.Range(0, n);

            int tmp = playerNums[i];
            playerNums[i] = playerNums[j];
            playerNums[j] = tmp;
        }

        return playerNums;
    }

    void RPC_SyncPlayerNums(int[] syncedPlayerNums)
    {
        photonView.RPC(nameof(SyncPlayerNums), RpcTarget.All, syncedPlayerNums);
    }

    [PunRPC]
    void SyncPlayerNums(int[] syncedPlayerNums)
    {
        playerNums = syncedPlayerNums.ToList();
    }


    public void StartSpeech(int i)
    {
        print("StartSpeech i : " + i);
        if (i - 1 >= 0)
        {
            images[i - 1].color = originalColor; // 전 플레이어는 이름표 색 원래 색으로
            players[i - 1].transform.position = playerPos; // 위치도 원위치
            stageScriptImgs[i - 1].gameObject.SetActive(false); // 전 플레이어의 자기소개 끄기
        }

        if(i < players.Count)
        {
            // 이름 UI 색깔 바꾸고
            images[i].color = Color.red;

            playerPos = players[i].transform.position;
            StartCoroutine(ChangePos(playerPos, i));

            stageScriptImgs[i].gameObject.SetActive(true);
            print("stageScriptImgs i : " + stageScriptImgs[i].GetComponentInChildren<TMP_Text>().text);
        }

        // "자기소개를 듣고 궁금했던 것들을 질문해봅시다" UI
        // 1분 뒤 질문받기
        // 순서대로 보이스 활성화
        // 4명 다 끝내면 "참 잘했어요!" UI
    }

    public IEnumerator ChangePos(Vector2 playerPos, int i)
    {
        while(true)
        {
            // 플레이어가 무대로 가게 한다
            players[i].transform.position = Vector3.Lerp(playerPos, stagePos, 0.05f);
            playerPos = players[i].transform.position;
            if (Vector3.Distance(playerPos, stagePos) < 0.1f) // 무대까지 거의 다 오면
            {
                playerPos = stagePos; // 도착점에 위치 맞춰준다

                spotlight.SetActive(true); // 스포트라이트

                ////////////////////// 나중에 보이스 연동
                
                // 밑에 자기소개
                stageScript.SetActive(true); 
                stageScript.GetComponentInChildren<TMP_Text>().text = selfIntroduceInput.text;

                // "친구들에게 말로 자기소개를 해 봅시다" UI
                speechGuide.SetActive(true);
                StartCoroutine(Deactivate(speechGuide));

                break;
            }
            yield return null;
        }
    }


    #endregion
}
