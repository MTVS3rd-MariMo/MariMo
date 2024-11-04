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

    public string prevText;
    public string prevOption1;
    public string prevOption2;
    public string prevOption3;
    public string prevOption4;

    public string nextText;
    public string nextOption1;
    public string nextOption2;
    public string nextOption3;
    public string nextOption4;

    public int count = 0;


    void Start()
    {
        btn_Prev.onClick.AddListener(OnclickPrev);
        btn_Next.onClick.AddListener(OnclickNext);

    }

    void Update()
    {
        if (count == 0)
            btn_Prev.interactable = false;
        else
            btn_Prev.interactable = true;

        if (count == 2)
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
            
            nextText = quizText.text;
            nextOption1 = quizOption1.text;
            nextOption2 = quizOption2.text;
            nextOption3 = quizOption3.text;
            nextOption4 = quizOption4.text;


            // 예시
            quizText.text = prevText;
            quizOption1.text = prevOption1;
            quizOption2.text = prevOption2;
            quizOption3.text = prevOption3;
            quizOption4.text = prevOption4;
        }
    }

    public void OnclickNext()
    {
        if (count >= 2)
        {
            panel_MakingAsk.gameObject.SetActive(true);
            // 다음 창으로 이동
            GetComponentInParent<P_CreatorToolController>().OnclickSelectStory();
        }
        else
        {
            count++;

            prevText = quizText.text;
            prevOption1 = quizOption1.text;
            prevOption2 = quizOption2.text;
            prevOption3 = quizOption3.text;
            prevOption4 = quizOption4.text;

            // 예시
            quizText.text = nextText;
            quizOption1.text = nextOption1;
            quizOption2.text = nextOption2;
            quizOption3.text = nextOption3;
            quizOption4.text = nextOption4;
        }
    }

    public void OpenQuizSet()
    {
        for (int i = 0; i < P_CreatorToolConnectMgr.Instance.GetQuizCount(); i++)
        {
            if (P_CreatorToolConnectMgr.Instance.GetQuiz(i).quizId == P_CreatorToolConnectMgr.Instance.selectedID[0])
            {
                prevText = P_CreatorToolConnectMgr.Instance.GetQuiz(i).question;
                prevOption1 = P_CreatorToolConnectMgr.Instance.GetQuiz(i).choices1;
                prevOption2 = P_CreatorToolConnectMgr.Instance.GetQuiz(i).choices2;
                prevOption3 = P_CreatorToolConnectMgr.Instance.GetQuiz(i).choices3;
                prevOption4 = P_CreatorToolConnectMgr.Instance.GetQuiz(i).choices4;
            }

            if (P_CreatorToolConnectMgr.Instance.GetQuiz(i).quizId == P_CreatorToolConnectMgr.Instance.selectedID[1])
            {
                nextText = P_CreatorToolConnectMgr.Instance.GetQuiz(i).question;
                nextOption1 = P_CreatorToolConnectMgr.Instance.GetQuiz(i).choices1;
                nextOption2 = P_CreatorToolConnectMgr.Instance.GetQuiz(i).choices2;
                nextOption3 = P_CreatorToolConnectMgr.Instance.GetQuiz(i).choices3;
                nextOption4 = P_CreatorToolConnectMgr.Instance.GetQuiz(i).choices4;
            }
        }

        quizText.text = prevText;
        quizOption1.text = prevOption1;
        quizOption2.text = prevOption2;
        quizOption3.text = prevOption3;
        quizOption4.text = prevOption4;
    }
}
