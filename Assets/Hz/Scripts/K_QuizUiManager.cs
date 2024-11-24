using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class K_QuizUiManager : MonoBehaviour
{
    public Image img_direction;
    public Image img_countDown;

    public Image img_correctA;
    public Image img_wrongA;

    // 열쇠
    //public Image img_getKey;

    // 카운트다운
    public TMP_Text text_countDown;

    //float currTime = 0;
    //// 퀴즈 타이머 길이 (sec)
    //public float quizDuration = 15f;
    //// 현재 퀴즈가 시작 되었는지
    //public bool isPlaying = false;
    //// 디렉션 표시 프레임 1번만 처리하기
    //public bool isDirecting = false;
    //// 카운트 다운 플래그
    //public bool isCounting = false;

    public static K_QuizUiManager instance;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        // 카운트 다운 && 15초 안내 UI 셋팅
        img_countDown.gameObject.SetActive(false);
        img_direction.gameObject.SetActive(false);

        // 정답, 틀림
        img_correctA.gameObject.SetActive(false);
        img_wrongA.gameObject.SetActive(false);

        // 정답 시 열쇠 획득 ui 
        //img_getKey.gameObject.SetActive(false);
    }

    void Update()
    {
        //// 플레이어 4명 모두 오면 활성화 -> 카운트 다운 하기
        //if (isPlaying)
        //{
        //    CountDown();
        //}
    }

    //public void CountDown()
    //{
    //    if (!isDirecting)
    //    {
    //        img_direction.gameObject.SetActive(true);
    //        StartCoroutine(HideDirection(2f));
    //        isDirecting = true;
    //    }

    //    // 카운트 다운 이미지 보이기 + 카운트 다운은 img_direction 사라지고 1초 후 활성화
    //    if (isCounting)
    //    {
    //        img_countDown.gameObject.SetActive(true);

    //        // 시간이 흐름
    //        currTime += Time.deltaTime;
    //        // 흐른 시간을 countDown에 셋팅하자
    //        //int second = 15 - (int)currTime;
    //        int second = (int)(quizDuration - currTime);

    //        // 만약에 second가 0 보다 크다면
    //        if (second > 0)
    //        {
    //            // second 값을 보여주자
    //            text_countDown.text = second.ToString();
    //        }
    //        else if (second == 0)
    //        {
    //            // 시간초과(텍스트) 보여주자.
    //            text_countDown.text = "시간 종료!";
    //            // 정답 판별 함수 호출
    //            CheckAnswer();
    //        }
    //        // 그렇지 않으면
    //        else
    //        {
    //            img_countDown.gameObject.SetActive(false);
    //        }
    //    }
    //}

    //public void CheckAnswer()
    //{
    //    // quiz1 오브젝트의 K_QuizPos_1 스크립트 접근
    //    K_QuizPos_1 quizPosScript = FindObjectOfType<K_QuizPos_1>();
    //    K_QuizPos_2 quizPosScript2 = FindObjectOfType<K_QuizPos_2>();

    //    if (quizPosScript != null)
    //    {
    //        // 퀴즈1의 CheckAnswer로 정답 여부 확인
    //        quizPosScript.CheckAnswer();
    //    }

    //    if (quizPosScript2 != null)
    //    {
    //        // 퀴즈1의 CheckAnswer로 정답 여부 확인
    //        quizPosScript2.CheckAnswer();
    //    }

    //    else
    //    {
    //        print("quizPosScript를 찾지 못했습니다.");
    //    }

    //    // 퀴즈 종료 처리
    //    EndQuiz();
    //}

    //public void EndQuiz()
    //{
    //    print("퀴즈 종료 호출 체크");
    //    isPlaying = false;
    //    isCounting = false;
    //    currTime = 0;
    //    img_countDown.gameObject.SetActive(false);

    //    // 현재 활성화 상태 activeSelf - bool 값
    //    if(img_wrongA.gameObject.activeSelf)
    //    {
    //        StartCoroutine(RestartQuiz(5f));
    //    }
    //}

    //IEnumerator RestartQuiz(float delay)
    //{
    //    yield return new WaitForSeconds(delay);

    //    isPlaying = true;
    //    currTime = 0;
    //    isCounting = true;
    //    //isDirecting = false;

    //    img_correctA.gameObject.SetActive(false);
    //    img_wrongA.gameObject.SetActive(false);

    //    // 카운트다운 다시 시작
    //    img_countDown.gameObject.SetActive(true);
    //    //StartCoroutine(HideDirection(2f));
    //}


    //private IEnumerator HideDirection(float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    img_direction.gameObject.SetActive(false);

    //    yield return new WaitForSeconds(1f);
    //    //img_countDown.gameObject.SetActive(true);
    //    isCounting = true;
    //}
}
