using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private float alpha = 0f;
    private float fadeSpeed = 0.5f;
    private SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        alpha += fadeSpeed * Time.deltaTime;
        spriteRenderer.color = new Color(1f, 1f, 1f, alpha);

        if (alpha >= 1f || alpha <= 0f)
        {
            fadeSpeed = -fadeSpeed;
        }
    }
}
