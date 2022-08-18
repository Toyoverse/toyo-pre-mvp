using UI;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class TrainingActionSelectScreen : UIController
{
    private TrainingModuleScreen _trainingModuleScreen => ScreenManager.Instance.trainingModuleScript;
    public string actionScrollName;
    public FontAsset fontAsset;
    public Texture2D backgroundLabelTexture;
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
                32, Color.white, Color.clear, backgroundLabelTexture);
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
        var _selectedID = TrainingConfig.Instance.selectedActionID;
        _trainingModuleScreen.RevealRemoveButton(_selectedID);
        TrainingConfig.Instance.AddToSelectedActionsDict(_selectedID, actionSo);
        TrainingConfig.Instance.ApplyBlowConfig();
        ClearPossibleActionsEvents();
        SetButtonSpriteVariables(_selectedID, actionSo.sprite);
        Loading.EndLoading += SetActionSprites;
        Loading.EndLoading += _trainingModuleScreen.RevealNextAction;
        ScreenManager.Instance.GoToScreen(ScreenState.TrainingModule);
    }
    
    private void SetActionSprites() => _trainingModuleScreen.SetActionsSprites(_buttonID, _actionSprite);

    private void SetButtonSpriteVariables(int id, Sprite sprite)
    {
        _buttonID = id;
        _actionSprite = sprite;
    }
}
