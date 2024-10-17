using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_DummyInteract : MonoBehaviour
{
    public int count;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        count++;

        if (count == 4)
        {

        }

    }

    private void OnTriggerExit(Collider other)
    {
        count--;
    }
}
