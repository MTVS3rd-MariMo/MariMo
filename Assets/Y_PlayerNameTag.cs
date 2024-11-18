using Photon.Pun;
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
        nameUI.GetComponentInChildren<TMP_Text>().text = GetComponentInParent<PhotonView>().Owner.NickName;
        if(pv.IsMine)
        {
            nameUI.GetComponent<Image>().sprite = myNameTag;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
