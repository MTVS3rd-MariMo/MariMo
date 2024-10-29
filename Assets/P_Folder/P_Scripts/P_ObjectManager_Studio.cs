using Cinemachine;
using Org.BouncyCastle.Asn1.Crmf;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class P_ObjectManager_Studio : MonoBehaviourPun
{
    public float testNum = 4;

    List<GameObject> players = new List<GameObject>();

    //public List<Transform> objectList = new List<Transform>();
    float triggerNum = 0;

    // UI들
    public GameObject studioUI_Panel;
    public Image studioUI1_Img;
    public Image studioUI2_Img;
    public TMP_Text timeCount_Text;


    // 투명벽 (플레이어 움직임을 멈춘다면 필요없을 예정)
    public GameObject wall;

    // 연출용
    public PlayableDirector timeline;
    public CinemachineVirtualCamera virtualCamera1;
    public CinemachineVirtualCamera virtualCamera2;
    public CinemachineVirtualCamera virtualCamera3;

    // 타임라인 실행을 한번만 하기위한 체크
    bool act = false;

    // 연출용 흑백화면
    public Image blackScreen;
    public Image whiteScreen;


    void Start()
    {

    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerNum++;
            print("+++++++++ : " + triggerNum);

            players.Add(other.gameObject);

            if (triggerNum >= testNum && !act)
            {
                act = true;
                wall.SetActive(true);

                if (photonView.IsMine)
                {
                    RPC_MoveControl(false);

                    RPC_Studio();
                }
            }
        }
    }

    
    void RPC_Studio()
    {
        photonView.RPC(nameof(Studio), RpcTarget.All);
    }

    [PunRPC]
    public void Studio()
    {
        StartCoroutine(Studio_UI_Player());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !act)
        {
            triggerNum--;
            print("-------- : " + triggerNum);

            players.Remove(other.gameObject);
        }
    }

    void RPC_MoveControl(bool canmove)
    {
        photonView.RPC(nameof(MoveControl), RpcTarget.All, canmove);
    }

    [PunRPC]
    void MoveControl(bool canmove)
    {
        foreach (GameObject obj in players)
        {
            obj.GetComponent<Y_PlayerMove>().movable = canmove;
        }
    }

    public IEnumerator Studio_UI_Player()
    {
        //Moving(false);

        // 타임라인 재생
        timeline.Play();

        yield return new WaitForSeconds(0.5f);

        virtualCamera1.gameObject.SetActive(true);

        yield return new WaitForSeconds(3.5f);

        virtualCamera2.gameObject.SetActive(true);


        // 페이드 아웃
        blackScreen.gameObject.SetActive(true);
        Color black = blackScreen.color;

        while(black.a <=1)
        {
            black.a += Time.deltaTime / 1.5f;

            blackScreen.color = black;

            yield return null;
        }

        yield return new WaitForSeconds(0.75f);

        // UI패널
        studioUI_Panel.SetActive(true);

        // 키박스 꺼주기
        K_LobbyUiManager.instance.img_KeyEmptyBox.gameObject.SetActive(false);

        virtualCamera3.gameObject.SetActive(true);

        // 타임라인 일시정지
        timeline.Pause();


        Dictionary<int, PhotonView> allPlayers = Y_BookController.Instance.allPlayers;

        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].gameObject.transform.localScale = allPlayers[i].gameObject.GetComponent<Y_PlayerAvatarSetting>().studioScale;

            // 플레이어 위치 이동
            allPlayers[i].transform.position = new Vector3(virtualCamera3.transform.position.x, allPlayers[i].transform.position.y, virtualCamera3.transform.position.z);
        }


        yield return new WaitForSeconds(1.5f);

        while (black.a >= 0)
        {
            black.a -= Time.deltaTime / 1.5f;

            blackScreen.color = black;

            yield return null;
        }

        // 사진관용 플레이어 조작 활성화
        MoveControl(true);

        // UI생성
        Color color1 = studioUI1_Img.color;

        while (color1.a <= 1)
        {
            color1.a += Time.deltaTime;

            studioUI1_Img.color = color1;

            yield return null;
        }

        yield return new WaitForSeconds(3f);

        while (color1.a >= 0)
        {
            color1.a -= Time.deltaTime;

            studioUI1_Img.color = color1;

            yield return null;
        }


        while (color1.a <= 1)
        {
            color1.a += Time.deltaTime;

            studioUI2_Img.color = color1;

            yield return null;
        }

        yield return new WaitForSeconds(2f);

        while (color1.a >= 0)
        {
            color1.a -= Time.deltaTime;

            studioUI2_Img.color = color1;

            yield return null;
        }
        
        timeCount_Text.gameObject.SetActive(true);

        Color tcolor = timeCount_Text.color;

        float timeCount = 5f;

        while (timeCount > 0)
        {
            tcolor.a = 1;

            timeCount_Text.color = tcolor;

            timeCount_Text.text = timeCount.ToString();

            timeCount--;

            while (tcolor.a >= 0)
            {
                tcolor.a -= Time.deltaTime;

                timeCount_Text.color = tcolor;

                yield return null;
            }
        }

        // 플레이어 움직임 멈춤
        MoveControl(false);

        whiteScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        whiteScreen.gameObject.SetActive(false);

        
        TakePicture();

        // 페이드 아웃
        while (black.a <= 1)
        {
            black.a += Time.deltaTime / 1.5f;

            blackScreen.color = black;

            yield return null;
        }

        // 타임라인 재생
        timeline.Play();

        // 플레이어 위치 이동
        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].transform.position = new Vector3(transform.position.x, allPlayers[i].transform.position.y, transform.position.z);
        }

        virtualCamera1.gameObject.SetActive(false);
        virtualCamera2.gameObject.SetActive(false);
        virtualCamera3.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        // 페이드 인
        while (black.a >= 0)
        {
            black.a -= Time.deltaTime / 1.5f;

            blackScreen.color = black;

            yield return null;
        }

        // 사진관 모든 UI 종료
        studioUI_Panel.SetActive(false);
        blackScreen.gameObject.SetActive(false);
        wall.SetActive(false);

        RPC_MoveControl(true);
    }


    void TakePicture()
    {
        string timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd");
        string fileName = "SCREENSHOT-" + timeStamp + ".png";

        // 플랫폼별 분기 
        //#if UNITY_IPONE || UNITY_ANDROID
        //        CaptureScreenForMoblie(fileName);
        //#else
        //        CaptureScreenForPC(fileName);
        //#endif

        // 플랫폼별 분기 사용 안할시
        // 파일 이름은 임시로 test.png 
        CaptureScreenForPC("test.png");
    }

    private void CaptureScreenForPC(string fileName)
    {
        // 다운로드 폴더에 저장 ( 실패 )
        //ScreenCapture.CaptureScreenshot("~/Downloads/" + fileName);

        // 경로 미지정시 프로젝트 파일에 저장
        ScreenCapture.CaptureScreenshot(fileName);
    }

    private void CaptureScreenForMobile(string fileName)
    {
        // 모바일로 사용시 추가 경로지정 필요
        ScreenCapture.CaptureScreenshot(fileName);
    }
}
