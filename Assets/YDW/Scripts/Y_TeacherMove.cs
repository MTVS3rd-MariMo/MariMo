using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Y_TeacherMove : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.Owner.IsMasterClient)
        {
            //Debug.LogError("playerAverage 널이니??? : " + (GetComponent<Y_SetCamera>().playerAverage == null)); -> False
            gameObject.transform.position = GetComponent<Y_SetCamera>().playerAverage.position;
        }
    }
}
