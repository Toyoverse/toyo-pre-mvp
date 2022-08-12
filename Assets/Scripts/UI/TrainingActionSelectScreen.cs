using UI;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class TrainingActionSelectScreen : UIController
{
    private TrainingModuleScreen _trainingModuleScreen => ScreenManager.Instance.trainingModuleScript;
    public string actionScrollName;
    public FontAsset fontAsset;
    public string previewActionName;
    private int _buttonID;
    private Sprite _actionSprite;

    private void SetPossibleActions(TrainingActionType type)
    {
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
    
    public void PunchesButton()
        => SetPossibleActions(TrainingActionType.Punch);

    public void KicksButton()
        => SetPossibleActions(TrainingActionType.Kick);

    public void MovesButton()
        => SetPossibleActions(TrainingActionType.Move);
    
    private void ConfirmAction(TrainingActionSO actionSo)
    {
        _trainingModuleScreen.RevealRemoveButton(TrainingConfig.Instance.selectedActionID);
        TrainingConfig.Instance.AddToSelectedActionsDict(TrainingConfig.Instance.selectedActionID, actionSo);
        //TrainingConfig.Instance.ApplyTrainingMode();
        TrainingConfig.Instance.ApplyBlowConfig();
        ClearPossibleActionsEvents();
        SetButtonSpriteVariables(TrainingConfig.Instance.selectedActionID, actionSo.sprite);
        Loading.EndLoading += SetActionSprite;
        Loading.EndLoading += _trainingModuleScreen.RevealNextAction;
        ScreenManager.Instance.GoToScreen(ScreenState.TrainingModule);
    }
    
    private void SetActionSprite()
        => _trainingModuleScreen.SetActionSprite(_buttonID, _actionSprite);

    private void SetButtonSpriteVariables(int id, Sprite sprite)
    {
        _buttonID = id;
        _actionSprite = sprite;
    }
}
