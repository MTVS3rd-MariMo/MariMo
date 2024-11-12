using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_AlbumInfo : MonoBehaviour
{
    string booktitle;
    string photo_url;

    public void SetBookTitle(string bookTitle)
    {
        booktitle = bookTitle;
    }

    public void SetUrl(string photo)
    {
        photo_url = photo;
    }
    

    public string GetUrl()
    {
        return photo_url;
    }
}
