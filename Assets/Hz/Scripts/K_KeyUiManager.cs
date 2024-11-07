﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class K_KeyUiManager : MonoBehaviour
{
    // 키 아이콘 배열
    public GameObject[] keyImages;

    public Image img_getKeyDir;
    public Image img_endKeyDir;
    public Image img_doorOpen;

    // 열린질문 - 책갈피 얻음 ui
    public Image img_QuestionBookmark;
    // 핫시팅 - 책갈피 얻음 ui
    public Image img_HotSeatBookmark;
    // 퀴즈1 - 책갈피 얻음 ui
    public Image img_Quiz1Bookmark;
    // 퀴즈2 - 책갈피 얻음 ui
    public Image img_Quiz2Bookmark;

    // Singleton
    public static K_KeyUiManager instance;

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
        img_getKeyDir.gameObject.SetActive(false);
        img_endKeyDir.gameObject.SetActive(false);
        img_doorOpen.gameObject.SetActive(false);

        img_QuestionBookmark.gameObject.SetActive(false);
        img_HotSeatBookmark.gameObject.SetActive(false);
        img_Quiz1Bookmark.gameObject.SetActive(false);
        img_Quiz2Bookmark.gameObject.SetActive(false);

        // 키 아이콘들 처음에 비활성화
        foreach (GameObject keyImage in keyImages)
        {
            keyImage.SetActive(false);
        }
    }   

    // 열쇠 획득 후 열쇠 아이콘 ui
    public void UpdateKeyUI(int totalKeys)
    {
        for(int i = 0; i < keyImages.Length; i++)
        {
            keyImages[i].SetActive(i < totalKeys);
        }
    }

    // 열쇠 획득했다는 안내 ui (활동 끝날때마다 띄워줘야함 - 2초 활성화 후 꺼지고 - 열쇠 아이콘 띄워주기)
    public void GetKeyDir()
    {
        // 열쇠 조각 획득했다는 안내 ui 띄워주기
        img_getKeyDir.gameObject.SetActive(true);
        // 안내 ui 숨겨주기
        //StartCoroutine(HideGetKeyDir(2f));
    }

    // 마지막 왕 열쇠 아이콘 ui
    public void EndKeyUi()
    {
        // 왕열쇠 이미지 띄워주고
        img_endKeyDir.gameObject.SetActive(true);
        // 1초뒤에 숨겨주고
        StartCoroutine(HideLastKey(1f));
        // 마지막 열쇠 아이콘 켜주기
        keyImages[3].SetActive(true);
    }


    // 코루틴 ~


    // 열쇠 얻은 안내 ui 2초 뒤에 사라짐
    public IEnumerator HideGetKeyDir(float delay)
    {
        
        yield return new WaitForSeconds(delay);
        img_getKeyDir.gameObject.SetActive(false);
        print("안내 창 없어지니?");
    }

    // 마지막 큰 열쇠 이미지 1초뒤에 사라짐
    public IEnumerator HideLastKey(float delay)
    {
        yield return new WaitForSeconds(delay);
        img_endKeyDir.gameObject.SetActive(false);

    }
    // 문 열어보세요 안내문 2초뒤에 사라짐
    public IEnumerator HideOpenDoor(float delay)
    {
        yield return new WaitForSeconds(delay);
        img_doorOpen.gameObject.SetActive(false);
    }
}
