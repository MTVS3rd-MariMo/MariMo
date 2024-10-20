using Cinemachine;
using Org.BouncyCastle.Asn1.Crmf;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class P_ObjectManager : MonoBehaviour
{
    //public List<Transform> objectList = new List<Transform>();
    float triggerNum = 0;

    public GameObject studioUI_Canvas;
    public Image studioUI1_Img;
    public TMP_Text studioUI1_Text;
    public TMP_Text timeCount_Text;

    public CinemachineVirtualCamera for_Directing1;
    public CinemachineVirtualCamera for_Directing2;

    public GameObject wall;

    public PlayableDirector timeline;

    bool act = false;

    float timeCount = 5f;

    public Image blackScreen;
    public Image whiteScreen;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(triggerNum >= 4 && !act)
        {
            act = true;
            wall.SetActive(true);

            StartCoroutine(Studio_UI_Player());

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        triggerNum++;
        print("+++++++++ : " + triggerNum);
    }

    private void OnTriggerExit(Collider other)
    {
        triggerNum--;
        print("-------- : " + triggerNum);
    }

    private void StudioAct()
    {

    }

    public IEnumerator Studio_UI_Player()
    {
        // 타임라인 재생
        timeline.Play();

        for_Directing1.gameObject.SetActive(true);
        for_Directing2.gameObject.SetActive(true);

        yield return new WaitForSeconds(4f);


        studioUI_Canvas.SetActive(true);

        Color black = blackScreen.color;

        while(black.a <=1)
        {
            black.a += Time.deltaTime / 1.5f;

            blackScreen.color = black;

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // 타임라인 일시정지
        timeline.Pause();

        yield return new WaitForSeconds(2f);


        while (black.a >= 0)
        {
            black.a -= Time.deltaTime / 1.5f;

            blackScreen.color = black;

            yield return null;
        }


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

        studioUI1_Text.text = "5초 뒤에\n사진을 찍습니다";

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
        
        timeCount_Text.gameObject.SetActive(true);

        Color tcolor = timeCount_Text.color;

        while (timeCount > 0)
        {
            tcolor.a = 1;

            timeCount_Text.color = tcolor;

            timeCount_Text.text = timeCount.ToString();

            timeCount--;

            while (tcolor.a >= 0)
            {
                tcolor.a -= Time.deltaTime;

                timeCount_Text.color = tcolor;

                yield return null;
            }
        }

        whiteScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        whiteScreen.gameObject.SetActive(false);

        studioUI_Canvas.SetActive(false);
        for_Directing1.gameObject.SetActive(false);
        for_Directing2.gameObject.SetActive(false);

        TakePicture();

        // 타임라인 재생
        timeline.Play();

    }


    void TakePicture()
    {
        string timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd");
        string fileName = "SCREENSHOT-" + timeStamp + ".png";

        // 플랫폼별 분기 
        //#if UNITY_IPONE || UNITY_ANDROID
        //        CaptureScreenForMoblie(fileName);
        //#else
        //        CaptureScreenForPC(fileName);
        //#endif

        // 플랫폼별 분기 사용 안할시
        // 파일 이름은 임시로 test.png 
        CaptureScreenForPC("test.png");
    }

    private void CaptureScreenForPC(string fileName)
    {
        // 다운로드 폴더에 저장 ( 실패 )
        //ScreenCapture.CaptureScreenshot("~/Downloads/" + fileName);

        // 경로 미지정시 프로젝트 파일에 저장
        ScreenCapture.CaptureScreenshot(fileName);
    }

    private void CaptureScreenForMobile(string fileName)
    {
        // 모바일로 사용시 추가 경로지정 필요
        ScreenCapture.CaptureScreenshot(fileName);
    }
}
