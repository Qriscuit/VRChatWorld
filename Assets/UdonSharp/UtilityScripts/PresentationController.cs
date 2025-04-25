
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class PresentationController : UdonSharpBehaviour
{
    [SerializeField] GameObject PresentationScreen;
    private RawImage screenImage;
    [SerializeField] Texture2D[] _TestSlides;

    [UdonSynced, FieldChangeCallback(nameof(SlideIndex))]
    [SerializeField] int slideIndex = 0;

    float SwitchTime = 2;
    float TimeElapsed;

    int SlideIndex
    {
        set{ 
            slideIndex = value;
            NextSlide();
        }

        get{ 
            return slideIndex;
        }
    }

    void Start()
    {
        if(PresentationScreen)
        {
            screenImage = PresentationScreen.GetComponent<RawImage>();
        }
        
        TimeElapsed = 0;
        NextSlide();
    }

    private void Update()
    {
        TimeElapsed += Time.deltaTime;

        if (TimeElapsed > SwitchTime && Networking.IsOwner(gameObject)) {
            TimeElapsed = 0;
            SlideIndex = (SlideIndex + 1) % _TestSlides.Length ;
        }
    }

    private void NextSlide()
    {
        screenImage.texture = _TestSlides[SlideIndex]; 
    }
}
