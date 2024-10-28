using Cinemachine;
using Org.BouncyCastle.Asn1.Crmf;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class P_ObjectManager_Question : MonoBehaviourPun
{
    //public List<Transform> objectList = new List<Transform>();
    float triggerNum = 0;

    List<GameObject> players = new List<GameObject>();

    // UI들
    public Image question_PopUp_Img;
    public GameObject questionUI_Panel;
    public TMP_Text question_Text;
    public Button btn_speaker;
    public TMP_InputField answer_InputField;
    public Button btn_Submit;
    public GameObject answerUI_Canvas;
    public TMP_Text answer_Text1;
    public TMP_Text answer_Text2;
    public TMP_Text answer_Test3;
    public TMP_Text answer_Test4;


    // 투명벽 (플레이어 움직임을 멈춘다면 필요없을 예정)
    public GameObject wall_Q;

    // 연출용
    public PlayableDirector timeline_Q;
    public CinemachineVirtualCamera virtual_Camera1;

    public Sprite[] btn_sprites;

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
        BtnState(false);
    }

    private void Update()
    {
        if(answer_InputField.text.Length >= 50)
        {
            BtnState(true);
        }
        else
        {
            BtnState(false);
        }
    }


    void BtnState(bool can)
    {
        btn_Submit.interactable = can;

        if (can)
        {
            btn_Submit.image.sprite = btn_sprites[1];
        }
        else
        {
            btn_Submit.image.sprite = btn_sprites[0];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerNum++;
            print("+++++++++ : " + triggerNum);

            players.Add(other.gameObject);


            if (triggerNum >= 4 && !act)
            {
                act = true;
                wall_Q.SetActive(true);

                StartCoroutine(Question_UI_Start());

                MoveControl(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerNum--;
            print("-------- : " + triggerNum);

            players.Remove(other.gameObject);
        }
    }

    void MoveControl(bool canmove)
    {
        foreach (GameObject obj in players)
        {
            obj.GetComponent<Y_PlayerMove>().movable = canmove;
        }
    }


    void Submit()
    {
        // 입력내용 저장


        // 포톤으로 실행
        RPC_NextStep();
    }

    [PunRPC]
    void RPC_NextStep()
    {
        photonView.RPC(nameof(NextStep), RpcTarget.All);
    }

    void NextStep()
    {
        answer_count++;


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

        // 포톤 isMine 일 때
        if (photonView.IsMine)
        {
            answerUI_Canvas.SetActive(true);
            answer_InputField.gameObject.SetActive(false);
            btn_Submit.gameObject.SetActive(false);

            // 인풋필드 비우기
            answer_InputField.text = "";
        }


        // 4명 모두 답을 제출하면
        // 테스트용으로 1로 설정
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

    //void Moving(bool can)
    //{
    //    for (int i = 0; i < players.Length; i++)
    //    {
    //        players[i].GetComponent<P_DummyPlayer>().canWalk = can;
    //    }
    //}

    public IEnumerator Question_UI_Start()
    {
        //Moving(false);

        // 타임라인 재생
        timeline_Q.Play();
        yield return new WaitForSeconds(0.5f);

        virtual_Camera1.gameObject.SetActive(true);

        yield return new WaitForSeconds(3.5f);


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


        // 안내 UI생성
        question_PopUp_Img.gameObject.SetActive(true);
        Color color1 = question_PopUp_Img.color;

        while (color1.a <= 1)
        {
            color1.a += Time.deltaTime;

            question_PopUp_Img.color = color1;

            yield return null;
        }

        yield return new WaitForSeconds(2f);

        while (color1.a >= 0)
        {
            color1.a -= Time.deltaTime;

            question_PopUp_Img.color = color1;

            yield return null;
        }

        question_PopUp_Img.gameObject.SetActive(false);

        // 다음 질문 세팅
        question_Text.text = "질문22222";
        answerUI_Canvas.SetActive(false);
        answer_InputField.gameObject.SetActive(true);
        btn_Submit.gameObject.SetActive(true);

        // 이전 질문 답변 지우기
        answer_Text1.text = "";
        answer_Text2.text = "";
        answer_Test3.text = "";
        answer_Test4.text = "";
    }

    public IEnumerator Question_UI_Answer2()
    {
        yield return new WaitForSeconds(5f);

        // 안내 UI생성
        question_PopUp_Img.gameObject.SetActive(true);
        Color color1 = question_PopUp_Img.color;

        while (color1.a <= 1)
        {
            color1.a += Time.deltaTime;

            question_PopUp_Img.color = color1;

            yield return null;
        }

        yield return new WaitForSeconds(2f);

        while (color1.a >= 0)
        {
            color1.a -= Time.deltaTime;

            question_PopUp_Img.color = color1;

            yield return null;
        }

        question_PopUp_Img.gameObject.SetActive(false);
        // 모든 질문에 답을 했으면
        StartCoroutine(Question_UI_End());
    }

    public IEnumerator Question_UI_End()
    {
        virtual_Camera1.gameObject.SetActive(false);
        questionUI_Panel.SetActive(false);
        // 타임라인 재생
        timeline_Q.Play();

        yield return new WaitForSeconds(2f);

        

        // 사진관 모든 UI 종료
        questionUI_Panel.SetActive(false);
        blackScreen.gameObject.SetActive(false);
        wall_Q.SetActive(false);

        MoveControl(true);

        K_KeyManager.instance.isDoneOpenQnA = true;
    }

}
