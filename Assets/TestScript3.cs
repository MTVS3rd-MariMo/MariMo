using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScript3 : MonoBehaviour
{
    public Text tt;

    // Start is called before the first frame update
    void Start()
    {
        tt = GetComponent<Text>();
    }

    int fff = 0;
    float currentTime = 0;

    // Update is called once per frame
    void Update()
    {
        fff++;

        currentTime += Time.deltaTime;

        if(currentTime > 1)
        {
            tt.text = fff.ToString();
            fff = 0;
            currentTime = 0;
        }
    }
}
