using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_KeyMgrTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // "Player" 태그를 가진 오브젝트와 충돌했을 때
        {
            print("되나?");
            // K_KeyManager의 GetKey 메서드 호출 (인스턴스 함수)
            K_KeyManager.instance.isDoneHotSeating = true;  // 인스턴스 함수 호출
        }
    }
}
