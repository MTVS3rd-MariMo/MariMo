using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class K_QuizManager : MonoBehaviourPun
{
    // 현재 시간
    float currTime = 0;
    // 퀴즈 타이머 길이 (sec)
    public float quizDuration = 15f;
    // 현재 퀴즈가 시작 되었는지
    public bool isPlaying = false;
    // 디렉션 표시 프레임 1번만 처리하기
    public bool isDirecting = false;
    // 카운트 다운 플래그
    public bool isCounting = false;

    // 정답 글씨 가져오기
    public K_QuizPos quizCorrect;
    public K_QuizSpawnMgr quizSpawnMgr;

    // 플레이어 선택한 선택지 저장 인덱스
    private int selectedIndex = -1;


    // 연출용 요소들
    //public Image blackScreen;
    //public PlayableDirector timeline;



    public Y_BookController bookController;

    private void Start()
    {
        bookController = GameObject.Find("BookCanvas").GetComponent<Y_BookController>();
    }

    void Update()
    {
        // 플레이어 4명 모두 오면 활성화 -> 카운트 다운 하기
        if (isPlaying)
        {
            // 입장
            // 연출 시작
            //StartCoroutine(Start_Production());
        }
    }

    // 카운트다운 함수 -> 코루틴으로 재실행
    public void CountDownStart()
    {
        StartCoroutine(Co_CountDown());
    }

    // 카운트다운 코루틴 함수
    IEnumerator Co_CountDown()
    {
        // 퀴즈 안내 UI 활성화 (앵글 고정 후 1초 뒤 활성화, 2초 뒤 사라짐)
        K_QuizUiManager.instance.img_direction.gameObject.SetActive(true);
        // 퀴즈 박스 비활성화
        K_LobbyUiManager.instance.img_KeyEmptyBox.gameObject.SetActive(false);
        // 2초 후 퀴즈 안내 UI 비활성화 함수
        StartCoroutine(HideDirection(2f));

        // 1초 대기 후 카운트 다운 띄워주기
        yield return new WaitForSeconds(1f);

        // 카운트다운 이미지 활성화
        K_QuizUiManager.instance.img_countDown.gameObject.SetActive(true);

        
        // second 셋팅
        int second = (int)(quizDuration - currTime);
        

        // second가 0보다 크면
        while (second > 0)
        {
            // int로 부여한다
            second = (int)(quizDuration - currTime);

            // minute 도 해줘야하나?
            int minutes = second / 60;
            int seconds = second % 60;

            // 퀴즈 카운트_초 텍스트 
            //K_QuizUiManager.instance.text_countDown.text = second.ToString();
            // 00 : 15로 변경하기
            K_QuizUiManager.instance.text_countDown.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);

            currTime += Time.deltaTime;

            yield return null;
        }

        // second <= 0 
        isPlaying = false;

        // 시간초과(텍스트) 보여주자.
        K_QuizUiManager.instance.text_countDown.text = "시간 종료!";

        // 선생님만 학생들의 답을 RPC로 받기
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(SetAnswer), RpcTarget.All);
        }

        // 모든 학생들의 정답을 기다리기
        StartCoroutine(WaitFourAnswer());

        // 2초 대기 후
        yield return new WaitForSeconds(1f);

        // 카운트다운 이미지 비활성화
        K_QuizUiManager.instance.img_countDown.gameObject.SetActive(false);
    }


    [PunRPC]
    public void CountDown()
    {
        // 카운트 다운 이미지 보이기 + 카운트 다운은 img_direction 사라지고 1초 후 활성화
        if (isCounting)
        {
            K_QuizUiManager.instance.img_countDown.gameObject.SetActive(true);

            // 시간이 흐름
            currTime += Time.deltaTime;
            // 흐른 시간을 countDown에 셋팅하자
            //int second = 15 - (int)currTime;
            int second = (int)(quizDuration - currTime);

            // 만약에 second가 0 보다 크다면
            if (second > 0)
            {
                // second 값을 보여주자
                K_QuizUiManager.instance.text_countDown.text = second.ToString();
            }
            else if (second == 0)
            {

                isPlaying = false;

                // 시간초과(텍스트) 보여주자.
                K_QuizUiManager.instance.text_countDown.text = "시간 종료!";

                if (photonView.IsMine)
                {
                    // 정답 제출 RPC로 실행

                    // 정답 판별 함수 호출
                    //내가 정답을 쏴주는 함수

                    //RPC_CHeckAnswer();
                }
                print("정답 RPC 됨?");

                if (!PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC(nameof(SetAnswer), RpcTarget.All);
                }
                //정답을 기다리는 함수
                StartCoroutine(WaitFourAnswer());

            }
            // 그렇지 않으면
            else
            {
                K_QuizUiManager.instance.img_countDown.gameObject.SetActive(false);
            }
        }

    }

    ////정답 리스트
    public List<bool> answerList = new List<bool>();

    [PunRPC]
    public void SetAnswer()
    {
        //내가 정답인지 아닌지 쏴준다
        K_QuizCorrect correctScript = quizCorrect.correct.GetComponent<K_QuizCorrect>();
        bool isCorrect = correctScript.isMinePlayerCorrect;

        print("제출한 답 : " + isCorrect);

        // 정답 받아서 리스트 저장시키기 (학생일 경우의 답만 저장)
        if(!PhotonNetwork.IsMasterClient)
        {
            RPC_GetAnswer(isCorrect);
        }
    }

    public void RPC_GetAnswer(bool answer)
    {
        photonView.RPC(nameof(GetAnswer), RpcTarget.All, answer);
    }


    [PunRPC]
    public void GetAnswer(bool playerAnswer)
    {
        print("받은 답 : " + playerAnswer);
        //정답인지 아닌지 받아서 리스트에 저장   
        answerList.Add(playerAnswer);
    }



    //정답이 4개 올때까지 기다리는 함수
    public IEnumerator WaitFourAnswer()
    {
        print("정답 기다리는중");
        yield return new WaitUntil(() => answerList.Count == 4);
        print("모든 정답 수집 완료");

        // 정답 판별 함수 호출 (정답 판별은 선생님이 한번만)
        if(PhotonNetwork.IsMasterClient)
        {
            RPC_CHeckAnswer();
        }
    }

    void RPC_CHeckAnswer()
    {
        photonView.RPC(nameof(CheckAnswer), RpcTarget.All);
    }

    //정답이 4개 들어오면 정답 체크
    [PunRPC]
    public void CheckAnswer()
    {
        print("정답체크");

        // isCorrect 초기에 false
        bool isCorrect = false;
        //정답 리스트를 for 문을 통해 모두가 정답인지 확인
        for (int i = 0; i < answerList.Count; i++)
        {
            if (answerList[i] == false)
            {
                isCorrect = false;
                break;
            }
            else
            {
                isCorrect = true;
            }
        }
        // isCorrect가 true인지를 CheckAnswer 함수를 통해 받아오기
        if (isCorrect)
        {
            print("isCorrect");
            // 정답이면 퀴즈 종료
            EndQuiz();
            // 연출 카메라 꺼주기 (원래대로 맵으로 돌아감)
            quizCorrect.virtualCamera.gameObject.SetActive(false);
            // 정답 맞출 시 글씨 색상 변경
            quizCorrect.text_Answer.color = Color.red;
            // 완료 시 책갈피 획득 UI 띄우기
            StartCoroutine(CompleteQuiz(2f));
        }
        else
        {
            print("isInCorrect");
            // 오답 UI
            K_QuizUiManager.instance.img_wrongA.gameObject.SetActive(true);
            // 2초 후에 꺼줄거임
            StartCoroutine(quizCorrect.HideWrongAnswer(2f));

            // 재시작 함수 실행
            StartCoroutine(RestartQuiz(5f));
            print("Restart??? YES");
        }
    }

    public void TriggerEndQuiz()
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(EndQuiz), RpcTarget.All);
        }
    }


    public void EndQuiz()
    {
        print("퀴즈 종료 호출 체크");
        isPlaying = false;
        isCounting = false;
        currTime = 0;
        // 카운트 다운 이미지 꺼주기
        //K_QuizUiManager.instance.img_countDown.gameObject.SetActive(false);

        // 정답입니다 이미지 켜주기
        K_QuizUiManager.instance.img_correctA.gameObject.SetActive(true);
        // 2초 후에 꺼줄거임
        StartCoroutine(quizCorrect.HideCorrectA(2f));

        // 디렉팅 false
        isDirecting = false;

        // 플레이어 정답 리스트 초기화
        answerList.Clear();

        Dictionary<int, PhotonView> allPlayers = bookController.allPlayers;

        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].gameObject.transform.localScale = allPlayers[i].gameObject.GetComponent<Y_PlayerAvatarSetting>().originalScale;
        }
    }

    private IEnumerator CompleteQuiz(float delay)
    {
        // 키 박스 다시 켜주자
        K_LobbyUiManager.instance.img_KeyEmptyBox.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        K_KeyManager.instance.isDoneQuiz_1 = true;
    }

    IEnumerator RestartQuiz(float delay)
    {
        // 플레이어 정답 리스트 초기화
        answerList.Clear();

        // 5초 대기
        yield return new WaitForSeconds(delay);

        // 카운트다운 시작 함수 호출
        CountDownStart();


        isPlaying = true;
        currTime = 0;
        isCounting = true;
        //isDirecting = false;

        K_QuizUiManager.instance.img_wrongA.gameObject.SetActive(false);

        // 카운트다운 다시 시작
        K_QuizUiManager.instance.img_countDown.gameObject.SetActive(true);
        //StartCoroutine(HideDirection(2f));
    }


    // 안내 UI 숨겨주는 함수
    private IEnumerator HideDirection(float delay)
    {
        yield return new WaitForSeconds(delay);
        K_QuizUiManager.instance.img_direction.gameObject.SetActive(false);

        //yield return new WaitForSeconds(1f);
        //img_countDown.gameObject.SetActive(true);
        //isCounting = true;
    }


    // 연출용 함수
    IEnumerator Start_Production()
    {
        if (!isDirecting)
        {
            K_QuizUiManager.instance.img_direction.gameObject.SetActive(true);
            // 퀴즈 박스 꺼주기
            K_LobbyUiManager.instance.img_KeyEmptyBox.gameObject.SetActive(false);
            StartCoroutine(HideDirection(2f));
            isDirecting = true;
        }

        //timeline.Play();

        yield return new WaitForSeconds(2f);

        //timeline.Pause();

        //CountDown(); // UI 시작
        // 이걸 업데이트에서 해주면 안댐;;
    }

    IEnumerator End_Production()
    {
        //timeline.Play();
        // 퀴즈 박스 다시 켜주기
        //K_LobbyUiManager.instance.img_KeyEmptyBox.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

    }
}
