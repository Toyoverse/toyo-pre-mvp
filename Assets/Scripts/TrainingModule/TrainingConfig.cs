using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.UI;

[Serializable]
public class TrainingConfig : Singleton<TrainingConfig>
{
    [Header("Debug")]
    public bool disableTrainingModule;
    public bool ignoreTrainingTimer;
    [Header("Base data")] public TrainingConfigSO trainingConfigSo;

    //Current config usable
    [HideInInspector] public TrainingActionSO[] possibleActions;
    [HideInInspector] public TrainingActionSO[] correctCombination;
    [HideInInspector] public TrainingMode[] trainingModes;
    [HideInInspector] public CardRewardSO cardRewardReward;
    [HideInInspector] public int minimumActionsToPlay;
    
    //Default strings
    [HideInInspector] public string eventTitle;
    [HideInInspector] public string inTrainingTitle;
    
    //Default phrases for reward description
    [HideInInspector] public string rewardTitle;
    [HideInInspector] public string losesMiniGame;
    [HideInInspector] public string alreadyWon;
    
    private TrainingMode _selectedMode;
    private BlowConfig _selectedBlowConfig;
    [HideInInspector] public TrainingMode GetSelectedMode() => _selectedMode;
    [HideInInspector] public void SetSelectedMode(TrainingMode trainingMode) => _selectedMode = trainingMode;
    [HideInInspector] public BlowConfig GetSelectedBlowConfig() => _selectedBlowConfig;
    [HideInInspector] public void SetSelectedBlowConfig(BlowConfig blowConfig) => _selectedBlowConfig = blowConfig;
    private TOYO_RARITY _selectedToyoRarity = TOYO_RARITY.COMMON; //TODO: Get rarity of Toyo selected
    [HideInInspector] public TOYO_RARITY GetSelectedToyoRarity() => _selectedToyoRarity;
    
    //TODO: Get real variables in server
    [HideInInspector] public long endEventTimeStamp = 0;
    [HideInInspector] public float investValue = 0;
    [HideInInspector] public float receiveValue = 0;
    [HideInInspector] public int durationValue = 0; //Duration in minutes

    public long endTrainingTimeStamp { get; private set; }
    //
    
    private TrainingActionType _oldTypeSelected;
    public void SetOldTypeActionSelected(TrainingActionType type) => _oldTypeSelected = type;
    public TrainingActionType GetOldTypeActionSelected() => _oldTypeSelected;
    public bool OldTypeSelectedIsNone() => _oldTypeSelected == TrainingActionType.None;
    

    //
    public int selectedActionID { get; private set; }
    public void SetSelectedID(int newID) => selectedActionID = newID;
    public Dictionary<int, TrainingActionSO> selectedActionsDict = new ();
    public List<TrainingActionSO> selectedTrainingActions;
    public bool IsMinimumActionsToPlay() => selectedActionsDict.Count >= minimumActionsToPlay;

    //
    private bool _inTraining = false;
    public void SetInTraining(bool on)
    {
        _inTraining = on;
        SetTrainingTimeStamp();
    }

    public bool IsInTraining() => _inTraining;
    public int GetTrainingTimeRemain()
    {
        var _secondsRemain = endTrainingTimeStamp - GetActualTimeStamp();
        return ConvertSecondsInMinutes((int)_secondsRemain);
    }
    
    public int GetEventTimeRemain()
    {
        var _secondsRemain = endEventTimeStamp - GetActualTimeStamp();
        return ConvertSecondsInMinutes((int)_secondsRemain);
    }

    private void Start()
    {
        possibleActions = trainingConfigSo.possibleActions;
        correctCombination = trainingConfigSo.correctCombination;
        cardRewardReward = trainingConfigSo.cardReward;
        trainingModes = trainingConfigSo.trainingModes;
        losesMiniGame = trainingConfigSo.losesMiniGame;
        alreadyWon = trainingConfigSo.alreadyWon;
        rewardTitle = trainingConfigSo.rewardTitle;
        eventTitle = trainingConfigSo.eventTitle;
        minimumActionsToPlay = trainingConfigSo.minimumActionsToPlay;
        inTrainingTitle = trainingConfigSo.inTrainingTitle;
        endEventTimeStamp = trainingConfigSo.endEventTimeStamp;
    }

    public List<TRAINING_RESULT> CompareCombination(List<TrainingActionSO> selectedActions)
    {
        var _listResult = new List<TRAINING_RESULT>();
        for (var _i = 0; _i < selectedActions.Count; _i++)
            _listResult.Add(GetResultToAction(selectedActions[_i], _i));

        return _listResult;
    }

