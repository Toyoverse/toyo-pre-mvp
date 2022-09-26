using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Newtonsoft.Json;
using Tools;
using UI;
using Unity.VisualScripting;
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
    public bool useOfflineTrainingControl;
    private Dictionary<string, ToyoTrainingInfo> _tempServerTraining = new();

    [Header("Source - Required to find objects when receiving information from the server")]
    public List<TrainingActionSO> allTrainingActionsInProject;

    [Header("Source - Required to find objects when receiving information from the server")]
    public List<CardRewardSO> allCardRewardsInProject;

    //Current config usable
    [HideInInspector] public string trainingEventID;
    [HideInInspector] public TrainingActionSO[] possibleActions;
    [HideInInspector] public BlowConfig[] blowConfigs;
    [HideInInspector] public int minimumActionsToPlay;
    [HideInInspector] public float bondReward;
    //Only offline use
    [HideInInspector] public CardRewardSO[] cardRewards; 
    [HideInInspector] public float bonusBondReward;
    
    //Default strings
    [HideInInspector] public string eventTitle;
    [HideInInspector] public string inTrainingMessage;
    [HideInInspector] public string eventStory;
    [HideInInspector] public string sendToyoToTrainingPopUp;
    [HideInInspector] public string selectActionsMessage = "Select your actions...";
    private string _trainingEventEndMessage = "This event has now ended.";

    //Default phrases for reward description
    [HideInInspector] public string rewardTitle = "TRAINING RESULTS";
    [HideInInspector] public string losesMiniGame;
    [HideInInspector] public string alreadyWon;

    public bool loreScreenAlreadyOpen { get; private set; }
    public void LoreScreenOpen() => loreScreenAlreadyOpen = true;
    private BlowConfig _selectedBlowConfig;
    [HideInInspector] public BlowConfig GetSelectedBlowConfig() => _selectedBlowConfig;
    [HideInInspector] public void SetSelectedBlowConfig(BlowConfig blowConfig) => _selectedBlowConfig = blowConfig;
    private TOYO_RARITY _selectedToyoRarity = TOYO_RARITY.COMMON; //TODO: Get rarity of Toyo selected
    [HideInInspector] public TOYO_RARITY GetSelectedToyoRarity() => _selectedToyoRarity;

    [HideInInspector] public long startEventTimeStamp;
    [HideInInspector] public long endEventTimeStamp;
    [HideInInspector] public float receiveValue;
    [HideInInspector] public int durationValue; //Duration in minutes
    
    public long GetSelectedToyoEndTrainingTimeStamp()
    {
        //var _result = ConvertMillisecondsToSeconds(GetCurrentTrainingInfo().endAt);
        var _result = GetCurrentTrainingInfo().endAt;
        //Print.Log("SelectedToyoEndTrainingTimeStamp: " + _result);
        return (long)_result;
    }

    private TrainingActionType _oldTypeSelected;
    public void SetOldTypeActionSelected(TrainingActionType type) => _oldTypeSelected = type;
    public TrainingActionType GetOldTypeActionSelected() => _oldTypeSelected;
    public bool OldTypeSelectedIsNone() => _oldTypeSelected == TrainingActionType.None;
    
    private List<ToyoTrainingInfo> _listOfToyosInTraining;
    public ToyoTrainingInfo GetToyoTrainingInfo(string tokenID)
        => _listOfToyosInTraining?.FirstOrDefault(training => tokenID == training.toyoTokenId);

    public ToyoTrainingInfo GetCurrentTrainingInfo()
    {
        var _tokenID = ToyoManager.GetSelectedToyo().tokenId;
        return Instance.GetToyoTrainingInfo(_tokenID);
    }

    //
    public int selectedActionID { get; private set; }
    public void SetSelectedID(int newID) => selectedActionID = newID;
    public List<ActionBkp> selectedActionsBkp = new ();
    public List<TrainingActionSO> selectedTrainingActions;
    
    public bool IsMinimumActionsToPlay() => selectedActionsBkp.Count >= minimumActionsToPlay;
    //

    public static string FailedGetEventMessage = "No training events are taking place at this time.";
    public static string GenericFailMessage = "Something went wrong, please reload the page and try again.";
    public static string SuccessClaimMessage = "You have successfully redeemed your reward!";
    private static string FailedClaimMessage = "Metamask Transaction Fail. Please try again.";
    public static string EventTimeDefaultText = "This event ends in: ";

    public bool trainingEventNotFound = false;

    public bool SelectedToyoIsInTraining()
    {
        var _tokenID = ToyoManager.GetSelectedToyo().tokenId;
        var _isInTraining = IsInTrainingCheckInServer(_tokenID);
        if (_isInTraining)
            SetTrainingActionList(GetToyoTrainingInfo(_tokenID).combination);

        return _isInTraining;
    }
    
    private bool IsInTrainingCheckInServer(string tokenID)
    {
        if(!useOfflineTrainingControl)
            return GetToyoTrainingInfo(tokenID) != null;

        if (!_tempServerTraining.ContainsKey(tokenID))
            _tempServerTraining.Add(tokenID, new ToyoTrainingInfo());
        return IsBiggerThenCurrentTimestamp(ConvertMillisecondsToSeconds(_tempServerTraining[tokenID].endAt));
    }

    public void SetInTrainingOnServer()
    {
        UpdateSelectedActionsByDict();
        
        if (useOfflineTrainingControl)
        {
            var _tokenID = ToyoManager.GetSelectedToyo().tokenId;
            _tempServerTraining[_tokenID].startAt = GetActualTimeStampInSeconds();
            _tempServerTraining[_tokenID].endAt = GetActualTimeStampInSeconds() + 60;
            _tempServerTraining[_tokenID].combination = GetCombinationInStringArray(selectedTrainingActions.ToArray());
            return;
        }
        
        DatabaseConnection.Instance.PostToyoInTraining(PostTrainingSendCallback, GetSelectedToyoTrainingInJSON());
    }

    private string GetSelectedToyoTrainingInJSON()
    {
        var _toyoTraining = new ToyoTrainingSendInfo
        {
            trainingId = trainingEventID,
            toyoTokenId = ToyoManager.GetSelectedToyo().tokenId,
            combination = GetCombinationInStringArray(selectedTrainingActions.ToArray())
        };
        var _jsonString = JsonUtility.ToJson(_toyoTraining);
        Print.Log("ToyoTrainingBody: " + _jsonString);
        return _jsonString;
    }
    
    public string[] GetCombinationInStringArray(TrainingActionSO[] combination)
    {
        var _result = new string[combination.Length];
        for (var _i = 0; _i < combination.Length; _i++)
            _result[_i] = combination[_i].id.ToString();
        
        return _result;
    }

    private void PostTrainingSendCallback(string json)
    {
        Print.Log("PostTrainingResult: " + json);
        DatabaseConnection.Instance.CallGetInTrainingList(CreateToyosInTrainingList);
    }

    public void ClaimCallInServer()
    {
        var _trainingInfo = Instance.GetCurrentTrainingInfo();
        if (!useOfflineTrainingControl)
        {
            DatabaseConnection.Instance.PutCloseTrainingValues(SuccessGetClaimParameters, 
                FailedGetClaimParameters, _trainingInfo.id);
            return;
        }
        
        //Using offline training control
        _tempServerTraining[ToyoManager.GetSelectedToyo().tokenId].endAt = GetActualTimeStampInSeconds();
    }

    /*public void GetRewardValues() => DatabaseConnection.Instance.GetRewardValues(SuccessGetClaimParameters,
                FailedGetClaimParameters, Instance.GetCurrentTrainingInfo().id);*/

    private void SuccessGetClaimParameters(string json)
    {
        var _trainingResult = JsonUtility.FromJson<TrainingResultJson>(json);
        if (_trainingResult.statusCode != 200)
        {
            GenericPopUp.Instance.ShowPopUp(GenericFailMessage);
            Loading.EndLoading?.Invoke();
            return;
        }

        var _claimParameters = new ClaimParameters
        {
            tokenID = ToyoManager.GetSelectedToyo().tokenId,
            bond = _trainingResult.body.bond,
            cardCode = _trainingResult.body.card.cardCode ?? "",
            signature = _trainingResult.body.signature,
            claimID = _trainingResult.body.id
        };
        
        DatabaseConnection.Instance.blockchainIntegration.ClaimToken(_claimParameters);
    }

    private void FailedGetClaimParameters(string json)
    {
        Loading.EndLoading?.Invoke();
        GenericPopUp.Instance.ShowPopUp(GenericFailMessage);
        Print.Log("FailedGetClaimJSON: " + json);
    }
    
    public void GenericFailedMessage()
    {
        Loading.EndLoading?.Invoke();
        GenericPopUp.Instance.ShowPopUp(GenericFailMessage);
    }

    public void SuccessClaim()
    {
        var _trainingInfo = Instance.GetCurrentTrainingInfo();
        DatabaseConnection.Instance.PostCloseTraining(PostCloseTrainingCallback, _trainingInfo.id);
    }

    private void PostCloseTrainingCallback(string json)
    {
        Loading.EndLoading += ShowSuccessClaimPopUp;
        DatabaseConnection.Instance.CallGetInTrainingList(UpdateInTrainingListAfterClaim);
    }

    private void ShowSuccessClaimPopUp() => GenericPopUp.Instance.ShowPopUp(SuccessClaimMessage, GoToMainMenu);
    private void ShowFailedClaimPopUp() => GenericPopUp.Instance.ShowPopUp(FailedClaimMessage, GoToMainMenu);

    public void FailedClaim()
    {
        /*Loading.EndLoading += ShowFailedClaimPopUp;
        DatabaseConnection.Instance.CallGetInTrainingList(UpdateInTrainingListAfterClaim);*/
        Loading.EndLoading?.Invoke();
        ShowFailedClaimPopUp();
    }

    private void GoToMainMenu() => ScreenManager.Instance.GoToScreen(ScreenState.MainMenu);
    
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
            DatabaseConnection.Instance.GetCurrentTrainingConfig(OnGetTrainingSuccess, OnGetTrainingFailed);
    }

    public void OnGetTrainingSuccess(string json)
    {
        var _myObject = JsonUtility.FromJson<TrainingConfigJSON>(json);
        SetTrainingConfigValues(_myObject);
        DatabaseConnection.Instance.CallGetInTrainingList(CreateToyosInTrainingList);
    }

    public void OnGetTrainingFailed(string json)
    {
        Instance.trainingEventNotFound = true;
        GenericPopUp.Instance.ShowPopUp(FailedGetEventMessage);
        /*ToyoManager.StartGame();
        Loading.EndLoading?.Invoke();*/
        DatabaseConnection.Instance.CallGetInTrainingList(CreateToyosInTrainingList);
    }
    
    public void CreateToyosInTrainingList(string json)
    {
        var _trainingScreen = _listOfToyosInTraining != null;
        CreateInTrainingList(json);
        Print.Log("InTrainingList Details Success! Toyos in training: " + _listOfToyosInTraining.Count);
        if(!_trainingScreen)
            ToyoManager.StartGame();
        else 
            ScreenManager.Instance.trainingModuleScript.UpdateTrainingAfterTrainingSuccess();
    }
    
    public void UpdateInTrainingListAfterClaim(string json)
    {
        CreateInTrainingList(json);
        DatabaseConnection.Instance.CallGetPlayerToyo(PlayerToyoCallback);
    }

    private void PlayerToyoCallback(string json) 
        => DatabaseConnection.Instance.blockchainIntegration.UpdateToyoIsStakedList(json, Loading.EndLoading);

    private void CreateInTrainingList(string json)
    {
        Print.Log("InTrainingListResult: " + json);
        var _myObject = JsonUtility.FromJson<ToyosInTrainingListJSON>(json);
        _listOfToyosInTraining = new();
        foreach (var _trainingInfo in _myObject.body)
            _listOfToyosInTraining.Add(_trainingInfo);
    }

    public int GetTrainingTimeRemainInMinutes()
    {
        var _secondsRemain = GetSelectedToyoEndTrainingTimeStamp() - GetActualTimeStampInSeconds();
        Print.Log("TrainingRemainInSeconds: " + _secondsRemain);
        if (_secondsRemain < 0)
            _secondsRemain = 0;
        return ConvertSecondsInMinutes((int)_secondsRemain);
    }

    public long GetTrainingTimeRemainInSeconds()
    {
        var _secondsRemain = GetSelectedToyoEndTrainingTimeStamp() - GetActualTimeStampInSeconds();
        if (_secondsRemain < 0)
            _secondsRemain = 0;
        return _secondsRemain;
    }

    public int GetEventTimeRemainInMinutes()
    {
        var _secondsRemain = endEventTimeStamp - GetActualTimeStampInSeconds();
        return ConvertSecondsInMinutes((int)_secondsRemain);
    }
    
    public int GetEventTimeRemainInSeconds() => (int)(endEventTimeStamp - GetActualTimeStampInSeconds());

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
            startEventTimeStamp = ConvertDateInfoInSecondsTimeStamp(trainingConfigSo.startEventDateInfo);
            endEventTimeStamp = ConvertDateInfoInSecondsTimeStamp(trainingConfigSo.endEventDateInfo);
            eventStory = trainingConfigSo.eventStory;
            sendToyoToTrainingPopUp = trainingConfigSo.sendToyoToTrainingPopUp;
            bondReward = trainingConfigSo.bondReward;
            bonusBondReward = trainingConfigSo.bonusBondReward;
            ToyoManager.StartGame();
            return;
        }

        if (trainingConfigJson == null)
        {
            Print.LogError("TrainingConfig Json is null.");
            return;
        }

        trainingEventID = trainingConfigJson.id;
        possibleActions = GetActionsFromIDs(trainingConfigJson.blows);
        //cardRewards = trainingConfigJson.cardRewards; //TODO GET CARDS
        blowConfigs = trainingConfigJson.blowsConfig;
        losesMiniGame = trainingConfigJson.losesMessage;
        alreadyWon = trainingConfigJson.rewardMessage;
        //rewardTitle = trainingConfigJson.rewardTitle; TODO: GET CARD NAME?
        eventTitle = trainingConfigJson.name;
        minimumActionsToPlay = GetMinimumActionsToPlay(blowConfigs); 
        inTrainingMessage = trainingConfigJson.inTrainingMessage;
        startEventTimeStamp = ConvertMillisecondsToSeconds(trainingConfigJson.startAt);
        endEventTimeStamp = ConvertMillisecondsToSeconds(trainingConfigJson.endAt);
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
        //allTrainingActionsInProject = GetAllTrainingActionInProject();
        var _resultList = new TrainingActionSO[ids.Length];
        for (var _index = 0; _index < ids.Length; _index++)
        {
            foreach (var _trainingAction in allTrainingActionsInProject)
            {
                if (int.Parse(ids[_index]) != _trainingAction.id)
                    continue;
                _resultList[_index] = _trainingAction;
                //Print.Log("POSSIBLE ACTION [" + _index + "]: " + _resultList[_index].name);
                break;
            }
        }

        return _resultList;
    }
    
    private TrainingActionSO GetActionByID(string id)
    {
        foreach (var _trainingAction in allTrainingActionsInProject)
        {
            if (int.Parse(id) != _trainingAction.id)
                continue;
            return _trainingAction;
        }

        Print.LogError("ID " + id + " not found in TrainingActionsList.");
        return null;
    }

    public List<TRAINING_RESULT> GetResultsByCombinationResult(BlowResult[] blowResults)
    {
        var _listResult = new List<TRAINING_RESULT>();
        for (var _i = 0; _i < blowResults.Length; _i++)
        {
            Print.Log("BlowResult[" + _i + "]: inc: " + blowResults[_i].includes 
                      + ", pos: " + blowResults[_i].position + ", blow: " + blowResults[_i].blow);
            var _result = GetResultToAction(blowResults[_i]);
            _listResult.Add(_result);
            Print.Log("BlowTrainingResult[" + _i + "]: " + _result);
        }

        return _listResult;
    }

    private TRAINING_RESULT GetResultToAction(BlowResult blowResult)
    {
        var _result = TRAINING_RESULT.TOTALLY_WRONG;
        if (blowResult.includes)
            _result += 1;
        if (blowResult.position)
            _result += 1;
        return _result;
    }

    public void ApplyBlowConfig()
    {
        foreach (var _blowConfig in blowConfigs)
        {
            if(_blowConfig.qty != selectedActionsBkp.Count)
                continue;
            SetSelectedBlowConfig(_blowConfig);
            return;
        }

        SetSelectedBlowConfig(null);
        Print.Log("BlowConfig not found. - ActionsCount: " + selectedActionsBkp.Count);
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
    
    private void SetTrainingActionList()
    {
        selectedTrainingActions = new List<TrainingActionSO>();
        for (var _index = 0; _index < selectedActionsBkp.Count; _index++)
            selectedTrainingActions.Add(FindActionOrderByPosition(_index).action);
    }

    private ActionBkp FindActionOrderByPosition(int position)
    {
        for(var _i = 0; _i < selectedActionsBkp.Count; _i++)
            if (selectedActionsBkp[_i].position == position)
                return selectedActionsBkp[_i];
        return null;
    }
    
    private ActionBkp FindActionOrderByButtonID(int buttonID)
    {
        for(var _i = 0; _i < selectedActionsBkp.Count; _i++)
            if (selectedActionsBkp[_i].buttonID == buttonID)
                return selectedActionsBkp[_i];
        return null;
    }
    
    private void SetTrainingActionList(string[] actionsIDs)
    {
        selectedTrainingActions = new List<TrainingActionSO>();

        for (var _i = 0; _i < actionsIDs.Length; _i++)
            selectedTrainingActions.Add(GetActionByID(actionsIDs[_i]));
    }
    
    public void ResetAllTrainingModule()
    {
        SetTrainingActionList();
        ResetSelectedActionsDictionary();
    }
    
    private void ResetSelectedActionsDictionary()
    {
        selectedActionsBkp.Clear();
        selectedActionsBkp = new List<ActionBkp>();
    }

    public List<TrainingActionSO> GetFilteredActions(TrainingActionType filter) 
        => possibleActions.Where(action => action.type == filter).ToList();
    
    public List<TrainingActionSO> GetFilteredActionsOnOldType() 
        => possibleActions.Where(action => action.type == _oldTypeSelected).ToList();

    public void AddToSelectedActionsDict(int buttonID, TrainingActionSO trainingActionSo)
    {
        Print.Log("Try add selectedActionDict: key_" + buttonID + "|action_" + trainingActionSo.name);

        if (FindActionOrderByButtonID(buttonID) == null)
            selectedActionsBkp.Add(new ActionBkp
            {
                buttonID = buttonID,
                position = selectedActionsBkp.Count,
                action = trainingActionSo
            });
        else
            FindActionOrderByButtonID(buttonID).action = trainingActionSo;
    }

    public void RemoveActionFromBKP(int buttonID)
    {
        var _position = FindActionOrderByButtonID(buttonID).position;
        selectedActionsBkp.Remove(FindActionOrderByButtonID(buttonID));
        ReloadActionsBkpAfterRemove(_position);
    }

    private void ReloadActionsBkpAfterRemove(int positionRemoved)
    {
        for (var _i = 0; _i < selectedActionsBkp.Count; _i++)
        {
            if(_i < positionRemoved)
                continue;
            selectedActionsBkp[_i].position -= 1;
        }
    }

    private bool IsBiggerThenCurrentTimestamp(long timestamp) => timestamp > GetActualTimeStampInSeconds();

    private void UpdateSelectedActionsByDict()
    {
        selectedTrainingActions = new List<TrainingActionSO>();
        for (var _index = 0; _index < selectedActionsBkp.Count; _index++)
            selectedTrainingActions.Add(FindActionOrderByPosition(_index).action);
    }

    public int GetTrainingTotalDuration(ToyoTrainingInfo trainingInfo)
    {
        Print.Log("end: " + trainingInfo.endAt + " start: " + trainingInfo.startAt);
        //var _sub = ConvertMillisecondsToSeconds(trainingInfo.endAt - trainingInfo.startAt);
        var _sub = trainingInfo.endAt - trainingInfo.startAt;
        var _result = ConvertSecondsInMinutes((int)_sub);
        Print.Log("trainingTotalDuration: " + _result);
        return _result;
    }

    public CardRewardSO GetCardFromID(int id)
    {
        for (var _i = 0; _i < allCardRewardsInProject.Count; _i++)
        {
            if (allCardRewardsInProject[_i].id != id)
                continue;
            return allCardRewardsInProject[_i];
        }

        Print.Log("CardReward not fond - ID: " + id);
        return null;
    }
    
    public string GetEventNameByActualTrainingId()
    {
        var _result = eventTitle;
        if (Instance.GetCurrentTrainingInfo().startAt < startEventTimeStamp)
            _result = _trainingEventEndMessage;
        return _result;
    }
}

[Serializable]
public class ActionBkp
{
    public int buttonID;
    public int position;
    public TrainingActionSO action;
}