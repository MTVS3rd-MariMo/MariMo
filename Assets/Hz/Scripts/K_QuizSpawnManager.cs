using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_QuizSpawnManager : MonoBehaviour
{
    public GameObject quiz1;
    //public GameObject quiz2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Test
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Vector3 pos = transform.position = Random.insideUnitSphere * 5f;

            pos.y = 0;

            // 오브젝트 생성
            GameObject ob = Instantiate(quiz1, pos, Quaternion.identity);

            Destroy(ob,1f);
        }
        
    }


}
