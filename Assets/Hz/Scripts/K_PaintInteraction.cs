using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_PaintInteraction : MonoBehaviour
{
    public Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            mainCamera.enabled = false;
            print("UI ON");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("UI OFF");
        }
    }
}
