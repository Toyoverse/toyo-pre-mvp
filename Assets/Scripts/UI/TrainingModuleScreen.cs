using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class TrainingModuleScreen : UIController
{
    public string titleName = "title";
    public string trainingTimeName = "time";
    public string[] combinationPoolNames;
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
    public int minimumActionsToPlay = 3;
    private List<TrainingActionSO> _selectedActions;
    public FontAsset fontAsset;

    [Header("Possible Actions")] public TrainingActionSO[] possibleActions;

    protected override void UpdateUI()
    {
        //TODO: Set title and training time
        //TODO: Set Toyo and combination pool images
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
        for (var _i = 0; _i < combinationPoolNames.Length; _i++)
        {
            Root?.Q<VisualElement>(combinationPoolNames[_i]).RegisterCallback<ClickEvent>
            (_ => 
                { 
                    OpenActionSelection();
                    SetSelectedID(_i);
                }
            );
        }
    }

    private void RemoveActionSelectEvents()
    {
        for (var _i = 0; _i < combinationPoolNames.Length; _i++)
        {
            Root?.Q<VisualElement>(combinationPoolNames[_i]).UnregisterCallback<ClickEvent>
            (_ =>
                { 
                    OpenActionSelection();
                    SetSelectedID(_i);
                }
            );
        }
    }

    private void SetSelectedID(int newID)
    {
        _selectedActionID = newID;
        Debug.Log("_selectedActionID = " + _selectedActionID);
    }

    public void SendToyoToQuestButton()
    {
        Debug.Log("Send Toyo to Quest button clicked!");
    }

    private void ConfirmAction(TrainingActionSO actionSo)
    {
        SetVisualElementSprite(combinationPoolNames[_selectedActionID], actionSo.sprite);
        CloseActionSelection();
        CheckActionsCount();
        _selectedActions.Add(actionSo);
    }

    private void RemoveAction(TrainingActionSO actionSo)
    {
        //TODO: Complete and use this method
        Root.Q<VisualElement>(combinationPoolNames[_selectedActionID]).visible = true;
        _selectedActions.Remove(actionSo);
    }

    private void CheckActionsCount()
    {
        if (_selectedActionID >= combinationPoolNames.Length - 1)
            return;
        Root.Q<VisualElement>(combinationPoolNames[_selectedActionID + 1]).visible = true;
        var _startButton = Root.Q<Button>(startTrainingButtonName);
        _startButton.visible = _selectedActionID >= minimumActionsToPlay;
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
        //_actionArea.style.display = DisplayStyle.Flex;
        _actionArea.visible = true;
    }
    
    private void CloseActionSelection()
    {
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
    }
}

public enum TrainingActionType
{
    None = 0,
    Punch = 1,
    Kick = 2,
    Move = 3
}
