﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class P_AlbumInfo : MonoBehaviour
{
    public TMP_Text booktitle;
    public TMP_Text date;

    public string photoUrl;

    public void SetBookTitle(string bookTitle)
    {
        booktitle.text = bookTitle;
    }

    public void SetPhotoUrl(string url)
    {
        photoUrl = url;
    }

    public void SetDate(string dateTime)
    {
        date.text = dateTime;
    }
}
