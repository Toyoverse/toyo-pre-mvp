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
    public string[] combPoolImagesNames;
    public string[] removePoolNames;
    public string startTrainingButtonName;
    public GameObject[] combPoolObjects;
    public UnityEngine.UI.Image[] combPoolImages;
    public Transform combPoolContainer;

    [Header("In Training names")] 
    public string inTrainingBoxName;
    public string inTrainingTimeButtonName;

    [Header("Reward names")]
    public string rewardTitleName;
    public string investName;
    public string receiveName;
    public string durationName;

    [Header("Actions Selection names")]
    public string actionSelectionAreaName;
    public string actionScrollName;
    public string previewActionName;
    public FontAsset fontAsset;

    protected override void UpdateUI()
    {
        CheckAndRevealStartButton();
        TrainingConfig.Instance.ApplyRewardsCalculation();
        SetTextInLabel(eventTitleName, TrainingConfig.Instance.eventTitle);
        SetTextInLabel(eventTimeName, ConvertMinutesToString(TrainingConfig.Instance.GetEventTimeRemain()));
        SetTextInLabel(investName, "Invest: $" + TrainingConfig.Instance.investValue);
        SetTextInLabel(receiveName, "Receive: $" + TrainingConfig.Instance.receiveValue);
        SetTextInLabel(durationName, "Duration: " + ConvertMinutesToString(TrainingConfig.Instance.durationValue));
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
        TrainingConfig.Instance.ResetAllTrainingModule(/*ClearPossibleActionsEvents*/);
        ClearActionsImages();
    }

    public void OpenActionSelection(int buttonID)
    {
        TrainingConfig.Instance.SetSelectedID(buttonID);
        OpenActionSelectionScreen();
    }

    private void ConfirmAction(TrainingActionSO actionSo)
    {
        combPoolObjects[TrainingConfig.Instance.selectedActionID].gameObject.SetActive(true);
        //EnableVisualElement(removePoolNames[TrainingConfig.Instance.selectedActionID]); //TODO: Add remove button
        SetActionSprite(TrainingConfig.Instance.selectedActionID, actionSo.sprite);
        TrainingConfig.Instance.AddToSelectedActionsDict(TrainingConfig.Instance.selectedActionID, actionSo);
        TrainingConfig.Instance.ApplyTrainingMode();
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

    private void RevealNextAction()
    {
        for (var _i = 0; _i < combPoolObjects.Length; _i++)
        {
            if (combPoolObjects[_i].gameObject.activeInHierarchy) 
                continue;
            SetActionToLastPosition(combPoolObjects[_i].gameObject);
            combPoolObjects[_i].gameObject.SetActive(true);
            break;
        }
    }

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
        => GenericPopUp.Instance.ShowPopUp("Are you sure?", SendToyoToTraining, () => {});

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
            //DisableAllRemoveButtons(); //TODO: Disable remove buttons
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
}

public enum TrainingActionType
{
    None = 0,
    Punch = 1,
    Kick = 2,
    Move = 3
}
