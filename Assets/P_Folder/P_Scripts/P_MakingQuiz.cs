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

    public int count = 0;

    public string[] Datas = { "a", "1a", "2a", "3a", "4a", "b", "1b", "2b", "3b", "4b", "c", "1c", "2c", "3c", "4c"};

    void Start()
    {
        btn_Prev.onClick.AddListener(OnclickPrev);
        btn_Next.onClick.AddListener(OnclickNext);

        quizText.text = Datas[count * 5];
        quizOption1.text = Datas[count * 5 + 1];
        quizOption2.text = Datas[count * 5 + 2];
        quizOption3.text = Datas[count * 5 + 3];
        quizOption4.text = Datas[count * 5 + 4];
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
            quizText.text = Datas[count * 5];
            quizOption1.text = Datas[count * 5 + 1];
            quizOption2.text = Datas[count * 5 + 2];
            quizOption3.text = Datas[count * 5 + 3];
            quizOption4.text = Datas[count * 5 + 4];
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

            // 예시
            quizText.text = Datas[count * 5];
            quizOption1.text = Datas[count * 5 + 1];
            quizOption2.text = Datas[count * 5 + 2];
            quizOption3.text = Datas[count * 5 + 3];
            quizOption4.text = Datas[count * 5 + 4];
        }
    }

    
}
