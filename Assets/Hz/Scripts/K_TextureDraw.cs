using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_TextureDraw : MonoBehaviour
{
    Texture2D tex;

    void Start()
    {
        // 그릴 수 있는 너비 높이
        tex = new Texture2D(128, 128);
        GetComponent<Renderer>().material.mainTexture = tex;
    }

    void Update()
    {
        if (tex != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    Vector2 uv;
                    uv.x = (hit.point.x - hit.collider.bounds.min.x) / hit.collider.bounds.size.x;
                    uv.y = (hit.point.y - hit.collider.bounds.min.y) / hit.collider.bounds.size.y;

                    tex.SetPixel((int)(-uv.x * tex.width), (int)(uv.y * tex.height), Color.red);
                    tex.SetPixel((int)(-uv.x * tex.width), (int)(uv.y * tex.height) + 1, Color.red);
                    tex.SetPixel((int)(-uv.x * tex.width) + 1, (int)(uv.y * tex.height), Color.red);
                    tex.SetPixel((int)(-uv.x * tex.width), (int)(uv.y * tex.height) - 1, Color.red);
                    tex.SetPixel((int)(-uv.x * tex.width) - 1, (int)(uv.y * tex.height), Color.red);
                    tex.SetPixel((int)(-uv.x * tex.width) + 1, (int)(uv.y * tex.height) - 1, Color.red);
                    tex.SetPixel((int)(-uv.x * tex.width) - 1, (int)(uv.y * tex.height) + 1, Color.red);
                    tex.SetPixel((int)(-uv.x * tex.width) + 1, (int)(uv.y * tex.height) - 1, Color.red);

                }
            }
        }
    }
}
