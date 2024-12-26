using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_TitleController : MonoBehaviour
{
    // 스타트 타이틀
    public GameObject img_startTitle;

    void Start()
    {
        // 애니메이션 꺼주는 코루틴 재생
        StartCoroutine(UnSeenTitle());
    }

    // 애니메이션 재생 후 UI 꺼주는 코루틴
    IEnumerator UnSeenTitle()
    {
        // 3초 후 UI 꺼주기
        yield return new WaitForSeconds(3f);
        img_startTitle.SetActive(false);
    }
}
