using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class K_QuizCorrect : MonoBehaviour
{
    int playerCount = 0;

    // 정답인지
    public bool isCorrect = false;

    // 퀴즈 매니저
    private K_QuizManager k_QuizManager;

    private void Start()
    {
        k_QuizManager = FindObjectOfType<K_QuizManager>();
    }

    private void OnTriggerEnter(Collider other)
    {       
        // 플레이어가 트리거 했는지, isCorrect인지
        if (other.CompareTag("Player") && !isCorrect)
        {
            playerCount++;

            if(playerCount >= 4)
            {
                // 정답 true
                isCorrect = true;
                print("정답구역");
                // 퀴즈 매니저에게 전달
                k_QuizManager.OnCorrectTrigger(this);

            }
        }
    }

    public IEnumerator HideCorrectA(float delay)
    {
        yield return new WaitForSeconds(delay);
        K_QuizUiManager.instance.img_correctA.gameObject.SetActive(false);
        K_QuizUiManager.instance.img_countDown.gameObject.SetActive(false);
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount--;
            if(playerCount < 4)
            isCorrect = false;
            print("정답구역 벗어남");
            // 퀴즈 매니저에게 전달
            k_QuizManager.OnCorrectTrigger(this);
        }
    }
}
