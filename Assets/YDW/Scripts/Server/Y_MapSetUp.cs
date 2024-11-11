using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Y_MapSetUp : MonoBehaviour
{
    ClassMaterial classMaterial;
    public TMP_Text bookTitle;
    public TMP_Text bookTitle2;
    string bookContent;
    public TMP_Text role1;
    public TMP_Text role2;
    public TMP_Text role3;
    public TMP_Text role4;


    public static Y_MapSetUp mapSetUp;
    public Y_BookController bookController;

    private void Awake()
    {

        print("awake 시작");

        bookController = GameObject.Find("BookCanvas").GetComponent<Y_BookController>();

        
        

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyClassMaterial()
    {

        classMaterial = Y_HttpRoomSetUp.GetInstance().realClassMaterial;

        bookController.text = classMaterial.bookContents;
        bookTitle.text = classMaterial.bookTitle;
        bookTitle2.text = classMaterial.bookTitle;
        print(classMaterial.bookContents);
        role1.text = classMaterial.lessonRoles[0];
        role2.text = classMaterial.lessonRoles[1];
        role3.text = classMaterial.lessonRoles[2];
        role4.text = classMaterial.lessonRoles[3];
    }
}
