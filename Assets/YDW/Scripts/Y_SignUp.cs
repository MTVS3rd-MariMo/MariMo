using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;



public class Y_SignUp : MonoBehaviour
{
    public TMP_InputField[] signUpInputs = new TMP_InputField[6];
    public TMP_InputField[] logInInputs = new TMP_InputField[2];
    public bool isTeacher = false;

    public GameObject signUpUI;
    public GameObject logInUI;
    public GameObject titleUI;
    public GameObject creatorUI;

    #region Button Sprite

    public GameObject[] teacherOrStudent;
    public Button[] buttons;
    public List<Sprite> sprites = new List<Sprite>();

    #endregion

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

    private void Update()
    {
        if (!isTeacher && (String.IsNullOrEmpty(signUpInputs[0].text) || String.IsNullOrEmpty(signUpInputs[1].text) 
            || String.IsNullOrEmpty(signUpInputs[2].text) || String.IsNullOrEmpty(signUpInputs[3].text) 
            || String.IsNullOrEmpty(signUpInputs[4].text) || String.IsNullOrEmpty(signUpInputs[5].text)))
        {
            buttons[2].GetComponent<Image>().sprite = sprites[4];
            buttons[2].interactable = false;
            return;
        }
        else if (isTeacher && (String.IsNullOrEmpty(signUpInputs[0].text) || String.IsNullOrEmpty(signUpInputs[6].text)
            || String.IsNullOrEmpty(signUpInputs[7].text) || String.IsNullOrEmpty(signUpInputs[4].text)
            || String.IsNullOrEmpty(signUpInputs[5].text)))
        {
            buttons[2].GetComponent<Image>().sprite = sprites[4];
            buttons[2].interactable = false;
            return;
        }
        else
        {
            buttons[2].GetComponent<Image>().sprite = sprites[5];
            buttons[2].interactable = true;
        }
    }

    public void ClickStudent()
    {
        isTeacher = false;
        buttons[0].GetComponent<Image>().sprite = sprites[1];
        buttons[1].GetComponent<Image>().sprite = sprites[2];
        teacherOrStudent[0].SetActive(true);
        teacherOrStudent[1].SetActive(false);
    }

    public void ClickTeacher()
    {
        isTeacher = true;
        buttons[0].GetComponent<Image>().sprite = sprites[0];
        buttons[1].GetComponent<Image>().sprite = sprites[3];
        teacherOrStudent[0].SetActive(false);
        teacherOrStudent[1].SetActive(true);
    }

    string username;
    string password;
    string school;

    string grade;
    string className;
    string studentNumber;

    public void ClickConfirm()
    {
        username = signUpInputs[4].text;
        password = signUpInputs[5].text;
        school = signUpInputs[0].text;

        grade = signUpInputs[1].text;
        className = signUpInputs[2].text;
        studentNumber = signUpInputs[3].text;

        if (isTeacher)
        {
            grade = signUpInputs[6].text;
            className = signUpInputs[7].text;
            studentNumber = "1";
        }

        StartCoroutine(Y_HttpLogIn.GetInstance().SignUpCoroutine(username, password, school, Int32.Parse(grade), Int32.Parse(className), Int32.Parse(studentNumber), isTeacher));

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
