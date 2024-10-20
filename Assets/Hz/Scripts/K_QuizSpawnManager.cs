using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_QuizSpawnManager : MonoBehaviour
{
    // 랜덤하게 배치할 퀴즈1, 2
    public GameObject quiz1;
    public GameObject quiz2;

    // 퀴즈1, 2스폰 구역 크기
    public Vector3 quiz1_spawnSize;
    public Vector3 quiz2_spawnSize;
    // 퀴즈1, 2 스폰 구역의 중앙
    public Vector3 quiz1_spawnCenter;
    public Vector3 quiz2_spawnCenter;
    

    void Start()
    {
        // 시작 시, 퀴즈 2개 랜덤위치에 배치한다
        SpawnObj(quiz1, quiz1_spawnCenter, quiz1_spawnSize);
        SpawnObj(quiz2, quiz2_spawnCenter, quiz2_spawnSize);
    }

    private void SpawnObj(GameObject obj, Vector3 center, Vector3 size)
    {
        Vector3 randomPos = GetRandomPosInArea(center, size);
        Instantiate(obj, randomPos, Quaternion.Euler(90,0,0));
    }

    private Vector3 GetRandomPosInArea(Vector3 center, Vector3 size)
    {
        Vector3 randomPos = new Vector3(
            Random.Range(-size.x / 2, size.x / 2),
            0, // Y축을 0으로 고정 (평면에 생성되도록)
            Random.Range(-size.z / 2, size.z / 2)
        );

        return center + randomPos;
    }
}
