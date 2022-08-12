using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class TrainingModuleScreen : UIController
{
    public string eventTitleName;

    public string eventTimeName;

    //public string combPoolContainer;
    //public string[] combPoolNames;
    /*public string[] combPoolImagesNames;
    public string[] removePoolNames;*/
    public string startTrainingButtonName;
    public GameObject[] combPoolObjects;
    public GameObject[] removeButtonsPool;
    public UnityEngine.UI.Image[] combPoolImages;
    public Transform combPoolContainer;

    [Header("In Training names")] public string inTrainingBoxName;
    public string inTrainingTimeButtonName;

    [Header("Reward names")] public string rewardTitleName;
    public string investName;
    public string receiveName;
    public string durationName;

    [Header("Actions Selection names")] public string actionSelectionAreaName;
    public string actionScrollName;
    public string previewActionName;
    public FontAsset fontAsset;

    protected override void UpdateUI()
    {
        CheckAndRevealStartButton();
        TrainingConfig.Instance.ApplyRewardsCalculation();
        SetTextInLabel(eventTitleName, TrainingConfig.Instance.eventTitle);
        SetTextInLabel(eventTimeName, ConvertMinutesToString(TrainingConfig.Instance.GetEventTimeRemain()));
        //SetTextInLabel(investName, "Invest: $" + TrainingConfig.Instance.investValue);
        SetTextInLabel(receiveName, /*"Receive: $" + */TrainingConfig.Instance.receiveValue.ToString());
        SetTextInLabel(durationName, /*"Duration: " + */ConvertMinutesToString(TrainingConfig.Instance.durationValue));
        CheckTrainingAndEnableTrainingUI();
    }

    public override void ActiveScreen()
    {
        base.ActiveScreen();
        ResetCombinationPool();
        CheckAndApplyInTrainingActions();
        UpdateUI();
    }

    public override void DisableScreen()
    {
        CheckToResetTrainingModule();
        base.DisableScreen();
    }

    private void CheckToResetTrainingModule()
    {
        if (ScreenManager.ScreenState == ScreenState.TrainingActionSelect)
            return;
        TrainingConfig.Instance.ResetAllTrainingModule( /*ClearPossibleActionsEvents*/);
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
        TrainingConfig.Instance.RemoveActionToDict(id);
        DisableActionObject(id);
        TrainingConfig.Instance.ApplyBlowConfig();
        RevealNextAction();
        UpdateUI();
    }

    private void ConfirmAction(TrainingActionSO actionSo)
    {
        combPoolObjects[TrainingConfig.Instance.selectedActionID].gameObject.SetActive(true);
        DisableRemoveButton(TrainingConfig.Instance.selectedActionID);
        SetActionSprite(TrainingConfig.Instance.selectedActionID, actionSo.sprite);
        TrainingConfig.Instance.AddToSelectedActionsDict(TrainingConfig.Instance.selectedActionID, actionSo);
        //TrainingConfig.Instance.ApplyTrainingMode();
        TrainingConfig.Instance.ApplyBlowConfig();
        UpdateUI();
    }

    public void SetActionSprite(int id, Sprite sprite) => combPoolImages[id].sprite = sprite;

    private void SetActionToLastPosition(GameObject actionObj)
    {
        actionObj.transform.SetParent(null);
        actionObj.transform.SetParent(combPoolContainer);
    }

    private void CheckAndRevealStartButton()
    {
        var _startButton = Root.Q<Button>(startTrainingButtonName);
        if (_startButton == null) return;
        _startButton.visible = (!TrainingConfig.Instance.IsInTraining() && TrainingConfig.Instance.IsMinimumActionsToPlay());
    }

    public void RevealNextAction()
    {
        if (GetActionsRevealCount() > TrainingConfig.Instance.selectedActionsDict.Count)
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
        foreach (var _obj in combPoolObjects)
            _obj.gameObject.SetActive(false);
        combPoolObjects[0].SetActive(true);
    }

    private void CheckAndApplyInTrainingActions()
    {
        if (!TrainingConfig.Instance.IsInTraining())
            return;
        for (var _i = 0; _i < TrainingConfig.Instance.selectedTrainingActions.Count; _i++)
        {
            TrainingConfig.Instance.SetSelectedID(_i);
            ConfirmAction(TrainingConfig.Instance.selectedTrainingActions[_i]);
        }
    }

    private void OpenActionSelectionScreen() => ScreenManager.Instance.GoToScreen(ScreenState.TrainingActionSelect);

    public void TrainingInfoButton() => ScreenManager.Instance.GoToScreen(ScreenState.LoreTheme);

    public override void BackButton() => ScreenManager.Instance.GoToScreen(ScreenState.MainMenu);

    public void StartButton()
        => GenericPopUp.Instance.ShowPopUp(TrainingConfig.Instance.sendToyoToTrainingPopUp,
            SendToyoToTraining, () => {});

    private void SendToyoToTraining()
    {
        TrainingConfig.Instance.SetInTraining(true);
        UpdateUI();
    }

    public void FinishTraining()
    {
        if (TrainingConfig.Instance.GetTrainingTimeRemain() > 0 && !TrainingConfig.Instance.ignoreTrainingTimer)
            return;
        GoToRewardScreen();
    }

    private void CheckTrainingAndEnableTrainingUI() 
    {
        if (TrainingConfig.Instance.IsInTraining())
        {
            EnableVisualElement(inTrainingBoxName);
            SetTextInButton(inTrainingTimeButtonName, ConvertMinutesToString(TrainingConfig.Instance.GetTrainingTimeRemain()));
            DisableAllRemoveButtons(); 
            DisableInteractableActionButtons();
        }
        else
        {
            DisableVisualElement(inTrainingBoxName);
            EnableInteractableActionButtons();
        }
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
}

public enum TrainingActionType
{
    None = 0,
    Punch = 1,
    Kick = 2,
    Move = 3
}
