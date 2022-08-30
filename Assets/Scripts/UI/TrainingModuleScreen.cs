using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class TrainingModuleScreen : UIController
{
    public string eventTitleName;
    public string eventTimeName;
    
    public string startTrainingButtonName;
    public GameObject[] combPoolObjects;
    public GameObject[] removeButtonsPool;
    public Image[] combPoolImages;
    public Image[] progressImages;
    public Sprite progressDefaultSprite;
    public Transform combPoolContainer;

    [Header("In Training names")] public string inTrainingBoxName;
    public string inTrainingTimeButtonName;

    [Header("Reward names")] public string rewardTitleName;
    public string receiveName;
    public string durationName;

    [Header("Actions Selection names")] public string actionSelectionAreaName;
    public string actionScrollName;
    public string previewActionName;
    public FontAsset fontAsset;

    protected override void UpdateUI()
    {
        CheckAndRevealStartButton();
        TrainingConfig.Instance.ApplyRewardsCalculation();
        UpdateTextsInUI();
        EnableTrainingUI();
        UpdateProgressTraining();
    }

    private void UpdateTextsInUI()
    {
        SetTextInLabel(eventTitleName, TrainingConfig.Instance.eventTitle);
        SetTextInLabel(eventTimeName, ConvertMinutesToString(TrainingConfig.Instance.GetEventTimeRemain()));
        SetTextInLabel(receiveName, /*"Receive: $" + */TrainingConfig.Instance.receiveValue.ToString());
        SetTextInLabel(durationName, /*"Duration: " + */ConvertMinutesToString(TrainingConfig.Instance.durationValue));
    }

    public override void ActiveScreen()
    {
        base.ActiveScreen();
        ResetCombinationPool();
        TrainingConfig.Instance.SetIsInTraining();
        ApplyInTrainingActions();
        UpdateUI();
    }

    public override void DisableScreen()
    {
        CheckToResetTrainingModule();
        base.DisableScreen();
    }

    private void CheckToResetTrainingModule()
    {
        if (ScreenManager.ScreenState == ScreenState.TrainingActionSelect
            /*|| TrainingConfig.Instance.IsInTraining()*/)
            return;
        TrainingConfig.Instance?.ResetAllTrainingModule( /*ClearPossibleActionsEvents*/);
        ClearActionsImages();
    }

    public void OpenActionSelection(int buttonID)
    {
        TrainingConfig.Instance.SetSelectedID(buttonID);
        OpenActionSelectionScreen();
    }

    public void RemoveAction(int id)
    {
        TrainingConfig.Instance.SetSelectedID(id);
        DisableRemoveButton(id);
        SetActionSprite(id, null);
        SetProgressSprite(id, progressDefaultSprite);
        TrainingConfig.Instance.RemoveActionToDict(id);
        DisableActionObject(id);
        TrainingConfig.Instance.ApplyBlowConfig();
        RevealNextAction();
        UpdateUI();
    }

    private void ConfirmInTrainingAction(TrainingActionSO actionSo)
    {
        var _selectedID = TrainingConfig.Instance.selectedActionID;
        combPoolObjects[_selectedID].gameObject.SetActive(true);
        DisableRemoveButton(_selectedID);
        SetActionsSprites(_selectedID, actionSo.sprite);
        TrainingConfig.Instance.AddToSelectedActionsDict(_selectedID, actionSo);
        TrainingConfig.Instance.ApplyBlowConfig();
        //UpdateUI();
    }

    public void SetActionsSprites(int id, Sprite sprite)
    {
        SetActionSprite(id, sprite);
        SetProgressSprite(id, sprite);
    }

    private void SetActionSprite(int id, Sprite sprite) => combPoolImages[id].sprite = sprite;
    private void SetProgressSprite(int id, Sprite sprite) => progressImages[id].sprite = sprite;

    private void SetActionToLastPosition(GameObject actionObj)
    {
        actionObj.transform.SetParent(null);
        actionObj.transform.SetParent(combPoolContainer);
    }

    private void CheckAndRevealStartButton()
    {
        var _startButton = Root.Q<Button>(startTrainingButtonName);
        if (_startButton == null) return;
        _startButton.visible = (!TrainingConfig.Instance.IsInTraining() && TrainingConfig.Instance.IsMinimumActionsToPlay());
    }

    public void RevealNextAction()
    {
        if (GetActionsRevealCount() > TrainingConfig.Instance.selectedActionsDict.Count)
            return;
        for (var _i = 0; _i < combPoolObjects.Length; _i++)
        {
            if (combPoolObjects[_i].gameObject.activeInHierarchy) 
                continue;
            SetActionToLastPosition(combPoolObjects[_i].gameObject);
            combPoolObjects[_i].gameObject.SetActive(true);
            break;
        }
    }

    private int GetActionsRevealCount()
        =>  combPoolObjects.Count(obj => obj.activeInHierarchy);

    private void DisableActionObject(int id) => combPoolObjects[id].SetActive(false);

    private void ResetCombinationPool() 
    {
        if (ScreenManager.OldScreenState == ScreenState.TrainingActionSelect)
            return;
        foreach (var _obj in combPoolObjects)
            _obj.gameObject.SetActive(false);
        combPoolObjects[0].SetActive(true);
    }

    private void ApplyInTrainingActions()
    {
        DisableAllProgressImages();
        if (!TrainingConfig.Instance.IsInTraining())
            return;
        for (var _i = 0; _i < TrainingConfig.Instance.selectedTrainingActions.Count; _i++)
        {
            TrainingConfig.Instance.SetSelectedID(_i);
            ConfirmInTrainingAction(TrainingConfig.Instance.selectedTrainingActions[_i]);
        }
        EnableAllProgressImages();
    }

    private void OpenActionSelectionScreen() => ScreenManager.Instance.GoToScreen(ScreenState.TrainingActionSelect);

    public void TrainingInfoButton() => ScreenManager.Instance.GoToScreen(ScreenState.LoreTheme);

    public override void BackButton() => ScreenManager.Instance.GoToScreen(ScreenState.MainMenu);

    public void StartButton()
        => GenericPopUp.Instance.ShowPopUp(TrainingConfig.Instance.sendToyoToTrainingPopUp,
            SendToyoToTraining, () => {});

    private void SendToyoToTraining()
    {
        //TrainingConfig.Instance.SetInTraining(true);
        TrainingConfig.Instance.SetInTrainingOnServer();
        EnableAllProgressImages();
        DisableLastEmptyAction();
        UpdateUI();
    }
    
    private void DisableLastEmptyAction()
    {
        if (GetActionsRevealCount() <= TrainingConfig.Instance.selectedActionsDict.Count)
            return;
        GetLastActionRevealed().SetActive(false);
    }

    private GameObject GetLastActionRevealed()
    {
        for (var _i = 0; _i < combPoolObjects.Length; _i++)
        {
            if(combPoolObjects[_i].activeInHierarchy)
                continue;
            return combPoolObjects[_i - 1];
        }
        return combPoolObjects.Last();
    }

    private void EnableAllProgressImages() => AllProgressImagesSetActive(true);
    private void DisableAllProgressImages() => AllProgressImagesSetActive(false);
    
    private void AllProgressImagesSetActive(bool isOn)
    {
        foreach (var _image in progressImages)
            _image.gameObject.SetActive(isOn);
    }

    public void FinishTraining()
    {
        if (TrainingConfig.Instance.GetTrainingTimeRemainInMinutes() > 0 && !TrainingConfig.Instance.ignoreTrainingTimer)
            return;
        GoToRewardScreen();
    }

    private void EnableTrainingUI() 
    {
        if (TrainingConfig.Instance.IsInTraining())
        {
            EnableVisualElement(inTrainingBoxName);
            SetTextInButton(inTrainingTimeButtonName, ConvertMinutesToString(TrainingConfig.Instance.GetTrainingTimeRemainInMinutes()));
            DisableAllRemoveButtons(); 
            DisableInteractableActionButtons();
        }
        else
        {
            DisableVisualElement(inTrainingBoxName);
            EnableInteractableActionButtons();
        }
    }

    private void EnableInteractableActionButtons() => ActionButtonsInteractable(true);
    private void DisableInteractableActionButtons() => ActionButtonsInteractable(false);

    private void ActionButtonsInteractable(bool on)
    {
        for(var _i = 0; _i < combPoolObjects.Length; _i++)
            combPoolObjects[_i].GetComponentInChildren<UnityEngine.UI.Button>().interactable = on;
    }

    private void GoToRewardScreen() => ScreenManager.Instance.GoToScreen(ScreenState.TrainingModuleRewards);
    
    private void ClearActionsImages()
    {
        for (var _i = 0; _i < combPoolImages.Length; _i++)
            combPoolImages[_i].sprite = null;
    }
    
    private void DisableRemoveButton(int id) => removeButtonsPool[id].SetActive(false);
    public void RevealRemoveButton(int id) => removeButtonsPool[id].SetActive(true);

    private void DisableAllRemoveButtons()
    {
        foreach (var _gameObject in removeButtonsPool)
            _gameObject.SetActive(false);
    }

    private void UpdateProgressTraining()
    {
        if (!TrainingConfig.Instance.IsInTraining())
            return;
        BlowConfig _blowConfig = null;
        if (TrainingConfig.Instance.GetSelectedBlowConfig() == null)
            TrainingConfig.Instance.ApplyBlowConfig();
        _blowConfig = TrainingConfig.Instance.GetSelectedBlowConfig();
        
        var _actualPercent = (((float)_blowConfig.duration - TrainingConfig.Instance.GetTrainingTimeRemainInMinutes())
                              / _blowConfig.duration) * 100;
        var _unitPercent = 100 / _blowConfig.qty;
        for (var _i = 0; _i < _blowConfig.qty; _i++)
        {
            if (_actualPercent > (_unitPercent * (_i + 1)))
            {
                GetProgressActiveImages()[_i].fillAmount = 0;
                continue;
            }
            var _inverseFill = (_actualPercent / (_blowConfig.qty - _i)) / 100;
            var _correctFill = _inverseFill != 0 ? 1 - _inverseFill : 1;
            GetProgressActiveImages()[_i].fillAmount = _correctFill;
            break;
        }
    }

    private List<Image> GetProgressActiveImages()
        =>  progressImages.Where(img => img.gameObject.activeInHierarchy).ToList();
}

public enum TrainingActionType
{
    None = 0,
    Punch = 1,
    Kick = 2,
    Move = 3
}
