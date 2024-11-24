using System.Collections;
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
    public bool isDoneHotSeating = false;

    // 획득한 총 열쇠 개수
    public int totalKeys = 0;

    
    private bool isBarrierOpened = false;

    // Singleton
    public static K_KeyManager instance;


    // 사용시
    // Hot Sitting 활동 완료 시 KeyManager의 bool 값을 true로 설정
    //K_KeyManager.Instance.isDoneHotSeating = true;

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

        if (isDoneHotSeating)
        {
            GetKey();
            isDoneHotSeating = false;
        }

        if (isDoneQuiz_2)
        {
            GetKey();
            isDoneQuiz_2 = false;
        }

        // 총 열쇠 3개 -> 투명벽 열림
        if (totalKeys >= 4 && !isBarrierOpened)
        {
            //// 스탬프 다모이면 큰 열쇠 이미지 띄워주는 함수
            //K_KeyUiManager.instance.EndKeyUi();
            //// 투명벽 열려
            //OpenBarrier();

            // 여기도 코루틴써야하나 (왕 열쇠 띄워주는 함수..?)
            Y_GameManager.instance.RPC_Unlock();
            isBarrierOpened = true;
        }
    }

    void GetKey(int keyAmount = 1)
    {
        // 키 먹어
        totalKeys += keyAmount;

        // 열쇠 획득 안내 ui (활동 끝날때마다 띄워줘야함 - 2초 활성화 후 꺼지고 - 열쇠 아이콘 띄워주기)
        if (totalKeys < 4)
        {
            StartCoroutine(DisplayGetKeyUI());
        }


        // 열쇠 아이콘 업데이트 함수
        //K_KeyUiManager.instance.UpdateKeyUI(totalKeys);
        print("열쇠 획득! 현재 열쇠 갯수 : " + totalKeys);
        
    }

    // 키 안내창 -> 키 아이콘 생성 딜레이 함수
    private IEnumerator DisplayGetKeyUI()
    {
        //// 열쇠 획득 안내 해주고
        //K_KeyUiManager.instance.img_getKeyDir.gameObject.SetActive(true);
        //// 2초 대기 하고
        //yield return new WaitForSeconds(2f);
        //// 열쇠 획득 안내 사라지셈
        //print("1번");
        //K_KeyUiManager.instance.img_getKeyDir.gameObject.SetActive(false);
        //yield return new WaitForSeconds(2f);
        //print("2번");

        //// 그리고 열쇠 아이콘 업데이트
        //K_KeyUiManager.instance.UpdateKeyUI(totalKeys);


        // 다시. 
        // 열린질문
        if (isDoneOpenQnA)
        {
            // 열린질문 성공 ui 띄워주고
            //K_KeyUiManager.instance.keyImages[0].gameObject.SetActive(true);
            K_KeyUiManager.instance.img_QuestionBookmark.gameObject.SetActive(true);
            // 북마크 사운드
            Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_BOOKMARK);
            yield return new WaitForSeconds(2f);
            K_KeyUiManager.instance.img_QuestionBookmark.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
            K_KeyUiManager.instance.keyImages[0].gameObject.SetActive(true);
            

        }
        // 핫시팅
        if (isDoneHotSeating)
        {
            K_KeyUiManager.instance.img_HotSeatBookmark.gameObject.SetActive(true);
            // 북마크 사운드
            Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_BOOKMARK);
            yield return new WaitForSeconds(2f);
            K_KeyUiManager.instance.img_HotSeatBookmark.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
            K_KeyUiManager.instance.keyImages[1].gameObject.SetActive(true);
            
        }
        // 퀴즈1
        if (isDoneQuiz_1)
        {
            K_KeyUiManager.instance.img_Quiz1Bookmark.gameObject.SetActive(true);
            // 북마크 사운드
            Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_BOOKMARK);
            yield return new WaitForSeconds(2f);
            K_KeyUiManager.instance.img_Quiz1Bookmark.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
            K_KeyUiManager.instance.keyImages[2].gameObject.SetActive(true);
            
        }
        // 퀴즈2
        if (isDoneQuiz_2)
        {
            K_KeyUiManager.instance.img_Quiz2Bookmark.gameObject.SetActive(true);
            // 북마크 사운드
            Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_BOOKMARK);
            yield return new WaitForSeconds(2f);
            K_KeyUiManager.instance.img_Quiz2Bookmark.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
            K_KeyUiManager.instance.keyImages[3].gameObject.SetActive(true);
            
        }

    }

}