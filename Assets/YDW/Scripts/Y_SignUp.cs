﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class Y_SignUp : MonoBehaviour
{
    public TMP_InputField[] signUpInputs = new TMP_InputField[6];
    public TMP_InputField[] logInInputs = new TMP_InputField[2];
    public bool isTeacher = false;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
            StartCoroutine(HttpManager.GetInstance().SignUpCoroutine(username, password, school, Int32.Parse(grade), Int32.Parse(className), Int32.Parse(studentNumber), isTeacher));
        }

        LobbyUIController.lobbyUI.signUpUI.SetActive(false);
        LobbyUIController.lobbyUI.logInUI.SetActive(true);
    }

    public void ClickBack()
    {
        LobbyUIController.lobbyUI.signUpUI.SetActive(false);
        LobbyUIController.lobbyUI.logInUI.SetActive(true);
    }

    public void ClickLogIn()
    {
        string username = logInInputs[0].text;
        string password = logInInputs[1].text;
        StartCoroutine(HttpManager.GetInstance().LogInCoroutine(username, password));
    }

}
