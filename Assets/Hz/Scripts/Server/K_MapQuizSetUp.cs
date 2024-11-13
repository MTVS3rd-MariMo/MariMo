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
    // 퀴즈 문제, 선지, 정답
    public TMP_Text Question;
    public TMP_Text[] Choices;
    public TMP_Text answer;

    // 퀴즈 안에 있는 스크립트
    K_QuizPos k_QuizPos;

    // 퀴즈 오브젝트 pv 
    private PhotonView quiz1pv;
    
    private PhotonView quiz2pv;

    private K_QuizPos quiz1Pos;
    private K_QuizPos quiz2Pos;

    // 퀴즈 스폰 받아오기
    private K_QuizSpawnMgr spawnMgr;

    public static K_MapQuizSetUp Instance {get; private set;}

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // SpawnMgr 접근
        spawnMgr = GameObject.FindObjectOfType<K_QuizSpawnMgr>();

        // 퀴즈
        quizzes  = Y_HttpRoomSetUp.GetInstance().realClassMaterial.quizzes;

        // classMaterial 받아오기
        classMaterial = Y_HttpRoomSetUp.GetInstance().realClassMaterial;
    }

    public void SetQuizObjects(GameObject quiz1, GameObject quiz2)
    {
        // RealQuiz1, RealQuiz2 찾아서 K_QuizPos에 접근
        quiz1pv = GameObject.Find("RealQuiz_1").GetComponent<PhotonView>();
        quiz2pv = GameObject.Find("RealQuiz_2").GetComponent<PhotonView>();

        quiz1Pos = GameObject.Find("RealQuiz_1").GetComponent<K_QuizPos>();
        quiz2Pos = GameObject.Find("RealQuiz_2").GetComponent<K_QuizPos>();
    }

    // private void ReSetQuizzes()
    // {
    //     if(classMaterial != null && classMaterial.quizzes.Count >= 2)
    //     {
    //         Quiz firstQuiz = classMaterial.quizzes[0];
    //         UpdateQuizText(quiz1Pos, firstQuiz.question, firstQuiz.choices1, firstQuiz.choices2, firstQuiz.choices3, firstQuiz.choices4, firstQuiz.answer);

    //         Quiz secondQuiz = classMaterial.quizzes[1];
    //         UpdateQuizText(quiz2Pos, secondQuiz.question, secondQuiz.choices1, secondQuiz.choices2, secondQuiz.choices3, secondQuiz.choices4, secondQuiz.answer);
        
    //     }
    // }
    
    // // 퀴즈1 텍스트 업뎃
    // public void UpdateQuizText(K_QuizPos quizPos, string question, string choice1, string choice2, string choice3, string choice4, int answer)
    // {
    //     if (quiz1Pos != null)
    //     {
    //         // 퀴즈 Question 텍스트 설정
    //         quiz1Pos.Question.text = question;

    //         // 문제
    //         quiz1Pos.choices[0].text = Choices[0].text;
    //         quiz1Pos.choices[1].text = Choices[1].text;
    //         quiz1Pos.choices[2].text = Choices[2].text;
    //         quiz1Pos.choices[3].text = Choices[3].text;

    //         // 답
    //         quiz1Pos.choices[3].text = answer.ToString();    

    //         Debug.Log("잘 들어감");    
    //     }
    //     else
    //     {
    //         Debug.LogError("퀴즈포즈업슴");
    //     }
    // }



    // // 퀴즈2 텍스트 업뎃
    // public void UpdateQuiz2Text(K_QuizPos quizPos, string question, string choice1, string choice2, string choice3, string choice4, int answer)
    // {
    //     quiz2Pos = GameObject.Find("RealQuiz2").GetComponent<K_QuizPos>();

    //     if(quiz2Pos != null)
    //     {
    //         // 퀴즈 Question 텍스트 설정
    //         quiz2Pos.Question.text = question;

    //         quiz2Pos.Question.text = question;
    //         quiz2Pos.choices[0] = Choices[0];
    //         quiz2Pos.choices[1] = Choices[1];
    //         quiz2Pos.choices[2] = Choices[2];
    //         quiz2Pos.choices[3] = Choices[3];

    //         // 답
    //         quiz2Pos.choices[3].text = answer.ToString();
    //     }
    //     else
    //     {
    //         Debug.LogError("퀴즈포즈업슴");
    //     }

    // }
}
