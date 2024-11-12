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
    // 정답 텍스트
    //public TextMeshPro text_Correct;

    // 퀴즈가 시작되었는지 
    public bool isQuizStarted = false;
    // 플레이어가 정답 구역에 있는지 아닌지
    public bool isInCorrectZone = false;

    private int playerCount = 0;

    private K_QuizManager quizManager;
    private K_MapQuizSetUp k_MapQuizSetUp;

    private void Start()
    {
        // 퀴즈 매니저 찾고
        quizManager = FindObjectOfType<K_QuizManager>();

        if (quizManager == null)
        {
            print("퀴즈매니저업슴");
        }

        // 퀴즈 셋업하는거 찾음 댐
        k_MapQuizSetUp = FindObjectOfType<K_MapQuizSetUp>();

        //if(k_MapQuizSetUp != null)
        //{
        //    k_MapQuizSetUp.UpdateQuiz1Text(Question, choices[].ToString());
        //    k_MapQuizSetUp.UpdateQuiz2Text(Question, choices[].ToString());
        //}

    }

    [PunRPC]
    public void InitializeQuiz(string question, string choice1, string choice2, string choice3, string choice4, int answer)
    {

        text_Question.text = question;

        text_Choices[0].text = choice1;
        text_Choices[1].text = choice2;
        text_Choices[2].text = choice3;
        text_Choices[3].text = choice4;

        text_Answer.text = answer.ToString();

    }

    private void OnTriggerEnter(Collider other)
    {
        //K_QuizUiManager.instance.isPlaying = true;

        //Debug.Log("트리거된 오브젝트: " + other.gameObject.name);

        if (other.CompareTag("Player") && !isQuizStarted)
        {
            playerCount++;
            if (playerCount >= 4 && photonView.IsMine)
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
        if (quizManager != null)
        {
            isQuizStarted = true;
            print("플레이어다 문제풀자");

            quizManager.isPlaying = true;
            //  K_QuizManager.instance.CountDown();
            quizManager.quizCorrect = this;

            // 연출 테스트
            virtualCamera.gameObject.SetActive(true);

            Dictionary<int, PhotonView> allPlayers = GameObject.Find("BookCanvas").GetComponent<Y_BookController>().allPlayers;

            for (int i = 0; i < allPlayers.Count; i++)
            {
                allPlayers[i].gameObject.transform.localScale = allPlayers[i].gameObject.GetComponent<Y_PlayerAvatarSetting>().quizScale;
            }
        }
        else
        {
            Debug.LogError("quiManager 못찾음!");
        }

        //isQuizStarted = true;
        //print("플레이어다 문제풀자");

        //K_QuizManager.instance.isPlaying = true;
        ////  K_QuizManager.instance.CountDown();
        //K_QuizManager.instance.quizCorrect = GetComponentInParent<K_QuizPos>();

        //// 연출 테스트
        //virtualCamera.gameObject.SetActive(true);

        //Dictionary<int, PhotonView> allPlayers = Y_BookController.Instance.allPlayers;

        //for (int i = 0; i < allPlayers.Count; i++)
        //{
        //    allPlayers[i].gameObject.transform.localScale = allPlayers[i].gameObject.GetComponent<Y_PlayerAvatarSetting>().quizScale;
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isQuizStarted)
            playerCount--;
    }

    public bool CheckAnswer()
    {

        // 다시
        K_QuizCorrect correctScript = correct.GetComponent<K_QuizCorrect>();
        // correct1 트리거에 플레이어가 있는지 확인
        if (correctScript != null && correctScript.isCorrect)
        {
            // 정답 처리
            print("정답!");
            K_QuizUiManager.instance.img_correctA.gameObject.SetActive(true);
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




        //// 다시
        //K_QuizCorrect correctScript = correct.GetComponent<K_QuizCorrect>();
        //// correct1 트리거에 플레이어가 있는지 확인
        //if (correctScript != null && correctScript.isCorrect)
        //{
        //    // 정답 처리
        //    print("정답!");
        //    K_QuizUiManager.instance.img_correctA.gameObject.SetActive(true);
        //    StartCoroutine(HideCorrectA(2f));

        //    // 연출 테스트
        //    virtualCamera.gameObject.SetActive(false);

        //    // 키 박스 다시 켜주기
        //    K_LobbyUiManager.instance.img_KeyEmptyBox.gameObject.SetActive(true);

        //    return true;

        //}
        //else
        //{
        //    // 오답 처리
        //    print("정답이 아님, 오답 처리");
        //    K_QuizUiManager.instance.img_wrongA.gameObject.SetActive(true);
        //    StartCoroutine(HideWrongAnswer(2f));
        //    return false;
        //}
        //ResetQuiz();
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

    public IEnumerator HideWrongAnswer(float delay)
    {
        yield return new WaitForSeconds(delay);
        K_QuizUiManager.instance.img_wrongA.gameObject.SetActive(false);

    }

    public IEnumerator HideCorrectA(float delay)
    {
        yield return new WaitForSeconds(delay);
        K_QuizUiManager.instance.img_correctA.gameObject.SetActive(false);
        K_QuizUiManager.instance.img_countDown.gameObject.SetActive(false);
    }
}
