using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class K_QuizSpawnMgr : MonoBehaviourPun
{
    int quizCount = 2;
    //public GameObject quiz;

    // 퀴즈 2개 배열
    public GameObject[] quizzes;
    public string[] quizzes_Names = { "RealQuiz_1", "RealQuiz_2" };

    public Vector3[] quiz_spawnSize;
    public Vector3[] quiz_spawnCenter;

    public GameObject [] quiz_correct;
    public Vector3 quiz_correctASize;

    private ClassMaterial classMaterial;
    private K_MapQuizSetUp quizSetup;


    void Start()
    {
        // 맴퀴즈셋업 가져와
        quizSetup = FindObjectOfType<K_MapQuizSetUp>();

        if(PhotonNetwork.IsMasterClient)
        {
            quiz_correct = new GameObject[quizCount];

            for (int i = 0; i < quizCount; i++)
            {
                //SpawnObj(quizzes[i], i);

                StartCoroutine(SpawnObj(quizzes_Names[i], i));

            }
        }
    }


    IEnumerator SpawnObj(string obj, int idx)
    {
        Vector3 center = quiz_spawnCenter[idx];
        Vector3 size = quiz_spawnSize[idx];
        Vector3 randomPos = GetRandomPosInArea(center, size);


        // Resources 폴더에서 quizName으로 프리팹을 로드하고 PhotonNetwork.Instantiate로 생성
        GameObject quizPrefab = Resources.Load<GameObject>(obj);

        

        if (quizPrefab != null)
        {
            GameObject quizInstance = PhotonNetwork.Instantiate(obj, randomPos, Quaternion.identity);

            yield return new WaitUntil(() => quizInstance != null);
            print("aaaa");
            

            ///////////////////////////////////
            //// 생성된 quizInstance에서 퀴즈 데이터 초기화
            //if (classMaterial != null && classMaterial.quizzes != null && classMaterial.quizzes.Count > idx)
            //{
            //    Quiz quizData = classMaterial.quizzes[idx];
            //    quizInstance.GetComponent<PhotonView>().RPC("InitializeQuiz", RpcTarget.AllBuffered, quizData.question, quizData.choices1, quizData.choices2, quizData.choices3, quizData.choices4, quizData.answer);
            //}


            // 생성된 quizInstance에서 자식 오브젝트인 quiz_correct를 찾음
            K_QuizPos k_QuizPos = quizInstance.GetComponent<K_QuizPos>();
            if (k_QuizPos != null)
            {
                quiz_correct[idx] = k_QuizPos.correct;
                //quizInstance.GetComponent<PhotonView>().RPC("InitializeQuiz", RpcTarget.AllBuffered);
                    
            }
            else
            {
                print("프리팹 있음");
            }
        }
        else
        {
            print("프리팹 없음");
        }    
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
