using System;
using Database;
using UnityEngine;

[CreateAssetMenu(fileName = "TrainingConfigSO", menuName = "ScriptableObject/TrainingConfigSO")]
public class TrainingConfigSO : ScriptableObject
{
    public TrainingActionSO[] possibleActions;
    public TrainingActionSO[] correctCombination;
    public int minimumActionsToPlay;
    public CardRewardSO cardRewardReward;
    public TrainingMode[] trainingModes;

    [Header("Default strings")] 
    public string eventTitle;
    public string inTrainingTitle;

    [Header("Default phrases for reward title and description")]
    public string rewardTitle;
    public string losesMiniGame;
    public string alreadyWon;
}

[Serializable]
public class TrainingMode
{
    public TOYO_RARITY[] toyoRarities;
    public BlowConfig[] blowConfigs;
}

[Serializable]
public class BlowConfig
{
    public int blows;
    public float invest;
    [Tooltip("Duration in minutes")]
    public int duration;
    public float reward;
}