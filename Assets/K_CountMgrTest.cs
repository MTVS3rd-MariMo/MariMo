using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_CountMgrTest : MonoBehaviour
{
    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // 4명 이상 들어오고 LobbyUiManager 있으면 실행시켜
        if (players.Length >= 4 && K_LobbyUiManager.instance != null)
        {
            K_LobbyUiManager.instance.isAllArrived = true;
            print("4명 다옴");
        }
        else if (K_LobbyUiManager.instance != null)
        {
            K_LobbyUiManager.instance.isAllArrived = false;
            print("아직 4명 안댐");
        }
    }
}
