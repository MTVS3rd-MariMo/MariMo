using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_QuizSpawnManager : MonoBehaviour
{
    // 랜덤하게 배치할 퀴즈1, 2
    public GameObject quiz1;
    public GameObject quiz2;

    // 랜덤하게 배치할 퀴즈1,2의 정답 구역
    private GameObject quiz1_correct;
    private GameObject quiz2_correct;

    // 퀴즈1, 2스폰 구역 크기
    public Vector3 quiz1_spawnSize;
    public Vector3 quiz2_spawnSize;
    // 퀴즈1, 2 스폰 구역의 중앙
    public Vector3 quiz1_spawnCenter;
    public Vector3 quiz2_spawnCenter;


    // 퀴즈1, 2 정답 구역 -> 퀴즈1,2 구역 4분할 했을 때 해당 구역에 랜덤 배치
    // 퀴즈1, 2스폰 구역 크기
    public Vector3 quiz1_correctASize;
    public Vector3 quiz2_correctASize;
    // 퀴즈1, 2 스폰 구역의 중앙
    public Vector3 quiz1_correctACenter;
    public Vector3 quiz2_correctACenter;

    int spawnCount = 0;
    void Start()
    {
        // 시작 시, 퀴즈 2개 랜덤위치에 배치한다
        SpawnObj(quiz1, quiz1_spawnCenter, quiz1_spawnSize);
        // 시작 시, 퀴즈 각각 정답 2개 랜덤하게 배치한다. -> 4분할
        SpawnCorrectA(quiz1_correctASize);

        SpawnObj(quiz2, quiz2_spawnCenter, quiz2_spawnSize);
        SpawnCorrectA(quiz2_correctASize);
    }

    private void SpawnObj(GameObject obj, Vector3 center, Vector3 size)
    {
        Vector3 randomPos = GetRandomPosInArea(center, size);
        GameObject go = Instantiate(obj, randomPos, Quaternion.Euler(90, 0, 0));
        

        if(spawnCount == 0)
        {
            quiz1_correctACenter = go.transform.position;
            K_QuizPos_1 k_QuizPos_1 = go.GetComponent<K_QuizPos_1>();
            quiz1_correct = k_QuizPos_1.correct1;
            
        }
        else if(spawnCount == 1)
        {
            quiz2_correctACenter = go.transform.position;
            K_QuizPos_2 k_QuizPos_2 = go.GetComponent<K_QuizPos_2>();
            quiz2_correct = k_QuizPos_2.correct2;
        }
        
    }

    private void SpawnCorrectA(Vector3 size)
    {
        float x = Random.Range(0, 2) * size.x;
        float y = Random.Range(0, 2) * size.y;
        // 크기를 1로 생각하기 (scale 10 생각 x)
        Vector3 randomPos = new Vector3(x, y, -0.01f) - (size * 0.5f);

        if (spawnCount == 0)
        {
            quiz1_correct.transform.localPosition = randomPos;
        }
        else if (spawnCount == 1)
        {
            quiz2_correct.transform.localPosition = randomPos;
        }
        spawnCount++;

        return;
        // 4분할 좌표, 중앙 구하기
        Vector3 quadSize = new Vector3(size.x / 2, size.y, size.z / 2);
        Vector3[] quadCenter = new Vector3[4];

        //quadCenter[0] = center + new Vector3(-quadSize.x / 2, 0, -quadSize.z / 2);
        //quadCenter[1] = center + new Vector3(quadSize.x / 2, 0, -quadSize.z / 2);
        //quadCenter[2] = center + new Vector3(-quadSize.x / 2, 0, quadSize.z / 2);
        //quadCenter[3] = center + new Vector3(quadSize.x / 2, 0, quadSize.z / 2);

      //  Vector3 randomPos = GetRandomPosInArea(center, size);

        if(spawnCount == 0)
        {
            quiz1_correct.transform.position = randomPos;
        }
        else if(spawnCount == 1)
        {
            quiz2_correct.transform.position = randomPos;
        }
        spawnCount++;
    }

    private Vector3 GetRandomPosInArea(Vector3 center, Vector3 size)
    {
        Vector3 randomPos = new Vector3(
            Random.Range(-size.x / 2, size.x / 2),
            0,
            Random.Range(-size.z / 2, size.z / 2)
        );

        return center + randomPos;
    }
}