    private TRAINING_RESULT GetResultToAction(TrainingActionSO selectedAction, int position)
    {
        var _result = TRAINING_RESULT.TOTALLY_WRONG;

        for (var _i = 0; _i < correctCombination.Length; _i++)
        {
            if (correctCombination[_i] != selectedAction) continue;
            _result += 1;
            if (position == _i)
                _result += 1;
        }
        Debug.Log(_result);
        return _result;
    }
    
    public void ApplyTrainingMode()
    {
        foreach (var _mode in trainingModes)
        {
            if (_mode.toyoRarities.Any(rarity => rarity == GetSelectedToyoRarity()))
            {
                SetSelectedMode(_mode);
                Debug.Log(_mode.toyoRarities[0]);
                return;
            }

            Debug.Log("ToyoRarity not found in TrainingModes. (rarity: " + GetSelectedToyoRarity());
        }
    }
    
    public void ApplyBlowConfig()
    {
        foreach (var _blowConfig in GetSelectedMode().blowConfigs)
        {
            if(_blowConfig.blows != selectedActionsDict.Count)
                continue;
            SetSelectedBlowConfig(_blowConfig);
            return;
        }

        SetSelectedBlowConfig(null);
        Debug.Log("BlowConfig not found. (actionsCount: " + selectedActionsDict.Count);
    }

    private void SetBlowConfigCalculationValues()
    {
        investValue = GetSelectedBlowConfig().invest;
        receiveValue = GetSelectedBlowConfig().reward;
        durationValue = GetSelectedBlowConfig().duration;
    }

    private void ResetCalculationValues()
    {
        investValue = 0;
        receiveValue = 0;
        durationValue = 0;
    }
    
    public void ApplyRewardsCalculation()
    {
        if (GetSelectedBlowConfig() == null)
        {
            ResetCalculationValues();
            return;
        }
        SetBlowConfigCalculationValues();
    }
    
    private void SetTrainingActionList(Dictionary<int, TrainingActionSO> actionsDict)
    {
        selectedTrainingActions = new();
        var _i = 0;
        while (_i < actionsDict.Count)
        {
            foreach (var _action in actionsDict.Where(action => _i == action.Key))
            {
                selectedTrainingActions.Add(_action.Value);
                _i++;
                break;
            }
        }
    }
    
    public void ResetAllTrainingModule(Action callback)
    {
        SetTrainingActionList(selectedActionsDict);
        ResetSelectedActionsDictionary();
        callback?.Invoke();
    }
    
    private void ResetSelectedActionsDictionary() => selectedActionsDict = new Dictionary<int, TrainingActionSO>();
    
    public List<TrainingActionSO> GetFilteredActions(TrainingActionType filter) 
        => possibleActions.Where(action => action.type == filter).ToList();
    
    public List<TrainingActionSO> GetFilteredActionsOnOldType() 
        => possibleActions.Where(action => action.type == _oldTypeSelected).ToList();

    public void SetTrainingTimeStamp()
    {
        if (GetSelectedBlowConfig() == null)
            return;
        SetEndTrainingTimeStamp(GetFinishTrainingEpoch(GetActualTimeStamp(), GetSelectedBlowConfig().duration));
    }
    
    private void SetEndTrainingTimeStamp(long timeStamp) => endTrainingTimeStamp = timeStamp;

    //TIMESTAMP METHODS
    private readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    protected long GetTimeStampFromDate(DateTime date)
    {
        var _elapsedTime = date - _epoch;
        return (long)_elapsedTime.TotalSeconds;
    }

    protected long GetActualTimeStamp()
        => (long)System.DateTime.UtcNow.Subtract(new System.DateTime(
            1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    
    private void TestEpoch() 
    {
        Debug.Log("GetActualTimeStamp: "+ GetActualTimeStamp());
        Debug.Log("GetTimeStampFromDate DateTime.UtcNow: "+ GetTimeStampFromDate(DateTime.UtcNow));
        Debug.Log("GetFinishTrainingEpoch (6h): "+ GetFinishTrainingEpoch(GetActualTimeStamp(), 360));
    }

    private long GetFinishTrainingEpoch(long startTrainingEpoch, int trainingDurationInMinutes) 
        => startTrainingEpoch + GetSecondsInMinutes(trainingDurationInMinutes);

    private int GetSecondsInMinutes(int minutes)
    {
        var _result = 0;
        while (minutes > 0)
        {
            _result += 60;
            minutes--;
        }
        return _result;
    }

    private int ConvertSecondsInMinutes(int seconds)
    {
        var _result = 0;
        while (seconds > 60)
        {
            _result++;
            seconds -= 60;
        }
        return _result;
    }
}