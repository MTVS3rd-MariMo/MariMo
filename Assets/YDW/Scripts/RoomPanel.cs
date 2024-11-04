﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomPanel : MonoBehaviour
{
    //public TMP_Text[] roomTexts = new TMP_Text[3];
    public Button btn_Room;
    //public Image img_MapImg;
    //public Sprite[] mapSprites;


    public void SetRoomInfo(RoomInfo room)
    {
        btn_Room.GetComponentInChildren<TMP_Text>().text = room.Name;
        //roomTexts[0].text = room.Name;
        //roomTexts[1].text = $"({room.PlayerCount}/{room.MaxPlayers})";
        //string masterName = room.CustomProperties["MASTER_NAME"].ToString();
        //roomTexts[2].text = masterName;
        // 맵 미리보기 이미지 넣기
        //int spriteNum = (int)room.CustomProperties["SCENE_NUMBER"] - 2;
        //img_MapImg.sprite = mapSprites[spriteNum];
    }
}