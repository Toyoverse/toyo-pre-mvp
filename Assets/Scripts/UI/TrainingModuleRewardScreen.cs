using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class TrainingModuleRewardScreen : UIController
{
    public string eventTitleName = "eventTitle";
    public string eventTimeName = "eventTime";
    public string rewardTitleName = "rewardTitle";
    public string rewardValueName = "rewardValue";
    public string rewardImageName = "cardImage";
    public string rewardDescriptionName = "rewardDescription";
    
    public GameObject[] combinationPoolObjects;
    public Image[] combinationPoolImages;
    public Image[] loadingPoolImages;
    public GameObject[] correctPoolObjects;
    public GameObject[] wrongPositionObjects;
    public GameObject[] totallyWrongObjects;

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
        TrainingConfig.Instance.ClaimCallInServer();
        CallRewardsTransaction(GoToMainMenu);
    }
    
    private void GoToMainMenu() => ScreenManager.Instance.GoToScreen(ScreenState.MainMenu);

    private void CallRewardsTransaction(Action callback)
    {
        //TODO
        callback?.Invoke();
    }

    private void ApplySelectedActions()
    {
        //TODO: Get selected actions to database
        for (var _i = 0; _i < GetTrainingActions().Count; _i++)
        {
            combinationPoolObjects[_i].SetActive(true);
            var _actionSprite = GetTrainingActions()[_i].sprite;
            combinationPoolImages[_i].sprite = _actionSprite;
            loadingPoolImages[_i].sprite = _actionSprite;
        }
    }

    private void ShowRewards()
    {
        SetTextInLabel(rewardTitleName, TrainingConfig.Instance.rewardTitle);
        SetTextInLabel(rewardValueName, TrainingConfig.Instance.bondReward + " " + _coinPrefix);
        CheckAndRewardCard();
    }

    private void CheckAndSetBordersAfterCompare()
    {
        DisableAllBorders();
        var _results = TrainingConfig.Instance.CompareCombination(GetTrainingActions());
        for (var _i = 0; _i < _results.Count; _i++)
        {
            switch (_results[_i])
            {
                case TRAINING_RESULT.TOTALLY_WRONG:
                    totallyWrongObjects[_i].SetActive(true);
                    loadingPoolImages[_i].gameObject.SetActive(true);
                    break;
                case TRAINING_RESULT.WRONG_POSITION:
                    wrongPositionObjects[_i].SetActive(true);
                    loadingPoolImages[_i].gameObject.SetActive(true);
                    break;
                case TRAINING_RESULT.TOTALLY_CORRECT:
                    loadingPoolImages[_i].gameObject.SetActive(false);
                    correctPoolObjects[_i].SetActive(true);
                    break;
            }
        }
    }

    private void DisableAllBorders()
    {
        for (var _i = 0; _i < GetTrainingActions().Count; _i++)
        {
            correctPoolObjects[_i].SetActive(false);
            wrongPositionObjects[_i].SetActive(false);
            totallyWrongObjects[_i].SetActive(false);
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
        return _hits == TrainingConfig.Instance.GetSelectedBlowConfig().qty;
    }

    private bool WinCardWithExactAmountOfMoves()
    {
        var _trainingResults = TrainingConfig.Instance.CompareCombination(GetTrainingActions());
        var _hits = _trainingResults.Count(result => result == TRAINING_RESULT.TOTALLY_CORRECT);
        return _hits == TrainingConfig.Instance.GetCardReward()?.correctCombination?.Length;
    }
}
