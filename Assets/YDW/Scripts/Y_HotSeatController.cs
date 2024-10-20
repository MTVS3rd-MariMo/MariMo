using Cinemachine.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Rendering.LookDev;
using UnityEngine.UI;

public class Y_HotSeatController : MonoBehaviour
{
    public GameObject guide;
    public GameObject selfIntroduce;
    public GameObject stage;

    // selfIntroduce;
    public TMP_InputField selfIntroduceInput;
    public RectTransform inputFieldRect;
    public Vector2 expandedSize = new Vector2(1200, 200); // 확장된 크기
    public Vector2 expandedPos = new Vector2(-345, 180); // 확장됐을 때 위치
    private Vector2 originalSize;
    private Vector2 originalPosition;
    private TouchScreenKeyboard keyboard;

    // stage
    public List<Image> images = new List<Image>();
    public List<GameObject> players = new List<GameObject>();
    public GameObject stageImg;
    public GameObject speechGuide;
    Color originalColor;
    int testNum = 0;
    public Vector2 playerPos;
    Vector2 stagePos;

    void Start()
    {
        originalSize = inputFieldRect.sizeDelta;
        originalPosition = inputFieldRect.gameObject.transform.localPosition;
        StartCoroutine(Deactivate(guide));

        originalColor = images[0].color;
        stagePos = new Vector2(stageImg.transform.position.x, stageImg.transform.position.y);
        playerPos = players[0].transform.position;
    }

    public IEnumerator Deactivate(GameObject gm)
    {
        yield return new WaitForSeconds(2);
        gm.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1) && testNum < players.Count)
        {
            if(testNum < players.Count)
            {
                testNum++;
                StartSpeech(testNum);
            }   
        }
    }

    #region SelfIntroduce

    // InputField가 선택되었을 때 호출
    public void OnSelect(BaseEventData eventData)
    {
        inputFieldRect.sizeDelta = expandedSize;
        inputFieldRect.gameObject.transform.localPosition = expandedPos;

        // 터치 키보드 호출 (모바일에서만 동작)
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    // InputField가 선택 해제되었을 때 호출
    public void OnDeselect(BaseEventData eventData)
    {
        inputFieldRect.sizeDelta = originalSize;
        inputFieldRect.gameObject.transform.localPosition = originalPosition;

        // 터치 키보드 닫기
        if (keyboard != null && keyboard.active)
        {
            keyboard.active = false;
        }
    }

    public void Submit()
    {
        selfIntroduce.SetActive(false);
        stage.SetActive(true);
        StartSpeech(0);
    }

    #endregion

    #region Stage

    public GameObject spotlight;
    public GameObject stageScript;

    public void StartSpeech(int i)
    {
        // 전 플레이어는 이름표 색 원래 색으로, 위치도 원위치
        if (i - 1 >= 0)
        {
            images[i - 1].color = originalColor;
            players[i - 1].transform.position = playerPos;
        }

        if(i < players.Count)
        {
            // 이름 UI 색깔 바꾸고
            images[i].color = Color.red;

            playerPos = players[i].transform.position;
            StartCoroutine(ChangePos(playerPos, i));
        }
        
        
        // "자기소개를 듣고 궁금했던 것들을 질문해봅시다" UI
        // 1분 뒤 질문받기
        // 순서대로 보이스 활성화
        // 4명 다 끝내면 "참 잘했어요!" UI
    }

    public IEnumerator ChangePos(Vector2 playerPos, int i)
    {
        while(true)
        {
            // 플레이어가 무대로 가게 한다
            players[i].transform.position = Vector3.Lerp(playerPos, stagePos, 0.05f);
            playerPos = players[i].transform.position;
            if (Vector3.Distance(playerPos, stagePos) < 0.1f) // 무대까지 거의 다 오면
            {
                playerPos = stagePos; // 도착점에 위치 맞춰준다

                spotlight.SetActive(true); // 스포트라이트

                ////////////////////// 나중에 보이스 연동
                
                // 밑에 자기소개
                stageScript.SetActive(true); 
                stageScript.GetComponentInChildren<TMP_Text>().text = selfIntroduceInput.text;

                // "친구들에게 말로 자기소개를 해 봅시다" UI
                speechGuide.SetActive(true);
                StartCoroutine(Deactivate(speechGuide));

                break;
            }
            yield return null;
        }
    }


    #endregion
}
