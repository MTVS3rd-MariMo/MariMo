using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScriptCollider : MonoBehaviour
{
    public int count = 0;

    private void OnTriggerEnter(Collider other)
    {
        count++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        count--;
    }
}
