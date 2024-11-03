using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class Y_SignUp : MonoBehaviour
{
    public TMP_InputField[] inputs;
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
        string username = inputs[5].text;
        string password = inputs[6].text;
        string school = inputs[1].text;
        string grade = inputs[2].text;
        string className = inputs[3].text;
        string studentNumber = inputs[4].text;

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
    }

    //private string SignUpCoroutine(string username, string password, string school, int grade, int className, int studentNumber, bool isTeacher)
    //{
    //    SignUpData registerData = new SignUpData { username = username, password = password, isTeacher = isTeacher, school = school, grade = grade, className = className, studentNumber = studentNumber };

    //}
}
