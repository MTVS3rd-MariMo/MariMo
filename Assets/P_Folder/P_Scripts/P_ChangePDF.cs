using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class P_ChangePDF : MonoBehaviour
{
    public TMP_InputField text_pdf;
    public Button btn_FinishChecking;
    public Button btn_GoBack;

    private void Start()
    {
        btn_FinishChecking.onClick.AddListener(FinishChecking);
        btn_GoBack.onClick.AddListener(GoBack);
    }

    public void OnStart()
    {
        text_pdf.text = P_CreatorToolConnectMgr.Instance.quizData.bookContents;
    }

    public void GoBack()
    {
        gameObject.SetActive(false);
    }

    public void FinishChecking()
    {
        P_CreatorToolConnectMgr.Instance.quizData.bookContents = text_pdf.text;

        gameObject.SetActive(false);
    }
}
