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


    public GameObject question_Canvas;
    // UI들
    public Image question_PopUp_Img;
    public TMP_Text question_PopUp_Text;
    public GameObject questionUI_Panel;
    public TMP_Text question_Text;
    public Button btn_speaker;
    public Image answer_Img;
    public TMP_InputField answer_InputField;
    public Button btn_Submit;
    public GameObject answerUI_Canvas;
    public TMP_Text answer_Text1;
    public TMP_Text answer_Text2;
    public TMP_Text answer_Test3;
    public TMP_Text answer_Test4;


    // 투명벽 (플레이어 움직임을 멈춘다면 필요없을 예정)
    public GameObject wall;

    // 연출용 타임라인
    public PlayableDirector timeline_Q;

    // 타임라인 실행을 한번만 하기위한 체크
    bool act = false;

    // 연출용 흑백화면
    public Image blackScreen;
    Color black;

    // 질문 카운트
    float question_count = 0f;
    // 답변 인원 카운트
    float answer_count = 0f;

    void Start()
    {
        btn_Submit.onClick.AddListener(Submit);
        btn_speaker.onClick.AddListener(Speaker);

        black = blackScreen.color;
    }

    void Submit()
    {
        // 입력내용 저장


        // 포톤으로 실행
        NextStep();
    }

    void NextStep()
    {
        answer_count++;

        // 포톤 isMine 일 때
        // if (PhotonView.isMine) {
        answerUI_Canvas.SetActive(true);
        answer_Img.gameObject.SetActive(false);
        btn_Submit.gameObject.SetActive(false);

        if (answer_count == 1)
        {
            answer_Text1.text = answer_InputField.text;
        }
        else if (answer_count == 2)
        {
            answer_Text2.text = answer_InputField.text;
        }
        else if (answer_count == 3)
        {
            answer_Test3.text = answer_InputField.text;
        }
        else if (answer_count == 4)
        {
            answer_Test4.text = answer_InputField.text;
        }

        // 인풋필드 비우기
        answer_InputField.text = "";
        //}

        // 4명 모두 답을 제출하면
        if (answer_count >= 4)
        {
            if (question_count == 0)
            {
                answer_count = 0;

                question_count = 1;
                StartCoroutine(Question_UI_Answer1());
            }
            else if (question_count == 1)
            {
                answer_count = 0;

                question_count = 2;
                StartCoroutine(Question_UI_Answer2());
            }
            
        }
    }

    void Speaker()
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
            question_Canvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        triggerNum--;
        print("-------- : " + triggerNum);
    }

    public IEnumerator Question_UI_Start()
    {
        // 타임라인 재생
        timeline_Q.Play();

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
        timeline_Q.Pause();

        // UI 패널
        questionUI_Panel.SetActive(true);

        // 질문 세팅
        question_Text.text = "질문 111111";
        answer_InputField.text = "";

        yield return new WaitForSeconds(1.5f);

        while (black.a >= 0)
        {
            black.a -= Time.deltaTime / 1.5f;

            blackScreen.color = black;

            yield return null;
        }
        blackScreen.gameObject.SetActive(false);
    }

    public IEnumerator Question_UI_Answer1()
    {
        // 다른사람 답변 읽기 대기
        yield return new WaitForSeconds(5f);

        question_PopUp_Text.text = "모두가 답변을 완료했어요!\n참 잘했어요!";

        // 안내 UI생성
        Color color1 = question_PopUp_Img.color;
        Color color2 = question_PopUp_Text.color;

        while (color1.a <= 1)
        {
            color1.a += Time.deltaTime;
            color2.a += Time.deltaTime;

            question_PopUp_Img.color = color1;
            question_PopUp_Text.color = color2;

            yield return null;
        }

        yield return new WaitForSeconds(2f);

        while (color1.a >= 0)
        {
            color1.a -= Time.deltaTime;
            color2.a -= Time.deltaTime;

            question_PopUp_Img.color = color1;
            question_PopUp_Text.color = color2;

            yield return null;
        }

        // 다음 질문 세팅
        question_Text.text = "질문22222";
        answerUI_Canvas.SetActive(false);
        answer_Img.gameObject.SetActive(true);
        btn_Submit.gameObject.SetActive(true);

        // 이전 질문 답변 지우기
        answer_Text1.text = "";
        answer_Text2.text = "";
        answer_Test3.text = "";
        answer_Test3.text = "";
    }

    public IEnumerator Question_UI_Answer2()
    { 
        // 안내 UI생성
        Color color1 = question_PopUp_Img.color;
        Color color2 = question_PopUp_Text.color;

        while (color1.a <= 1)
        {
            color1.a += Time.deltaTime;
            color2.a += Time.deltaTime;

            question_PopUp_Img.color = color1;
            question_PopUp_Text.color = color2;

            yield return null;
        }

        yield return new WaitForSeconds(2f);

        while (color1.a >= 0)
        {
            color1.a -= Time.deltaTime;
            color2.a -= Time.deltaTime;

            question_PopUp_Img.color = color1;
            question_PopUp_Text.color = color2;

            yield return null;
        }

        // 모든 질문에 답을 했으면
        StartCoroutine(Question_UI_End());
    }

    public IEnumerator Question_UI_End()
    {
        blackScreen.gameObject.SetActive(true);

        while (black.a <= 1)
        {
            black.a += Time.deltaTime / 1.5f;

            blackScreen.color = black;

            yield return null;
        }

        questionUI_Panel.SetActive(false);
        // 타임라인 재생
        timeline_Q.Play();

        yield return new WaitForSeconds(2f);

        while (black.a >= 0)
        {
            black.a -= Time.deltaTime / 1.5f;

            blackScreen.color = black;

            yield return null;
        }

        // 사진관 모든 UI 종료
        question_Canvas.SetActive(false);
    }

}
