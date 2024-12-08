using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class P_LookCharacter : MonoBehaviour
{
    public TMP_InputField[] input_Characters;


    public void OnStart()
    {
        for (int i = 0; i < input_Characters.Length; i++)
        {
            input_Characters[i].text = P_CreatorToolConnectMgr.Instance.quizData.roleList[i].roleName;
        }
    }

    public void FinishCharacter()
    {
        for (int i = 0; i < input_Characters.Length; i++)
        {
            P_CreatorToolConnectMgr.Instance.quizData.roleList[i].roleName = input_Characters[i].text;
        }
    }
}
