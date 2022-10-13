using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using UI;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class TrainingModuleScreen : UIController
{
    public string eventTitleName;
    public string eventTimeName;
    public string actionTitleName;

    public string startTrainingButtonName;
    public GameObject[] combPoolObjects;
    public GameObject[] removeButtonsPool;
    public Image[] combPoolImages;
    public Image[] progressImages;
    public Sprite progressDefaultSprite;
    public Transform combPoolContainer;

    [Header("In Training names")] public string inTrainingBoxName;
    public string inTrainingTimeButtonName;

    [Header("Reward names")] public string rewardTitleName;
    public string receiveName;
    public string durationName;

    [Header("Actions Selection names")] public string actionSelectionAreaName;
    public string actionScrollName;
    public string previewActionName;
    public FontAsset fontAsset;
    
    private bool _inSecondTimeCheck = false;

    protected override void UpdateUI()
    {
        TrainingConfig.Instance.ApplyRewardsCalculation();
        UpdateTextsInUI();
        EnableTrainingUI();
        UpdateProgressTraining();
        CheckAndRevealStartButton();
        ChangeTrainingTitleName();
    }

    private void UpdateTextsInUI()
    {
        SetTextInLabel(eventTitleName, TrainingConfig.Instance.eventTitle);
        UpdateEventTimeRemainUI();
        TrainingConfig.Instance.ApplyBlowConfig();
        SetTextInLabel(receiveName, /*"Receive: $" + */TrainingConfig.Instance.receiveValue.ToString());
        SetTextInLabel(durationName, /*"Duration: " + */ConvertMinutesToString(TrainingConfig.Instance.durationValue));
    }

    private void UpdateEventTimeRemainUI()
    => SetTextInLabel(eventTimeName, TrainingConfig.EventTimeDefaultText 
                                     + ConvertMinutesToString(TrainingConfig.Instance.GetEventTimeRemainInMinutes()));

    private void ChangeUpdateTrainingTimeToSecond()
    {
        CancelInvoke(nameof(UpdateTrainingTimeRemainUI));
        InvokeRepeating(nameof(UpdateTrainingTimeRemainUI), 0, 1.0f);
        _inSecondTimeCheck = true;
    }
    
    private void UpdateTrainingTimeRemainUI()
    {
        if (!TrainingConfig.Instance.SelectedToyoIsInTraining())
            return;
        var _trainSecRemain = (int)TrainingConfig.Instance.GetTrainingTimeRemainInSeconds();
        if (_trainSecRemain < 60)
        {
            SetTextInButton(inTrainingTimeButtonName, _trainSecRemain + "s");
            if (!_inSecondTimeCheck)
                ChangeUpdateTrainingTimeToSecond();
        }
        else
        {
            SetTextInButton(inTrainingTimeButtonName, ConvertMinutesToString(TrainingConfig.Instance.GetTrainingTimeRemainInMinutes()));
            UpdateProgressTraining();
        }
    }

    public override void ActiveScreen()
    {
        base.ActiveScreen();
        ResetCombinationPool();
        CheckIfIsToyoOrAutomata();
        ApplyInTrainingActions();
        UpdateUI();
        InvokeRepeating(nameof(UpdateTrainingTimeRemainUI), 0, 60);
        InvokeRepeating(nameof(UpdateEventTimeRemainUI), 0, 60);
    }

    public override void DisableScreen()
    {
        CancelInvoke(nameof(UpdateTrainingTimeRemainUI));
        CancelInvoke(nameof(UpdateEventTimeRemainUI));
        _inSecondTimeCheck = false;
        CheckToResetTrainingModule();
        base.DisableScreen();
    }

    private void CheckToResetTrainingModule()
    {
        if (ScreenManager.ScreenState == ScreenState.TrainingActionSelect)
            return;
        TrainingConfig.Instance?.ResetAllTrainingModule();
        ClearActionsImages();
    }

    public void OpenActionSelection(int buttonID)
    {
        TrainingConfig.Instance.SetSelectedID(buttonID);
        OpenActionSelectionScreen();
    }

    public void RemoveAction(int id)
    {
        TrainingConfig.Instance.SetSelectedID(id);
        DisableRemoveButton(id);
        SetActionSprite(id, null);
        SetProgressSprite(id, progressDefaultSprite);
        TrainingConfig.Instance.RemoveActionFromBKP(id);
        DisableActionObject(id);
        TrainingConfig.Instance.ApplyBlowConfig();
        RevealNextAction();
        UpdateUI();
    }

    private void ConfirmInTrainingAction(TrainingActionSO actionSo)
    {
        var _selectedID = TrainingConfig.Instance.selectedActionID;
        combPoolObjects[_selectedID].gameObject.SetActive(true);
        DisableRemoveButton(_selectedID);
        SetActionsSprites(_selectedID, actionSo.sprite);
        TrainingConfig.Instance.AddToSelectedActionsDict(_selectedID, actionSo);
        TrainingConfig.Instance.ApplyBlowConfig();
    }

    public void SetActionsSprites(int id, Sprite sprite)
    {
        SetActionSprite(id, sprite);
        SetProgressSprite(id, sprite);
    }

    private void SetActionSprite(int id, Sprite sprite) => combPoolImages[id].sprite = sprite;
    private void SetProgressSprite(int id, Sprite sprite) => progressImages[id].sprite = sprite;

    private void SetActionToLastPosition(GameObject actionObj)
    {
        actionObj.transform.SetParent(null);
        actionObj.transform.SetParent(combPoolContainer);
    }

    private void CheckAndRevealStartButton()
    {
        var _startButton = Root.Q<Button>(startTrainingButtonName);
        if (_startButton == null) return;
        _startButton.visible = (!TrainingConfig.Instance.SelectedToyoIsInTraining() && TrainingConfig.Instance.IsMinimumActionsToPlay());
    }

    private void ChangeTrainingTitleName()
    {
        var _text = "";
        if (TrainingConfig.Instance.SelectedToyoIsInTraining())
            _text = TrainingConfig.Instance.inTrainingMessage;
        else
            _text = TrainingConfig.Instance.selectActionsMessage;
        SetTextInLabel(actionTitleName, _text);
    }

    public void RevealNextAction()
    {
        if (GetActionsRevealCount() > TrainingConfig.Instance.selectedActionsBkp.Count)
            return;
        for (var _i = 0; _i < combPoolObjects.Length; _i++)
        {
            if (combPoolObjects[_i].gameObject.activeInHierarchy) 
                continue;
            SetActionToLastPosition(combPoolObjects[_i].gameObject);
            combPoolObjects[_i].gameObject.SetActive(true);
            break;
        }
    }

    private int GetActionsRevealCount()
        =>  combPoolObjects.Count(obj => obj.activeInHierarchy);

    private void DisableActionObject(int id) => combPoolObjects[id].SetActive(false);

    private void ResetCombinationPool() 
    {
        if (ScreenManager.OldScreenState == ScreenState.TrainingActionSelect)
            return;
        DisableAllRemoveButtons();
        foreach (var _obj in combPoolObjects)
            _obj.gameObject.SetActive(false);
        combPoolObjects[0].SetActive(true);
    }

    private void ApplyInTrainingActions()
    {
        DisableAllProgressImages();
        if (!TrainingConfig.Instance.SelectedToyoIsInTraining())
            return;
        for (var _i = 0; _i < TrainingConfig.Instance.selectedTrainingActions.Count; _i++)
        {
            TrainingConfig.Instance.SetSelectedID(_i);
            ConfirmInTrainingAction(TrainingConfig.Instance.selectedTrainingActions[_i]);
        }
        EnableAllProgressImages();
    }

    private void OpenActionSelectionScreen() => ScreenManager.Instance.GoToScreen(ScreenState.TrainingActionSelect);

    public void TrainingInfoButton() => ScreenManager.Instance.GoToScreen(ScreenState.LoreTheme);

    public override void BackButton() => ScreenManager.Instance.GoToScreen(ScreenState.MainMenu);

    public void StartButton()
        => GenericPopUp.Instance.ShowPopUp(TrainingConfig.Instance.sendToyoToTrainingPopUp,
            CallSendToyoToTrainingFlux, () => {});

    private void CallSendToyoToTrainingFlux()
    {
        Loading.StartLoading?.Invoke();
        if (ToyoManager.GetSelectedToyo().isAutomata)
        {
            SendToyoToTrainingOnServer();
            return;
        }
        DatabaseConnection.Instance.CallGetPlayerToyo(PlayerToyoCallback);
    }

    private void PlayerToyoCallback(string json)
        => DatabaseConnection.Instance.blockchainIntegration.UpdateToyoIsStakedList(json, ConfirmSendTraining);

    private void ConfirmSendTraining()
    {
        if(ToyoManager.GetSelectedToyo().isStaked)
            SendToyoToTrainingOnServer();
        else
            DatabaseConnection.Instance.blockchainIntegration.ToyoApproveNftTransfer(ToyoManager.GetSelectedToyo().tokenId);
    }
    
    public void SendToyoToTrainingOnServer() => TrainingConfig.Instance.SetInTrainingOnServer();

    public void UpdateTrainingAfterTrainingSuccess()
    {
        EnableAllProgressImages();
        DisableLastEmptyAction();
        UpdateUI();
        Loading.EndLoading?.Invoke();
    }
    
    private void DisableLastEmptyAction()
    {
        if (GetActionsRevealCount() <= TrainingConfig.Instance.selectedActionsBkp.Count)
            return;
        GetLastActionRevealed()?.SetActive(false);
    }

    private GameObject GetLastActionRevealed()
    {
        for (var _i = 0; _i < combPoolObjects.Length; _i++) 
        {
            if (combPoolObjects[_i].activeInHierarchy && !IsButtonIdExistInSelectedActions(_i))
                return combPoolObjects[_i];
        }

        return null;
    }

    private bool IsButtonIdExistInSelectedActions(int id) 
        =>  TrainingConfig.Instance.selectedActionsBkp.Any(_actionBkp => _actionBkp.buttonID == id);

    private void EnableAllProgressImages() => AllProgressImagesSetActive(true);
    private void DisableAllProgressImages() => AllProgressImagesSetActive(false);
    
    private void AllProgressImagesSetActive(bool isOn)
    {
        foreach (var _image in progressImages)
            _image.gameObject.SetActive(isOn);
    }

    public void FinishTraining()
    {
        if (TrainingConfig.Instance.GetTrainingTimeRemainInSeconds() > 0 && !TrainingConfig.Instance.ignoreTrainingTimer)
            return;
        GoToRewardScreen();
    }

    private void EnableTrainingUI() 
    {
        DisableVisualElement(inTrainingBoxName);
        EnableInteractableActionButtons();

        if (!TrainingConfig.Instance.SelectedToyoIsInTraining()) 
            return;
        
        EnableVisualElement(inTrainingBoxName);
        SetTextInButton(inTrainingTimeButtonName, ConvertMinutesToString(TrainingConfig.Instance.GetTrainingTimeRemainInMinutes()));
        DisableAllRemoveButtons(); 
        DisableInteractableActionButtons();
    }

    private void EnableInteractableActionButtons() => ActionButtonsInteractable(true);
    private void DisableInteractableActionButtons() => ActionButtonsInteractable(false);

    private void ActionButtonsInteractable(bool on)
    {
        for(var _i = 0; _i < combPoolObjects.Length; _i++)
            combPoolObjects[_i].GetComponentInChildren<UnityEngine.UI.Button>().interactable = on;
    }

    private void GoToRewardScreen() => ScreenManager.Instance.GoToScreen(ScreenState.TrainingModuleRewards);
    
    private void ClearActionsImages()
    {
        for (var _i = 0; _i < combPoolImages.Length; _i++)
            combPoolImages[_i].sprite = null;
    }
    
    private void DisableRemoveButton(int id) => removeButtonsPool[id].SetActive(false);
    public void RevealRemoveButton(int id) => removeButtonsPool[id].SetActive(true);

    private void DisableAllRemoveButtons()
    {
        foreach (var _gameObject in removeButtonsPool)
            _gameObject.SetActive(false);
    }

    private BlowConfig GetBlowConfigByTrainingInfo(ToyoTrainingInfo trainingInfo)
    {
        var _blowConfig = new BlowConfig
        {
            duration = TrainingConfig.Instance.GetTrainingTotalDuration(trainingInfo),
            qty = trainingInfo.combination.Length
        };
        return _blowConfig;
    }

    private int GetActualPercentInActualTraining(BlowConfig blowConfig)
    {
        var _pastTime = blowConfig.duration - TrainingConfig.Instance.GetTrainingTimeRemainInMinutes();
        var _totalDuration = blowConfig.duration != 0 ? blowConfig.duration : 1;
        return (_pastTime * 100) / _totalDuration;
    }
    
    private int GetUnitPercentByBlowConfig(BlowConfig blowConfig) => 100 / blowConfig.qty;

    private void SetCorrectProgress(BlowConfig blowConfig, int actualPercent, int unitPercent)
    {
        var _progressActiveImages = GetProgressActiveImages();
        for (var _i = 0; _i < blowConfig.qty; _i++)
        {
            if (actualPercent > unitPercent)
            {
                _progressActiveImages[_i].fillAmount = 0;
                actualPercent -= unitPercent;
                continue;
            }
            var _percent = ((float)actualPercent / unitPercent)  * 100;
            var _inverseFill = _percent  * 0.01f;
            var _correctFill = _inverseFill != 0 ? 1 - _inverseFill : 1;
            _progressActiveImages[_i].fillAmount = _correctFill;
            break;
        }
    }
    
    private void UpdateProgressTraining()
    {
        if (!TrainingConfig.Instance.SelectedToyoIsInTraining())
            return;
        
        var _blowConfig = GetBlowConfigByTrainingInfo(TrainingConfig.Instance.GetCurrentTrainingInfo());
        var _actualPercent = GetActualPercentInActualTraining(_blowConfig);
        var _unitPercent = GetUnitPercentByBlowConfig(_blowConfig);
        
        ActiveProgressImagesInActiveActions();
        ResetProgressImages();

        SetCorrectProgress(_blowConfig, _actualPercent, _unitPercent);
    }

    private List<Image> GetProgressActiveImages()
        =>  progressImages.Where(img => img.gameObject.activeInHierarchy).ToList();
    
    private void ActiveProgressImagesInActiveActions()
    {
        for (var _i = 0; _i < combPoolImages.Length; _i++)
        {
            if(combPoolImages[_i].gameObject.activeInHierarchy)
                progressImages[_i].gameObject.SetActive(true);
        }
    }

    private void ResetProgressImages()
    {
        var _images = GetProgressActiveImages();
        for (var _i = 0; _i < _images.Count; _i++)
            _images[_i].fillAmount = 1;
    }

    private void CheckIfIsToyoOrAutomata()
    {
        if (ToyoManager.GetSelectedToyo().isAutomata)
        {
            EnableVisualElement("rewardsBackgroundAutomata");
            DisableVisualElement("rewardsBackgroundToyo");
        }
        else
        {
            EnableVisualElement("rewardsBackgroundToyo");
            DisableVisualElement("rewardsBackgroundAutomata");
        }
    }
}

public enum TrainingActionType
{
    None = 0,
    Punch = 1,
    Kick = 2,
    Move = 3
}
