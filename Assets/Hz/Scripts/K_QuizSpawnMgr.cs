﻿using Org.BouncyCastle.Asn1.Crmf;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class K_QuizSpawnMgr : MonoBehaviourPun
{
    int quizCount = 2;
    //public GameObject quiz;

    // 퀴즈 2개 배열
    public GameObject[] quizzes;
    public string[] quizzes_Names = { "RealQuiz_1", "RealQuiz_2" };

    public Vector3[] quiz_spawnSize;
    public Vector3[] quiz_spawnCenter;

    public GameObject [] quiz_correct;
    public Vector3 quiz_correctASize;

    // 수업자료
    ClassMaterial classMaterial;

    PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();

        StartCoroutine(DelayStart(2f));
        //// classMaterial 받아오기
        //classMaterial = Y_HttpRoomSetUp.GetInstance().realClassMaterial;

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    quiz_correct = new GameObject[quizCount];

        //    for (int i = 0; i < quizCount; i++)
        //    {
        //        //SpawnObj(quizzes[i], i);

        //        StartCoroutine(SpawnObj(quizzes_Names[i], i));

        //    }
        //}
    }

    IEnumerator DelayStart(float delay)
    {
        yield return new WaitForSeconds(delay);

        // classMaterial 받아오기
        classMaterial = Y_HttpRoomSetUp.GetInstance().realClassMaterial;

        if (PhotonNetwork.IsMasterClient)
        {
            quiz_correct = new GameObject[quizCount];

            for (int i = 0; i < quizCount; i++)
            {
                //SpawnObj(quizzes[i], i);

                StartCoroutine(SpawnObj(quizzes_Names[i], i));

            }
        }
    }


    IEnumerator SpawnObj(string obj, int idx)
    {
        Vector3 center = quiz_spawnCenter[idx];
        Vector3 size = quiz_spawnSize[idx];
        Vector3 randomPos = GetRandomPosInArea(center, size);


        // Resources 폴더에서 quizName으로 프리팹을 로드하고 PhotonNetwork.Instantiate로 생성
        GameObject quizPrefab = Resources.Load<GameObject>(obj);

        

        if (quizPrefab != null)
        {
            GameObject quizInstance = PhotonNetwork.Instantiate(obj, randomPos, Quaternion.identity);

            yield return new WaitUntil(() => quizInstance != null);
           

            // 생성된 quizInstance에서 자식 오브젝트인 quiz_correct를 찾음
            K_QuizPos k_QuizPos = quizInstance.GetComponent<K_QuizPos>();
            if (k_QuizPos != null)
            {
                quiz_correct[idx] = k_QuizPos.correct;

                print("퀴즈 받을거임");
                /////////////////// 퀴즈데이터
                if(idx < classMaterial.quizzes.Count)
                {
                    print("퀴즈 받았니?");
                    Quiz quizData = classMaterial.quizzes[idx];
                    // Pv
                    PhotonView quizPv = quizInstance.GetComponent<PhotonView>();

                    print("널?");
                    if(quizPv != null)
                    {
                        print("널2??");
                        //UpdateQuizText(k_QuizPos, quizData);
                        pv.RPC(nameof(UpdateQuizText), RpcTarget.AllBuffered, idx, quizData.question,
                              quizData.choices1, quizData.choices2, quizData.choices3, quizData.choices4, quizData.answer);
                    }
                    
                }
            }
            else
            {
                print("프리팹 있음");
            }
        }
        else
        {
            print("프리팹 없음");
        }    
    }


    
    public int answerNumber = 0;
    [PunRPC]
    // 퀴즈1 텍스트 업뎃
    public void UpdateQuizText(int idx, string question, string choice1, string choice2, string choice3, string choice4, int answerIndex)
    {
        // 참조해
        //K_QuizPos quizPos, Quiz quiz
        //int idx, string question, string choice1, string choice2, string choice3, string choice4, int answerIndex
        K_QuizPos quizPos = quizzes[idx].GetComponent<K_QuizPos>();

        if (quizPos != null)
        {
            // 퀴즈 Question 텍스트 설정
            quizPos.text_Question.text = question;

            // 문제
            quizPos.text_Choices[0].text = choice1;
            quizPos.text_Choices[1].text = choice2;
            quizPos.text_Choices[2].text = choice3;
            quizPos.text_Choices[3].text = choice4;

            // 답 (서버에서 int로 줌)
            // 서버로부터 받은 정답 인덱스 기반으로 올바른 선택지의 텍스트를 가져와서 저장
            if (answerIndex >= 0 && answerIndex < quizPos.text_Choices.Length)
            {
                string correctAnswerText = quizPos.text_Choices[answerIndex -1].text;
                answerNumber = answerIndex - 1; // 정답 번호 저장 (정답 인덱스)

                Debug.Log($"정답은: {correctAnswerText} (인덱스: {answerIndex - 1})");
            }
            //int correctIndex = answer;

            //string correctAnswerText = quizPos.text_Choices[correctIndex].text;
            //answerNumber = correctIndex;


            // 퀴즈 Question 텍스트 설정
            //quizPos.text_Question.text = quiz.question;

            //// 문제
            //quizPos.text_Choices[0].text = quiz.choices1;
            //quizPos.text_Choices[1].text = quiz.choices2;
            //quizPos.text_Choices[2].text = quiz.choices3;
            //quizPos.text_Choices[3].text = quiz.choices4;

            //// 답 (서버에서 int로 줌)
            //int correctIndex = quiz.answer;

            //string correctAnswerText = quizPos.text_Choices[correctIndex].text;
            //answerNumber = correctIndex;

            Debug.Log("퀴즈 잘 들어감");
        }
        else
        {
            Debug.LogError("퀴즈업슴");
        }
    }


    private void SpawnCorrectA(int idx)
    {
        float x = Random.Range(0, 2) * quiz_correctASize.x;
        float y = Random.Range(0, 2) * quiz_correctASize.y;
        // 크기를 1로 생각하기 (scale 10 생각 x)
        Vector3 randomPos = new Vector3(x, y, -0.01f) - (quiz_correctASize * 0.5f);

        quiz_correct[idx].transform.localPosition = randomPos;
    }

    private Vector3 GetRandomPosInArea(Vector3 center, Vector3 size)
    {
        Vector3 randomPos = new Vector3(
            Random.Range(-size.x / 2, size.x / 2),
            0,
            Random.Range(-size.z / 2, size.z / 2)
        );

        return center + randomPos;
    }
}
