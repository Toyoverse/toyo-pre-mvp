using UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class TrainingModuleScreen : UIController
{
    public string titleName = "title";
    public string trainingTimeName = "time";
    public string[] combinationPoolNames;

    public string rewardTitleName = "rewardTitle";
    public string investName = "invest";
    public string receiveName = "receive";
    public string durationName = "duration";

    public string actionSelectionAreaName = "actionsSelectorBox";
    private int _selectedActionID = 0;

    protected override void UpdateUI()
    {
        //TODO: Set title and training time
        //TODO: Set Toyo and combination pool images
    }
    
    public void SendToyoToQuestButton()
    {
        Debug.Log("Send Toyo to Quest button clicked!");
    }

    private void ConfirmAction(Sprite sprite)
    {
        //TODO: Pegar sprite do golpe
        SetVisualElementSprite(combinationPoolNames[_selectedActionID], sprite);
    }

    public void PunchesButton()
    {
        SetPossibleActions(TrainingAction.Punch);
        Debug.Log("Punches button clicked!");
    }
    
    public void KicksButton()
    {
        SetPossibleActions(TrainingAction.Kick);
        Debug.Log("Kicks button clicked!");
    }
    
    public void MovesButton()
    {
        SetPossibleActions(TrainingAction.Move);
        Debug.Log("Moves button clicked!");
    }

    private void SetPossibleActions(TrainingAction actionType)
    {
        //TODO: Preencher content do scroll com possíveis ações
    }

    public void OpenActionSelection()
    {
        var _actionArea = Root.Q(actionSelectionAreaName);
        _actionArea.style.display = DisplayStyle.Flex;
    }
    
    public void CloseActionSelection()
    {
        var _actionArea = Root.Q(actionSelectionAreaName);
        _actionArea.style.display = DisplayStyle.None;
    }

    public void TrainingInfoButton() => ScreenManager.Instance.GoToScreen(ScreenState.LoreTheme);

    public override void BackButton() => ScreenManager.Instance.GoToScreen(ScreenState.MainMenu);

    public void SetInvestUI(float value) => SetTextInLabel(investName, "INVEST: $" + value);
    
    public void SetDurationUI(float value) => SetTextInLabel(durationName, "DURATION: " + value + "H");
    
    public void SetReceiveUI(float value) => SetTextInLabel(receiveName, "RECEIVE: $" + value);

    public void SetPoolImage(Sprite sprite, int position)
        => SetVisualElementSprite(combinationPoolNames[position], sprite);

    public void StartButton()
    {
        Debug.Log("Send Toyo to Quest button clicked!");
    }
}

public enum TrainingAction
{
    None = 0,
    Punch = 1,
    Kick = 2,
    Move = 3
}
