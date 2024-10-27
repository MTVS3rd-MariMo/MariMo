using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatePaint : MonoBehaviour
{
    public static InstantiatePaint Instance { get; private set; }

    private void Awake()
    {
        // Singleton 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
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
