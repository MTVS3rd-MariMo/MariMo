using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class P_FileInfo : MonoBehaviour
{
    public Sprite[] sprites; 

    public Button btn_OpenFile;
    public TMP_Text fileName;
    public TMP_Text creationTime;
    public TMP_Text fileSize;


    public void SetFileImage(int i)
    {
        btn_OpenFile.image.sprite = sprites[i];
    }

    public void SetFileName(string name)
    {
        fileName.text = name;
    }

    public void SetCreationTime(string time)
    {
        creationTime.text = time; 
    }

    public void SetFileSize(string size)
    {
        fileSize.text = size;
    }
}
