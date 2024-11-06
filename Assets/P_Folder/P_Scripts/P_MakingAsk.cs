using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class P_MakingAsk : MonoBehaviour
{
    private P_CreatorToolController _creatorToolController;

    public Button btn_Prev;
    public Button btn_Next;
    public Button btn_OK;

    public TMP_InputField askText;

    public int count = 0;


    void Start()
    {
        _creatorToolController = GetComponentInParent<P_CreatorToolController>();
        btn_Prev.onClick.AddListener(OnclickPrev);
        btn_Next.onClick.AddListener(OnclickNext);
        btn_OK.onClick.AddListener(OnclickNext);

        // 예시
        askText.text = P_CreatorToolConnectMgr.Instance.GetOpenQuestion(0).questionTitle;
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
            P_CreatorToolConnectMgr.Instance.ModifyQuestion(askText.text, count);
            
            count--;


            // 예시
            askText.text = P_CreatorToolConnectMgr.Instance.GetOpenQuestion(0).questionTitle;
        }

    }

    public void OnclickNext()
    {
        if (count >= 1)
        {
            // 데이터 저장
            P_CreatorToolConnectMgr.Instance.ModifyQuestion(askText.text, count);

            // 다음 창으로 이동
            GoTitle();
        }    
        else
        {
            P_CreatorToolConnectMgr.Instance.ModifyQuestion(askText.text, count);

            count++;

            // 예시
            askText.text = P_CreatorToolConnectMgr.Instance.GetOpenQuestion(1).questionTitle;
        }
    }


    public void GoTitle()
    {
        // 데이터 송신
        QuizData data = P_CreatorToolConnectMgr.Instance.GetAllData();

        _creatorToolController.OnFinishMaking(data);

        // 초기 화면으로
        _creatorToolController.OnclickSelectStory();
    }
}
