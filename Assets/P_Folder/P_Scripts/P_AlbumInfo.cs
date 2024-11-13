using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class P_AlbumInfo : MonoBehaviour
{
    public TMP_Text booktitle;

    public void SetBookTitle(string bookTitle)
    {
        booktitle.text = bookTitle;
    }
}
