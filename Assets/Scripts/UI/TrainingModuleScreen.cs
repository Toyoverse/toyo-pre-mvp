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
    public string combPoolContainer;
    public string[] combPoolNames;
    public string[] combPoolImagesNames;
    public string[] removePoolNames;
    public string startTrainingButtonName;

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
        CheckActionsCountAndRevealOrNo();
        TrainingConfig.Instance.ApplyRewardsCalculation();
        SetTextInLabel(eventTitleName, TrainingConfig.Instance.eventTitle);
        SetTextInLabel(eventTimeName, ConvertMinutesToString(TrainingConfig.Instance.eventTime));
        SetTextInLabel(investName, "Invest: $" + TrainingConfig.Instance.investValue);
        SetTextInLabel(receiveName, "Receive: $" + TrainingConfig.Instance.receiveValue);
        SetTextInLabel(durationName, "Duration: " + ConvertMinutesToString(TrainingConfig.Instance.durationValue));
        CheckTrainingAndEnableTrainingUI();
    }
    
    public override void ActiveScreen()
    {
        base.ActiveScreen();
        //CreateTrainingConfig();
        AddActionSelectEvents();
        DisableAllCombinationPoolVisualElements();
        CheckAndApplyInTrainingActions();
        UpdateUI();
    }

    public override void DisableScreen()
    {
        RemoveActionSelectEvents();
        TrainingConfig.Instance.ResetAllTrainingModule(ClearPossibleActionsEvents);
        base.DisableScreen();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        RemoveActionSelectEvents();
        ClearPossibleActionsEvents();
    }
    
    private void AddActionSelectEvents()
    {
        for (var _i = 0; _i < combPoolImagesNames.Length; _i++)
        {
            var _selectedActionIndex = _i;
            var _addActionButton = Root?.Q<VisualElement>(combPoolImagesNames[_i]);
            _addActionButton?.RegisterCallback<ClickEvent>
            (_ =>
                {
                    TrainingConfig.Instance.SetSelectedID(_selectedActionIndex);
                    OpenActionSelectionScreen();
                }
            );
            var _deleteActionButton = Root?.Q<VisualElement>(removePoolNames[_i]);
            _deleteActionButton?.RegisterCallback<ClickEvent>
            (_ => 
                {
                    RemoveAction(_selectedActionIndex);
                }
            );
        }
    }

    private void RemoveActionSelectEvents()
    {
        for (var _i = 0; _i < combPoolImagesNames.Length; _i++)
        {
            var _selectedActionIndex = _i;
            var _addActionButton = Root?.Q<VisualElement>(combPoolImagesNames[_i]);
            _addActionButton?.UnregisterCallback<ClickEvent>
            (_ =>
                {
                    TrainingConfig.Instance.SetSelectedID(_selectedActionIndex);
                    OpenActionSelectionScreen();
                }
            );
            var _deleteActionButton = Root?.Q<VisualElement>(removePoolNames[_i]);
            _deleteActionButton?.UnregisterCallback<ClickEvent>
            (_ => 
                {
                    RemoveAction(_selectedActionIndex);
                }
            );
        }
    }

    private void ConfirmAction(TrainingActionSO actionSo)
    {
        EnableVisualElement(combPoolNames[TrainingConfig.Instance.selectedActionID]);
        EnableVisualElement(removePoolNames[TrainingConfig.Instance.selectedActionID]);
        SetVisualElementSprite(combPoolImagesNames[TrainingConfig.Instance.selectedActionID], actionSo.sprite);
        CloseActionSelectionScreen();
        if (!TrainingConfig.Instance.selectedActionsDict.ContainsKey(TrainingConfig.Instance.selectedActionID))
            TrainingConfig.Instance.selectedActionsDict.Add(TrainingConfig.Instance.selectedActionID, actionSo);
        else
            TrainingConfig.Instance.selectedActionsDict[TrainingConfig.Instance.selectedActionID] = actionSo;
        TrainingConfig.Instance.ApplyTrainingMode();
        TrainingConfig.Instance.ApplyBlowConfig();
        UpdateUI();
    }

    private void RemoveAction(int i)
    {
        TrainingConfig.Instance.selectedActionsDict.Remove(i);
        SetVisualElementSprite(combPoolImagesNames[i], null);
        DisableVisualElement(combPoolNames[i]);
        DisableVisualElement(removePoolNames[i]);
        TrainingConfig.Instance.ApplyBlowConfig();
        UpdateUI();
    }

    private void DisableAllRemoveButtons()
    {
        foreach (var _buttonName in removePoolNames)
            DisableVisualElement(_buttonName);
    }

    private void SetActionToLastPosition(string actionName)
    {
        var _action = Root.Q<VisualElement>(actionName);
        var _actionContainer = Root.Q<VisualElement>(combPoolContainer);
        _actionContainer?.Insert(_actionContainer.childCount, _action);
    }

    private void CheckActionsCountAndRevealOrNo()
    {
        if (TrainingConfig.Instance.selectedActionsDict.Count >= combPoolNames.Length)
            return;
        CheckAndRevealNextAction();
        CheckAndRevealStartButton();
    }

    private void CheckAndRevealStartButton()
    {
        var _startButton = Root.Q<Button>(startTrainingButtonName);
        if (_startButton == null) return;
        _startButton.visible = !TrainingConfig.Instance.IsInTraining() && TrainingConfig.Instance.IsMinimumActionsToPlay();
    }

    private void CheckAndRevealNextAction()
    {
        if (!IsNecessaryRevealNewAction())
            return;
        RevealNextAction();
    }

    private bool IsNecessaryRevealNewAction()
    {
        var _activeActionCount = 0;
        foreach (var _name in combPoolNames)
        {
            if (ActionIsEnabled(_name))
                _activeActionCount++;
        }
        return TrainingConfig.Instance.selectedActionsDict.Count >= _activeActionCount;
    }

    private void RevealNextAction()
    {
        for (var _i = 0; _i < combPoolNames.Length; _i++)
        {
            if (ActionIsEnabled(combPoolNames[_i])) 
                continue;
            SetActionToLastPosition(combPoolNames[_i]);
            EnableVisualElement(combPoolNames[_i]);
            break;
        }
    }

    private void DisableAllCombinationPoolVisualElements()
    {
        foreach (var _name in combPoolNames)
            DisableVisualElement(_name);
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

    private bool ActionIsEnabled(string actionName)
    {
        var _visualE = Root?.Q<VisualElement>(actionName);
        if (_visualE != null)
            return _visualE.style.display != DisplayStyle.None;
        else
        {
            Debug.Log(actionName + "is null!");
            return false;
        }
    }

    public void PunchesButton()
        => SetPossibleActions(TrainingActionType.Punch);

    public void KicksButton()
        => SetPossibleActions(TrainingActionType.Kick);

    public void MovesButton()
        => SetPossibleActions(TrainingActionType.Move);

    private void SetPossibleActions(TrainingActionType type)
    {
        ClearPossibleActionsEvents();
        var _scrollView = Root.Q<ScrollView>(actionScrollName);
        _scrollView.Clear();
        TrainingConfig.Instance.SetOldTypeActionSelected(type);
        foreach (var _action in TrainingConfig.Instance.GetFilteredActions(type))
        {
            var _label = CreateNewLabel(_action.id.ToString(), _action.name, fontAsset, 
                                        32, Color.black, Color.white);
            _label.RegisterCallback<MouseEnterEvent>(_ 
                => SetVisualElementSprite(previewActionName, _action.sprite));
            _label.RegisterCallback<MouseUpEvent>(_
                => ConfirmAction(_action));
            _scrollView.Add(_label);
        }
    }

    private void ClearPossibleActionsEvents()
    {
        if(TrainingConfig.Instance.OldTypeSelectedIsNone())
            return;
        foreach (var _action in TrainingConfig.Instance.GetFilteredActionsOnOldType())
        {
            var _label = Root.Q<Label>(_action.id.ToString());
            _label.UnregisterCallback<MouseEnterEvent>(_ 
                => SetVisualElementSprite(previewActionName, _action.sprite));
            _label.UnregisterCallback<MouseUpEvent>(_
                => ConfirmAction(_action));
        }
    }

    private void OpenActionSelectionScreen()
    {
        var _actionArea = Root.Q<GroupBox>(actionSelectionAreaName);
        _actionArea.visible = true;
        SetVisualElementSprite(previewActionName, null);
    }

    private void CloseActionSelectionScreen()
    {
        ClearPossibleActionsEvents();
        var _scrollView = Root.Q<ScrollView>(actionScrollName);
        _scrollView.Clear();
        TrainingConfig.Instance.SetOldTypeActionSelected(TrainingActionType.None);
        var _actionArea = Root.Q<GroupBox>(actionSelectionAreaName);
        _actionArea.visible = false;
    }

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
        if (TrainingConfig.Instance.GetTrainingTimeRemain() > 0)
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
        }
        else
            DisableVisualElement(inTrainingBoxName);
    }

    public void GoToRewardScreen() => ScreenManager.Instance.GoToScreen(ScreenState.TrainingModuleRewards);
}

public enum TrainingActionType
{
    None = 0,
    Punch = 1,
    Kick = 2,
    Move = 3
}
