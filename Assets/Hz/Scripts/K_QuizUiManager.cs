using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class K_QuizUiManager : MonoBehaviour
{
    public Image img_direction;
    public Image img_countDown;

    public TMP_Text text_countDown;
    float currTime = 0;

    // 현재 퀴즈가 시작 되었는지
    public bool isPlaying = false;
    // 디렉션 표시 프레임 1번만 처리하기
    public bool isDirecting = false;
    // 카운트 다운 플래그
    public bool isCounting = false;

    public static K_QuizUiManager instance;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        img_countDown.gameObject.SetActive(false);
        img_direction.gameObject.SetActive(false);
    }

    void Update()
    {
        // 플레이어 4명 모두 오면 활성화 -> 카운트 다운 하기
        if(isPlaying)
        {
            CountDown();
        }
        
    }

    public void CountDown()
    {
        //img_countDown.gameObject.SetActive(true);

        if(!isDirecting)
        {
            img_direction.gameObject.SetActive(true);

            StartCoroutine(HideDirection(2f));
            isDirecting = true;
        }
        
        // 카운트 다운 이미지 보이기 + 카운트 다운은 img_direction 사라지고 1초 후 활성화
        if(isCounting)
        {
            img_countDown.gameObject.SetActive(true);

            // 시간이 흐름
            //currTime -= Time.deltaTime;
            currTime += Time.deltaTime;
            // 흐른 시간을 countDown에 셋팅하자
            int second = 15 - (int)currTime;
            //int second = (int)(16 - currTime);

            // 만약에 second가 0 보다 크다면
            if (second > 0)
            {
                // second 값을 보여주자
                text_countDown.text = second.ToString();
            }
            else if (second == 0)
            {
                // 시간초과(텍스트) 보여주자.
                text_countDown.text = "시간 종료!";
            }
            // 그렇지 않으면
            else
            {
                img_countDown.gameObject.SetActive(false);
            }
        }
        
    }


    private IEnumerator HideDirection(float delay)
    {
        yield return new WaitForSeconds(delay);
        img_direction.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);
        //img_countDown.gameObject.SetActive(true);
        isCounting = true;
    }


}
