﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Y_MapSetUp : MonoBehaviour
{
    ClassMaterial classMaterial;
    public TMP_Text bookTitle;
    string bookContent;
    public TMP_Text role1;
    public TMP_Text role2;
    public TMP_Text role3;
    public TMP_Text role4;

    public static Y_MapSetUp mapSetUp;

    private void Awake()
    {
        if (mapSetUp == null)
        {
            mapSetUp = this;
        }
        else
        {
            Destroy(gameObject);
        }

        classMaterial = Y_HttpRoomSetUp.GetInstance().realClassMaterial;
        //Y_BookController.Instance.text = classMaterial.bookContents;
        //role1.text = classMaterial.lessonRoles[0];
        //role2.text = classMaterial.lessonRoles[1];
        //role3.text = classMaterial.lessonRoles[2];
        //role4.text = classMaterial.lessonRoles[3];
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}