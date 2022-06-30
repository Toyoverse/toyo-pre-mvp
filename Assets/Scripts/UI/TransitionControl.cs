using System;
using UnityEngine;
using UnityEngine.UI;

public class TransitionControl : Singleton<TransitionControl>
{
    public Animator animator;
    public event Action OnCompletelyHiddenScreen;
    public Image image;

    public void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (image == null)
            image = GetComponent<Image>();
        DisableTransition();
    }

    public void PlayTransition()
    {
        EnableTransition();
        animator.Play("BaseLayer.Transition", 0, 0);
    }
    
    public void OnCompletelyHiddenScreenFunction() => OnCompletelyHiddenScreen?.Invoke();

    public void OnEndTransitionFunction() => DisableTransition();

    private void EnableTransition()
    {
        animator.enabled = true;
        image.enabled = true;
    }
    
    private void DisableTransition()
    {
        ClearTransitionEvents();
        animator.enabled = false;
        image.enabled = false;
    }

    private void ClearTransitionEvents() => OnCompletelyHiddenScreen = null;
    
    private void OnDestroy() => ClearTransitionEvents();
}
