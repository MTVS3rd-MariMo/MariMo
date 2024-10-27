using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Y_SchoolManager : MonoBehaviour
{
    List<GameObject> players = new List<GameObject>();
    public Button btn_enter;

    void Start()
    {
        btn_enter.onClick.AddListener(MoveControl);
    }

    void MoveControl()
    {
        foreach (GameObject obj in players)
        {
            obj.GetComponent<Y_PlayerMove>().movable = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            players.Add(other.gameObject);
        }
    }

    void Update()
    {
        
    }
}
