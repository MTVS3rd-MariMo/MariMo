using Cinemachine;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.UtilityScripts;
using Photon.Voice.PUN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Rendering.LookDev;
using UnityEngine.UI;
using UnityEngine.Video;

public class Y_HotSeatController : MonoBehaviourPun
{
    public GameObject guide;
    public GameObject selfIntroduce;
    public GameObject panel_waiting;
    public GameObject stage;

    Y_PlayerAvatarSetting myAvatarSetting;

    // selfIntroduce;
    public GameObject panel_selfIntroduce;
    public TMP_InputField selfIntroduceInput;
    public TMP_Text Txt_TitleText;
    public Image myAvatarImage;
    public RectTransform inputFieldRect;
    public Vector2 expandedSize = new Vector2(1200, 200); // 확장된 크기
    public Vector2 expandedPos = new Vector2(-450, 180); // 확장됐을 때 위치
    private Vector2 originalSize;
    private Vector2 originalPosition;
    private TouchScreenKeyboard keyboard;

    // stage
    int selfInt_count = 0;
    public List<Image> images = new List<Image>();
    public List<GameObject> players = new List<GameObject>();
    public Dictionary<int, string> selfIntroduces = new Dictionary<int, string>();
    public Dictionary<int, PhotonView> shuffledAllPlayers = new Dictionary<int, PhotonView>();
    public TMP_Text[] characterNames;
    public GameObject stageImg;
    public GameObject speechGuide;
    public int testNum = 0;
    public Vector2 playerPos;
    public Transform stagePos;
    public Image[] stageScriptImgs;
    public TMP_Text[] stageScriptTxts;
    public GameObject panel_question;
    public GameObject panel_good;

    public GameObject[] buttons;
    public Sprite[] sprites;

    public TMP_Text txt_playerName;

    public Y_BookController bookController;

    public Button[] characterImages;

    public GameObject VirtualCamera;

