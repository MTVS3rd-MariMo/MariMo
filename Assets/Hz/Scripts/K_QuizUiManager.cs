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

}
