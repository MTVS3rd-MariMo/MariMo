using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class P_CreatorToolController : MonoBehaviour
{
    public GameObject panel_MakeStory;
    public GameObject panel_CreateRoom;
    public TMP_Dropdown dropdown;

    void Start()
    {
        
    }

    public void OnclickMakeStory()
    {
        panel_CreateRoom.SetActive(false);
        panel_MakeStory.SetActive(true);
    }

    public void OnclickCreateRoom()
    {
        panel_MakeStory.SetActive(false);
        panel_CreateRoom.SetActive(true);
    }

    public void OnclickLibrary()
    {

    }
}
