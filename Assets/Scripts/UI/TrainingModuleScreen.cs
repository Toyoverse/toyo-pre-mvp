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
        SetTextInLabel(eventTimeName, TrainingConfig.EventTimeDefaultText 
                                      + ConvertMinutesToString(TrainingConfig.Instance.GetEventTimeRemain()));
        TrainingConfig.Instance.ApplyBlowConfig();
        SetTextInLabel(receiveName, /*"Receive: $" + */TrainingConfig.Instance.receiveValue.ToString());
        SetTextInLabel(durationName, /*"Duration: " + */ConvertMinutesToString(TrainingConfig.Instance.durationValue));
    }

    public override void ActiveScreen()
    {
        base.ActiveScreen();
        ResetCombinationPool();
        //TrainingConfig.Instance.SetSelectedToyoIsInTraining();
        ApplyInTrainingActions();
        UpdateUI();
    }

    public override void DisableScreen()
    {
        CheckToResetTrainingModule();
        base.DisableScreen();
    }

    private void CheckToResetTrainingModule()
    {
        if (ScreenManager.ScreenState == ScreenState.TrainingActionSelect
            /*|| TrainingConfig.Instance.IsInTraining()*/)
            return;
        TrainingConfig.Instance?.ResetAllTrainingModule( /*ClearPossibleActionsEvents*/);
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
        //UpdateUI();
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
            CallBlockchainTrainingTransactions, () => {});

    private void CallBlockchainTrainingTransactions()
    {
        Loading.StartLoading?.Invoke();
        if(ToyoManager.GetSelectedToyo().isStaked)
            ScreenManager.Instance.trainingModuleScript.SendToyoToTraining();
        else
            DatabaseConnection.Instance.blockchainIntegration.ToyoApproveNftTransfer(ToyoManager.GetSelectedToyo().tokenId);
    }
    
    public void SendToyoToTraining() => TrainingConfig.Instance.SetInTrainingOnServer();

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

    private void UpdateProgressTraining()
    {
        if (!TrainingConfig.Instance.SelectedToyoIsInTraining())
            return;

        var _toyoTrainingInfo = TrainingConfig.Instance.GetCurrentTrainingInfo();
        var _blowConfig = new BlowConfig
        {
            duration = TrainingConfig.Instance.GetTrainingTotalDuration(_toyoTrainingInfo),
            qty = _toyoTrainingInfo.combination.Length
        };

        var _actualPercent = Mathf.Round((_blowConfig.duration - TrainingConfig.Instance.GetTrainingTimeRemainInMinutes())
                              / (_blowConfig.duration != 0 ? _blowConfig.duration : 1) * 100);
        var _unitPercent = 100 / _blowConfig.qty;
        ActiveProgressImagesInActiveActions();
        for (var _i = 0; _i < _blowConfig.qty; _i++)
        {
            if (_actualPercent > (_unitPercent * (_i + 1)))
            {
                GetProgressActiveImages()[_i].fillAmount = 0;
                continue;
            }
            var _inverseFill = (_actualPercent / (_blowConfig.qty - _i)) / 100;
            var _correctFill = _inverseFill != 0 ? 1 - _inverseFill : 1;
            GetProgressActiveImages()[_i].fillAmount = _correctFill;
            break;
        }
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
}

public enum TrainingActionType
{
    None = 0,
    Punch = 1,
    Kick = 2,
    Move = 3
}
