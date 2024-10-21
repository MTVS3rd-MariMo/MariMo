using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_QuizCorrect1 : MonoBehaviour
{
    public bool isCorrect = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isCorrect)
        {
            isCorrect = true;
            K_QuizUiManager.instance.img_correctA.gameObject.SetActive(true);
            print("정답인디");

            StartCoroutine(HideCorrectA(2f));
        }
        //else
        //{
        //    isCorrect = false;
        //    K_QuizUiManager.instance.img_wrongA.gameObject.SetActive(true);
        //    print("틀렸는디");
        //}
    }

    IEnumerator HideCorrectA(float delay)
    {
        yield return new WaitForSeconds(delay);
        K_QuizUiManager.instance.img_correctA.gameObject.SetActive(false);
    }


    // 나가는 조건이 아니라 다른 오답 구역에 있을때로 바꿔야할듯

    
}
