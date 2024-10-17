using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class P_MakingAsk : MonoBehaviour
{
    public Button btn_Prev;
    public Button btn_Next;

    public TMP_InputField quizText;
    public TMP_Text quizNum;

    public int count = 0;

    // 예시
    public string[] Datas = { "a", "1a", "2a", "3a", "4a", "b", "1b", "2b", "3b", "4b", "c", "1c", "2c", "3c", "4c", "question1", "question2", "question3" };

    void Start()
    {
        btn_Prev.onClick.AddListener(OnclickPrev);
        btn_Next.onClick.AddListener(OnclickNext);

        // 예시
        SetQuizNum(count);
        quizText.text = Datas[0] + "\n" + Datas[1] + "\n" + Datas[2] + "\n" + Datas[3] + "\n" + Datas[4];
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

            // 예시
            quizText.text = Datas[count*5] + "\n" + Datas[count * 5 + 1] + "\n" + Datas[count * 5 + 2] + "\n" + Datas[count * 5 + 3] + "\n" + Datas[count * 5 + 4] + "\n";
        }

        SetQuizNum(count);
    }

    public void OnclickNext()
    {
        if (count >= 2)
        {
            // 다음 창으로 이동
        }    
        else
        {
            count++;

            // 예시
            quizText.text = Datas[count * 5] + "\n" + Datas[count * 5 + 1] + "\n" + Datas[count * 5 + 2] + "\n" + Datas[count * 5 + 3] + "\n" + Datas[count * 5 + 4] + "\n";
        }

        SetQuizNum(count);
    }

    public void SetQuizNum(int count)
    {
        int num = count + 1;
        quizNum.text = ("Create Quiz " +  num).ToString();
    }
}
