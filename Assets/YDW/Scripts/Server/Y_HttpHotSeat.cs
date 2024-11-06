using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_HttpHotSeat : MonoBehaviour
{
    static Y_HttpHotSeat instance;

    public static Y_HttpHotSeat GetInstance()
    {
        if (instance == null)
        {
            GameObject go = new GameObject();
            go.name = "HttpHotSeat";
            go.AddComponent<Y_HttpHotSeat>();
        }

        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