    void Start()
    {
        Y_SoundManager.instance.StopBgmSound();

        if(PhotonNetwork.IsMasterClient)
        {
            panel_waiting.SetActive(true);
            RPC_AllReady();
        }

        // 자기소개 인풋필드 터치 키보드 올라오면 위치/크기 변경할 준비
        originalSize = inputFieldRect.sizeDelta;
        originalPosition = inputFieldRect.gameObject.transform.localPosition;

        // stage 부분 사전 준비/저장
        playerPos = players[0].transform.position;

        //bookController = GameObject.Find("BookCanvas").GetComponent<Y_BookController>();

        // 자기소개 화면 본인 닉네임과 캐릭터, 이미지 표시
        myAvatarSetting = bookController.myAvatar;
        txt_playerName.text = myAvatarSetting.pv.Owner.NickName;
        if(bookController.characterNum - 1 >= 0)
        {
            Txt_TitleText.text = characterNames[bookController.characterNum - 1].text;
            myAvatarImage.sprite = characterImages[bookController.characterNum - 1].GetComponent<Image>().sprite;
        }
        else
        {
            Txt_TitleText.text = "선생님";
        }

        // 안내 가이드 띄워주기
        guide.SetActive(true);
        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_INFO);
        StartCoroutine(Deactivate(guide));
    }

    float canvasAlphaTime = 0;

    // 오브젝트 2초뒤 꺼주기
    public IEnumerator Deactivate(GameObject gm)
    {
        CanvasRenderer[] canvasRenderers = gm.GetComponentsInChildren<CanvasRenderer>();

        while (true)
        {
            canvasAlphaTime += Time.deltaTime;

            foreach (CanvasRenderer canvasRenderer in canvasRenderers)
            {
                Color originalColor = canvasRenderer.GetColor();
                originalColor.a = canvasAlphaTime;
                canvasRenderer.SetColor(originalColor);
            }

            if (canvasAlphaTime > 1)
            {
                canvasAlphaTime = 0;
                break;
            }

            yield return null;
        }

        if (gm == guides[4]) // 마지막 "참 잘했어요!" UI 의 경우
        {
            yield return new WaitForSeconds(3f);
            // 연출 끝남
            VirtualCamera.SetActive(false);
            // 1초 딜레이
            yield return new WaitForSeconds(2f);
            // 핫시팅 완료
            K_KeyManager.instance.isDoneHotSeating = true;
            K_LobbyUiManager.instance.img_KeyEmptyBox.gameObject.SetActive(true);
            
            GameObject.Find("Object_HotSeat").GetComponent<Y_HotSeatManager>().MoveControl(true);
            UnMuteAllPlayers(); ///////////// 원래는 RPC 였음!
            Y_SoundManager.instance.PlayBgmSound(Y_SoundManager.EBgmType.BGM_MAIN);
            gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(2);
            gm.SetActive(false);
        }
    }

    bool isEnd = false;
    bool over80 = false;

    public float requiredHoldTime = 2.0f; // 세 손가락을 유지해야 하는 시간
    private float touchHoldTime = 0f;


    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Alpha0) && testNum <= players.Count)
        //{
        //    RPC_ProtoTest();
        //} 
        
        if(selfIntroduceInput.text.Length >= 80 && !over80)
        {
            buttons[0].GetComponent<Button>().interactable = true;
            buttons[0].GetComponent<Image>().sprite = sprites[5];
            over80 = true;
        }

        // 모바일용 치트키
        // 세 손가락 터치가 유지되고 있는지 확인
        if (Input.touchCount == 3)
        {
            Debug.Log("치트키???");
            touchHoldTime += Time.deltaTime; // 터치 유지 시간 증가
            if (touchHoldTime >= requiredHoldTime)
            {
                Debug.Log("치트키 발동!");
                StartCoroutine(LastCoroutine());
                touchHoldTime = 0f; // 초기화
            }
        }
        else
        {
            touchHoldTime = 0f; // 세 손가락에서 벗어나면 시간 초기화
        }
    

        //if(Input.GetKeyDown(KeyCode.Alpha0) && testNum == players.Count)
        //{

        //}

        //if(Input.GetKeyDown(KeyCode.N))
        //{
        //    RPC_TurnOff();
        //}

        // 네 명이 다 인터뷰 하면 자동으로 활동 종료
        //if(testNum > players.Count && !isEnd)
        //{
        //    isEnd = true;
        //    StartCoroutine(LastCoroutine());
        //}

        //if(Input.GetKeyDown(KeyCode.Alpha8))
        //{
        //    photonView.RPC(nameof(MuteOtherPlayers), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        //    //MuteOtherPlayers(PhotonNetwork.LocalPlayer.ActorNumber);
        //}

        //if(Input.GetKeyDown(KeyCode.Alpha7))
        //{
        //    photonView.RPC(nameof(UnMuteAllPlayers), RpcTarget.All);
        //    //UnMuteAllPlayers();
        //}

        //if(Input.GetKeyDown(KeyCode.Alpha9))
        //{
        //    StartCoroutine(LastCoroutine());
        //}
    }

    

    void RPC_ProtoTest()
    {
        photonView.RPC(nameof(ProtoTest), RpcTarget.All);
    }

    [PunRPC]
    void ProtoTest()
    {
        testNum++;
        Debug.LogError("testNum : " + testNum);
        if(PhotonNetwork.IsMasterClient) RPC_StartSpeech(testNum);
    }

    void RPC_TurnOff()
    {
        photonView.RPC(nameof(TurnOff), RpcTarget.All);
    }

    [PunRPC]
    void TurnOff()
    {
        gameObject.SetActive(false);
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

    // 제출하기 버튼
    public void Submit()
    {
        selfIntroduce.SetActive(false);
        panel_waiting.SetActive(true);

        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_BUTTON);

        // 플레이어 순서 랜덤으로 섞음
        Shuffle();

        // 작성한 순서대로 추가됨
        RPC_AddSelfIntroduce(PhotonNetwork.LocalPlayer.ActorNumber - 1, selfIntroduceInput.text);

        RPC_AllReady();
    }

    List<int> playerNums = new List<int>();
    int[] playerNumsArray;

    // 셔플 돌린 후 싱크 맞춤
    public void Shuffle()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 2) // 원래는 PhotonNetwork.IsMasterClient
        {
            playerNums = ShuffleList(bookController.allPlayers);
            //foreach (int playerNum in playerNums)
            //{
            //    print("?????? " + playerNum);
            //}
            RPC_SyncPlayerNums(playerNums.ToArray());
        }
    }

    List<int> ShuffleList(Dictionary<int, PhotonView> playerDict)
    {
        foreach (int key in playerDict.Keys)
        {
            playerNums.Add(key);
        }

        int n = playerNums.Count;

        for (int i = 0; i < n; i++)
        {
            int j = UnityEngine.Random.Range(0, n);

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

    void RPC_AddSelfIntroduce(int actorNumber, string selfIntroduce)
    {
        photonView.RPC(nameof(AddSelfIntroduce), RpcTarget.All, actorNumber, selfIntroduce);
    }

    // 학생이 쓴 자기소개를 리스트에 넣어줌
    [PunRPC]
    void AddSelfIntroduce(int actorNumber, string selfIntroduce)
    {
        if(actorNumber > 0)
        selfIntroduces.Add(actorNumber, selfIntroduce); 
    }

    void RPC_AllReady()
    {
        photonView.RPC(nameof(startAllReadyCrt), RpcTarget.All);
    }

    [PunRPC]
    public void startAllReadyCrt()
    {
        StartCoroutine(AllReady());
    }

    // 4명 전부 들어오면 실행
    public IEnumerator AllReady()
    {

        // 서브밋 누른 플레이어 수 늘림
        selfInt_count++;

        // 5명이 다 차면
        if (selfInt_count >= 5)
        {
            // 상단의 이름표, 중간의 캐릭터 애니메이션, 하단의 자기소개 순서 모두 랜덤으로 돌린 순서랑 맞춰 줌
            MatchNameTags();
            MatchPlayerPos();
            MatchSelfIntroduce();

            yield return new WaitForSeconds(1f);

            // 순서 다 정렬하고 셋액티브
            panel_waiting.SetActive(false);
            if(PhotonNetwork.IsMasterClient)
            {
                panel_selfIntroduce.SetActive(false);
            }
            stage.SetActive(true);

            // 자기소개 보낸다
            // 먼저 자기의 자기소개 순서를 알아야 한다
            int selfIntCount = 0;
            for(int i = 0; i < playerNums.Count; i++)
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber - 2 == playerNums[i])
                {
                    selfIntCount = i;
                }
            }

            Debug.LogError("selfIntCount : " + selfIntCount);

            if(!PhotonNetwork.IsMasterClient) Y_HttpHotSeat.GetInstance().StartSendIntCoroutine(selfIntCount);

            if(PhotonNetwork.IsMasterClient) RPC_StartSpeech(0);
        }
    }

    #endregion

    #region Stage

    public GameObject spotlight;
    public GameObject stageScript;
    
    // Stage 단계의 상단 이름표 부분 구현 : 랜덤으로 순서 섞어서 보여주기
    void MatchNameTags()
    {
        // playerNums 순서대로 이름표 안의 텍스트 배치
        for (int i = 0; i < characterNameTags.Length; i++)
        {
            string name = bookController.allPlayers[playerNums[i]].Owner.NickName;
            int avatarIndex = bookController.allPlayers[playerNums[i]].GetComponent<Y_PlayerAvatarSetting>().avatarIndex;
            characterNameTags[i].text = characterNames[avatarIndex].text; // 캐릭터 이름
            playerNameTags[i].text = name; // 플레이어 이름
        }
    }

    public RenderTexture[] renderTextures;
    public Material[] materials;

    // Stage 단계의 플레이어 이미지를 상단 이름표의 순서에 맞춰 배치
    void MatchPlayerPos()
    {
        // playerNums 순서대로 캐릭터 MP4 순서대로 배치
        for (int i = 0; i < rawImages.Length; i++)
        {
            //int avatarIndex = bookController.allPlayers[playerNums[i]].GetComponent<Y_PlayerAvatarSetting>().avatarIndex;
            //Debug.LogError("avatarIndex 뭔데? : " + avatarIndex);
            //rawImages[i].material = new Material(rawImages[i].material);
            //rawImages[i].texture = renderTextures[playerNums[i]];
            //rawImages[i].GetComponentInChildren<VideoPlayer>().targetTexture = renderTextures[playerNums[i]];
            materials[playerNums[i]].mainTexture = renderTextures[playerNums[i]];
            rawImages[i].material = materials[playerNums[i]];
            
            Debug.LogWarning("되나요 : " + playerNums[i]);
            //rawImages[i].material.mainTexture = rawImages[i].GetComponentInChildren<VideoPlayer>().targetTexture;
            //rawImages[i].GetComponentInChildren<VideoPlayer>().url = Y_GameManager.instance.urls[playerNums[i]]; 
            //rawImages[i].GetComponentInChildren<VideoPlayer>().clip = myAvatarSetting.videoClips[avatarIndex];
        }
    }

    // 각 플레이어가 쓴 자기소개를 순서에 따라 넣어놓기
    void MatchSelfIntroduce()
    {
        for (int i = 0; i < playerNums.Count; i++)
        {
            int playerNum = playerNums[i];

            stageScriptTxts[i].text = selfIntroduces[playerNum + 1]; // selfIntroduces[playerNum + 1] -> selfIntroduces[playerNum]
        }
    }

    public GameObject[] timerImgs;
    public int selfIntNum = 0;

    void RPC_StartSpeech(int index)
    {
        photonView.RPC(nameof(StartSpeech), RpcTarget.All, index);
    }

    Vector3 originalPos;

    [PunRPC]
    // 순서대로 자기소개 - 질문
    public void StartSpeech(int index)
    {
        //Debug.LogError("StartSpeech 시작!!");
        if (index - 1 >= 0 && index - 1 < images.Count)
        {
            images[index - 1].sprite = sprites[2]; // 전 플레이어는 이름표 색 원래 색으로
            buttons[5 + index - 1].GetComponent<Image>().sprite = sprites[0];
            players[index - 1].GetComponent<RectTransform>().anchoredPosition = originalPos; // 이미지 위치도 원위치
            stageScriptImgs[index - 1].gameObject.SetActive(false); // 전 플레이어의 자기소개 끄기
            spotlight.SetActive(false); // 스포트라이트 끔
            //Debug.LogError("전 플레이어 원위치!");
            // 녹음 끄기
            //  Y_VoiceManager.Instance.StopRecording(playerNums[index-1], "hotseatingInterview" + playerNums[index-1].ToString());
            // 보이스 꺼 줘야 함
            //Y_VoiceManager.Instance.recorder.TransmitEnabled = false;
        }

        if (index < players.Count)
        {
            // 이름 UI 색깔 바꾸고
            images[index].sprite = sprites[3];
            buttons[5 + index].GetComponent<Image>().sprite = sprites[1];

            // 플레이어 무대로 가게 한다
            originalPos = players[index].GetComponent<RectTransform>().anchoredPosition;
            playerPos = players[index].GetComponent<RectTransform>().anchoredPosition;
            StartCoroutine(ChangePos(playerPos, index));
            selfIntNum++;
        }

        if (index == 4 && PhotonNetwork.IsMasterClient && !isEnd)
        {
            //StartCoroutine(LastCoroutine());
            RPC_startLastCrt();
            isEnd = true;
        }

    }

    RectTransform rtStage;

    public IEnumerator ChangePos(Vector2 playerPos, int i)
    {
        rtStage = stagePos.GetComponent<RectTransform>();
        RectTransform rtPlayer = players[i].GetComponent<RectTransform>();
        while (true)
        {
            if(Vector3.Distance(playerPos, stagePos.position) > 160f)
            {
                // 플레이어가 무대로 가게 한다
                rtPlayer.anchoredPosition = Vector2.Lerp(playerPos, rtStage.anchoredPosition, 0.05f);
                playerPos = rtPlayer.anchoredPosition;
                yield return null;
            }
            else
            {
                rtPlayer.anchoredPosition = rtStage.anchoredPosition;
                break;
            }
        }

        if(PhotonNetwork.IsMasterClient) RPC_OnStage(i);

    }

    void RPC_OnStage(int i)
    {
        photonView.RPC(nameof(OnStage), RpcTarget.All, i);
    }

    [PunRPC]
    void OnStage(int i)
    {
        Debug.LogError("OnStage Index : " + i);
        //Debug.LogError("무대까지 거의 다 왔다");
        playerPos = rtStage.anchoredPosition; // 도착점에 위치 맞춰준다

        spotlight.SetActive(true); // 스포트라이트 켜준다

        //Debug.LogError("자기소개 박스 켜준다");
        stageScriptImgs[i].gameObject.SetActive(true);

        // "친구들에게 말로 자기소개를 해 봅시다" UI
        //Debug.LogError("친구들에게 말로 자기소개를 해 봅시다 UI");
        speechGuide.SetActive(true);
        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_INFO);
        StartCoroutine(Deactivate(speechGuide));

        //int recordTime;

        // 처음 순서면 15초, 아니면 5초 타이머 시작
        if (i == 0 && PhotonNetwork.IsMasterClient) // 테스트용으로 5초, 시연 땐 15초 정도 할까 /////////////////////////
        {
            if (PhotonNetwork.IsMasterClient) RPC_StartTimer(i, 10); 
                                    //recordTime = 15;

        }
        else if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.IsMasterClient) RPC_StartTimer(i, 5);
            //recordTime = 5;
        }

        // 소리 나머지 뮤트
        MuteOtherPlayers(playerNums[i] + 1);
        print((playerNums[i] + 1) + " 빼고 뮤트됨! - 자기소개");

        // 자기소개 켜 줌
        stageScriptImgs[i].gameObject.SetActive(true);
    }

    PhotonVoiceView[] allVoiceViews;

    public void RPC_MuteOtherPlayers(int playerNum)
    {
        photonView.RPC(nameof(MuteOtherPlayers), RpcTarget.All, playerNum);
    }

    [PunRPC]
    public void MuteOtherPlayers(int playerNum)
    {
        int myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        if (PhotonNetwork.LocalPlayer.IsMasterClient) return;

        if(myActorNumber - 1 != playerNum)
        {
            Y_VoiceManager.Instance.recorder.TransmitEnabled = false;
            Y_VoiceManager.Instance.noVoiceIcon.gameObject.SetActive(true);
            Y_VoiceManager.Instance.voiceIcon.gameObject.SetActive(false);
            print(myActorNumber - 1 + "번 플레이어 뮤트됨");
        }
        else
        {
            Y_VoiceManager.Instance.recorder.TransmitEnabled = true;
            Y_VoiceManager.Instance.noVoiceIcon.gameObject.SetActive(false);
            Y_VoiceManager.Instance.voiceIcon.gameObject.SetActive(true);
            print(myActorNumber - 1 + "번 플레이어는 뮤트되지 않음");
        }
    }

    public void RPC_UnMuteAllPlayers()
    {
        photonView.RPC(nameof(UnMuteAllPlayers), RpcTarget.All);
    }

    [PunRPC]
    public void UnMuteAllPlayers()
    {
        Y_VoiceManager.Instance.recorder.TransmitEnabled = true;
        Y_VoiceManager.Instance.noVoiceIcon.gameObject.SetActive(false);
        Y_VoiceManager.Instance.voiceIcon.gameObject.SetActive(true);
        print("전체 언뮤트 됨");
        //foreach (var voiceView in allVoiceViews)
        //{
        //    PhotonView photonVoiceView = voiceView.GetComponent<PhotonView>();
        //    voiceView.RecorderInUse.TransmitEnabled = true;
        //}
    }

    float currTime = 0;
    int hotSeating_count = 0;
    public GameObject[] guides;
    public TMP_Text[] timerTxts;

    void RPC_StartTimer(int i, int time)
    {
        photonView.RPC(nameof(StartTimerCoroutine), RpcTarget.All, i, time);
    }

    [PunRPC]
    void StartTimerCoroutine(int i, int time)
    {
        StartCoroutine(StartTimer(i, time));
    }

    // 타이머 시작
    IEnumerator StartTimer(int i, int time)
    {
        yield return new WaitForSeconds(3f); // UI 사라질 때까지 기다리기
        timerImgs[i].gameObject.SetActive(true);

        while (currTime < time)
        {
            // DeltaTime을 사용하여 경과 시간 계산
            currTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기

            // 경과 시간을 분:초 형식으로 변환
            TimeSpan timeSpan = TimeSpan.FromSeconds(currTime);
            string timeText = string.Format("{0:00}:{1:00}",
                timeSpan.Minutes, Mathf.Max(time - timeSpan.Seconds, 0));

            timerTxts[i].text = timeText;

            if (time - timeSpan.Seconds <= 0)
            {
               timerImgs[i].gameObject.SetActive(false);

                // 인터뷰로 넘어감
                if(PhotonNetwork.IsMasterClient) RPC_InterviewCrt(i);
            }
        }

        // "자기소개를 듣고 궁금한 것들을 질문해봅시다" -> 2초 뒤 자동으로 deactivate
        ActivateGuide(3);

        hotSeating_count++;
        currTime = 0;
    }

    void RPC_InterviewCrt(int index)
    {
        photonView.RPC(nameof(StartInterviewCrt), RpcTarget.All, index);
    }

    [PunRPC]
    void StartInterviewCrt(int index)
    {
        StartCoroutine(InterviewCoroutine(index));
    }

    public GameObject[] myTurnImgs;

    IEnumerator InterviewCoroutine(int index)
    {
        Debug.LogError("InterviewCrt index : " + index);

        yield return new WaitForSeconds(2f);

        for(int i = 0; i <= players.Count; i++)
        {
            if(i < players.Count && i != index) // playerNums[index]
            {
                myTurnImgs[i].SetActive(true);

                // 질문하는 사람 보이스 켜주고 녹음 시작
                MuteOtherPlayers(playerNums[i] + 1); 
                print((playerNums[i] + 1) + " 빼고 뮤트됨! - 인터뷰 질문");
                RecordVoice(playerNums[i] + 2);

                // 원래는 30초인데 테스트용 5초
                yield return new WaitForSeconds(10f);
                // 녹음 종료
                StopRecordVoice(playerNums[i] + 2, index);

                myTurnImgs[i].SetActive(false);

                myTurnImgs[index].SetActive(true);

                // 자기소개 한 사람(답변할 사람) 보이스 켜주고 녹음 시작
                MuteOtherPlayers(playerNums[index] + 1);
                print((playerNums[index] + 1) + " 빼고 뮤트됨! - 인터뷰 답변");
                RecordVoice(playerNums[index] + 2);

                // 원래는 60초인데 일단 5초
                yield return new WaitForSeconds(10f);
                // 녹음 종료
                StopRecordVoice(playerNums[index] + 2, index);

                myTurnImgs[index].SetActive(false);

                //break; // 도원 시연용
            }

            if (i == players.Count)
            {
                if (PhotonNetwork.IsMasterClient) RPC_ProtoTest();
                print("다음 사람 자기소개로 넘어갑니다");
            } // 도원 시연용으로 삭제
        }
    }

    public void RPC_RecordVoice(int i)
    {
        photonView.RPC(nameof(RecordVoice), RpcTarget.All, i);
    }

    [PunRPC]
    public void RecordVoice(int i)
    {
        Y_VoiceManager.Instance.StartRecording(i, 600);
    }

    public void RPC_StopRecordVoice(int i, int selfIntNum)
    {
        photonView.RPC(nameof(StopRecordVoice), RpcTarget.All, i, selfIntNum);
    }

    [PunRPC]
    public void StopRecordVoice(int i, int selfIntNum)
    {
        Y_VoiceManager.Instance.StopRecording(i, selfIntNum);
        print("몇 번째 자기소개입니까? : " + selfIntNum);
    }


    #endregion

    void RPC_startLastCrt()
    {
        photonView.RPC(nameof(StartLastCrt), RpcTarget.All);
    }

    [PunRPC]
    void StartLastCrt()
    {
        StartCoroutine(LastCoroutine());
    }

    // 최종 단계
    IEnumerator LastCoroutine()
    {
        //yield return new WaitForSeconds(2.5f);
        yield return null;
        Y_VoiceManager.Instance.recorder.TransmitEnabled = true;
        if(PhotonNetwork.IsMasterClient) RPC_ActivateGuide(4);
        
    }

    void RPC_ActivateGuide(int index)
    {
        photonView.RPC(nameof(ActivateGuide), RpcTarget.All, index);
    }

    [PunRPC]
    void ActivateGuide(int index)
    {
        guides[index].SetActive(true);
        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_INFO);
        StartCoroutine(Deactivate(guides[index]));
    }
}
