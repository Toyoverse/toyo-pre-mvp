using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TrainingConfig : Singleton<TrainingConfig>
{
    [Header("Debug")]
    public bool disableTrainingModule;
    public bool ignoreTrainingTimer;
    [Header("Base data")] public TrainingConfigSO trainingConfigSo;

    //TODO: Get correct Toyo selected
    public ToyoPersonaSO selectedToyoPersona;

    //Current config usable
    [HideInInspector] public TrainingActionSO[] possibleActions;
    [HideInInspector] public BlowConfig[] blowConfigs;
    [HideInInspector] public CardRewardSO[] cardRewards;
    [HideInInspector] public int minimumActionsToPlay;
    [HideInInspector] public float boundReward;
    [HideInInspector] public float bonusBoundReward;
    
    //Default strings
    [HideInInspector] public string eventTitle;
    [HideInInspector] public string inTrainingMessage;
    [HideInInspector] public string eventStory;
    [HideInInspector] public string sendToyoToTrainingPopUp;

    //Default phrases for reward description
    [HideInInspector] public string rewardTitle;
    [HideInInspector] public string losesMiniGame;
    [HideInInspector] public string alreadyWon;

    public bool loreScreenAlreadyOpen { get; private set; }
    public void LoreScreenOpen() => loreScreenAlreadyOpen = true;
    private BlowConfig _selectedBlowConfig;
    [HideInInspector] public BlowConfig GetSelectedBlowConfig() => _selectedBlowConfig;
    [HideInInspector] public void SetSelectedBlowConfig(BlowConfig blowConfig) => _selectedBlowConfig = blowConfig;
    private TOYO_RARITY _selectedToyoRarity = TOYO_RARITY.COMMON; //TODO: Get rarity of Toyo selected
    [HideInInspector] public TOYO_RARITY GetSelectedToyoRarity() => _selectedToyoRarity;
    
    //TODO: Get real variables in server
    [HideInInspector] public long startEventTimeStamp = 0;
    [HideInInspector] public long endEventTimeStamp = 0;
    //[HideInInspector] public float investValue = 0;
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
    public int GetTrainingTimeRemainInMinutes()
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
        //allCorrectCombinationList = trainingConfigSo.correctCombinations;
        cardRewards = trainingConfigSo.cardRewards;
        blowConfigs = trainingConfigSo.blowConfigs;
        losesMiniGame = trainingConfigSo.losesMessage;
        alreadyWon = trainingConfigSo.alreadyWon;
        rewardTitle = trainingConfigSo.rewardTitle;
        eventTitle = trainingConfigSo.eventTitle;
        minimumActionsToPlay = trainingConfigSo.minimumActionsToPlay;
        inTrainingMessage = trainingConfigSo.inTrainingMessage;
        startEventTimeStamp = ConvertDateInfoInTimeStamp(trainingConfigSo.startEventDateInfo);
        endEventTimeStamp = ConvertDateInfoInTimeStamp(trainingConfigSo.endEventDateInfo);
        eventStory = trainingConfigSo.eventStory;
        sendToyoToTrainingPopUp = trainingConfigSo.sendToyoToTrainingPopUp;
        boundReward = trainingConfigSo.boundReward;
        bonusBoundReward = trainingConfigSo.bonusBoundReward;
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
        TrainingActionSO[] _correctCombination = null;
        foreach (var _card in cardRewards)
        {
            if (selectedToyoPersona != _card.toyoPersona) //TODO: Need testing
                continue;
            _correctCombination = _card.correctCombination; //TODO: Need testing
        }
        if(_correctCombination == null)
            Debug.LogError("Correct combination not found - ToyoPersona: " + selectedToyoPersona.toyoName);

        for (var _i = 0; _i < _correctCombination.Length; _i++)
        {
            if (_correctCombination[_i] != selectedAction) continue;
            _result += 1; 
            if (position == _i)
                _result += 1;
        }
        Debug.Log(_result);
        return _result;
    }

    public void ApplyBlowConfig()
    {
        foreach (var _blowConfig in blowConfigs)
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
        //investValue = GetSelectedBlowConfig().invest;
        //receiveValue = GetSelectedBlowConfig().reward;
        receiveValue = boundReward;
        durationValue = GetSelectedBlowConfig().duration;
    }

    private void ResetCalculationValues()
    {
        //investValue = 0;
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
        var _timeOut = 0;
        while (_i < actionsDict.Count)
        {
            if (_i != _timeOut)
                _i = 99;
            foreach (var _action in actionsDict)
            {
                if (_i == _action.Key)
                {
                    selectedTrainingActions.Add(_action.Value);
                    _i++;
                    break;
                }
            }

            _timeOut++;
        }
    }
    
    public void ResetAllTrainingModule()
    {
        SetTrainingActionList(selectedActionsDict);
        ResetSelectedActionsDictionary();
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

    private long ConvertDateInfoInTimeStamp(DateInfo dateInfo)
    {
        var _dateTime = new DateTime(dateInfo.year, dateInfo.month, dateInfo.day, 
            dateInfo.hour, dateInfo.minute, dateInfo.second, DateTimeKind.Utc);
        return GetTimeStampFromDate(_dateTime);
    }
    //

    public void AddToSelectedActionsDict(int key, TrainingActionSO action)
    {
        Debug.Log("Try add selectedActionDict: key_" + key + "|action_" + action.name);
        if (selectedActionsDict.ContainsKey(key))
            selectedActionsDict[key] = action;
        else
            selectedActionsDict.Add(key, action);
    }

    public void RemoveActionToDict(int key) => selectedActionsDict.Remove(key);
    
    public CardRewardSO GetCardReward()
    {
        for (var _i = 0; _i < Instance.cardRewards.Length; _i++)
        {
            if(Instance.cardRewards[_i].toyoPersona != Instance.selectedToyoPersona)
                continue;
            return Instance.cardRewards[_i];
        }

        Debug.LogError("CardReward not found - Persona: " + Instance.selectedToyoPersona.toyoName);
        return null;
    }
}