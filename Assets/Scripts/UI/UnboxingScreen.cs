using System;
using System.Collections;
using Cinemachine;
using UI;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class UnboxingScreen : UIController
{
    public PlayableDirector playableDirector;
    public float timeoutButton = 10.0f;
    public Transform unboxingPivot;
    public Transform boxInfoPivot;

    private BoxConfig _boxConfig;

    public CinemachineVirtualCamera unboxingVirtualCamera;
    
    public void ConfirmButton() => ScreenManager.Instance.GoToScreen(ScreenState.OpenBox);

    protected override void UpdateUI()
    {
        _boxConfig = ToyoManager.GetSelectedBox().GetComponent<BoxConfig>();
        MoveSelectedBoxToUnboxingScreen();
        TriggerUnboxingAnimation();
        StartCoroutine(ActivateButtonSafeCheck());
    }

    private void MoveSelectedBoxToUnboxingScreen()
    {
        ToyoManager.GetSelectedBox().transform.SetParent(unboxingPivot);
        var _newPosition = ToyoManager.GetSelectedBox().transform.position - new Vector3(0, 1, 0);
        ToyoManager.GetSelectedBox().transform.position = _newPosition;
    }


    private void MoveSelectedBoxToBoxInfoScreen()
    {
        ToyoManager.GetSelectedBox().transform.SetParent(boxInfoPivot);
        var _newPosition = ToyoManager.GetSelectedBox().transform.position + new Vector3(0, 1, 0);
        ToyoManager.GetSelectedBox().transform.position = _newPosition;
    }
        

    private IEnumerator ActivateButtonSafeCheck()
    {
        yield return new WaitForSeconds(timeoutButton);
        var _confirmButton = Root?.Q<Button>("confirmButton");
        if (_confirmButton != null && !_confirmButton.visible) _confirmButton.visible = true;
    }

    private void TriggerUnboxingAnimation()
    {
        SwitchTo3D();
        unboxingVirtualCamera.LookAt = _boxConfig.model3D.transform;
        
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
        {
            ScreenManager.Instance.GoToScreen(ScreenState.OpenBox);
            MoveSelectedBoxToBoxInfoScreen();
            SwitchTo2D();
        }
            
    }
    
    void SwitchTo2D()
    {
        _boxConfig.model2D.SetActive(true);
        _boxConfig.model3D.SetActive(false);
    }

    void SwitchTo3D()
    {
        _boxConfig.model2D.SetActive(false);
        _boxConfig.model3D.SetActive(true);
    }

    void OnDisable()
    {
        playableDirector.stopped -= OnPlayableDirectorStopped;
    }
    
    //private void OnAnimationComplete() => continueButton.SetActive(true);
}
