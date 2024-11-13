using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class K_SetQuiz : MonoBehaviourPun

{
    public TMP_Text text_Question;
    public TMP_Text[] text_Choices;
    public TMP_Text text_Answer;

    ClassMaterial classMaterial;

    void Start()
    {
        // classMaterial 받아오기
        //classMaterial = Y_HttpRoomSetUp.GetInstance().realClassMaterial;
    }

    [PunRPC]
    public void InitializeQuiz(string question, string choice1, string choice2, string choice3, string choice4, int answer)
    {
        if(classMaterial != null)
        {
            text_Question.text = question;

            text_Choices[0].text = choice1;
            text_Choices[1].text = choice2;
            text_Choices[2].text = choice3;
            text_Choices[3].text = choice4;

            text_Answer.text = answer.ToString();
            
        }
    }
}
