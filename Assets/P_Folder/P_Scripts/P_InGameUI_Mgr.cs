using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_InGameUI_Mgr : MonoBehaviour
{
    public GameObject Wall;

    float time;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public IEnumerator Timer(float time)
    {
        float now = 0f;

        while (now < time)
        {

            now += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

    }
}
