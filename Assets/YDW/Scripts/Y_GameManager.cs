using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_GameManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(SpawnPlayer());
    }

    IEnumerator SpawnPlayer()
    {
        // 룸에 입장이 완료될 때까지 기다린다.
        yield return new WaitUntil(() => { return PhotonNetwork.InRoom; });

        Vector2 randomPos = Random.insideUnitCircle * 5.0f;
        Vector3 initPosition = new Vector3(randomPos.x, 1.0f, randomPos.y);

        PhotonNetwork.Instantiate("Player", initPosition, Quaternion.identity);
    }

    void Update()
    {

    }
}
