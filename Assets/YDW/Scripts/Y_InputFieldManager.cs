﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Y_InputFieldManager : MonoBehaviour
{
    private TouchScreenKeyboard keyboard;

    void Update()
    {

    }

    public void OnSelect(BaseEventData eventData)
    {
        // 터치 키보드 호출 (모바일에서만 동작)
        keyboard = TouchScreenKeyboard.Open(gameObject.GetComponent<TMP_InputField>().text, TouchScreenKeyboardType.Default);
    }

    // InputField가 선택 해제되었을 때 호출
    public void OnDeselect(BaseEventData eventData)
    {
        // 터치 키보드 닫기
        if (keyboard != null && keyboard.active)
        {
            gameObject.GetComponent<TMP_InputField>().text = keyboard.text;
            keyboard.active = false;
        }
    }
}
