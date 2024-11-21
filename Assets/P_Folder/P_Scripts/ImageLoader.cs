using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class ImageLoader : MonoBehaviour
{
    // 이미지 표시할 UI 컴포넌트
    public RawImage rawImage;
    public Image uiImage;

    // URL로부터 이미지를 불러와서 적용
    public void LoadImageFromUrl(string url)
    {
        StartCoroutine(DownloadImage(url));
    }

    // 이미지를 다운로드하는 코루틴
    private IEnumerator DownloadImage(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("이미지 다운로드 실패: " + request.error);
                yield break;
            }

            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;

            // RawImage에 적용
            if (rawImage != null)
            {
                rawImage.texture = texture;
                //rawImage.SetNativeSize(); // 이미지 크기 자동 조정
            }

            // Image 컴포넌트에 적용
            if (uiImage != null)
            {
                uiImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                //uiImage.SetNativeSize(); // 이미지 크기 자동 조정
            }
        }
    }
}
