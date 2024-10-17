using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Utilities;

public class Y_PlayerMove : MonoBehaviour
{
    public float speed;
    CharacterController conn;

    public NavMeshAgent agent;
    int layerMaskGround;

    public GameObject test;

    // Start is called before the first frame update
    void Start()
    {
        speed = 20;
        conn = GetComponent<CharacterController>();
        layerMaskGround = LayerMask.GetMask("Ground");
        agent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if ((touch.phase == TouchPhase.Began) || (touch.phase == TouchPhase.Moved))
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 9999f, layerMaskGround))
                {
                    agent.SetDestination(hit.point);
                }
            }
        }
    }

    
}
