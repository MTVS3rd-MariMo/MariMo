using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_RoomTest : MonoBehaviour
{
    public ConnectionManager cm;

    // Start is called before the first frame update
    void Start()
    {
        cm = GameObject.Find("LobbyManager").GetComponent<ConnectionManager>();
        //cm.CreateRoom();
        //cm.CreateRoom();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
