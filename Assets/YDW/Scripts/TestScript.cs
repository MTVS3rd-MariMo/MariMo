using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class TestScript : MonoBehaviour
{
    public VideoPlayer vp;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadVideo());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadVideo()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Video.mp4");
        print("111 : " + filePath);
        if (!filePath.Contains("://"))
        {
            filePath = "file://" + filePath;
        }


        UnityWebRequest request = UnityWebRequest.Get(filePath);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            byte[] videoData = request.downloadHandler.data;
            print("파일 존재 : " + videoData.Length);
            // videoData를 사용하거나 임시 경로로 저장 후 사용
            string tempPath = Application.persistentDataPath + "/Video.mp4";
            File.WriteAllBytes(tempPath, videoData);
            yield return null;

            tempPath = "file://" + tempPath;
            print("222 : " + tempPath + ", " + File.Exists(Application.persistentDataPath + "/Video.mp4"));
            vp.url = tempPath;
            vp.Play();

            //PlayVideo(tempPath); // 임시 경로로 비디오 플레이
        }

    }
}
