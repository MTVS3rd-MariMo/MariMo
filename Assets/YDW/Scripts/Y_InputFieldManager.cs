using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Y_InputFieldManager : MonoBehaviour
{
    public TMP_InputField chatInputField;

    private TouchScreenKeyboard keyboard;

    void Update()
    {
        OpenVirtualKeyboard();
    }

    void OpenVirtualKeyboard()
    {
        // InputField가 포커스되었을 때 키보드를 띄우기
        if (chatInputField.isFocused && keyboard == null)
        {
            // 텍스트와 옵션을 설정해 가상 키보드 열기
            keyboard = TouchScreenKeyboard.Open(chatInputField.text, TouchScreenKeyboardType.Default, false, false, false, false);
        }

        // 키보드가 닫혔을 때 텍스트를 갱신
        if (keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Done)
        {
            chatInputField.text = keyboard.text;
            keyboard = null;
        }
    }

}
