﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_Player : MonoBehaviour
{

    
    void Update()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        Vector3 dirV = transform.forward * v;
        Vector3 dirH = transform.right * h;
        Vector3 dir = dirV + dirH;

        dir.Normalize();
    }
}
