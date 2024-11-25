using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class P_MakingAsk : MonoBehaviour
{
    // 인풋필드 사이즈 조정
    public RectTransform inputFieldRect;
    public Vector2 expandedSize = new Vector2(1200, 300); // 확장된 크기
    public Vector2 expandedPos = new Vector2(-345, 200); // 확장됐을 때 위치
    private Vector2 originalSize;
    private Vector2 originalPosition;

    private P_CreatorToolController _creatorToolController;

    public Button btn_Prev;
    public Button btn_Next;
    public Button btn_OK;

    public TMP_InputField askText;

    public int count = 0;

    public Sprite[] sprites;

    public Image[] img_Dot;
    public Sprite[] dot_sprites;

    void Start()
    {
        originalSize = inputFieldRect.sizeDelta;
        originalPosition = inputFieldRect.gameObject.transform.localPosition;

        _creatorToolController = GetComponentInParent<P_CreatorToolController>();
        btn_Prev.onClick.AddListener(OnclickPrev);
        btn_Next.onClick.AddListener(OnclickNext);
        btn_OK.onClick.AddListener(OnclickOK);

    }

    public void Open_MakingAsk()
    {
        count = 0;
        if (P_CreatorToolConnectMgr.Instance.GetOpenQuestion(0) != null)
            askText.text = P_CreatorToolConnectMgr.Instance.GetOpenQuestion(0).questionTitle;
    }

    void Update()
    {
        if (count == 0)
        {
            btn_Prev.interactable = false;
            img_Dot[0].sprite = dot_sprites[1];
            img_Dot[1].sprite = dot_sprites[0];
        }
        else
        {
            btn_Prev.interactable = true;
            img_Dot[0].sprite = dot_sprites[0];
            img_Dot[1].sprite = dot_sprites[1];
        }

        if (count == 1)
        {
            btn_OK.image.sprite = sprites[1];
            btn_OK.interactable = true;
        }
        else
        {
            btn_OK.image.sprite = sprites[0];
            btn_OK.interactable = false;
        }
    }

    public void OnclickPrev()
    {
        if (count <= 0)
            return;
        else
        {
            P_CreatorToolConnectMgr.Instance.ModifyQuestion(askText.text, count);
            
            count--;


            // 예시
            if (P_CreatorToolConnectMgr.Instance.GetOpenQuestion(0) != null)
                askText.text = P_CreatorToolConnectMgr.Instance.GetOpenQuestion(0).questionTitle;
        }

    }

    public void OnclickNext()
    {
        if (count >= 1)
        {
            return;
        }    
        else
        {
            P_CreatorToolConnectMgr.Instance.ModifyQuestion(askText.text, count);

            count++;

            // 예시
            if (P_CreatorToolConnectMgr.Instance.GetOpenQuestion(1) != null)
                askText.text = P_CreatorToolConnectMgr.Instance.GetOpenQuestion(1).questionTitle;
        }
    }

    public void OnclickOK()
    {
        // 데이터 저장
        P_CreatorToolConnectMgr.Instance.ModifyQuestion(askText.text, count);

        // 다음 창으로 이동
        GoTitle();
    }

    public void GoTitle()
    {
        // 데이터 송신
        QuizData data = P_CreatorToolConnectMgr.Instance.GetAllData();

        _creatorToolController.OnFinishMaking(data);

        // 초기 화면으로
        _creatorToolController.OnclickSelectStory();
    }

    // InputField가 선택되었을 때 호출
    public void OnSelect(BaseEventData eventData)
    {
        inputFieldRect.sizeDelta = expandedSize;
        inputFieldRect.gameObject.transform.localPosition = expandedPos;

    }

    // InputField가 선택 해제되었을 때 호출
    public void OnDeselect(BaseEventData eventData)
    {
        inputFieldRect.sizeDelta = originalSize;
        inputFieldRect.gameObject.transform.localPosition = originalPosition;

    }
}
