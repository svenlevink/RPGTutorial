using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Fader : MonoBehaviour
{
    Image image;
    
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    //fades the image to the full alpha value (dus volledig zwart, daar staat die 1f voor)
    public IEnumerator FadeIn(float time)
    {
        yield return image.DOFade(1f, time).WaitForCompletion();
    }

    //fade de image to 0 alpha value (de 0f) dus helemaal ontzichtbaar
    public IEnumerator FadeOut(float time)
    {
        yield return image.DOFade(0f, time).WaitForCompletion();
    }
}
