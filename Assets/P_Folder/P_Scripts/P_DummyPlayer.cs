using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 변수 : 데이터를 담는 그릇
// 변수 만드는 법 : 자료형 변수이름;

public class PlayerMove : MonoBehaviour
{
    // 이동 속력
    public float speed = 5;

    void Start()
    {
    }

    void Update()
    {
        // 사용자의 입력을 받아서
        // A : -1, D : 1, 누르지 않으면 0
        float h = Input.GetAxis("Horizontal");
        // S : -1, W : 1, 누르지 않으면 0
        float v = Input.GetAxis("Vertical");

        // 방향을 구하자.
        // 수평 방향
        Vector3 dirH = transform.right * h;
        // 수직 방향
        Vector3 dirV = transform.forward * v;
        // 최종적으로 이동해야 하는 방향
        Vector3 finalDir = dirH + dirV;
        // finalDir 을 정규화 하자 (벡터의 크기를 1로 만든다)
        finalDir.Normalize();

        // 구해진 방향으로 계속 이동하고 싶다.
        // 오른쪽으로 이동하고 싶다.
        //transform.Translate(finalDir * speed * Time.deltaTime);
        // P = P0 + vt (이동공식)
        transform.position += finalDir * speed * Time.deltaTime;
    }
}
