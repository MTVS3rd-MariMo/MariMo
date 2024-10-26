﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_KeyManager : MonoBehaviour
{
    // 열린 질문 활동 끝났는가
    public bool isDoneOpenQnA = false;
    // 퀴즈1 끝났는가 
    public bool isDoneQuiz_1 = false;
    // 퀴즈2 끝났는가
    public bool isDoneQuiz_2 = false;
    // 핫시팅 끝났는가 
    public bool isDoneHotSitting = false;

    // 획득한 총 열쇠 개수
    public int totalKeys = 0;

    // 벽 오브젝트
    public GameObject barrier;
    private bool isBarrierOpened = false;

    // Singleton
    public static K_KeyManager instance;


    // 사용시
    // Hot Sitting 활동 완료 시 KeyManager의 bool 값을 true로 설정
    //K_KeyManager.Instance.isDoneHotSitting = true;

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

    void Update()
    {
        if (isDoneOpenQnA)
        {
            GetKey();
            isDoneOpenQnA = false;
        }

        if (isDoneQuiz_1)
        {
            GetKey();
            isDoneQuiz_1 = false;
        }

        if (isDoneHotSitting)
        {
            GetKey();
            isDoneHotSitting = false;
        }

        if (isDoneQuiz_2)
        {
            GetKey();
            isDoneQuiz_2 = false;
        }

        // 총 열쇠 4개 -> 투명벽 열림
        if (totalKeys >= 4 && !isBarrierOpened)
        {
            //// 스탬프 다모이면 큰 열쇠 이미지 띄워주는 함수
            //K_KeyUiManager.instance.EndKeyUi();
            //// 투명벽 열려
            //OpenBarrier();

            // 여기도 코루틴써야하나 (왕 열쇠 띄워주는 함수..?)
            StartCoroutine(UnlockBarrierAfterKeyUI());
        }
    }

    void GetKey(int keyAmount = 1)
    {
        // 키 먹어
        totalKeys += keyAmount;

        // 열쇠 획득 안내 ui (활동 끝날때마다 띄워줘야함 - 2초 활성화 후 꺼지고 - 열쇠 아이콘 띄워주기)
        if (totalKeys < 4)
        {
            // 일단 없애고 코루틴으로 변경해보기
            //K_KeyUiManager.instance.img_getKeyDir.gameObject.SetActive(true);
            //K_KeyUiManager.instance.StartCoroutine(K_KeyUiManager.instance.HideGetKeyDir(2f));

            StartCoroutine(DisplayGetKeyUI());
        }


        // 열쇠 아이콘 업데이트 함수
        K_KeyUiManager.instance.UpdateKeyUI(totalKeys);
        print("열쇠 획득! 현재 열쇠 갯수 : " + totalKeys);
        
    }

    // 키 안내창 -> 키 아이콘 생성 딜레이 함수
    private IEnumerator DisplayGetKeyUI()
    {
        // 열쇠 획득 안내 해주고
        K_KeyUiManager.instance.img_getKeyDir.gameObject.SetActive(true);
        // 2초 대기 하고
        yield return new WaitForSeconds(2f);
        // 열쇠 획득 안내 사라지셈
        print("1번");
        K_KeyUiManager.instance.img_getKeyDir.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        print("2번");

        // 그리고 열쇠 아이콘 업데이트
        K_KeyUiManager.instance.UpdateKeyUI(totalKeys);

    }

    // 왕 아이콘 1초 후 -> 투명벽 열렸다는 딜레이 함수
    private IEnumerator UnlockBarrierAfterKeyUI()
    {
        K_KeyUiManager.instance.EndKeyUi();
        yield return new WaitForSeconds(1f);
        OpenBarrier();
    }

    void OpenBarrier()
    {
        print("문열림");
        Destroy(barrier);

        isBarrierOpened = true;

        // 문 열렸다는 이미지 
        K_KeyUiManager.instance.img_doorOpen.gameObject.SetActive(true);
        // 문 열렸다는 이미지 2초뒤에 꺼줘
        K_KeyUiManager.instance.StartCoroutine(K_KeyUiManager.instance.HideOpenDoor(2f));

        // 왕 아이콘을 먼저 띄워줘야함..
        // 문열렸다는 안내 이미지 함수 -> 코루틴까지 처리하고
        
    }
}