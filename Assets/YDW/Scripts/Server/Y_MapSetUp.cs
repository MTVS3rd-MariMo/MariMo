using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Y_MapSetUp : MonoBehaviour
{
    ClassMaterial classMaterial;
    public TMP_Text bookTitle;
    public TMP_Text bookTitle2;
    public TMP_Text bookTitleFirst;
    public TMP_Text bookAuthor;
    string bookContent;
    public TMP_Text role1;
    public TMP_Text role2;
    public TMP_Text role3;
    public TMP_Text role4;
    public TMP_Text paintTitle;

    public static Y_MapSetUp mapSetUp;
    public Y_BookController bookController;
    public GameObject img_loading;
    public GameObject Btn_Left;
    public GameObject Btn_Right;

    private void Awake()
    {
        print("awake 시작");

        bookController = GameObject.Find("BookCanvas").GetComponent<Y_BookController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Btn_Left = GameObject.Find("Btn_Left");
        Btn_Right = GameObject.Find("Btn_Right");
        img_loading = GameObject.Find("Img_Loading");
        bookTitleFirst = GameObject.Find("Txt_Title").GetComponent<TMP_Text>();
        bookAuthor = GameObject.Find("Txt_Author").GetComponent<TMP_Text>();
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
        bookTitleFirst.text = classMaterial.bookTitle;
        bookAuthor.text = classMaterial.author;
        paintTitle.text = classMaterial.bookTitle;
        print(classMaterial.bookContents);
        role1.text = classMaterial.lessonRoles[0];
        role2.text = classMaterial.lessonRoles[1];
        role3.text = classMaterial.lessonRoles[2];
        role4.text = classMaterial.lessonRoles[3];

        img_loading.SetActive(false);
        Btn_Left.GetComponent<Button>().interactable = true;
        Btn_Right.GetComponent<Button>().interactable = true;
    }
}
