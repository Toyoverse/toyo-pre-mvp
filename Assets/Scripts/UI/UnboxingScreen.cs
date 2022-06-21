using System;
using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class UnboxingScreen : UIController
{
    public PlayableDirector playableDirector;
    public float timeoutButton = 10.0f;
    
    public void ConfirmButton() => ScreenManager.Instance.GoToScreen(ScreenState.OpenBox);

    protected override void UpdateUI()
    {
        TriggerUnboxingAnimation();
        StartCoroutine(ActivateButtonSafeCheck());
    }

    private IEnumerator ActivateButtonSafeCheck()
    {
        yield return new WaitForSeconds(timeoutButton);
        var _confirmButton = Root?.Q<Button>("confirmButton");
        if (_confirmButton != null && !_confirmButton.visible) _confirmButton.visible = true;
    }

    private void TriggerUnboxingAnimation()
    {
        playableDirector.Play();
    }

    private void Awake()
    {
        if (playableDirector == null)
            playableDirector = GetComponentInChildren<PlayableDirector>();
    }
    
    void OnEnable()
    {
        playableDirector.stopped += OnPlayableDirectorStopped;
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (playableDirector == aDirector)
            ScreenManager.Instance.GoToScreen(ScreenState.OpenBox);
    }

    void OnDisable()
    {
        playableDirector.stopped -= OnPlayableDirectorStopped;
    }
    
    //private void OnAnimationComplete() => continueButton.SetActive(true);
}
