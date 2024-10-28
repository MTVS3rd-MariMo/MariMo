using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class K_QuizSpawnMgr : MonoBehaviour
{
    int quizCount = 2;
    public GameObject quiz;

    // 퀴즈 2개 배열
    public GameObject[] quizzes; 

    public Vector3[] quiz_spawnSize;
    public Vector3[] quiz_spawnCenter;

    public GameObject [] quiz_correct;
    public Vector3 quiz_correctASize;



    // Start is called before the first frame update
    void Start()
    {
        quiz_correct = new GameObject[quizCount];

        for (int i = 0; i < quizCount; i++)
        {
            SpawnObj(quizzes[i], i);
            //SpawnObj(quiz, i);
            //SpawnCorrectA(i);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            for (int i = 0; i < quizCount; i++)
            {
                SpawnCorrectA(i);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            for (int i = 0; i < quizCount; i++)
            {
                SpawnObj(quizzes[i], i);
                //SpawnObj(quiz, i);
            }
        }
    }

    private void SpawnObj(GameObject obj, int idx)
    {
        Vector3 center = quiz_spawnCenter[idx];
        Vector3 size = quiz_spawnSize[idx];
        Vector3 randomPos = GetRandomPosInArea(center, size);
        GameObject go = Instantiate(obj, randomPos, Quaternion.Euler(0, 0, 0));
        // x축 90에서 0으로 변경

        K_QuizPos k_QuizePos = go.GetComponent<K_QuizPos>();
        quiz_correct[idx] = k_QuizePos.correct;        
    }

    private void SpawnCorrectA(int idx)
    {
        float x = Random.Range(0, 2) * quiz_correctASize.x;
        float y = Random.Range(0, 2) * quiz_correctASize.y;
        // 크기를 1로 생각하기 (scale 10 생각 x)
        Vector3 randomPos = new Vector3(x, y, -0.01f) - (quiz_correctASize * 0.5f);

        quiz_correct[idx].transform.localPosition = randomPos;
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
