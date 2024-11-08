using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem.Android;

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


    void Start()
    {
        
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


}
