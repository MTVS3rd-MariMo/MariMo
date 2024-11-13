using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
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

    //public static K_QuizManager instance;


    // 연출용 요소들
    //public Image blackScreen;
    //public PlayableDirector timeline;



    //private void Awake()
    //{
    //    if (null == instance)
    //    {
    //        instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else
    //    {
    //        Destroy(this.gameObject);
    //    }
    //}

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
            StartCoroutine(Start_Production());
        }
    }

    // Server
    public void UpdateQuizFromServer()
    {
        // 
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
                
                // 시간초과(텍스트) 보여주자.
                K_QuizUiManager.instance.text_countDown.text = "시간 종료!";

                if (photonView.IsMine)
                {
                    // 정답 판별 함수 호출
                    RPC_CHeckAnswer();
                }
                
            }
            // 그렇지 않으면
            else
            {
                K_QuizUiManager.instance.img_countDown.gameObject.SetActive(false);
            }
        }

    }

    void RPC_CHeckAnswer()
    {
        photonView.RPC(nameof(CheckAnswer), RpcTarget.All);
    }


    [PunRPC]
    public void CheckAnswer()
    {
        if (quizCorrect != null)
        {
            bool isCorrect = quizCorrect.CheckAnswer();

            if(isCorrect)
            {
                // 오류 ->> EndQuiz를 RPC로 해줘야하나?
                EndQuiz();
                // 정답 맞출 시 글씨 색상 변경
                quizCorrect.text_Choices[quizSpawnMgr.answerNumber].color = Color.red;
                StartCoroutine(CompleteQuiz(2f));
            }
            else
            {
                StartCoroutine(RestartQuiz(5f));
            }    
        }
    }

    public void TriggerEndQuiz()
    {
        if(photonView.IsMine)
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
        K_QuizUiManager.instance.img_countDown.gameObject.SetActive(false);

        isDirecting = false;

        Dictionary<int, PhotonView> allPlayers = bookController.allPlayers;

        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].gameObject.transform.localScale = allPlayers[i].gameObject.GetComponent<Y_PlayerAvatarSetting>().originalScale;
        }


        // 현재 활성화 상태 activeSelf - bool 값
        if (K_QuizUiManager.instance.img_wrongA.gameObject.activeSelf)
        {
            StartCoroutine(RestartQuiz(5f));
        }
        else if (K_QuizUiManager.instance.img_correctA.gameObject.activeSelf)
        {
            // 마무리 연출 시작
        }
    }

    private IEnumerator CompleteQuiz(float delay)
    {
        yield return new WaitForSeconds(delay);
        K_KeyManager.instance.isDoneQuiz_1 = true;
    }

    IEnumerator RestartQuiz(float delay)
    {
        yield return new WaitForSeconds(delay);

        isPlaying = true;
        currTime = 0;
        isCounting = true;
        //isDirecting = false;

        K_QuizUiManager.instance.img_correctA.gameObject.SetActive(false);
        K_QuizUiManager.instance.img_wrongA.gameObject.SetActive(false);

        // 카운트다운 다시 시작
        K_QuizUiManager.instance.img_countDown.gameObject.SetActive(true);
        //StartCoroutine(HideDirection(2f));
    }


    private IEnumerator HideDirection(float delay)
    {
        yield return new WaitForSeconds(delay);
        K_QuizUiManager.instance.img_direction.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);
        //img_countDown.gameObject.SetActive(true);
        isCounting = true;
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

        CountDown(); // UI 시작
    }

    IEnumerator End_Production()
    {
        //timeline.Play();
        // 퀴즈 박스 다시 켜주기
        //K_LobbyUiManager.instance.img_KeyEmptyBox.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);
        
    }
}
