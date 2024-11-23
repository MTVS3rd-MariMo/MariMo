using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_TitleController : MonoBehaviour
{
    // 스타트 타이틀
    public GameObject img_startTitle;
    // 스타트 타이틀 애니메이션
    public Animation anim_Title;

    void Start()
    {
        // 애니메이션 컴포넌트 찾아주고
        anim_Title = GetComponent<Animation>();
        // 스타트 타이틀 재생 끝나면 UI 꺼버림
        //anim_Title.Play();
        StartCoroutine(UnSeenTitle());
    }

    // 애니메이션 재생 후 UI 꺼주는 코루틴
    IEnumerator UnSeenTitle()
    {
        //yield return new WaitForSeconds(delay);

        //// 애니메이션 플레이
        anim_Title.Play("Title_Animation");
        // 3초 후 UI 꺼주기
        yield return new WaitForSeconds(3f);
        img_startTitle.SetActive(false);
    }
}
