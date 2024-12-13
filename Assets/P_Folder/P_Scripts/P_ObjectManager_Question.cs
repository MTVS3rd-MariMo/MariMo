using Cinemachine;
using Org.BouncyCastle.Asn1.Crmf;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.Playables;
using UnityEngine.UI;
using static P_ObjectManager_Question;

public class P_ObjectManager_Question : MonoBehaviourPun
{
    public class QuestionAnswer
    {
        public int lessnId;
        public int questionId;
        public string answer;
    }

    // 인풋필드 사이즈 조정
    public RectTransform inputFieldRect;
    public Vector2 expandedSize = new Vector2(1200, 200); // 확장된 크기
    public Vector2 expandedPos = new Vector2(-345, 180); // 확장됐을 때 위치
    private Vector2 originalSize;
    private Vector2 originalPosition;


    float testNum = 4;

    float triggerNum = 0;

    List<GameObject> players = new List<GameObject>();

    // UI들
    public Image question_PopUp_Img;
    public GameObject questionUI_Panel;
    public Image Img_Question;
    public TMP_Text question_Text;
    public Button btn_speaker;
    public TMP_InputField answer_InputField;
    public TMP_Text wordCount;
    public Button btn_Submit;
    public GameObject answerUI_Canvas;
    public TMP_Text answer_Text1;
    public TMP_Text answer_Text2;
    public TMP_Text answer_Test3;
    public TMP_Text answer_Test4;

    string q_answer;

    // 연출용
    public PlayableDirector timeline_Q;
    public CinemachineVirtualCamera virtual_Camera1;

    public Sprite[] btn_sprites;
    public Sprite[] input_sprites;

    // 타임라인 실행을 한번만 하기위한 체크
    bool act = false;

    // 연출용 흑백화면
    public Image blackScreen;
    Color black;

    // 질문 카운트
    int question_count = 1;
    // 답변 인원 카운트
    int answer_count = 0;

    // 바닥 그림
    public GameObject draw_Question;
    // 애니메이션 오브젝트
    public GameObject Ani_Object;

    void Start()
    {
        originalSize = inputFieldRect.sizeDelta;
        originalPosition = inputFieldRect.gameObject.transform.localPosition;

        btn_Submit.onClick.AddListener(Submit);

        black = blackScreen.color;
        BtnState(false);
    }

    private void Update()
    {
        if (answer_InputField != null && answer_InputField.isActiveAndEnabled)
        {
            WordCount();
        }

        if (answer_InputField.text.Length >= 30 || PhotonNetwork.IsMasterClient)
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
                if (photonView.IsMine)
                {
                    act = true;

                    RPC_MoveControl(false);

                    RPC_StartQuestion();
                }
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


    void RPC_StartQuestion()
    {
        photonView.RPC(nameof(StartQuestion), RpcTarget.All);
    }

    [PunRPC]
    public void StartQuestion()
    {
        StartCoroutine(Question_UI_Start());
        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_3D_OBJECT_02);
        draw_Question.SetActive(false);
        Ani_Object.SetActive(true);
    }

    [PunRPC]
    void MoveControl(bool canmove)
    {
        foreach (GameObject obj in players)
        {
            obj.GetComponent<Y_PlayerMove>().movable = canmove;

            obj.transform.position = new Vector3(obj.transform.position.x, 3, obj.transform.position.z);
        }
    }


    void Submit()
    {
        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_BUTTON);
        // 입력내용 저장
        q_answer = answer_InputField.text;
        answerUI_Canvas.SetActive(true);
        answer_InputField.gameObject.SetActive(false);
        btn_Submit.gameObject.SetActive(false);

        if (!PhotonNetwork.IsMasterClient)
        {
            QuestionAnswer questionAnswer = new QuestionAnswer()
            {
                lessnId = Y_HttpRoomSetUp.GetInstance().userlessonId,
                questionId = Y_HttpRoomSetUp.GetInstance().realClassMaterial.openQuestions[question_count].questionId,
                answer = q_answer,
            };

            // 데이터 백엔드에 전송
            SendAnswer(questionAnswer);

        }
        // 포톤으로 실행
        RPC_NextStep(q_answer);

        // 인풋필드 비우기
        answer_InputField.text = "";
    }

    void WordCount()
    {
        wordCount.text = answer_InputField.text.Length.ToString() + "/30";
    }

    void SendAnswer(QuestionAnswer answer)
    {
        HttpInfo info = new HttpInfo();
        info.url = Y_HttpLogIn.GetInstance().mainServer + "api/open-question";
        info.body = JsonUtility.ToJson(answer);
        info.contentType = "application/json";
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            Debug.Log("저장 성공");
        };

        StartCoroutine(HttpManager.GetInstance().PutOpenQ(info, Y_HttpLogIn.GetInstance().userId.ToString()));
    }
    
    void RPC_NextStep(string answer)
    {
        photonView.RPC(nameof(NextStep), RpcTarget.All, answer);
    }

    [PunRPC]
    public void NextStep(string answer)
    {
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

        answer_count++;

        // 4명 모두 답을 제출하면
        // 테스트용으로 1로 설정
        if (answer_count >= testNum + 1)
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
        question_Text.text = Y_HttpRoomSetUp.GetInstance().realClassMaterial.openQuestions[question_count].questionTitle;
        answer_InputField.text = "";

        Color color = questionUI_Panel.GetComponent<Image>().color;

        while (color.a < 1)
        {
            color.a += Time.deltaTime / 1.5f;

            questionUI_Panel.GetComponent<Image>().color = color;
            Img_Question.color = color;
            question_Text.color = color;
            answer_InputField.GetComponent<Image>().color = color;
            btn_Submit.GetComponent<Image>().color = color;

            yield return null;
        }

        if (PhotonNetwork.IsMasterClient)
            Submit();

        //yield return new WaitForSeconds(1.5f);

        //while (black.a >= 0)
        //{
        //    black.a -= Time.deltaTime / 1.5f;

        //    blackScreen.color = black;

        //    yield return null;
        //}
        //blackScreen.gameObject.SetActive(false);
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
        question_Text.text = Y_HttpRoomSetUp.GetInstance().realClassMaterial.openQuestions[question_count].questionTitle;
        answerUI_Canvas.SetActive(false);
        answer_InputField.gameObject.SetActive(true);
        btn_Submit.gameObject.SetActive(true);

        // 이전 질문 답변 지우기
        answer_Text1.text = "";
        answer_Text2.text = "";
        answer_Test3.text = "";
        answer_Test4.text = "";

        if (PhotonNetwork.IsMasterClient)
            Submit();
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

        

        questionUI_Panel.SetActive(false);
        blackScreen.gameObject.SetActive(false);

        MoveControl(true);

        K_LobbyUiManager.instance.img_KeyEmptyBox.gameObject.SetActive(true);

        K_KeyManager.instance.isDoneOpenQnA = true;
    }


    // InputField가 선택되었을 때 호출
    public void OnSelect(BaseEventData eventData)
    {
        inputFieldRect.sizeDelta = expandedSize;
        inputFieldRect.gameObject.transform.localPosition = expandedPos;

        answer_InputField.image.sprite = input_sprites[1];
    }

    // InputField가 선택 해제되었을 때 호출
    public void OnDeselect(BaseEventData eventData)
    {
        inputFieldRect.sizeDelta = originalSize;
        inputFieldRect.gameObject.transform.localPosition = originalPosition;

        answer_InputField.image.sprite = input_sprites[1];
    }

}
