using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_QuizPos : MonoBehaviour
{
    public GameObject quiz1;
    public GameObject quiz2;

    void Start()
    {
        // 처음에는 둘다 비활성화
        quiz1.gameObject.SetActive(false);
        quiz2.gameObject.SetActive(false);
    }


    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("트리거된 오브젝트: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            print("플레이어다");
            //quiz1.gameObject.SetActive(true);
            quiz2.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {

    }
}
