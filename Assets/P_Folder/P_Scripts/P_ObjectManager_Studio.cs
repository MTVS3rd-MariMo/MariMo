using Cinemachine;
using Org.BouncyCastle.Asn1.Crmf;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class P_ObjectManager_Studio : MonoBehaviourPun
{
    public float testNum = 4;

    List<GameObject> players = new List<GameObject>();

    float triggerNum = 0;

    // UI들
    public GameObject studioUI_Panel;
    public Image studioUI1_Img;
    public Image studioUI2_Img;
    public TMP_Text timeCount_Text;
    public Image film_Img;
    public GameObject finishUI_Panel;
    public Button btn_Finish;


    // 사진배경용
    public GameObject backgrond;
    public Material backgrondMaterial;

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

    // 바닥 그림
    public GameObject draw_Photo;
    // 오브젝트 애니메이션
    public GameObject Ani_Object;

    public Y_BookController bookController;

    private void Start()
    {
        btn_Finish.onClick.AddListener(OnclickFinish);

        bookController = GameObject.Find("BookCanvas").GetComponent<Y_BookController>();


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
        draw_Photo.SetActive(false);
        Ani_Object.SetActive(true);
        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_3D_OBJECT_05);

        SetBackgroundFromURL(Y_HttpRoomSetUp.GetInstance().realClassMaterial.backgroundUrl);
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

            obj.transform.position = new Vector3(obj.transform.position.x, 3, obj.transform.position.z);
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

        //// 책갈피 UI 꺼주기
        int j = 0;
        foreach(GameObject KeyImage in K_KeyUiManager.instance.keyImages)
        {
            KeyImage.SetActive(false);
            j++;
        }



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

        // 오브젝트 회전 돌리기
        this.transform.rotation = new Quaternion(0, 0, 0, 0);

        Dictionary<int, PhotonView> allPlayers = bookController.allPlayers;

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
        RPC_MoveControl(true);

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
        RPC_MoveControl(false);

        whiteScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        whiteScreen.gameObject.SetActive(false);

        Y_SoundManager.instance.PlayEftSound(Y_SoundManager.ESoundType.EFT_CAMERA);

        Debug.Log("사진찍기 호출");

        if (PhotonNetwork.IsMasterClient)
        {
            TakePicture();
        }

        // 사진 틀 이미지 띄우기
        film_Img.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        film_Img.gameObject.SetActive(false);

        blackScreen.gameObject.SetActive(false);

        finishUI_Panel.SetActive(true);


        ////////////////////////////////////////////////////////////// 월드로 돌아가지 않고 포톤 방 나가기
        //// 페이드 아웃
        //while (black.a <= 1)
        //{
        //    black.a += Time.deltaTime / 1.5f;

        //    blackScreen.color = black;

        //    yield return null;
        //}

        //film_Img.gameObject.SetActive(false);

        //// 타임라인 재생
        //timeline.Play();

        //// 플레이어 위치 이동
        //for (int i = 0; i < allPlayers.Count; i++)
        //{
        //    allPlayers[i].transform.position = new Vector3(transform.position.x, allPlayers[i].transform.position.y, transform.position.z);
        //}

        //virtualCamera1.gameObject.SetActive(false);
        //virtualCamera2.gameObject.SetActive(false);
        //virtualCamera3.gameObject.SetActive(false);

        //yield return new WaitForSeconds(2f);

        //// 페이드 인
        //while (black.a >= 0)
        //{
        //    black.a -= Time.deltaTime / 1.5f;

        //    blackScreen.color = black;

        //    yield return null;
        //}

        //// 사진관 모든 UI 종료
        //studioUI_Panel.SetActive(false);
        //blackScreen.gameObject.SetActive(false);

        //RPC_MoveControl(true);
    }

    void OnclickFinish()
    {
        Y_HttpLogIn.GetInstance().GetComponent<ConnectionManager>().FinishLesson();
    }


    void TakePicture()
    {
        string timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd");
        string fileName = "SCREENSHOT-" + timeStamp + ".png";

        Debug.Log("플랫폼 분기 확인");
        // 플랫폼별 분기 
        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("모바일 생성");
            CaptureScreenForMobile(fileName);
            return;
        }
        else
        {
            Debug.Log("PC 생성");
            CaptureScreenForPC(fileName);
            return;
        }
    }

    private void CaptureScreenForPC(string fileName)
    {
        string path = System.IO.Path.Combine(Application.dataPath, fileName);

        Debug.Log("PC 사진찍기");
        StartCoroutine(CaptureAndSend(path));
    }

    private void CaptureScreenForMobile(string fileName)
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, fileName);

        Debug.Log("모바일 사진찍기");
        StartCoroutine(CaptureAndSend(path));
    }

    public IEnumerator CaptureAndSend(string filePath)
    {
        // 스크린샷 캡처
        ScreenCapture.CaptureScreenshot(filePath);

        yield return new WaitForSeconds(0.5f);

        Debug.Log("파일경로 존재 확인");

        if (System.IO.File.Exists(filePath))
        {
            Debug.Log($"스크린샷 저장 경로: {filePath}");

            SendCapture(filePath);
        }
        else
        {
            Debug.LogError($"스크린샷 파일을 찾을 수 없습니다: {filePath}");
        }

    }

    public void SendCapture(string filePath)
    {
        HttpInfo info = new HttpInfo();
        info.url = Y_HttpLogIn.GetInstance().mainServer + "api/photo/" + Y_HttpRoomSetUp.GetInstance().userlessonId;
        info.body = filePath;
        info.contentType = "multipart/form-data";
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            // 완료시 실행
            Debug.Log("사진전송 완료");

            // 스크린샷 삭제
            System.IO.File.Delete(filePath);
        };

        StartCoroutine(HttpManager.GetInstance().PutPicture(info));
    }

    // 배경 이미지 다운로드
    public void SetBackgroundFromURL(string url)
    {
        StartCoroutine(DownloadImage(url));
    }

    private IEnumerator DownloadImage(string url)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download image: {uwr.error}");
            }
            else
            {
                // Texture2D 생성
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

                // StudioSet 메서드 호출하여 머터리얼에 적용
                StudioSet(texture);
            }
        }
    }

    public void StudioSet(Texture2D texture)
    {
        backgrondMaterial.mainTexture = texture;

        backgrond.GetComponent<MeshRenderer>().material = backgrondMaterial;
    }
}
