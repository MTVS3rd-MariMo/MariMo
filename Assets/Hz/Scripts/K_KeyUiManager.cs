using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class K_KeyUiManager : MonoBehaviour
{
    // 키 아이콘 배열
    public GameObject[] keyImages;

    public Image img_getKeyDir;
    // 애니메이션 적용된 마지막 책갈피 UI
    public GameObject img_FinalKeyDir;
    // 책갈피 애니메이션
    public Animator anim_FinalBookMark;
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
            //DontDestroyOnLoad(gameObject);
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
        // 책갈피로 변경해줌 (애니메이션 적용)
        img_FinalKeyDir.gameObject.SetActive(false);
        // Key_UI 애니메이션 찾아주기
        anim_FinalBookMark = GetComponentInChildren<Animator>();
        anim_FinalBookMark.enabled = false;
        // ..
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
        StartCoroutine(HideGetKeyDir(2f));
    }

    // 마지막 왕 열쇠 아이콘 ui
    public void EndKeyUi()
    {
        

        

        // 마지막 열쇠 아이콘 켜주기
        //keyImages[3].SetActive(true);

        // 왕열쇠 이미지 띄워주고 (이거 KEY_UI 프리팹으로 교체해야함)
        //img_endKeyDir.gameObject.SetActive(true);
        img_FinalKeyDir.SetActive(true);
        StartCoroutine(HideLastKey());
        //anim_FinalBookMark.Play("Key Animation");
        // 1초뒤에 숨겨주고 -> 이것도 애니메이션 초에 맞게 UI false
        //StartCoroutine(HideLastKey(3f));
        //StartCoroutine(HideLastKey(1f));

    }

    // 코루틴 ~
    // 열쇠 얻은 안내 ui 2초 뒤에 사라짐
    public IEnumerator HideGetKeyDir(float delay)
    {
        
        yield return new WaitForSeconds(delay);
        img_getKeyDir.gameObject.SetActive(false);
        print("안내 창 없어지니?");
    }

    // 마지막 큰 열쇠 이미지 3초뒤에 사라짐
    public IEnumerator HideLastKey()
    {
        //yield return new WaitForSeconds();

        // 애니메이션 이미지 켜주고
        //img_FinalKeyDir.SetActive(true);
        anim_FinalBookMark.enabled = true;
        //anim_FinalBookMark.Play("Key Animation");
        // 3초 후
        yield return new WaitForSeconds(3f);
        //img_endKeyDir.gameObject.SetActive(false);
        // 이미지 꺼주기
        img_FinalKeyDir.SetActive(false);

    }
    // 문 열어보세요 안내문 2초뒤에 사라짐
    public IEnumerator HideOpenDoor(float delay)
    {
        yield return new WaitForSeconds(delay);
        img_doorOpen.gameObject.SetActive(false);
    }


    /// <summary>
    /// /////////시연용
    /// </summary>
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            Y_GameManager.instance.RPC_Unlock();
        }
    }
}
