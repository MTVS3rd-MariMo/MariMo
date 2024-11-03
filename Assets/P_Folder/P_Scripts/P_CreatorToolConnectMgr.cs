using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static P_CreatorToolConnectMgr;


// 데이터를 받아올 구조체
[Serializable]
public class QuizData
{
    public List<Quiz> quizzes;
    public List<OpenQuestion> openQuestions;
}

[Serializable]
public class Quiz
{
    public string question;
    public string answer;
    public string firstChoice;
    public string secondChoice;
    public string thirdChoice;
    public string fourthChoice;
}

[Serializable]
public class OpenQuestion
{
    public string questionTitle;
}


public class P_CreatorToolConnectMgr : MonoBehaviour
{
    private static P_CreatorToolConnectMgr instance;
    public static P_CreatorToolConnectMgr Instance
    {
        get
        {
            if (instance == null)
            {
                // 씬에서 QuizManager 찾기
                instance = FindObjectOfType<P_CreatorToolConnectMgr>();

                // 씬에 없다면 새로 생성
                if (instance == null)
                {
                    GameObject go = new GameObject("P_CreatorToolConnectMgr");
                    instance = go.AddComponent<P_CreatorToolConnectMgr>();
                }
            }
            return instance;
        }
    }

    private QuizData quizData;
    public string pdfPath = null;

    // 데이터 파싱 이벤트 구독 델리게이트
    public delegate void OnDataParsedDelegate();
    public event OnDataParsedDelegate OnDataParsed;

    // 파싱 성공/실패 확인
    public bool IsDataLoaded { get; private set; }

    // 데이터 로드 시간
    public DateTime LastLoadTime { get; private set; }  

    void Awake()
    {
        // 싱글톤 설정
        if (instance != null && instance != this)
        {
            // 이미 인스턴스가 있다면 이 객체 제거
            Destroy(gameObject);
            return;
        }

        // 첫 인스턴스라면 설정
        instance = this;
        // 씬 전환시에도 유지
        DontDestroyOnLoad(gameObject);

        InitializeConnectMgr();
    }

    void InitializeConnectMgr()
    {
        // 초기 데이터 로드 등 초기화 작업
        quizData = new QuizData
        {
            quizzes = new List<Quiz>(),
            openQuestions = new List<OpenQuestion>()
        };
    }

    // json 문자열 파싱
    public void ParseQuizData(string jsonString)
    {
        try
        {
            quizData = JsonUtility.FromJson<QuizData>(jsonString);
            IsDataLoaded = true;
            LastLoadTime = DateTime.Now;

            // 데이터 검증
            ValidateData();

            // 파싱 완료 이벤트 발생
            OnDataParsed?.Invoke();

            // 디버그용 데이터 출력
            DisplayQuizData();
        }
        catch (Exception e)
        {
            IsDataLoaded = false;
            Debug.LogError($"Quiz 데이터 파싱 실패: {e.Message}");
            throw;
        }
    }

    // 특정 퀴즈 받아오는 함수
    public Quiz GetQuiz(int index)
    {
        if (quizData.quizzes != null && index < quizData.quizzes.Count)
        {
            return quizData.quizzes[index];
        }
        return null;
    }

    // 전체 퀴즈 개수 가져오기
    public int GetQuizCount()
    {
        return quizData.quizzes?.Count ?? 0;
    }

    // 특정 열린질문 가져오는 함수
    public OpenQuestion GetOpenQuestion(int index)
    {
        if (quizData.openQuestions != null && index < quizData.openQuestions.Count)
        {
            return quizData.openQuestions[index];
        }
        return null;
    }

    // 전체 주관식 문제 개수 가져오기
    public int GetOpenQuestionCount()
    {
        return quizData.openQuestions?.Count ?? 0;
    }

    // json 파일 데이터 로드
    public void LoadFromFile(TextAsset jsonFile)
    {
        if (jsonFile != null)
        {
            quizData = JsonUtility.FromJson<QuizData>(jsonFile.text);
        }
    }


    // 데이터 확인용
    private void DisplayQuizData()
    {
        Debug.Log("=== 퀴즈 데이터 ===");
        if (quizData.quizzes != null)
        {
            for (int i = 0; i < quizData.quizzes.Count; i++)
            {
                Quiz quiz = quizData.quizzes[i];
                Debug.Log($"\n퀴즈 {i + 1}");
                Debug.Log($"문제: {quiz.question}");
                Debug.Log($"정답: {quiz.answer}");
                Debug.Log("보기:");
                Debug.Log($"1) {quiz.firstChoice}");
                Debug.Log($"2) {quiz.secondChoice}");
                Debug.Log($"3) {quiz.thirdChoice}");
                Debug.Log($"4) {quiz.fourthChoice}");
            }
        }

        Debug.Log("\n=== 주관식 문제 ===");
        if (quizData.openQuestions != null)
        {
            for (int i = 0; i < quizData.openQuestions.Count; i++)
            {
                Debug.Log($"{i + 1}. {quizData.openQuestions[i].questionTitle}");
            }
        }
    }



    // 데이터 유효성 검사
    private void ValidateData()
    {
        if (quizData.quizzes == null)
        {
            quizData.quizzes = new List<Quiz>();
        }

        if (quizData.openQuestions == null)
        {
            quizData.openQuestions = new List<OpenQuestion>();
        }
    }

    // 데이터가 성공적으로 로드되었는지 확인
    public bool ValidateLoadedData()
    {
        return IsDataLoaded &&
               quizData != null &&
               quizData.quizzes != null &&
               quizData.openQuestions != null;
    }

    // 데이터 초기화
    public void ClearData()
    {
        quizData = new QuizData
        {
            quizzes = new List<Quiz>(),
            openQuestions = new List<OpenQuestion>()
        };
        IsDataLoaded = false;
    }
}
