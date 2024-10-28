using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_QuizPos : MonoBehaviour
{
    // 연출용
    public CinemachineVirtualCamera virtualCamera;

    // 정답 구역의 오브젝트
    public GameObject correct;

    // 퀴즈가 시작되었는지 
    public bool isQuizStarted = false;
    // 플레이어가 정답 구역에 있는지 아닌지
    public bool isInCorrectZone = false;

    private void OnTriggerEnter(Collider other)
    {
        //K_QuizUiManager.instance.isPlaying = true;

        //Debug.Log("트리거된 오브젝트: " + other.gameObject.name);

        if (other.CompareTag("Player") && !isQuizStarted)
        {
            isQuizStarted = true;
            print("플레이어다 문제풀자");

            K_QuizManager.instance.isPlaying = true;
            //  K_QuizManager.instance.CountDown();
            K_QuizManager.instance.quizCorrect = GetComponentInParent<K_QuizPos>();

            // 연출 테스트
            virtualCamera.gameObject.SetActive(true);

            Dictionary<int, PhotonView> allPlayers = Y_BookController.Instance.allPlayers;

            for (int i = 0; i < allPlayers.Count; i++)
            {
                allPlayers[i].gameObject.transform.localScale = allPlayers[i].gameObject.GetComponent<Y_PlayerAvatarSetting>().quizScale;
            }

        }

        if (other.gameObject == correct)
        {
            isInCorrectZone = true;
            print("정답 구역 들어감");
        }
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
        isInCorrectZone = false;
        isQuizStarted = false;
        K_QuizManager.instance.isPlaying = false;
        K_QuizManager.instance.isCounting = false;
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
