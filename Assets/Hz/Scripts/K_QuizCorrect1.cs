using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_QuizCorrect1 : MonoBehaviour
{
    // 정답인지
    public bool isCorrect = false;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 트리거 했는지, isCorrect인지
        if(other.CompareTag("Player") && !isCorrect)
        {
            // 정답 true
            isCorrect = true;
            print("정답구역");
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
        if(other.CompareTag("Player"))
        {
            isCorrect = false;
            print("정답구역 벗어남");
        }
    }


    // 나가는 조건이 아니라 다른 오답 구역에 있을때로 바꿔야할듯


}
