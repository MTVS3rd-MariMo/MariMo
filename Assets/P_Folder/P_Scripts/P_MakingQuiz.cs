using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class P_MakingQuiz : MonoBehaviour
{
    // 인풋필드 사이즈 조정
    public Vector2 expandedSize = new Vector2(1200, 250); // 확장된 크기
    public Vector2 expandedPos = new Vector2(-345, 270); // 확장됐을 때 위치
    private Vector2 originalSize;
    private Vector2 originalPosition;

    public GameObject panel_MakingAsk;

    public Button btn_Prev;
    public Button btn_Next;
    public Button btn_OK;

    public TMP_InputField quizText;
    public TMP_InputField quizOption1;
    public TMP_InputField quizOption2;
    public TMP_InputField quizOption3;
    public TMP_InputField quizOption4;

    Quiz dummyquiz = new Quiz();

    public int count = 0;

    public Sprite[] sprites;

    void Start()
    {
        btn_Prev.onClick.AddListener(OnclickPrev);
        btn_Next.onClick.AddListener(OnclickNext);
        btn_OK.onClick.AddListener(OnclickOK);

        AddInputFieldEventTriggers(quizText);
        AddInputFieldEventTriggers(quizOption1);
        AddInputFieldEventTriggers(quizOption2);
        AddInputFieldEventTriggers(quizOption3);
        AddInputFieldEventTriggers(quizOption4);
    }

    void Update()
    {
        if (count == 0)
            btn_Prev.interactable = false;
        else
            btn_Prev.interactable = true;

        if (count == 1)
        {
            btn_OK.image.sprite = sprites[1];
            btn_OK.interactable = true;
        }
        else
        {
            btn_OK.image.sprite = sprites[0];
            btn_OK.interactable = false;
        }
    }

    public void OnclickPrev()
    {
        if (count <= 0)
            return;
        else
        {
            Savetext();
            P_CreatorToolConnectMgr.Instance.ModifyQuiz(dummyquiz, count);

            count--;

            OpenQuizSet(count);
        }
    }

    public void OnclickNext()
    {
        if (count >= 1)
            return;
        else
        {
            Savetext();
            P_CreatorToolConnectMgr.Instance.ModifyQuiz(dummyquiz, count);

            count++;

            OpenQuizSet(count);
        }
    }

    public void OnclickOK()
    {
        panel_MakingAsk.gameObject.SetActive(true);
    }

    public void OpenQuizSet(int index)
    {

        dummyquiz.question = P_CreatorToolConnectMgr.Instance.GetQuiz(index).question;
        dummyquiz.choices1 = P_CreatorToolConnectMgr.Instance.GetQuiz(index).choices1;
        dummyquiz.choices2 = P_CreatorToolConnectMgr.Instance.GetQuiz(index).choices2;
        dummyquiz.choices3 = P_CreatorToolConnectMgr.Instance.GetQuiz(index).choices3;
        dummyquiz.choices4 = P_CreatorToolConnectMgr.Instance.GetQuiz(index).choices4;

        quizText.text = dummyquiz.question;
        quizOption1.text = dummyquiz.choices1;
        quizOption2.text = dummyquiz.choices2;
        quizOption3.text = dummyquiz.choices3;
        quizOption4.text = dummyquiz.choices4;

    }


    public void Savetext()
    {
        dummyquiz.question = quizText.text;
        dummyquiz.choices1 = quizOption1.text;
        dummyquiz.choices2 = quizOption2.text;
        dummyquiz.choices3 = quizOption3.text;
        dummyquiz.choices4 = quizOption4.text;
    }

    // 이벤트 트리거 추가 함수 ( 공부 필요 )
    void AddInputFieldEventTriggers(TMP_InputField inputField)
    {
        EventTrigger trigger = inputField.gameObject.AddComponent<EventTrigger>();

        // OnSelect 이벤트 추가
        EventTrigger.Entry selectEntry = new EventTrigger.Entry { eventID = EventTriggerType.Select };
        selectEntry.callback.AddListener((eventData) =>
            OnSelectInputField(inputField.GetComponent<RectTransform>())
        );
        trigger.triggers.Add(selectEntry);

        // OnDeselect 이벤트 추가
        EventTrigger.Entry deselectEntry = new EventTrigger.Entry { eventID = EventTriggerType.Deselect };
        deselectEntry.callback.AddListener((eventData) =>
            OnDeselectInputField(inputField.GetComponent<RectTransform>())
        );
        trigger.triggers.Add(deselectEntry);
    }

    // InputField가 선택되었을 때 호출
    public void OnSelectInputField(RectTransform inputFieldRect)
    {
        // 선택된 InputField의 원래 크기와 위치를 저장
        originalSize = inputFieldRect.sizeDelta;
        originalPosition = inputFieldRect.gameObject.transform.localPosition;

        // 크기와 위치를 확장
        inputFieldRect.sizeDelta = expandedSize;
        inputFieldRect.gameObject.transform.localPosition = expandedPos;
    }

    // InputField가 선택 해제되었을 때 호출
    public void OnDeselectInputField(RectTransform inputFieldRect)
    {
        // 원래 크기와 위치로 복원
        inputFieldRect.sizeDelta = originalSize;
        inputFieldRect.gameObject.transform.localPosition = originalPosition;
    }
}
