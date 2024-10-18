using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_QuizPos_1 : MonoBehaviour
{
    public GameObject quiz1;

    // Start is called before the first frame update
    void Start()
    {
        // 처음에는 둘다 비활성화
        quiz1.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("트리거된 오브젝트: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            print("플레이어다");
            quiz1.gameObject.SetActive(true);
        }
    }
}
