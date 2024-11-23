using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class Test : MonoBehaviour
{
    public VideoPlayer vp;
    // Start is called before the first frame update
    void Start()
    {
        string dataPath = Application.dataPath;
        string streamingAssetsPath = Application.streamingAssetsPath;
        string persistentDataPath = Application.persistentDataPath;
        print(dataPath + "\n" + streamingAssetsPath + "\n" + persistentDataPath);

        StartCoroutine(LoadVideo());
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
            print("���� ���� : " + videoData.Length);
            // videoData�� ����ϰų� �ӽ� ��η� ���� �� ���
            string tempPath = Application.persistentDataPath + "/Video.mp4";
            File.WriteAllBytes(tempPath, videoData);
            yield return null;
            
            tempPath = "file://" + tempPath;
            print("222 : " + tempPath + ", " + File.Exists(Application.persistentDataPath + "/Video.mp4"));
            vp.url = tempPath;
            vp.Play();

            //PlayVideo(tempPath); // �ӽ� ��η� ���� �÷���
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
