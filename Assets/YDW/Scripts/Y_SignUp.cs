using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class Y_SignUp : MonoBehaviour
{
    public TMP_InputField[] signUpInputs = new TMP_InputField[6];
    public TMP_InputField[] logInInputs = new TMP_InputField[2];
    public bool isTeacher = false;

    public GameObject signUpUI;
    public GameObject logInUI;
    public GameObject titleUI;

    public static Y_SignUp signUp;

    private void Awake()
    {
        if (signUp == null)
        {
            signUp = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ClickStudent()
    {
        isTeacher = false;
    }

    public void ClickTeacher()
    {
        isTeacher = true;
    }

    public void ClickConfirm()
    {
        string username = signUpInputs[4].text;
        string password = signUpInputs[5].text;
        string school = signUpInputs[0].text;
        string grade = signUpInputs[1].text;
        string className = signUpInputs[2].text;
        string studentNumber = signUpInputs[3].text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("입력 안 한 거 있음!!");
            // UI에 에러 메시지 표시
            return;
        }
        else
        {
            StartCoroutine(Y_HttpLogIn.GetInstance().SignUpCoroutine(username, password, school, Int32.Parse(grade), Int32.Parse(className), Int32.Parse(studentNumber), isTeacher));
        }

        signUpUI.SetActive(false);
        logInUI.SetActive(true);
    }

    public void ClickBack()
    {
        signUpUI.SetActive(false);
        logInUI.SetActive(true);
    }

    public void ClickLogIn()
    {
        string username = logInInputs[0].text;
        string password = logInInputs[1].text;
        StartCoroutine(Y_HttpLogIn.GetInstance().LogInCoroutine(username, password));
    }

    public void ClickJoin()
    {
        logInUI.SetActive(false);
        signUpUI.SetActive(true);
    }

}
