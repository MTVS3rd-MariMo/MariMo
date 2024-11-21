using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class P_StoryInfo : MonoBehaviour
{
    public TMP_Text title;
    public Button btn_Delete;
    public int lessonMaterialId;


    public void SetTitle(string booktitle)
    {
        title.text = booktitle;
    }

    public void SetLessonMaterialId(int Id)
    {
        lessonMaterialId = Id;
    }
}
