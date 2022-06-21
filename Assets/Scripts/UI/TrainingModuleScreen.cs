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
    public string eventTitleName = "eventTitle";
    public string eventTimeName = "eventTime";
    public string combPoolContainer = "actions";
    public string[] combPoolNames;
    public string[] combPoolImagesNames;
    public string[] removePoolNames;
    public string startTrainingButtonName = "startButton";
    
    [Header("Reward names")]
    public string rewardTitleName = "rewardTitle";
    public string investName = "invest";
    public string receiveName = "receive";
    public string durationName = "duration";

    [Header("Actions Selection names")]
    public string actionSelectionAreaName = "actionsSelectorBox";
    public string actionScrollName = "actionScroll";
    public string previewActionName = "actionImage";
    private int _selectedActionID = 0;
    private TrainingActionType _oldTypeSelected;
    private int _minimumActionsToPlay = 3;
    private Dictionary<int, TrainingActionSO> _selectedActions = new Dictionary<int, TrainingActionSO>();
    public FontAsset fontAsset;

    [Header("Possible Actions")] public TrainingActionSO[] possibleActions;
    
    //TODO: Get real variables in server
    private int _eventTime = 5436;
    private string _eventTitle = "TRAINING MODULE SEASON ONE!";
    private int _investValue = 0;
    private int _receiveValue = 0;
    private int _durationValue = 0;

    protected override void UpdateUI()
    {
        SetTextInLabel(eventTitleName, _eventTitle);
        SetTextInLabel(eventTimeName, GetDurationConvert(_eventTime));
        SetTextInLabel(investName, "Invest: $" + _investValue);
        SetTextInLabel(receiveName, "Receive: $" + _receiveValue);
        SetTextInLabel(durationName, "Duration: " + GetDurationConvert(_durationValue));
    }
    
    public override void ActiveScreen()
    {
        base.ActiveScreen();
        AddActionSelectEvents();
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
            var _i1 = _i;
            Root?.Q<VisualElement>(combPoolImagesNames[_i]).RegisterCallback<ClickEvent>
            (_ =>
                {
                    SetSelectedID(_i1);
                    OpenActionSelection();
                }
            );
            Root?.Q<VisualElement>(removePoolNames[_i]).RegisterCallback<ClickEvent>
            (_ => 
                {
                    RemoveAction(_i1);
                }
            );
        }
    }

    private void RemoveActionSelectEvents()
    {
        for (var _i = 0; _i < combPoolImagesNames.Length; _i++)
        {
            var _i1 = _i;
            Root?.Q<VisualElement>(combPoolImagesNames[_i]).UnregisterCallback<ClickEvent>
            (_ =>
                {
                    SetSelectedID(_i1);
                    OpenActionSelection();
                }
            );
            Root?.Q<VisualElement>(removePoolNames[_i]).UnregisterCallback<ClickEvent>
            (_ => 
                {
                    RemoveAction(_i1);
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
        Root.Q<VisualElement>(combPoolNames[_selectedActionID]).style.display = DisplayStyle.Flex;
        Root.Q<VisualElement>(removePoolNames[_selectedActionID]).style.display = DisplayStyle.Flex;
        SetVisualElementSprite(combPoolImagesNames[_selectedActionID], actionSo.sprite);
        CloseActionSelection();
        _selectedActions.Add(_selectedActionID, actionSo);
        CheckActionsCount();
        ApplyRewardsCalculation();
        UpdateUI();
    }

    private void RemoveAction(int i)
    {
        //TODO: Complete and use this method
        _selectedActions.Remove(i);
        SetVisualElementSprite(combPoolImagesNames[i], null);
        Root.Q<VisualElement>(combPoolNames[i]).style.display = DisplayStyle.None;
        Root.Q<VisualElement>(removePoolNames[i]).style.display = DisplayStyle.None;
        CheckActionsCount();
        ApplyRewardsCalculation();
        UpdateUI();
    }

    private void SetActionToLastPosition(string actionName)
    {
        var _action = Root.Q<VisualElement>(actionName);
        var _actionContainer = Root.Q<VisualElement>(combPoolContainer);
        _actionContainer.Insert(_actionContainer.childCount, _action);
    }

    private void CheckActionsCount()
    {
        if (_selectedActions.Count >= combPoolNames.Length)
            return;
        RevealNextAction();
        var _startButton = Root.Q<Button>(startTrainingButtonName);
        _startButton.visible = _selectedActions.Count >= _minimumActionsToPlay;
    }

    private void RevealNextAction()
    {
        var _activeActionCount = 0;
        foreach (var _name in combPoolNames)
        {
            var _action = Root.Q<VisualElement>(_name);
            if (_action.style.display == DisplayStyle.Flex)
                _activeActionCount++;
        }
        Debug.Log("activeActionCount: " + _activeActionCount);
        if (_activeActionCount > _selectedActions.Count)
            return;
        
        for (var _i = 0; _i < combPoolNames.Length; _i++)
        {
            var _action = Root.Q<VisualElement>(combPoolNames[_i]);
            if (_action.style.display == DisplayStyle.Flex) 
                continue;
            SetActionToLastPosition(combPoolNames[_i]);
            _action.style.display = DisplayStyle.Flex;
            Debug.Log("Next Action Reveal!");
            break;
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
            var _label = new Label()
            {
                name = _action.id.ToString(),
                text = _action.name,
                style =
                {
                    fontSize = 32,
                    unityFontDefinition = new StyleFontDefinition(fontAsset),
                    backgroundColor = Color.white
                }
            };
            _label.RegisterCallback<MouseEnterEvent>(_ 
                => SetVisualElementSprite(previewActionName, _action.sprite));
            _label.RegisterCallback<ClickEvent>(_
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
            _label.UnregisterCallback<ClickEvent>(_
                => ConfirmAction(_action));
        }
    }

    private List<TrainingActionSO> GetFilteredActions(TrainingActionType filter) 
        => possibleActions.Where(action => action.type == filter).ToList();

    private void OpenActionSelection()
    {
        var _actionArea = Root.Q<GroupBox>(actionSelectionAreaName);
        _actionArea.visible = true;
        SetVisualElementSprite(previewActionName, null);
        if (_selectedActions.ContainsKey(_selectedActionID))
            _selectedActions.Remove(_selectedActionID);
    }

    private void CloseActionSelection()
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

    public void SetInvestUI(float value) => SetTextInLabel(investName, "INVEST: $" + value);
    
    public void SetDurationUI(float value) => SetTextInLabel(durationName, "DURATION: " + value + "H");
    
    public void SetReceiveUI(float value) => SetTextInLabel(receiveName, "RECEIVE: $" + value);

    public void StartButton()
    {
        Debug.Log("Send Toyo to Quest button clicked!");
        foreach (var _action in _selectedActions)
        {
            Debug.Log(_action.Value.name + ", id: " + _action.Value.id);
        }
        
        GenericPopUp.Instance.ShowPopUp("Are you sure?", SendToyoToTraining, () => {});
    }

    private void SendToyoToTraining()
    {
        //ScreenManager.Instance.GoToScreen(ScreenState.Welcome);
        Debug.Log("Sending Toyo to Training!");
    }

    private void ApplyRewardsCalculation()
    {
        //TODO: Apply real calculation
        _investValue = 0;
        _receiveValue = 0;
        _durationValue = 0;
        foreach (var _action in _selectedActions)
        {
            _investValue += 80;
            _receiveValue += 95;
            _durationValue += 75;
        }
    }

    private string GetDurationConvert(int durationInMinutes)
    {
        var _minutes = durationInMinutes;
        var _hours = 0;
        var _days = 0;
        while (_minutes >= 60)
        {
            _hours += 1;
            _minutes -= 60;
        }
        while (_hours >= 24)
        {
            _days += 1;
            _hours -= 24;
        }

        var _result = _days > 0 ? _days + "d " : "";
        _result += _hours > 0 ? _hours + "h " : "";
        _result += _minutes + "m";
        return _result;
    }
}

public enum TrainingActionType
{
    None = 0,
    Punch = 1,
    Kick = 2,
    Move = 3
}
