using Cinemachine;
using Org.BouncyCastle.Asn1.Crmf;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Playables;
using UnityEngine.UI;

public class P_ObjectManager_Question : MonoBehaviourPun
{
    public class QuestionAnswer
    {
        public int lessnId;
        public int questionId;
        public string answer;
    }


    public float testNum = 4;

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

    string q_answer;

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
    int answer_count = 0;

    void Start()
    {
        btn_Submit.onClick.AddListener(Submit);

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


            if (triggerNum >= testNum && !act)
            {
                act = true;
                wall_Q.SetActive(true);

                RPC_MoveControl(false);

                StartCoroutine(Question_UI_Start());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !act)
        {
            triggerNum--;
            print("-------- : " + triggerNum);

            players.Remove(other.gameObject);
        }
    }

    void RPC_MoveControl(bool canmove)
    {
        photonView.RPC(nameof(MoveControl), RpcTarget.All, canmove);
    }


    [PunRPC]
    void MoveControl(bool canmove)
    {
        foreach (GameObject obj in players)
        {
            obj.GetComponent<Y_PlayerMove>().movable = canmove;

            print(obj.GetComponent<Y_PlayerMove>().movable);
        }
    }


    void Submit()
    {
        // 입력내용 저장
        q_answer = answer_InputField.text;
        answerUI_Canvas.SetActive(true);
        answer_InputField.gameObject.SetActive(false);
        btn_Submit.gameObject.SetActive(false);

        QuestionAnswer questionAnswer = new QuestionAnswer()
        {
            lessnId = 1,
            questionId = 3,//Y_HttpRoomSetUp.GetInstance().realClassMaterial.openQuestions[answer_count].questionId,
            answer = q_answer,
        };

        // 인풋필드 비우기
        answer_InputField.text = "";

        // 데이터 백엔드에 전송
        HttpInfo info = new HttpInfo();
        info.url = Y_HttpLogIn.GetInstance().mainServer + "api/open-question";
        info.body = JsonUtility.ToJson(questionAnswer);
        info.contentType = "application/json";
        info.onComplete = (DownloadHandler downloadHandler) =>
        {

        };

        StartCoroutine(HttpManager.GetInstance().PutOpenQ(info, Y_HttpLogIn.GetInstance().userId.ToString()));

        // 포톤으로 실행
        RPC_NextStep(q_answer);
    }

    
    void RPC_NextStep(string answer)
    {
        photonView.RPC(nameof(NextStep), RpcTarget.All, answer);
    }

    [PunRPC]
    public void NextStep(string answer)
    {
        answer_count++;


        if (answer_count == 1)
        {
            answer_Text1.text = answer;
        }
        else if (answer_count == 2)
        {
            answer_Text2.text = answer;
        }
        else if (answer_count == 3)
        {
            answer_Test3.text = answer;
        }
        else if (answer_count == 4)
        {
            answer_Test4.text = answer;
        }

        // 4명 모두 답을 제출하면
        // 테스트용으로 1로 설정
        if (answer_count >= testNum)
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

        // KeyBox false 
        K_LobbyUiManager.instance.img_KeyEmptyBox.gameObject.SetActive(false);

        // UI 패널
        questionUI_Panel.SetActive(true);

        // 질문 세팅
        question_Text.text = "수남이가 주인 영감님을 좋아하는 이유는 무엇인가요?\n여러분에게도 주변에서 격려와 응원을 해 주는 사람이 있나요?";
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
        question_Text.text = "수남이는 자전거를 타고 도망치면서 자유와 해방감을 느꼈습니다.\n여러분에게도 기분이 좋아지는 특별한 활동이 있나요? 그 활동을 하면 어떤 기분이 드나요?";
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

        K_LobbyUiManager.instance.img_KeyEmptyBox.gameObject.SetActive(true);

        K_KeyManager.instance.isDoneOpenQnA = true;
    }

}
