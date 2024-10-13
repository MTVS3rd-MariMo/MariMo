using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class P_CreatorToolController : MonoBehaviour
{
    public GameObject panel_SelectStory;
    public GameObject panel_CreateRoom;
    public GameObject panel_NewStory;
    public GameObject panel_FileViewer;
    
    public TMP_Dropdown dropdown;
    

    void Start()
    {
        
    }

    public void OnclickSelectStory()
    {
        panel_SelectStory.SetActive(true);
        panel_CreateRoom.SetActive(false);
        panel_NewStory.SetActive(false);

    }

    public void OnclickNewStory()
    {
        panel_SelectStory.SetActive(false);
        panel_CreateRoom.SetActive(false);
        panel_NewStory.SetActive(true);
    }

    public void OnclickPDF()
    {

    }

    public void OnclickWrite()
    {

    }


    public void OnclickCreateRoom()
    {
        panel_SelectStory.SetActive(false);
        panel_CreateRoom.SetActive(true);
        panel_NewStory.SetActive(false);
    }

    public void OnclickLibrary()
    {

    }
}
