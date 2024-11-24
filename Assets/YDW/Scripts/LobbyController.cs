using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class LobbyController : MonoBehaviour
{
    public GameObject panel_login;
    public Button btn_login;
    public TMP_InputField input_nickName;
    public GameObject panel_Title;
    public static LobbyController lobbyUI;
                                                                                                                                                                                                                                                                                                                                   
    public GameObject enterRoom;
    public GameObject album;

    public Button[] buttons;
    public Sprite[] sprites;

    string log;

    private void Awake()
    {
        if(lobbyUI == null)
        {
            lobbyUI = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowRoomPanel()
    {
        btn_login.interactable = true;
        panel_login.gameObject.SetActive(false);
        panel_Title.SetActive(true);
    }
    
    public void PrintLog(string message)
    {
        log += message + '\n';
        print(message);
    }

    public void ShowAlbum()
    {
        buttons[0].GetComponent<Image>().sprite = sprites[2];
        buttons[1].GetComponent<Image>().sprite = sprites[1];

        enterRoom.SetActive(false);
        album.SetActive(true);
    }

    public void ShowEnterRoom()
    {
        buttons[0].GetComponent<Image>().sprite = sprites[3];
        buttons[1].GetComponent<Image>().sprite = sprites[0];

        album.SetActive(false);
        enterRoom.SetActive(true);
    }

    public RectTransform inputFieldRect;
    private Vector2 originalSize;
    private Vector2 originalPosition;
    public Vector2 expandedSize = new Vector2(1200, 100); // 확장된 크기
    public Vector2 expandedPos = new Vector2(0, 637); // 확장됐을 때 위치

    private void Start()
    {
        originalSize = inputFieldRect.sizeDelta;
        originalPosition = inputFieldRect.gameObject.transform.localPosition;
    }

    public void OnSelect(BaseEventData eventData)
    {
        inputFieldRect.sizeDelta = expandedSize;
        inputFieldRect.gameObject.transform.localPosition = expandedPos;

        //// 터치 키보드 호출 (모바일에서만 동작)
        //keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    // InputField가 선택 해제되었을 때 호출
    public void OnDeselect(BaseEventData eventData)
    {
        inputFieldRect.sizeDelta = originalSize;
        inputFieldRect.gameObject.transform.localPosition = originalPosition;

        //// 터치 키보드 닫기
        //if (keyboard != null && keyboard.active)
        //{
        //    keyboard.active = false;
        //}
    }
}
