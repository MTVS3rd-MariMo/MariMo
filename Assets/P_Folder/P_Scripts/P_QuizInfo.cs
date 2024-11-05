using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class P_QuizInfo : MonoBehaviour
{
    private P_QuizSelect p_QuizSelect;

    public TMP_Text quiz;
    public TMP_Text choice1;
    public TMP_Text choice2;
    public TMP_Text choice3;
    public TMP_Text choice4;
    public Toggle select;

    public int quizNum;

    public void Setquiz(int i)
    {
        Quiz quizdata = P_CreatorToolConnectMgr.Instance.GetQuiz(i);
        quiz.text = quizdata.question;
        choice1.text = quizdata.choices1;
        choice2.text = quizdata.choices2;
        choice3.text = quizdata.choices3;
        choice4.text = quizdata.choices4;

        quizNum = quizdata.quizId;
    }

    

    void Start()
    {
        p_QuizSelect = GetComponentInParent<P_QuizSelect>();

        select.onValueChanged.AddListener(CountCheck);
    }

    void Update()
    {
        
    }

    void CountCheck(bool check)
    {
        if (check)
        {
            p_QuizSelect.selectCount++;

            P_CreatorToolConnectMgr.Instance.selectedID.Add(quizNum);

            if (p_QuizSelect.selectCount > 2)
            {
                select.isOn = false;
            }
        }
        else
        {
            p_QuizSelect.selectCount--;
            P_CreatorToolConnectMgr.Instance.selectedID.Remove(quizNum);
        }
    }
}
