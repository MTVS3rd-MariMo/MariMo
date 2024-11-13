using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class K_MapQuizSetUp : MonoBehaviour
{
    // 서버에서 받은 퀴즈 저장 데이터 리스트
    public List<Quiz> quizzes;
    // 수업자료
    ClassMaterial classMaterial;

    //// 퀴즈 문제, 선지, 정답
    //public TMP_Text Question;
    //public TMP_Text[] Choices;
    //public TMP_Text answer;

    // 퀴즈 안에 있는 스크립트
    K_QuizPos k_QuizPos;

    // 퀴즈 오브젝트 pv 
    private PhotonView quiz1pv;   
    private PhotonView quiz2pv;

    private K_QuizPos quiz1Pos;
    private K_QuizPos quiz2Pos;


    //public bool isFive;

    public static K_MapQuizSetUp Instance {get; private set;}

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // classMaterial 받아오기
        classMaterial = Y_HttpRoomSetUp.GetInstance().realClassMaterial;
        quizzes = classMaterial.quizzes;

        //if(quiz1Pos != null && quiz2Pos != null)
        //{
        //    ReSetQuizzes();
        //}
    }


    public void SetQuizObjects(GameObject quiz1, GameObject quiz2)
    {
        print("호출댐? ");

        if(quiz1 != null)
        {
            quiz1pv = quiz1.GetComponent<PhotonView>();
            quiz1Pos = quiz1.GetComponent<K_QuizPos>();
        }

        if(quiz2 !=null)
        {
            quiz2pv = quiz2.GetComponent<PhotonView>();
            quiz2Pos = quiz2.GetComponent<K_QuizPos>();
        }

        if(quiz1Pos != null && quiz2Pos != null)
        {
            ReSetQuizzes();
        }
    }

    // 퀴즈 리셋후 조정
    private void ReSetQuizzes()
    {
        print("호출댐2? ");

        if (classMaterial != null && classMaterial.quizzes.Count >= 2)
        {
            Quiz firstQuiz = classMaterial.quizzes[0];
            UpdateQuizText(quiz1Pos, firstQuiz);

            Quiz secondQuiz = classMaterial.quizzes[1];
            UpdateQuizText(quiz2Pos, secondQuiz);

        }
        else
        {
            Debug.Log("수업자료 비었음");
        }
    }

    // 퀴즈1 텍스트 업뎃
    public void UpdateQuizText(K_QuizPos quizPos, Quiz quiz)
    {
        if (quizPos != null)
        {
            // 퀴즈 Question 텍스트 설정
            quizPos.text_Question.text = quiz.question;

            // 문제
            quizPos.text_Choices[0].text = quiz.choices1;
            quizPos.text_Choices[1].text = quiz.choices2;
            quizPos.text_Choices[2].text = quiz.choices3;
            quizPos.text_Choices[3].text = quiz.choices4;

            // 답
            int correctIndex = quiz.answer;
            quizPos.text_Choices[3].text = correctIndex.ToString();

            Debug.Log("퀴즈 잘 들어감");
        }
        else
        {
            Debug.LogError("퀴즈업슴");
        }
    }
}
