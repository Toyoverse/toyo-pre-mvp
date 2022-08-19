using System;
using System.Collections.Generic;
using Database;
using UnityEngine;
using static TimeTools;

[CreateAssetMenu(fileName = "TrainingConfigSO", menuName = "ScriptableObject/TrainingConfigSO")]
public class TrainingConfigSO : ScriptableObject
{
    [Header("Event Default strings")] 
    public string eventTitle;
    public string inTrainingMessage;
    public string rewardTitle;
    public string losesMessage;
    public string alreadyWon;
    public string eventStory;
    public string sendToyoToTrainingPopUp;

    [Header("Events dates")]
    public DateInfo startEventDateInfo;
    public DateInfo endEventDateInfo;
    
    [Header("Actions")]
    public TrainingActionSO[] possibleActions;
    public int minimumActionsToPlay;
    public BlowConfig[] blowConfigs;
    
    [Header(("Rewards"))]
    public CardRewardSO[] cardRewards;
    public float boundReward;
    public float bonusBoundReward;

    [Header("Training Event ID")]
    public string id;

    public void SendToServer()
    {
        Debug.Log("Send TrainingConfig to server initializing...");
        DatabaseConnection.Instance.PostTrainingConfig(SendCallback, GetTrainingParametersInJSONString());
    }

    private void SendCallback(string json)
        => Debug.Log("TrainingConfig posted to server successfully!");

    private string GetTrainingParametersInJSONString()
    {
        var _blows = new string[possibleActions.Length];
        for (var _i = 0; _i < possibleActions.Length; _i++)
            _blows[_i] = possibleActions[_i].id.ToString();

        var _trainingConfigJSON = new TrainingConfigJSON
        {
            name = eventTitle,
            startAt = ConvertDateInfoInTimeStamp(startEventDateInfo).ToString(),
            endAt = ConvertDateInfoInTimeStamp(endEventDateInfo).ToString(),
            story = eventStory,
            isOngoing = false,
            bondReward = this.boundReward,
            bonusBondReward = this.bonusBoundReward,
            toyoTrainingConfirmationMessage = sendToyoToTrainingPopUp,
            inTrainingMessage = this.inTrainingMessage,
            losesMessage = this.losesMessage,
            rewardMessage = alreadyWon,
            blows = _blows,
            blowsConfig = blowConfigs
        };

        var _jsonString = JsonUtility.ToJson(_trainingConfigJSON);
        return _jsonString;
    }

    private List<int> GetPossibleActionsIdList()
    {
        var _list = new List<int>();
        foreach (var _trainingActionSo in possibleActions) 
            _list.Add(_trainingActionSo.id);

        return _list;
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