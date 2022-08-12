using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

public class TrainingModuleRewardScreen : UIController
{
    public string eventTitleName = "eventTitle";
    public string eventTimeName = "eventTime";
    public string rewardTitleName = "rewardTitle";
    public string rewardValueName = "rewardValue";
    public string rewardImageName = "cardImage";
    public string rewardDescriptionName = "rewardDescription";
    public string[] combinationPoolNames;
    public string[] combinationPoolImageNames;
    public string[] borderCorrectImageNames;
    public string[] borderWrongPositionImageNames;
    public string[] borderTotallyWrongImageNames;

    private string _eventTitle => TrainingConfig.Instance.eventTitle;

    private int _eventTime => TrainingConfig.Instance.GetEventTimeRemain();

    //TODO: Get correct variables in server
    public bool cardCollected;
    private string _coinPrefix = "$TOYO";

    private List<TrainingActionSO> GetTrainingActions() => TrainingConfig.Instance.selectedTrainingActions;

    public override void ActiveScreen()
    {
        base.ActiveScreen();
        CheckAndSetBordersAfterCompare();
    }

    protected override void UpdateUI()
    {
        ApplySelectedActions();
        SetTextInLabel(eventTitleName, _eventTitle);
        SetTextInLabel(eventTimeName, ConvertMinutesToString(_eventTime));
        ShowRewards();
    }

    public void ClaimButton()
    {
        //TODO: Claim Rewards
        Debug.Log("Claim Rewards");
        ScreenManager.Instance.GoToScreen(ScreenState.MainMenu);
        TrainingConfig.Instance.SetInTraining(false);
    }

    private void ApplySelectedActions()
    {
        //TODO: Get selected actions to database
        for (var _i = 0; _i < GetTrainingActions().Count; _i++)
        {
            EnableVisualElement(combinationPoolNames[_i]);
            //Root.Q<VisualElement>(combinationPoolNames[_i]).style.display = DisplayStyle.Flex;
            SetVisualElementSprite(combinationPoolImageNames[_i], GetTrainingActions()[_i].sprite);
        }
    }

    private void ShowRewards()
    {
        SetTextInLabel(rewardTitleName, TrainingConfig.Instance.rewardTitle);
        //SetTextInLabel(rewardValueName, TrainingConfig.Instance.GetSelectedBlowConfig().reward + " " + _coinPrefix);
        SetTextInLabel(rewardValueName, TrainingConfig.Instance.boundReward + " " + _coinPrefix);
        CheckAndRewardCard();
    }

    private void CheckAndSetBordersAfterCompare()
    {
        DisableAllBorders();
        var _results = TrainingConfig.Instance.CompareCombination(GetTrainingActions());
        for (var _i = 0; _i < _results.Count; _i++)
        {
            EnableVisualElement(_results[_i] switch
            {
                TRAINING_RESULT.TOTALLY_WRONG => borderTotallyWrongImageNames[_i],
                TRAINING_RESULT.WRONG_POSITION => borderWrongPositionImageNames[_i],
                TRAINING_RESULT.TOTALLY_CORRECT => borderCorrectImageNames[_i]
            });
        }
    }

    private void DisableAllBorders()
    {
        for (var _i = 0; _i < GetTrainingActions().Count; _i++)
        {
            Debug.Log("Disable Borders " + _i);
            DisableVisualElement(borderTotallyWrongImageNames[_i]);
            DisableVisualElement(borderWrongPositionImageNames[_i]);
            DisableVisualElement(borderCorrectImageNames[_i]);
        }
    }

    private void CheckAndRewardCard()
    {
        if (IsCardCollected())
        {
            SetVisualElementSprite(rewardImageName, null); //TODO: Add default image for card has already been collected
            SetTextInLabel(rewardDescriptionName, TrainingConfig.Instance.alreadyWon);
            return;
        }

        if (!WonTheCard())
        {
            SetVisualElementSprite(rewardImageName, null); //TODO: Add default image for no card won
            SetTextInLabel(rewardDescriptionName, TrainingConfig.Instance.losesMiniGame);
            return;
        }

        var _card = TrainingConfig.Instance.GetCardReward();
        SetVisualElementSprite(rewardImageName, _card.cardImage);
        SetTextInLabel(rewardDescriptionName, _card.description);
        SetCardCollected();
    }

    private void SetCardCollected()
    {
        if(IsCardCollected())
            return;
        cardCollected = true;
    }

    private bool IsCardCollected() => cardCollected;

    private bool WonTheCard()
    {
        return WinCardWithExactAmountOfMoves(); 
        //I made three methods to test if we should deliver the letter because
        //I didn't know if this would vary according to the training mode
        //or if it would only be possible with 5 hits.
    }

    private bool WonCardWithOnlyFiveHits()
    {
        var _trainingResults = TrainingConfig.Instance.CompareCombination(GetTrainingActions());
        var _hits = _trainingResults.Count(result => result == TRAINING_RESULT.TOTALLY_CORRECT);
        return _hits > 4;
    }
    
    private bool WonCardWithSpecificTrainingMode() 
    {
        var _trainingResults = TrainingConfig.Instance.CompareCombination(GetTrainingActions());
        var _hits = _trainingResults.Count(result => result == TRAINING_RESULT.TOTALLY_CORRECT);
        return _hits == TrainingConfig.Instance.GetSelectedBlowConfig().blows;
    }

    private bool WinCardWithExactAmountOfMoves()
    {
        var _trainingResults = TrainingConfig.Instance.CompareCombination(GetTrainingActions());
        var _hits = _trainingResults.Count(result => result == TRAINING_RESULT.TOTALLY_CORRECT);
        //return _hits == TrainingConfig.Instance.correctCombination.actions.Length;
        return _hits == TrainingConfig.Instance.GetCardReward().correctCombination.Length;
    }
}
