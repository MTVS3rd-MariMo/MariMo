using Cinemachine;
using Org.BouncyCastle.Asn1.Crmf;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class P_ObjectManager_Question : MonoBehaviour
{
    //public List<Transform> objectList = new List<Transform>();
    float triggerNum = 0;

    // UI들
    public GameObject questionUI_Canvas;
    public Image studioUI1_Img;
    public TMP_Text studioUI1_Text;
    public Button speaker;


    // 투명벽 (플레이어 움직임을 멈춘다면 필요없을 예정)
    public GameObject wall;

    // 연출용 타임라인
    public PlayableDirector timeline;

    // 타임라인 실행을 한번만 하기위한 체크
    bool act = false;

    // 연출용 흑백화면
    public Image blackScreen;

    Color black;

    void Start()
    {
        black = blackScreen.color;
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        triggerNum++;
        print("+++++++++ : " + triggerNum);

        // 포톤 isMine일때
        //other.GetComponent<P_DummyPlayer>().CanWalk(false);

        if (triggerNum >= 4 && !act)
        {
            act = true;
            wall.SetActive(true);

            StartCoroutine(Question_UI_Start());

        }
    }

    private void OnTriggerExit(Collider other)
    {
        triggerNum--;
        print("-------- : " + triggerNum);
    }



    private void StudioAct()
    {

    }

    public IEnumerator Question_UI_Start()
    {
        // 타임라인 재생
        timeline.Play();

        yield return new WaitForSeconds(4f);

        // 페이드 아웃
        

        while (black.a <= 1)
        {
            black.a += Time.deltaTime / 1.5f;

            blackScreen.color = black;

            yield return null;
        }

        yield return new WaitForSeconds(0.75f);

        // 타임라인 일시정지
        timeline.Pause();

        // UI캔버스
        questionUI_Canvas.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        // 페이드 인
        while (black.a >= 0)
        {
            black.a -= Time.deltaTime / 1.5f;

            blackScreen.color = black;

            yield return null;
        }

    }

    public IEnumerator Question_UI_Answer()
    { 
        // 안내 UI생성
        Color color1 = studioUI1_Img.color;
        Color color2 = studioUI1_Text.color;

        while (color1.a <= 1)
        {
            color1.a += Time.deltaTime;
            color2.a += Time.deltaTime;

            studioUI1_Img.color = color1;
            studioUI1_Text.color = color2;

            yield return null;
        }

        yield return new WaitForSeconds(3f);

        while (color1.a >= 0)
        {
            color1.a -= Time.deltaTime;
            color2.a -= Time.deltaTime;

            studioUI1_Img.color = color1;
            studioUI1_Text.color = color2;

            yield return null;
        }

        while (color1.a <= 1)
        {
            color1.a += Time.deltaTime;
            color2.a += Time.deltaTime;

            studioUI1_Img.color = color1;
            studioUI1_Text.color = color2;

            yield return null;
        }

        yield return new WaitForSeconds(2f);

        while (color1.a >= 0)
        {
            color1.a -= Time.deltaTime;
            color2.a -= Time.deltaTime;

            studioUI1_Img.color = color1;
            studioUI1_Text.color = color2;

            yield return null;
        }

        // 페이드 아웃

        // 모든 질문에 답을 했으면
        StartCoroutine(Question_UI_End());
    }


    public IEnumerator Question_UI_End()
    {
        while (black.a <= 1)
        {
            black.a += Time.deltaTime / 1.5f;

            blackScreen.color = black;

            yield return null;
        }

        // 타임라인 재생
        timeline.Play();

        yield return new WaitForSeconds(2f);

        // 페이드 인
        while (black.a >= 0)
        {
            black.a -= Time.deltaTime / 1.5f;

            blackScreen.color = black;

            yield return null;
        }

        // 사진관 모든 UI 종료
        questionUI_Canvas.SetActive(false);
    }

}
