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
    public int testNum = 0;
    public Vector2 playerPos;
    public Transform stagePos;
    public Image[] stageScriptImgs;
    public TMP_Text[] stageScriptTxts;
    public GameObject panel_question;
    public GameObject panel_good;


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

    public TMP_Text txt_playerName;

    void Start()
    {
        // 자기소개 인풋필드 터치 키보드 올라오면 위치/크기 변경할 준비
        originalSize = inputFieldRect.sizeDelta;
        originalPosition = inputFieldRect.gameObject.transform.localPosition;

        // stage 부분 사전 준비/저장
        originalColor = images[0].color;
        playerPos = players[0].transform.position;

        // 자기소개 화면 본인 닉네임과 캐릭터, 이미지 표시
        myAvatarSetting = Y_BookController.Instance.myAvatar;
        txt_playerName.text = myAvatarSetting.pv.Owner.NickName;
        Txt_TitleText.text = characterNames[Y_BookController.Instance.characterNum - 1].text;
        myAvatarImage.sprite = myAvatarSetting.images[myAvatarSetting.avatarIndex];

        // 안내 가이드 띄워주기
        guide.SetActive(true);
        StartCoroutine(Deactivate(guide));
    }

    // 오브젝트 2초뒤 꺼주기
    public IEnumerator Deactivate(GameObject gm)
    {
        yield return new WaitForSeconds(2);
        gm.SetActive(false);
        if(gm == guides[4]) // 마지막 "참 잘했어요!" UI 의 경우
        {
            yield return new WaitForSeconds(3f);
            K_KeyManager.instance.isDoneHotSitting = true;
            Y_HotSeatManager.Instance.MoveControl(true);
            gameObject.SetActive(false);
        }
    }

    bool isEnd = false;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0) && testNum <= players.Count)
        {
            RPC_ProtoTest();
        }  

        //if(Input.GetKeyDown(KeyCode.Alpha0) && testNum == players.Count)
        //{

        //}

        //if(Input.GetKeyDown(KeyCode.N))
        //{
        //    RPC_TurnOff();
        //}

        // 네 명이 다 인터뷰 하면 자동으로 활동 종료
        if(testNum >= players.Count && !isEnd)
        {
            isEnd = true;
            StartCoroutine(LastCoroutine());
        }

        if(Input.GetKeyDown(KeyCode.Alpha8))
        {
            MuteOtherPlayers(PhotonNetwork.LocalPlayer.ActorNumber);
        }

        if(Input.GetKeyDown(KeyCode.Alpha7))
        {
            UnMuteAllPlayers();
        }
    }

    

    void RPC_ProtoTest()
    {
        photonView.RPC(nameof(ProtoTest), RpcTarget.All);
    }

    [PunRPC]
    void ProtoTest()
    {
        testNum++;
        StartSpeech(testNum);
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

        // 플레이어 순서 랜덤으로 섞음
        Shuffle();

        // 작성한 순서대로 추가됨
        RPC_AddSelfIntroduce(PhotonNetwork.LocalPlayer.ActorNumber, selfIntroduceInput.text);

        RPC_AllReady();
    }

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
        selfIntroduces.Add(actorNumber, selfIntroduce); 
    }

    void RPC_AllReady()
    {
        photonView.RPC(nameof(AllReady), RpcTarget.All);
    }

    // 4명 전부 들어오면 실행
    [PunRPC]
    public void AllReady()
    {
        // 보이스 일단 4명 다 꺼줌
        //Y_VoiceManager.Instance.recorder.TransmitEnabled = false;

        // 서브밋 누른 플레이어 수 늘림
        selfInt_count++;

        // 4명이 다 차면
        if (selfInt_count >= 4)
        {
            panel_waiting.SetActive(false);
            stage.SetActive(true);

            // 상단의 이름표, 중간의 캐릭터 애니메이션, 하단의 자기소개 순서 모두 랜덤으로 돌린 순서랑 맞춰 줌
            MatchNameTags();
            MatchPlayerPos();
            MatchSelfIntroduce();

            Y_VoiceManager.Instance.recorder.TransmitEnabled = false; // 인터뷰 시작하기 전에 일단은 모두 보이스 끈다
            StartSpeech(0);
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
            int playerNum = playerNums[i];

            stageScriptTxts[i].text = selfIntroduces[playerNum + 1];
        }
    }

    public GameObject[] timerImgs;

    // 순서대로 자기소개 - 질문
    public void StartSpeech(int index)
    {
        if (index - 1 >= 0)
        {
            images[index - 1].color = originalColor; // 전 플레이어는 이름표 색 원래 색으로
            players[index - 1].transform.position = playerPos; // 이미지 위치도 원위치
            stageScriptImgs[index - 1].gameObject.SetActive(false); // 전 플레이어의 자기소개 끄기
            spotlight.SetActive(false); // 스포트라이트 끔

            // 녹음 끄기
            //  Y_VoiceManager.Instance.StopRecording(playerNums[index-1], "hotseatingInterview" + playerNums[index-1].ToString());
            // 보이스 꺼 줘야 함
            //Y_VoiceManager.Instance.recorder.TransmitEnabled = false;
        }

        if (index < players.Count)
        {
            // 이름 UI 색깔 바꾸고
            images[index].color = Color.red;

            // 플레이어 무대로 가게 한다
            playerPos = players[index].transform.position;
            StartCoroutine(ChangePos(playerPos, index));
        }

    }

    public IEnumerator ChangePos(Vector2 playerPos, int i)
    {
        while(true)
        {
            // 플레이어가 무대로 가게 한다
            players[i].transform.position = Vector3.Lerp(playerPos, stagePos.position, 0.05f);
            playerPos = players[i].transform.position;
            if (Vector3.Distance(playerPos, stagePos.position) < 0.1f) // 무대까지 거의 다 오면
            {
                playerPos = stagePos.position; // 도착점에 위치 맞춰준다

                spotlight.SetActive(true); // 스포트라이트 켜준다


                stageScriptImgs[i].gameObject.SetActive(true);

                // "친구들에게 말로 자기소개를 해 봅시다" UI
                speechGuide.SetActive(true);
                StartCoroutine(Deactivate(speechGuide));

                //int recordTime;

                // 마스터면 15초, 아니면 5초 타이머 시작
                if (i == 0 && PhotonNetwork.IsMasterClient) // 테스트용으로 5초, 시연 땐 15초 정도 할까 /////////////////////////
                {
                    RPC_StartTimer(i, 5);
                    //recordTime = 15;
                    
                }
                else if(PhotonNetwork.IsMasterClient)
                {
                    RPC_StartTimer(i, 5);
                    //recordTime = 5;
                }

                // 소리 나머지 뮤트
                MuteOtherPlayers(playerNums[i]);
                // 내 차례 됐을 때에만 보이스 켜줌
                //if (PhotonNetwork.LocalPlayer.ActorNumber == playerNums[i])
                //{
                //    Y_VoiceManager.Instance.recorder.TransmitEnabled = true;
                //    //Y_VoiceManager.Instance.StartRecording(playerNums[i], recordTime);
                //}

                // 자기소개 켜 줌
                stageScriptImgs[i].gameObject.SetActive(true);

                break;
            }
            yield return null;
        }
    }

    PhotonVoiceView[] allVoiceViews;

    public void MuteOtherPlayers(int playerNum)
    {
        int myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        // 모든 PhotonVoiceView를 가진 객체를 검색
        allVoiceViews = FindObjectsOfType<PhotonVoiceView>();

        foreach (var voiceView in allVoiceViews)
        {
            PhotonView photonVoiceView = voiceView.GetComponent<PhotonView>();

            // 플레이어가 null이 아닌지 확인하고, 현재 차례 플레이어를 제외하고 전부 음소거
            if (photonVoiceView != null && photonVoiceView.Owner != null && myActorNumber != playerNum)
            {
                voiceView.RecorderInUse.TransmitEnabled = false;
            }
            else if(myActorNumber == playerNum) // 한 명만 음소거 아니게
            {
                voiceView.RecorderInUse.TransmitEnabled = true;
            }
        }
    }

    public void UnMuteAllPlayers()
    {
        foreach (var voiceView in allVoiceViews)
        {
            PhotonView photonVoiceView = voiceView.GetComponent<PhotonView>();
            voiceView.RecorderInUse.TransmitEnabled = true;
        }
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
                StartCoroutine(InterviewCoroutine(i));
            }
        }

        // "자기소개를 듣고 궁금한 것들을 질문해봅시다" -> 2초 뒤 자동으로 deactivate
        ActivateGuide(3);

        hotSeating_count++;
        currTime = 0;
    }

    public GameObject[] myTurnImgs;
    IEnumerator InterviewCoroutine(int index)
    {
        yield return new WaitForSeconds(2f);

        for(int i = 0; i < players.Count; i++)
        {
            if(i != playerNums[index])
            {
                myTurnImgs[i].SetActive(true);

                // 질문하는 사람 보이스 켜주고 녹음 시작
                MuteOtherPlayers(playerNums[i]);
                Y_VoiceManager.Instance.StartRecording(playerNums[i], 600);

                // 원래는 30초인데 테스트용 5초
                yield return new WaitForSeconds(5f);
                // 녹음 종료
                Y_VoiceManager.Instance.StopRecording(playerNums[i], "InterviewFile");

                myTurnImgs[i].SetActive(false);

                // 자기소개 한 사람(답변할 사람) 보이스 켜주고 녹음 시작
                MuteOtherPlayers(playerNums[index]);
                Y_VoiceManager.Instance.StartRecording(playerNums[index], 600);

                // 원래는 60초인데 일단 5초
                yield return new WaitForSeconds(5f);
                // 녹음 종료
                Y_VoiceManager.Instance.StopRecording(playerNums[index], "InterviewFile");
            }
        }

        if(index == players.Count - 1)
        {
            RPC_ProtoTest();
        }
    }


    #endregion

    // 최종 단계
    IEnumerator LastCoroutine()
    {
        yield return new WaitForSeconds(2.5f);
        Y_VoiceManager.Instance.recorder.TransmitEnabled = true;
        RPC_ActivateGuide(4);
    }
    void RPC_ActivateGuide(int index)
    {
        photonView.RPC(nameof(ActivateGuide), RpcTarget.All, index);
    }

    [PunRPC]
    void ActivateGuide(int index)
    {
        guides[index].SetActive(true);
        StartCoroutine(Deactivate(guides[index]));
    }
}
