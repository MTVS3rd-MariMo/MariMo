﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Y_PlayerNameTag : MonoBehaviourPun
{
    public Sprite myNameTag;
    public GameObject nameUI;
    public PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponentInParent<PhotonView>();
        nameUI.GetComponentInChildren<TMP_Text>().text = PhotonNetwork.NickName;
        if(pv.IsMine)
        {
            print("nameUI.GetComponent<Image>().sprite : " + (nameUI.GetComponent<Image>().sprite == null));
            print("myNameTage : " + (myNameTag == null));
            nameUI.GetComponent<Image>().sprite = myNameTag;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
