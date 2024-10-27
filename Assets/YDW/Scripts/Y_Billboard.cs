using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_Billboard : MonoBehaviour
{
    void Update()
    {
        if(Camera.main != null)
        //나의 앞방향을 카메라의 앞 방향으로 설정
        transform.forward = Camera.main.transform.forward;
    }

}
