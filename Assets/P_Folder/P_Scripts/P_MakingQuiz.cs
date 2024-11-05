using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class P_MakingQuiz : MonoBehaviour
{
    public GameObject panel_MakingAsk;

    public Button btn_Prev;
    public Button btn_Next;

    public TMP_InputField quizText;
    public TMP_InputField quizOption1;
    public TMP_InputField quizOption2;
    public TMP_InputField quizOption3;
    public TMP_InputField quizOption4;

    Quiz dummyquiz = new Quiz();

    public int count = 0;


    void Start()
    {
        btn_Prev.onClick.AddListener(OnclickPrev);
        btn_Next.onClick.AddListener(OnclickNext);

        dummyquiz = P_CreatorToolConnectMgr.Instance.dummydata.quizList[0];
    }

    void Update()
    {
        if (count == 0)
            btn_Prev.interactable = false;
        else
            btn_Prev.interactable = true;

        if (count == 1)
            btn_Next.GetComponentInChildren<TMP_Text>().text = "Done";
        else
            btn_Next.GetComponentInChildren<TMP_Text>().text = "Next";
    }

    public void OnclickPrev()
    {
        if (count <= 0)
            return;
        else
        {
            count--;

            P_CreatorToolConnectMgr.Instance.ModifyQuiz(dummyquiz, 1);


            OpenQuizSet(0);
        }
    }

    public void OnclickNext()
    {
        if (count >= 1)
        {
            // 데이터 저장

            panel_MakingAsk.gameObject.SetActive(true);
        }
        else
        {
            count++;

            P_CreatorToolConnectMgr.Instance.ModifyQuiz(dummyquiz, 0);

            // 예시
            OpenQuizSet(1);
        }
    }

    public void OpenQuizSet(int index)
    {
        // 디버깅
        Debug.Log(dummyquiz.question);

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

}
