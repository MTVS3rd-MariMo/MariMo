using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_PaintController : MonoBehaviour
{
    public GameObject img_drawGuide;

  
    void Update()
    {
        OnDrawGuide();
    }

    void OnDrawGuide()
    {
        StartCoroutine(OffDrawGuide(2f));
    }

    private IEnumerator OffDrawGuide(float delay)
    {
        yield return new WaitForSeconds(delay);

        img_drawGuide.SetActive(false);
    }
}
