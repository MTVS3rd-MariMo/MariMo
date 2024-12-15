using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class K_PaintController : MonoBehaviour
{
    public GameObject img_drawGuide;

    public GameObject img_MakingRect;
    public GameObject img_MakingBar;


    private void Start()
    {
        img_MakingRect = GameObject.Find("img_MakingRect");
        img_MakingBar = GameObject.Find("img_MakingBar");

        // 첨엔 로딩 UI 꺼놓기
        img_MakingRect.SetActive(false);

    }

    void Update()
    {
        OnDrawGuide();
    }

    // 아바타 로딩 UI 코루틴 함수
    public IEnumerator AvatarLoading()
    {
        // UI 띄워주기
        img_MakingRect.SetActive(true);

        float time = 0;
        // 임시 셋팅 (더미 데이터 기준)
        while(time < 1.5f)
        {
            time += Time.deltaTime;
            img_MakingBar.GetComponentInChildren<Image>().fillAmount = Mathf.Lerp(img_MakingBar.GetComponent<Image>().fillAmount, 1, time / 2);
            yield return null;
        }

        // UI 꺼주기
        img_MakingRect.SetActive(false);
    }

    // 그림판 안내 가이드
    void OnDrawGuide()
    {
        StartCoroutine(OffDrawGuide(2f));
    }

    private IEnumerator OffDrawGuide(float delay)
    {
        yield return new WaitForSeconds(delay);

        img_drawGuide.SetActive(false);
    }
}
