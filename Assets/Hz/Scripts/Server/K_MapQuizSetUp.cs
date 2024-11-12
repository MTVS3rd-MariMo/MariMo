using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class K_MapQuizSetUp : MonoBehaviour
{
    ClassMaterial classMaterial;
    public TMP_Text Question;
    public TMP_Text[] Choices;
    public TMP_Text answer;

    K_QuizPos k_QuizPos;

    // 퀴즈 오브젝트 pv 
    private PhotonView quiz1pv;
    private PhotonView quiz2pv;

    public Transform spawnPosition;


    void Start()
    {

        // RealQuiz1, RealQuiz2 찾아서 K_QuizPos에 접근
        //quiz1pv = GameObject.Find("RealQuiz_1").GetComponent<PhotonView>();
        //quiz2pv = GameObject.Find("RealQuiz_2").GetComponent<PhotonView>();

        classMaterial = Y_HttpRoomSetUp.GetInstance().realClassMaterial;

        //StartCoroutine(SpawnQuiz());


    }

    //IEnumerator SpawnQuiz()
    //{
    //    GameObject player = PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);
    //}
    
    public void ApplyQuizMaterial()
    {
        classMaterial = Y_HttpRoomSetUp.GetInstance().realClassMaterial;
    }

    public void UpdateQuiz1Text(string question, string[] choices)
    {
        k_QuizPos = GameObject.Find("RealQuiz1").GetComponent<K_QuizPos>();

        if(k_QuizPos != null)
        {
            // 퀴즈 Question 텍스트 설정
            k_QuizPos.Question.text = question;
        }
    }
}
