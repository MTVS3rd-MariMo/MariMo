using System.Collections;
using System.Collections.Generic;
using UnityEditor.VisionOS;
using UnityEngine;

public class Y_PlayerUIManager : MonoBehaviour
{
    public string text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            TouchScreenKeyboard.Open(text, TouchScreenKeyboardType.Default, true, false, false, false);
        }
    }

}
