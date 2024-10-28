using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_QuizStart : MonoBehaviour
{
    float triggerNum = 0;
    bool act = false;
    List<GameObject> players = new List<GameObject>();
    //public GameObject hotSeatCanvas;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerNum++;
            print("+++++++++ : " + triggerNum);

            players.Add(other.gameObject);


            if (triggerNum >= 4 && !act)
            {
                act = true;

                K_QuizManager.instance.isPlaying = true;

                //MoveControl(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerNum--;
            print("-------- : " + triggerNum);

            players.Remove(other.gameObject);
        }
    }

    void MoveControl(bool canmove)
    {
        foreach (GameObject obj in players)
        {
            obj.GetComponent<Y_PlayerMove>().movable = canmove;
        }
    }
}
