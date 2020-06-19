using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    private const float V = 0.02f;
    float fadeSpeed = V;
    float red, green, blue, alfa;

    public bool isFadeOut = false;
    public bool isFadeIn = false;

    Image fadeImage;
    // Start is called before the first frame update
    void Start()
    {
        fadeImage = GetComponent<Image>();
        red = fadeImage.color.r;
        green = fadeImage.color.g;
        blue = fadeImage.color.b;
        alfa = fadeImage.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFadeIn)
        {
            StartFadeIn();
        }

        if (isFadeOut)
        {
            StartFadeOut();
        }
    }

    void StartFadeIn()
    {
        alfa -= fadeSpeed;                
        SetAlpha();                      
        if (alfa <= 0)
        {                    
            isFadeIn = false;
            fadeImage.enabled = false;    
        }
    }

    void StartFadeOut()
    {
        fadeImage.enabled = true;  
        alfa += fadeSpeed;         
        SetAlpha();               
        if (alfa >= 1)
        {             
            isFadeOut = false;
            isFadeIn = true;
        }
    }

    void SetAlpha()
    {
        fadeImage.color = new Color(red, green, blue, alfa);
    }
}
