using System;
using System.Collections.Generic;
using System.Linq;
using Database;
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
    //public bool cardCollected;
    private string _coinPrefix = "$BOND";
    private int _maxCharactersInBondValue = 3;
    public Sprite defaultCardWrong;

    private List<TrainingActionSO> GetTrainingActions() => TrainingConfig.Instance.selectedTrainingActions;

    public override void ActiveScreen()
    {
        base.ActiveScreen();
        DatabaseConnection.Instance.GetRewardValues(ShowResults, FailedGetParameters,
            TrainingConfig.Instance.GetCurrentTrainingInfo().id);
    }

    protected override void UpdateUI()
    {
        ApplySelectedActions();
        SetTextInLabel(eventTitleName, _eventTitle);
        SetTextInLabel(eventTimeName, TrainingConfig.EventTimeDefaultText + ConvertMinutesToString(_eventTime));
        //ShowRewards();
    }

    public void ClaimButton()
    {
        Debug.Log("Claim Rewards");
        Loading.StartLoading?.Invoke();
        TrainingConfig.Instance.ClaimCallInServer();
    }

    private void ApplySelectedActions()
    {
        DisableAllPoolObjects(); //TODO TEST
        for (var _i = 0; _i < GetTrainingActions().Count; _i++)
        {
            combinationPoolObjects[_i].SetActive(true);
            var _actionSprite = GetTrainingActions()[_i].sprite;
            combinationPoolImages[_i].sprite = _actionSprite;
            loadingPoolImages[_i].sprite = _actionSprite;
        }
    }

    private void ShowRewards(TrainingResultJson trainingResult)
    {
        //SetTextInLabel(rewardTitleName, TrainingConfig.Instance.rewardTitle);
        var _bondString = trainingResult.body.bond.Substring(0, _maxCharactersInBondValue);
        SetTextInLabel(rewardValueName, _bondString + " " + _coinPrefix);
        CheckCard(trainingResult);
    }

    private void CheckCard(TrainingResultJson trainingResult)
    {
        if (trainingResult.body.isCombinationCorrect)
        {
            var _card = TrainingConfig.Instance.GetCardFromID(int.Parse(trainingResult.body.card.cardId));
            if (_card == null) //TODO TEST
            {
                SetVisualElementSprite(rewardImageName, defaultCardWrong);
                SetTextInLabel(rewardDescriptionName, TrainingConfig.Instance.alreadyWon); 
                return;
            }
            SetVisualElementSprite(rewardImageName, _card.cardImage);
            SetTextInLabel(rewardDescriptionName, _card.description);
        }
        else
        {
            SetVisualElementSprite(rewardImageName, defaultCardWrong); 
            SetTextInLabel(rewardDescriptionName, TrainingConfig.Instance.losesMiniGame);
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

    private void ShowResults(string jsonParameters)
    {
        //CheckAndSetBordersAfterCompare();
        var _trainingResult = JsonUtility.FromJson<TrainingResultJson>(jsonParameters);
        SetCombinationResult(TrainingConfig.Instance.GetResultsByCombinationResult
            (_trainingResult.body.combinationResult.result));
        ShowRewards(_trainingResult);
        Loading.EndLoading?.Invoke();
    }

    private void SetCombinationResult(List<TRAINING_RESULT> results)
    {
        DisableAllBorders();
        for (var _i = 0; _i < results.Count; _i++)
        {
            switch (results[_i])
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

    private void FailedGetParameters(string json)
    {
        Debug.Log("FailedGetParameters: " + json);
        Loading.EndLoading?.Invoke();
        TrainingConfig.Instance.GenericFailedMessage();
    }

    private void DisableAllPoolObjects()
    {
        for(var _i = 0; _i < combinationPoolObjects.Length; _i++)
            combinationPoolObjects[_i].SetActive(false);
    }
}
