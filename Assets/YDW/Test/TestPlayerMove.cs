using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerMove : MonoBehaviour
{
    public float speed;
    public CharacterController cc;

    // Start is called before the first frame update
    void Start()
    {
        //speed = 30;
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
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

        //// 구해진 방향으로 계속 이동하고 싶다.
        //// 오른쪽으로 이동하고 싶다.
        ////transform.Translate(finalDir * speed * Time.deltaTime);
        //// P = P0 + vt (이동공식)
        //transform.position += finalDir * speed * Time.deltaTime;

        cc.Move(finalDir);
    }
}
