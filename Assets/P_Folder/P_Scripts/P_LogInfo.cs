﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class P_LogInfo : MonoBehaviour
{
    public TMP_Text book;
    public TMP_Text[] players;
    public TMP_Text date;
    public int lessonId;

    public void SetBook(string bookName)
    {
        book.text = bookName;
    }

    public void SetPlayers(string[] playersName)
    {
        for (int i = 0; i < playersName.Length; i++)
        {
            players[i].text = playersName[i];
        }
    }

    public void SetDate(string dateTime)
    {
        date.text = dateTime; 
    }

    public void SetLessonId(int Id)
    {
        lessonId = Id;
    }

    public int GetLessonId()
    {
        return lessonId;
    }
}