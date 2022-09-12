using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum FadeMode
{
    Scene = 0,
    UI = 1000
}

public enum AnimationMode
{
    Hard,
    Fade
}

public class FadeController : Singleton<FadeController>
{
    private static UnityEvent<FadeMode, AnimationMode, onFinishAnimation> InEvent = new FadeEvent();
    private static UnityEvent<FadeMode, AnimationMode, onFinishAnimation> OutEvent = new FadeEvent();

    public static bool InFade = false;

    public delegate void onFinishAnimation();
    public Canvas canvas;
    public Image panel;
    
    private static void ResetCanvasOrder() => Instance.canvasOrder(0);

    private void OnEnable()
    {
        InEvent.AddListener(InHandler);
        OutEvent.AddListener(OutHandler);
    }

    private void OnDisable()
    {
        InEvent.RemoveListener(InHandler);
        OutEvent.RemoveListener(OutHandler);
    }

    public static void In(FadeMode fadeMode = FadeMode.UI, AnimationMode animationMode = AnimationMode.Fade, onFinishAnimation onFinished = null) 
    {
        InFade = true;
        InEvent.Invoke(fadeMode, animationMode, onFinished);
    }
    private void InHandler(FadeMode fadeMode, AnimationMode animationMode, onFinishAnimation onFinished)
    {
        TweenCallback _onComplete = () => onFinished();

        canvasOrder((int)fadeMode);

        switch (animationMode)
        {
            case AnimationMode.Fade:
                fade(1, onFinished);
                break;
            case AnimationMode.Hard:
                panel.DOFade(1, 0)
                    .SetAutoKill(true)
                    .onComplete = _onComplete;
                break;
        }
    }

    public static void Out(FadeMode fadeMode = FadeMode.UI, AnimationMode animationMode = AnimationMode.Fade,
        onFinishAnimation onFinished = null)
    {
        InFade = false;
        onFinished ??= ResetCanvasOrder;
        OutEvent.Invoke(fadeMode, animationMode, onFinished);
    }

    private void OutHandler(FadeMode fadeMode, AnimationMode animationMode, onFinishAnimation onFinished)
    {
        TweenCallback _onComplete = () => onFinished();

        canvasOrder((int)fadeMode);

        switch (animationMode)
        {
            case AnimationMode.Fade:
                fade(0, onFinished);
                break;
            case AnimationMode.Hard:
                panel.DOFade(0, 0)
                    .SetAutoKill(true)
                    .onComplete = _onComplete;
                break;
        }
    }

    private void fade(float endValue, onFinishAnimation onFinished)
    {
        TweenCallback _onComplete = () => onFinished();
        
        panel.DOFade(endValue, 0.7f)
                 .SetAutoKill(true)
                 .Play()
                 .SetUpdate(true)
                 .onComplete = _onComplete;
    }   

    private void canvasOrder(int newOrder) => canvas.sortingOrder = newOrder;

}

public class FadeEvent : UnityEvent<FadeMode, AnimationMode, FadeController.onFinishAnimation> { }

/*
yield return new WaitUntil(()=> callback);
FadeController.Out(FadeMode.UI, AnimationMode.Fade, FadeCallBack);
yield return new WaitUntil(() => !isPlayingCredits);
FadeController.In(FadeMode.UI, AnimationMode.Fade, FadeCallBack);
yield return new WaitUntil(() => callback);
isPlayingCredits = false;
FadeController.Out(FadeMode.UI, AnimationMode.Fade, FadeCallBack);
*/