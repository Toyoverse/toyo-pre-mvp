using System;
using UnityEngine;

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
    
    public void SendToServer()
    {
        //TODO: Implement post to server
        Debug.Log("Send to server TEST! Yay!");
    }
}

[Serializable]
public class BlowConfig
{
    public int blows;
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