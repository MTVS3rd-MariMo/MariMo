using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_ObjectManager : MonoBehaviour
{
    //public List<Transform> objectList = new List<Transform>();
    float triggerNum = 0;
    Transform wall;

    // Start is called before the first frame update
    void Start()
    {
        wall = GetComponentInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //NearObject();
        if(triggerNum >= 4)
        {
            wall.gameObject.SetActive(true);
        }
    }

    //void NearObject()
    //{
    //    foreach (Transform trfm in objectList)
    //    {
    //        //print(Vector3.Distance(gameObject.transform.position, trfm.position));
    //        if (Vector3.Distance(gameObject.transform.position, trfm.position) < 15f)
    //        {
    //            print("!!!!!!!!!!!!!!!!");
    //        }
    //    }
    //}

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
