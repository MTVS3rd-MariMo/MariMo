using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_QuizPos_1 : MonoBehaviour
{
    // 정답 구역의 오브젝트
    public GameObject correct1;

    private bool isQuizStarted = false;

    private void Update()
    {
        
    }

    void checkAnswer()
    {
        // 플레이어가 correct1 구역안에 있다면
        // 정답으로 처리한다
        // 구역 밖이라면
        // 오답으로 처리한다
    }

    private void OnTriggerEnter(Collider other)
    {
        K_QuizUiManager.instance.isPlaying = true;

        Debug.Log("트리거된 오브젝트: " + other.gameObject.name);

        if (other.CompareTag("Player") && !isQuizStarted)
        {
            isQuizStarted = true;
            print("플레이어다 문제풀자");
            K_QuizUiManager.instance.CountDown();
        }
    }
}
