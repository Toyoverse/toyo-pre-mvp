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
    private int _cardCount = 0;

    public void SendToServer()
    {
        Debug.Log("Send TrainingConfig to server in JSON: " + GetTrainingParametersInJSONString());
        DatabaseConnection.Instance.PostTrainingConfig(PostTrainingCallback, GetTrainingParametersInJSONString());
    }

    private void PostTrainingCallback(string json)
    {
        var _myObject = JsonUtility.FromJson<TrainingConfigCallbackID>(json);
        _trainingID = _myObject.body;
        Debug.Log("ResultMessage:" + _myObject.message + " | Body: " + _trainingID);
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
        DatabaseConnection.Instance.PostCardReward(CallPostCardReward, _cardJson);
        _cardCount++;
    }

    private string GetTrainingParametersInJSONString()
    {
        var _trainingConfigJson = new TrainingConfigJSON()
        {
            name = eventTitle,
            startAt = (int)ConvertDateInfoInTimeStamp(startEventDateInfo),
            endAt = (int)ConvertDateInfoInTimeStamp(endEventDateInfo),
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
        var _cardRewardJson = new CardRewardEventJSON()
        {
            trainingEventId = _trainingID,
            toyoPersonaId = card.toyoPersona.objectId,
            correctBlowsCombinationIds = GetCombinationInStringArray(card.correctCombination),
            cardReward = new()
            {
                name = card.name,
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

    private string[] GetCombinationInStringArray(TrainingActionSO[] combination)
    {
        var _result = new string[combination.Length];
        for (var _i = 0; _i < combination.Length; _i++)
            _result[_i] = combination[_i].id.ToString();
        
        return _result;
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