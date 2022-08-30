using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using UnityEngine;
using UnityEngine.Serialization;
using static TimeTools;

[Serializable]
public class TrainingConfig : Singleton<TrainingConfig>
{
    [Header("Debug")]
    public bool disableTrainingModule;
    public bool ignoreTrainingTimer;
    
    [Header("If offline - base data")] 
    public bool useOfflineData;
    public TrainingConfigSO trainingConfigSo;

    [Header("Source - Required to find objects when receiving information from the server")]
    public List<TrainingActionSO> allTrainingActionsInProject;
    
    public ToyoObject selectedToyoObject { get; private set; }
    public void SetSelectedToyoObject(ToyoObject toyoObject) => selectedToyoObject = toyoObject;

    //Current config usable
    [HideInInspector] public TrainingActionSO[] possibleActions;
    [HideInInspector] public BlowConfig[] blowConfigs;
    [HideInInspector] public CardRewardSO[] cardRewards;
    [HideInInspector] public int minimumActionsToPlay;
    [HideInInspector] public float bondReward;
    [HideInInspector] public float bonusBondReward;
    
    //Default strings
    [HideInInspector] public string eventTitle;
    [HideInInspector] public string inTrainingMessage;
    [HideInInspector] public string eventStory;
    [HideInInspector] public string sendToyoToTrainingPopUp;

    //Default phrases for reward description
    [HideInInspector] public string rewardTitle = "Congratulations! Here's your reward";
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
    
    private List<ToyoTrainingInfo> _listOfToyosInTraining;
    public List<ToyoTrainingInfo> GetListOfToyosInTraining => _listOfToyosInTraining;

    //
    public int selectedActionID { get; private set; }
    public void SetSelectedID(int newID) => selectedActionID = newID;
    public Dictionary<int, TrainingActionSO> selectedActionsDict = new ();
    public List<TrainingActionSO> selectedTrainingActions;
    public bool IsMinimumActionsToPlay() => selectedActionsDict.Count >= minimumActionsToPlay;

    //
    private bool _selectedToyoIsInTraining;
    public bool IsInTraining() => _selectedToyoIsInTraining;

    public void SetIsInTraining()
    {
        var _tokenID = ToyoManager.GetSelectedToyo().tokenId;
        var _isInTraining = IsInTrainingCheckInServer(_tokenID);
        if (_isInTraining)
            SetTrainingTimeStamp();
        _selectedToyoIsInTraining = _isInTraining;
    }

    private bool IsInTrainingCheckInServer(string tokenID)
    {
        //TODO: GET TRAINING IN SERVER
        return false;
    }

    public void SetInTrainingOnServer()
    {
        var _tokenID = ToyoManager.GetSelectedToyo().tokenId;
        //TODO: SEND TOYO TO TRAINING IN SERVER ...AND BLOCKCHAIN?
    }

    public void ClaimCallInServer()
    {
        //TODO: SEND CLAIM TO SERVER ...AND BLOCKCHAIN?
    }
    
    public void InitializeTrainingModule()
    {
        if (Instance.disableTrainingModule)
        {
            ToyoManager.StartGame();
            return;
        }

        if (useOfflineData)
            SetTrainingConfigValues();
        else
            DatabaseConnection.Instance.GetCurrentTrainingConfig(OnGetTrainingSuccess);
    }

    public void OnGetTrainingSuccess(string json)
    {
        var _myObject = JsonUtility.FromJson<TrainingConfigJSON>(json);
        SetTrainingConfigValues(_myObject);
        ToyoManager.StartGame();
        return; //TODO: Remover StartGame e return quando API de Toyos em treinamento estiver pronta
        DatabaseConnection.Instance.CallGetInTrainingList(CreateToyosInTrainingList);
    }
    
    public void CreateToyosInTrainingList(string json)
    {
        var _myObject = JsonUtility.FromJson<ToyosInTrainingListJSON>(json);
        //TODO: GET ALL TRAININGS AND CREATE LIST
        _listOfToyosInTraining = new();
        foreach (var _trainingInfo in _myObject.trainingInfos)
            _listOfToyosInTraining.Add(_trainingInfo);
        
        Debug.Log("InTrainingList Details Success! Toyos in training: " + _listOfToyosInTraining.Count);
        ToyoManager.StartGame();
    }

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

