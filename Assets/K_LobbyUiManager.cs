using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class K_LobbyUiManager : MonoBehaviour
{
    // 시작 안내창
    public Image img_StartInfo;
    // 열쇠 UI 창
    public Image img_KeyEmptyBox;
    // 열쇠 UI 창 강조 테두리
    public Image img_KeyBoxBorder;

    // 모든 학생이 들어왔는지
    public bool isAllArrived = false;
    private bool hasDisplayInfo = false;

    public static K_LobbyUiManager instance;


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
        // 처음엔 비활성화
        img_StartInfo.gameObject.SetActive(false);
        //img_KeyEmptyBox.gameObject.SetActive(false);
        img_KeyBoxBorder.gameObject.SetActive(false);
        img_KeyEmptyBox.gameObject.SetActive(false);
    }

    void Update()
    {
        //로비에 모든 학생들 입장하게 되면 1초뒤에 활성화 시키기
        if (isAllArrived && !hasDisplayInfo)
        {
            StartCoroutine(DisplayInfoWithDelay(1f));
            hasDisplayInfo = true;
        }
    }

    IEnumerator DisplayInfoWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DisplayInfo();
    }

    // 안내 창 + 강조 창
    void DisplayInfo()
    {
        img_StartInfo.gameObject.SetActive(true);
        img_KeyBoxBorder.gameObject.SetActive(true);
        img_KeyEmptyBox.gameObject.SetActive(true);

        // UI 사운드
        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_INFO);

        // 5초 뒤 비활성화
        StartCoroutine(HideInfo(5f));      
    }

    IEnumerator HideInfo(float delay)
    {
        yield return new WaitForSeconds(5f);

        img_StartInfo.gameObject.SetActive(false);
        img_KeyBoxBorder.gameObject.SetActive(false);
    }

}
