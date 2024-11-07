using System;
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
}
