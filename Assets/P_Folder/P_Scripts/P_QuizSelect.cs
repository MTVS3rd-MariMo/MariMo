using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_QuizSelect : MonoBehaviour
{
    public P_CreatorToolController p_CreatorToolController;

    public Button btn_SelectComplete;

    public Transform contentpanel;

    public GameObject quizPrefab;

    public int selectCount = 0;

    void Start()
    {
        P_CreatorToolConnectMgr.Instance.OnDataParsed += QuizSetting;

        btn_SelectComplete.onClick.AddListener(OnclickSelectComplete);
    }

    void Update()
    {
        if (selectCount == 2)
        {
            btn_SelectComplete.interactable = true;
        }
        else
        {
            btn_SelectComplete.interactable = false;
        }
    }

    private void QuizSetting()
    {
        for (int i = 0; i < P_CreatorToolConnectMgr.Instance.GetQuizCount(); i++)
        {
            GameObject newQuiz = Instantiate(quizPrefab, contentpanel);
            newQuiz.GetComponent<P_QuizInfo>().Setquiz(i);
        }
    }


    // 선택한 퀴즈 데이터 전송
    void OnclickSelectComplete()
    {
        p_CreatorToolController.panel_MakingAsk.SetActive(true);

        gameObject.SetActive(false);
    }
}
