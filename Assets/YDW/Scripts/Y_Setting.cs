using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_Setting : MonoBehaviour
{
    public GameObject settings;

    public GameObject panel_login;
    public GameObject panel_title;

    public void closeSetting()
    {
        settings.SetActive(false);
    }

    public void clickSetting()
    {
        settings.SetActive(true);
    }

    public void SignOut()
    {
        PhotonNetwork.Disconnect();

        panel_title.SetActive(false);
        panel_login.SetActive(true);
    }
}
