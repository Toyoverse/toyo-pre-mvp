using System;
using System.Collections.Generic;
using Database;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using static TimeTools;

[CreateAssetMenu(fileName = "TrainingConfigSO", menuName = "ScriptableObject/TrainingConfigSO")]
public class TrainingConfigSO : ScriptableObject
{
    [Header("Event Default strings")] 
    public string eventTitle;
    public string inTrainingMessage;
    //public string rewardTitle;
    public string losesMessage;
    public string alreadyWon;
    public string eventStory;
    public string sendToyoToTrainingPopUp;

    [Header("Events dates")]
    public DateInfo startEventDateInfo;
    public DateInfo endEventDateInfo;
    
    [Header("Actions")]
    public TrainingActionSO[] possibleActions;
    public BlowConfig[] blowConfigs;
    
    [Header(("Rewards"))]
    public CardRewardSO[] cardRewards;
    public float bondReward;
    public float bonusBondReward;
    
    //Intern control to send cardRewards
    private string _trainingID;
    private int _cardCount;

    public void SendToServer()
    {
        Debug.Log("Send TrainingConfig to server in JSON: " + GetTrainingParametersInJSONString());
        DatabaseConnection.Instance.PostTrainingConfig(PostTrainingCallback, GetTrainingParametersInJSONString());
    }

    private void PostTrainingCallback(string json)
    {
        var _myObject = JsonUtility.FromJson<TrainingConfigCallbackID>(json);
        _trainingID = _myObject.body;
        _cardCount = 0;
        CallPostCardReward();
    }

    private void CallPostCardReward(string json = null)
    {
        if(json != null)
            Debug.Log("CardPostResult: " + json);
        if (_cardCount >= cardRewards.Length)
        {
            Debug.Log("All card rewards have been posted on the server!");
            return;
        }
        var _cardJson = GetCardRewardInJSONString(cardRewards[_cardCount]);
        Debug.Log("Send card to server in JSON: " + _cardJson);
        _cardCount++;
        DatabaseConnection.Instance.PostCardReward(CallPostCardReward, _cardJson);
    }

    private string GetTrainingParametersInJSONString()
    {
        var _trainingConfigJson = new TrainingConfigJSON
        {
            name = eventTitle,
            startAt = ConvertSecondsToMilliseconds(ConvertDateInfoInTimeStamp(startEventDateInfo)),
            endAt = ConvertSecondsToMilliseconds(ConvertDateInfoInTimeStamp(endEventDateInfo)),
            story = eventStory,
            isOngoing = false,
            bondReward = this.bondReward,
            bonusBondReward = this.bonusBondReward,
            toyoTrainingConfirmationMessage = sendToyoToTrainingPopUp,
            inTrainingMessage = this.inTrainingMessage,
            losesMessage = this.losesMessage,
            rewardMessage = alreadyWon,
            blows = GetPossibleActionsStrings(),
            blowsConfig = blowConfigs
        };

        var _jsonString = JsonUtility.ToJson(_trainingConfigJson);
        return _jsonString;
    }

    private string[] GetPossibleActionsStrings()
    {
        var _blows = new string[possibleActions.Length];
        for (var _i = 0; _i < possibleActions.Length; _i++)
            _blows[_i] = possibleActions[_i].id.ToString();

        return _blows;
    }

    private string GetCardRewardInJSONString(CardRewardSO card)
    {
        var _cardRewardJson = new CardRewardEventJSON
        {
            trainingEventId = _trainingID,
            toyoPersonaId = DatabaseConnection.Instance.blockchainIntegration.isProduction 
                ? card.toyoPersona.objectId_prod : card.toyoPersona.objectId_dev,
            correctBlowsCombinationIds = TrainingConfig.Instance.GetCombinationInStringArray(card.correctCombination),
            cardReward = new CardRewardJSON
            {
                name = card.cardName,
                description = card.description,
                rotText = card.memory,
                type = card.cardType.ToString(),
                cardId = card.id.ToString(),
                imageUrl = card.imageURL
            }
        };
        
        var _jsonString = JsonUtility.ToJson(_cardRewardJson);
        return _jsonString;
    }
}

[Serializable]
public class BlowConfig
{
    [Header("Number of blows")]
    public int qty;
    [Header("Duration in minutes")]
    public int duration;
}

[Serializable]
public class DateInfo
{
    [Header("Date")]
    public int day;
    public int month;
    public int year;
    [Header("Time UTC")]
    public int hour;
    public int minute;
    public int second;
}