using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyUIController : MonoBehaviour
{
    public GameObject panel_login;
    public Button btn_login;
    public TMP_InputField input_nickName;
    //public GameObject panel_joinOrCreateRoom;
    public GameObject panel_Title;
    public static LobbyUIController lobbyUI;
    public TMP_InputField[] roomSetting;
    public TMP_Text text_logText;

    public GameObject enterRoom;
    public GameObject album;

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


    void Start()
    {
        //drop_mapSelection.onValueChanged.AddListener(ShowSelectedMapImage);
    }

    //void ShowSelectedMapImage(int num)
    //{
    //    img_mapImage.sprite = mapSprites[num];
    //}

    public void ShowRoomPanel()
    {
        btn_login.interactable = true;
        panel_login.gameObject.SetActive(false);
        //panel_joinOrCreateRoom.SetActive(true);
        panel_Title.SetActive(true);
    }

    //public void ShowSignIn()
    //{
    //    logInUI.SetActive(false);
    //    signUpUI.SetActive(true);
    //}
    
    public void PrintLog(string message)
    {
        log += message + '\n';
        text_logText.text = log;
    }

    public void ShowAlbum()
    {
        enterRoom.SetActive(false);
        album.SetActive(true);
    }

    public void ShowEnterRoom()
    {
        album.SetActive(false);
        enterRoom.SetActive(true);
    }

    //public int GetSelectedMapNumber()
    //{
    //    return drop_mapSelection.value;
    //}
}
