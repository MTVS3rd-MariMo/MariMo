using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Dummy2DPlayer : MonoBehaviour
{
    public float speed = 5f;


    bool inPhoto = false;
    

    void Update()
    {
        if (inPhoto)
            Move();
    }

    public void Move()
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

        // 화면 밖으로 나가지 못하게 고정
        Vector3 viewPortPoint = Camera.main.WorldToViewportPoint(transform.position);

        // 당겨지는 강도 
        float pulling = 100f;

        if (viewPortPoint.x < 0.1f)
        {
            transform.position += Vector3.right * (0.1f -viewPortPoint.x) * pulling * Time.deltaTime;
        }

        if (viewPortPoint.x > 0.9f)
        {
            transform.position += Vector3.right * (0.9f - viewPortPoint.x) * pulling * Time.deltaTime;
        }

        if (viewPortPoint.y < 0.1f)
        {
            transform.position += Vector3.forward * (0.1f - viewPortPoint.y) * pulling * Time.deltaTime;
        }

        if (viewPortPoint.y > 0.9f)
        {
            transform.position += Vector3.forward * (0.9f - viewPortPoint.y) * pulling * Time.deltaTime;
        }
    }

    public void InPhoto(bool ismoving)
    {
        inPhoto = ismoving;
    }

    public void PosLock()
    {
        
    }
}
