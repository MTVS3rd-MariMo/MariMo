using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_QuizPos_1 : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        K_QuizUiManager.instance.isPlaying = true;

        Debug.Log("트리거된 오브젝트: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            print("플레이어다");
            K_QuizUiManager.instance.CountDown();
        }
    }
}
