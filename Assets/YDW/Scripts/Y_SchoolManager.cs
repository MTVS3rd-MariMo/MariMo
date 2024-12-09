using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Y_SchoolManager : MonoBehaviour
{
    List<GameObject> players = new List<GameObject>();


    public void startMovableCoroutine()
    {
        StartCoroutine(movableCoroutine());
    }

    IEnumerator movableCoroutine()
    {
        yield return new WaitForSeconds(6f);
        MoveControl(true);
    }

    void MoveControl(bool movable)
    {
        foreach (GameObject obj in players)
        {
            obj.GetComponent<Y_PlayerMove>().movable = movable;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            players.Add(other.gameObject);
        }
    }

}
