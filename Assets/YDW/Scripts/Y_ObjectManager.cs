using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_ObjectManager : MonoBehaviour
{
    //public List<Transform> objectList = new List<Transform>();
    float triggerNum = 0;
    public GameObject wall;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(triggerNum >= 4)
        {
            wall.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        triggerNum++;
        print("+++++++++ : " + triggerNum);
    }

    private void OnTriggerExit(Collider other)
    {
        triggerNum--;
        print("-------- : " + triggerNum);
    }
}
