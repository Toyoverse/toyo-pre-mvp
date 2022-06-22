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
    public string inTrainingTitleName;
    public string inTrainingTimeName;
    
    [Header("Reward names")]
    public string rewardTitleName;
    public string investName;
    public string receiveName;
    public string durationName;

    [Header("Actions Selection names")]
    public string actionSelectionAreaName;
    public string actionScrollName;
    public string previewActionName;
    private int _selectedActionID = 0;
    private TrainingActionType _oldTypeSelected;
    private int _minimumActionsToPlay = 3;
    public Dictionary<int, TrainingActionSO> selectedActions = new Dictionary<int, TrainingActionSO>();
    public FontAsset fontAsset;

    [Header("Possible Actions")] public TrainingActionSO[] possibleActions;
    
    //TODO: Get real variables in server
    private int _eventTime = 5436;
    private string _eventTitle = "TRAINING MODULE SEASON ONE!";
    private int _investValue = 0;
    private int _receiveValue = 0;
    private int _durationValue = 0;
    
    private bool _inTraining = false;
    private int _inTrainingTimeLeft = 866;
    private string _inTrainingTitle = "Toyo in training...";

    protected override void UpdateUI()
    {
        CheckActionsCount();
        ApplyRewardsCalculation();
        SetTextInLabel(eventTitleName, _eventTitle);
        SetTextInLabel(eventTimeName, ConvertMinutesToString(_eventTime));
        SetTextInLabel(investName, "Invest: $" + _investValue);
        SetTextInLabel(receiveName, "Receive: $" + _receiveValue);
        SetTextInLabel(durationName, "Duration: " + ConvertMinutesToString(_durationValue));
        CheckToyoIsTraining();
    }
    
    public override void ActiveScreen()
    {
        base.ActiveScreen();
        AddActionSelectEvents();
        DisableAllCombinationPoolVisualElements();
        UpdateUI();
    }

    public override void DisableScreen()
    {
        base.DisableScreen();
        RemoveActionSelectEvents();
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
                    SetSelectedID(_selectedActionIndex);
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
                    SetSelectedID(_selectedActionIndex);
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

    private void SetSelectedID(int newID)
    {
        _selectedActionID = newID;
        Debug.Log("_selectedActionID = " + _selectedActionID);
    }

    private void ConfirmAction(TrainingActionSO actionSo)
    {
        EnableVisualElement(combPoolNames[_selectedActionID]);
        EnableVisualElement(removePoolNames[_selectedActionID]);
        SetVisualElementSprite(combPoolImagesNames[_selectedActionID], actionSo.sprite);
        CloseActionSelectionScreen();
        if (!selectedActions.ContainsKey(_selectedActionID))
            selectedActions.Add(_selectedActionID, actionSo);
        else
            selectedActions[_selectedActionID] = actionSo;
        UpdateUI();
    }

    private void RemoveAction(int i)
    {
        selectedActions.Remove(i);
        SetVisualElementSprite(combPoolImagesNames[i], null);
        DisableVisualElement(combPoolNames[i]);
        DisableVisualElement(removePoolNames[i]);
        UpdateUI();
    }

    private void SetActionToLastPosition(string actionName)
    {
        var _action = Root.Q<VisualElement>(actionName);
        var _actionContainer = Root.Q<VisualElement>(combPoolContainer);
        _actionContainer?.Insert(_actionContainer.childCount, _action);
    }

    private void CheckActionsCount()
    {
        if (selectedActions.Count >= combPoolNames.Length)
            return;
        CheckAndRevealNextAction();
        CheckAndRevealStartButton();
    }

    private void CheckAndRevealStartButton()
    {
        var _startButton = Root.Q<Button>(startTrainingButtonName);
        if(_startButton != null) 
            _startButton.visible = selectedActions.Count >= _minimumActionsToPlay;
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
        return selectedActions.Count >= _activeActionCount;
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
        _oldTypeSelected = type;
        foreach (var _action in GetFilteredActions(type))
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
        if(_oldTypeSelected == TrainingActionType.None)
            return;
        foreach (var _action in GetFilteredActions(_oldTypeSelected))
        {
            var _label = Root.Q<Label>(_action.id.ToString());
            _label.UnregisterCallback<MouseEnterEvent>(_ 
                => SetVisualElementSprite(previewActionName, _action.sprite));
            _label.UnregisterCallback<MouseUpEvent>(_
                => ConfirmAction(_action));
        }
    }

    private List<TrainingActionSO> GetFilteredActions(TrainingActionType filter) 
        => possibleActions.Where(action => action.type == filter).ToList();

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
        _oldTypeSelected = TrainingActionType.None;
        var _actionArea = Root.Q<GroupBox>(actionSelectionAreaName);
        _actionArea.visible = false;
    }

    public void TrainingInfoButton() => ScreenManager.Instance.GoToScreen(ScreenState.LoreTheme);

    public override void BackButton() => ScreenManager.Instance.GoToScreen(ScreenState.MainMenu);

    public void StartButton()
        => GenericPopUp.Instance.ShowPopUp("Are you sure?", SendToyoToTraining, () => {});

    private void SendToyoToTraining()
    {
        _inTraining = true;
        UpdateUI();
    }

    private void ApplyRewardsCalculation()
    {
        //TODO: Apply real calculation
        _investValue = 0;
        _receiveValue = 0;
        _durationValue = 0;
        foreach (var _action in selectedActions)
        {
            _investValue += 80;
            _receiveValue += 95;
            _durationValue += 75;
        }
    }

    private void CheckToyoIsTraining() //TODO: Change name
    {
        var _trainingBox = Root.Q<GroupBox>(inTrainingBoxName);
        if (_inTraining)
        {
            _trainingBox.visible = true;
            SetTextInLabel(inTrainingTitleName, _inTrainingTitle);
            SetTextInLabel(inTrainingTimeName, ConvertMinutesToString(_inTrainingTimeLeft));
        }
        else
            _trainingBox.visible = false;
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
