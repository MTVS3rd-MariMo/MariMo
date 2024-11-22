using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_QuizSelect : MonoBehaviour
{
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
        RemoveAllChildren();

        for (int i = 0; i < P_CreatorToolConnectMgr.Instance.GetQuizCount(); i++)
        {
            GameObject newQuiz = Instantiate(quizPrefab, contentpanel);
            newQuiz.GetComponent<P_QuizInfo>().Setquiz(i);
        }
    }

    public void RemoveAllChildren()
    {
        // 자식 오브젝트들을 역순으로 순회하며 삭제
        for (int i = contentpanel.childCount - 1; i >= 0; i--)
        {
            Destroy(contentpanel.GetChild(i).gameObject);
        }
    }
}
