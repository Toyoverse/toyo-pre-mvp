using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TrainingConfigSO", menuName = "ScriptableObject/TrainingConfigSO")]
public class TrainingConfigSO : ScriptableObject
{
    [Header("Default strings")] 
    public string eventTitle;
    public string inTrainingTitle;
    public string rewardTitle;
    public string losesMiniGame;
    public string alreadyWon;
    public string lore;

    [Header("Events dates")]
    public DateInfo startEventDateInfo;
    public DateInfo endEventDateInfo;
    
    [Header("Actions")]
    public TrainingActionSO[] possibleActions;
    public ToyoActionCombination[] correctCombinations;
    public int minimumActionsToPlay;
    public TrainingMode[] trainingModes;
    
    [Header(("Reward"))]
    public CardRewardSO cardReward;
    
    public void SendToServer()
    {
        //TODO: Implement post to server
        Debug.Log("Send to server TEST! Yay!");
    }
}

[Serializable]
public class TrainingMode
{
    //public TOYO_RARITY[] toyoRarities;
    public BlowConfig[] blowConfigs;
}

[Serializable]
public class BlowConfig
{
    public int blows;
    public float reward;
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

[Serializable]
public class ToyoActionCombination
{
    public ToyoPersonaSO toyoPersona;
    public TrainingActionSO[] actions;
}