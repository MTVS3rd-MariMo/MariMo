using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_QuizSelect : MonoBehaviour
{
    public P_CreatorToolController p_CreatorToolController;

    public Button btn_SelectComplete;
    public Sprite[] sp_SelectComplete;

    public Transform contentpanel;

    public GameObject quizPrefab;

    public int selectCount = 0;

    void Start()
    {
        P_CreatorToolConnectMgr.Instance.OnDataParsed += QuizSetting;

    }

    void Update()
    {
        if (selectCount == 2)
        {
            btn_SelectComplete.interactable = true;
            btn_SelectComplete.image.sprite = sp_SelectComplete[1];
        }
        else
        {
            btn_SelectComplete.interactable = false;
            btn_SelectComplete.image.sprite = sp_SelectComplete[0];
        }
    }

    public void QuizSetting()
    {
        //foreach (Transform child in contentpanel)
        //{
        //    Destroy(child.gameObject);
        //}

        for (int i = 0; i < P_CreatorToolConnectMgr.Instance.GetQuizCount(); i++)
        {
            GameObject newQuiz = Instantiate(quizPrefab, contentpanel);
            newQuiz.GetComponent<P_QuizInfo>().Setquiz(i);
        }
    }

}
