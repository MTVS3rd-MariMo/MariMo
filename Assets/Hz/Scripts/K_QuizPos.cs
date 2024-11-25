using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class K_QuizPos : MonoBehaviourPun
{
    // ServerSettings 

    public TMP_Text text_Question;
    public TMP_Text[] text_Choices;
    public TMP_Text text_Answer;

    // 연출용
    public CinemachineVirtualCamera virtualCamera;

    // 정답 구역의 오브젝트
    public GameObject correct;

    // 퀴즈가 시작되었는지 
    public bool isQuizStarted = false;
    // 플레이어가 정답 구역에 있는지 아닌지
    public bool isInCorrectZone = false;
    // 플레이어 수 카운트
    private int playerCount = 0;

    // 퀴즈 매니저
    private K_QuizManager quizManager;
    // 북 컨트롤러
    public Y_BookController bookController;

    private void Start()
    {
        // 퀴즈 매니저 찾고
        quizManager = FindObjectOfType<K_QuizManager>();

        if (quizManager == null)
        {
            print("퀴즈매니저업슴");
        }


        bookController = GameObject.Find("BookCanvas").GetComponent<Y_BookController>();
    }

    [PunRPC]
    public void InitializeQuiz(int idx, string question, string choice1, string choice2, string choice3, string choice4, int answerIndex)
    {
        text_Question.text = question;
        text_Choices[0].text = choice1;
        text_Choices[1].text = choice2;
        text_Choices[2].text = choice3;
        text_Choices[3].text = choice4;
        // 답도 마찬가지로 설정해줘야함
        //text_Answer.text = answerIndex.ToString();

        // 답 (서버에서 int로 줌)
        // 서버로부터 받은 정답 인덱스 기반으로 올바른 선택지의 텍스트를 가져와서 저장
        if (answerIndex > 0 && answerIndex <= text_Choices.Length)
        {
            // 정답 선택지에 해당하는 오브젝트를 correct로 설정한다!
            correct = text_Choices[answerIndex - 1].gameObject;
            // 답도 마찬가지로 설정해줘야함
            text_Answer = text_Choices[answerIndex - 1];

            // QuizCorrect 동적으로 추가
            if(correct != null)
            {
                if(correct.GetComponent<K_QuizCorrect>() == null)
                {
                    correct.AddComponent<K_QuizCorrect>();
                }
            }
            //string correctAnswerText = text_Choices[answerIndex - 1].text;
            //text_Answer.text = text_Choices[answerIndex].text; // 정답 번호 저장 (정답 인덱스)

            Debug.Log("정답 오브젝트 설정 완료");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //K_QuizUiManager.instance.isPlaying = true;

        //Debug.Log("트리거된 오브젝트: " + other.gameObject.name);

        if (other.CompareTag("Player") && !isQuizStarted)
        {
            playerCount++;
            if (playerCount >= 4 && PhotonNetwork.IsMasterClient)
            {
                RPC_StartQuiz();
            }

        }

        if (other.gameObject == correct)
        {
            isInCorrectZone = true;
            print("정답 구역 들어감");
        }
    }

    void RPC_StartQuiz()
    {
        photonView.RPC(nameof(StartQuiz), RpcTarget.All);
    }

    [PunRPC]
    void StartQuiz()
    {
        // 퀴즈매니저 찾기
        if (quizManager != null)
        {
            // 퀴즈 스타트 true
            isQuizStarted = true;
            // isPlaying은 true
            quizManager.isPlaying = true;
            // HZ 여기서 LobbyCanvas 싹다 꺼주기..? Test!!!!!!!!!
            K_KeyUiManager.instance.gameObject.SetActive(false);
            // 카운트다운 시작
            quizManager.CountDownStart();
            // 시작 연출 코루틴 함수 실행
            quizManager.StartCoroutine("Start_Production");
            // 퀴즈매니저 quizCorrect는 나
            quizManager.quizCorrect = this;

            // 연출 테스트
            virtualCamera.gameObject.SetActive(true);
            // 플레이어 리스트 받아오기
            Dictionary<int, PhotonView> allPlayers = bookController.allPlayers;

            for (int i = 0; i < allPlayers.Count; i++)
            {
                allPlayers[i].gameObject.transform.localScale = allPlayers[i].gameObject.GetComponent<Y_PlayerAvatarSetting>().quizScale;
            }
        }
        else
        {
            Debug.LogError("quiManager 못찾음!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isQuizStarted)
            playerCount--;
    }

    public bool CheckAnswer()
    {
        // 정답 판별 스크립트 가져오기
        K_QuizCorrect correctScript = correct.GetComponent<K_QuizCorrect>();
        // correct 스크립트가 존재하고 isCorrect가 true, correct랑 플레이어 선택이 같다면
        if (correctScript != null && correctScript.isCorrect)
        {
            // 정답 처리
            print("정답!");
            // 정답 UI 띄워주기
            K_QuizUiManager.instance.img_correctA.gameObject.SetActive(true);
            // 정답 UI 2초 뒤에 숨겨주기
            StartCoroutine(HideCorrectA(2f));

            // 연출 테스트
            virtualCamera.gameObject.SetActive(false);

            // 키 박스 다시 켜주기
            K_LobbyUiManager.instance.img_KeyEmptyBox.gameObject.SetActive(true);

            return true;

        }
        else
        {
            // 오답 처리
            print("정답이 아님, 오답 처리");
            K_QuizUiManager.instance.img_wrongA.gameObject.SetActive(true);
            StartCoroutine(HideWrongAnswer(2f));
            return false;
        }
        ResetQuiz();
    }


    // 퀴즈 상태 초기화
    private void ResetQuiz()
    {
        if (quizManager != null)
        {
            isInCorrectZone = false;
            isQuizStarted = false;
            quizManager.isPlaying = false;
            quizManager.isCounting = false;
        }

        //isInCorrectZone = false;
        //isQuizStarted = false;
        //K_QuizManager.instance.isPlaying = false;
        //K_QuizManager.instance.isCounting = false;
    }

    // 오답입니다 UI 코루틴 함수
    public IEnumerator HideWrongAnswer(float delay)
    {
        yield return new WaitForSeconds(delay);
        K_QuizUiManager.instance.img_wrongA.gameObject.SetActive(false);

    }

    // 정답입니다 UI 코루틴 함수
    public IEnumerator HideCorrectA(float delay)
    {
        yield return new WaitForSeconds(delay);
        K_QuizUiManager.instance.img_correctA.gameObject.SetActive(false);
        K_QuizUiManager.instance.img_countDown.gameObject.SetActive(false);
    }
}