    private void SetTrainingConfigValues(TrainingConfigJSON trainingConfigJson = null)
    {
        if (useOfflineData)
        {
            possibleActions = trainingConfigSo.possibleActions;
            cardRewards = trainingConfigSo.cardRewards;
            blowConfigs = trainingConfigSo.blowConfigs;
            losesMiniGame = trainingConfigSo.losesMessage;
            alreadyWon = trainingConfigSo.alreadyWon;
            //rewardTitle = trainingConfigSo.rewardTitle; TODO: GET CARD NAME?
            eventTitle = trainingConfigSo.eventTitle;
            minimumActionsToPlay = GetMinimumActionsToPlay(blowConfigs); 
            inTrainingMessage = trainingConfigSo.inTrainingMessage;
            startEventTimeStamp = ConvertDateInfoInTimeStamp(trainingConfigSo.startEventDateInfo);
            endEventTimeStamp = ConvertDateInfoInTimeStamp(trainingConfigSo.endEventDateInfo);
            eventStory = trainingConfigSo.eventStory;
            sendToyoToTrainingPopUp = trainingConfigSo.sendToyoToTrainingPopUp;
            bondReward = trainingConfigSo.bondReward;
            bonusBondReward = trainingConfigSo.bonusBondReward;
            return;
        }

        if(trainingConfigJson == null)
            Debug.LogError("TrainingConfig Json is null.");
        possibleActions = GetActionsFromIDs(trainingConfigJson.blows);
        //cardRewards = trainingConfigJson.cardRewards; //TODO GET CARDS
        blowConfigs = trainingConfigJson.blowsConfig;
        losesMiniGame = trainingConfigJson.losesMessage;
        alreadyWon = trainingConfigJson.rewardMessage;
        //rewardTitle = trainingConfigJson.rewardTitle; TODO: GET CARD NAME?
        eventTitle = trainingConfigJson.name;
        minimumActionsToPlay = GetMinimumActionsToPlay(blowConfigs); 
        inTrainingMessage = trainingConfigJson.inTrainingMessage;
        startEventTimeStamp = trainingConfigJson.startAt;
        endEventTimeStamp = trainingConfigJson.endAt;
        eventStory = trainingConfigJson.story;
        sendToyoToTrainingPopUp = trainingConfigJson.toyoTrainingConfirmationMessage;
        bondReward = trainingConfigJson.bondReward;
        bonusBondReward = trainingConfigJson.bonusBondReward;
    }

    private int GetMinimumActionsToPlay(BlowConfig[] blowConfigs)
    {
        var _result = 5; //Maximum possible blows
        foreach (var _blowConfig in blowConfigs)
        {
            if (_result > _blowConfig.qty)
                _result = _blowConfig.qty;
        }
        
        return _result;
    }

    private TrainingActionSO[] GetActionsFromIDs(string[] ids)
    {
        var _resultList = new TrainingActionSO[ids.Length];
        for (var _index = 0; _index < ids.Length; _index++)
        {
            foreach (var _trainingAction in allTrainingActionsInProject)
            {
                if (int.Parse(ids[_index]) != _trainingAction.id)
                    continue;
                _resultList[_index] = _trainingAction;
                break;
            }
        }

        return _resultList;
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
            if (Instance.selectedToyoObject.GetToyoName() != _card.toyoPersona.toyoName) //TODO: Need testing
                continue;
            _correctCombination = _card.correctCombination; //TODO: Need testing
        }

        if (_correctCombination == null)
        {
            Debug.LogError("Correct combination not found - ToyoPersona: " + Instance.selectedToyoObject.GetToyoName());
            return TRAINING_RESULT.NONE;
        }

        for (var _i = 0; _i < _correctCombination.Length; _i++)
        {
            if (_correctCombination[_i] != selectedAction) continue;
            _result += 1; 
            if (position == _i)
                _result += 1;
        }
        //Debug.Log(_result);
        return _result;
    }

    public void ApplyBlowConfig()
    {
        foreach (var _blowConfig in blowConfigs)
        {
            if(_blowConfig.qty != selectedActionsDict.Count)
                continue;
            SetSelectedBlowConfig(_blowConfig);
            return;
        }

        SetSelectedBlowConfig(null);
        Debug.Log("BlowConfig not found. - ActionsCount: " + selectedActionsDict.Count);
    }

    private void SetBlowConfigCalculationValues()
    {
        //investValue = GetSelectedBlowConfig().invest;
        //receiveValue = GetSelectedBlowConfig().reward;
        receiveValue = bondReward;
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

    private long GetFinishTrainingEpoch(long startTrainingEpoch, int trainingDurationInMinutes) 
        => startTrainingEpoch + GetSecondsInMinutes(trainingDurationInMinutes);

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
            if(Instance.cardRewards[_i].toyoPersona.toyoName != Instance.selectedToyoObject.GetToyoName())
                continue;
            return Instance.cardRewards[_i];
        }

        Debug.LogError("CardReward not found - Persona: " + Instance.selectedToyoObject.GetToyoName());
        return null;
    }
}