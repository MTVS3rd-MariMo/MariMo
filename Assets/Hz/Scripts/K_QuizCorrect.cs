using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class K_QuizCorrect : MonoBehaviour
{
    int playerCount = 0;

    // 각각의 플레이어가 정답인지 (단일 체크)
    public bool isMinePlayerCorrect = false;

    // 정답인지 (멀티 동시 체크)
    public bool isCorrect = false;

    // 퀴즈 매니저
    private K_QuizManager k_QuizManager;

    private void Start()
    {
        k_QuizManager = FindObjectOfType<K_QuizManager>();
    }

    private void OnTriggerEnter(Collider other)
    {   
        
        if(other.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            isMinePlayerCorrect = true;
        }


        // 플레이어가 트리거 했는지, isCorrect인지
        if (other.CompareTag("Player") && !isCorrect)
        {
            playerCount++;

            if(playerCount >= 4)
            {
                // 정답 true
                isCorrect = true;
                print("정답구역");

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
        if (other.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            isMinePlayerCorrect = false;
        }

        if (other.CompareTag("Player"))
        {
            playerCount--;
            if(playerCount < 4)
            isCorrect = false;
            print("정답구역 벗어남");
        }
    }
}
