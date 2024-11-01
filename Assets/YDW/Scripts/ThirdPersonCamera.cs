using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;
    private CinemachineVirtualCamera virtualCamera;

    // Start is called before the first frame update
    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        if (virtualCamera == null)
        {
            Debug.LogError("버츄얼카메라내놔");
        }

        //currentDistance = maxDistance;

        if (player != null)
        {
            virtualCamera.Follow = player;
            virtualCamera.LookAt = player;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return;
        }

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            virtualCamera.transform.position = player.position;
        }
    }

    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;

        if (virtualCamera != null)
        {
            virtualCamera.Follow = player;
            virtualCamera.LookAt = player;
        }
    }
}
