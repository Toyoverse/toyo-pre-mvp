using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UI
{
    public class MainMenuScreen : UIController
    {
        private VisualElement _trainingBackground;
        public string modeBackgroundName = "TrainingBackground";
        public string normalModeName = "NormalMode";
        public string rankedModeName = "RankedMode"; 
        public string trainingModeName = "TrainingMode";
        public string brbName = "Brb_Sign";
        
        public List<CustomButton> trainingModuleButtons;

        public static GAME_MODE GameMode { get; private set; }

        private List<CustomButton> _trainingButtonsBkp = null;

        public override void ActiveScreen()
        {
            base.ActiveScreen();
            if(_trainingButtonsBkp == null)
                SaveButtonEventsBkp();
            if (TrainingConfig.Instance.disableTrainingModule
                || (TrainingConfig.Instance.trainingEventNotFound
                && !TrainingConfig.Instance.SelectedToyoIsInTraining()))
            {
                AddComingSoonPopUp(trainingModuleButtons);
                FadeTrainingModuleButtons();
            }
            else if(TrainingConfig.Instance.SelectedToyoIsInTraining())
            {
                ReassignDefaultTrainingButtons();
                RevealTrainingModuleButtons();
            }
            
            ApplyBrbIfIsInTraining();
            EnableButtonEvents(trainingModuleButtons);
        }

        public override void DisableScreen()
        {
            //if (TrainingConfig.Instance != null && TrainingConfig.Instance.disableTrainingModule)
                DisableButtonEvents(trainingModuleButtons);
            base.DisableScreen();
        }

        private void AddModeEvents()
        {
            Root?.Q<VisualElement>(normalModeName).RegisterCallback<ClickEvent>
            (_ 
                    => {   /*DisableTrainingModeScreen(); 
                    SetMode(GAME_MODE.Normal);*/
                    ShowComingSoonPopUp(); }
            );
            Root?.Q<VisualElement>(rankedModeName).RegisterCallback<ClickEvent>
            (_ 
                    => { /*DisableTrainingModeScreen(); 
                    SetMode(GAME_MODE.Ranked);*/
                    ShowComingSoonPopUp(); }
            );
            Root?.Q<VisualElement>(trainingModeName).RegisterCallback<ClickEvent>
            (_ 
                    => { DisableTrainingModeScreen();  
                    SetMode(GAME_MODE.Training); }
            );
        }

        private void RemoveModeEvents()
        {
            Root?.Q<VisualElement>(normalModeName).UnregisterCallback<ClickEvent>
            (_ 
                    => { /*DisableTrainingModeScreen(); 
                    SetMode(GAME_MODE.Normal);*/
                    ShowComingSoonPopUp(); }
            );
            Root?.Q<VisualElement>(rankedModeName).UnregisterCallback<ClickEvent>
            (_ 
                    => { /*DisableTrainingModeScreen(); 
                    SetMode(GAME_MODE.Ranked);*/
                    ShowComingSoonPopUp(); }
            );
            Root?.Q<VisualElement>(trainingModeName).UnregisterCallback<ClickEvent>
            (_ 
                    => { DisableTrainingModeScreen(); 
                    SetMode(GAME_MODE.Training); }
            );
        }

        protected override void UpdateUI() => ToyoManager.MoveToyoToCenterMainMenu();

        public void StartButton()
        {
            TrainingConfig.Instance.SetSelectedToyoObject(ToyoManager.GetSelectedToyo());
            if(TrainingConfig.Instance.loreScreenAlreadyOpen || TrainingConfig.Instance.trainingEventNotFound)
                ScreenManager.Instance.GoToScreen(ScreenState.TrainingModule);
            else
            {
                TrainingConfig.Instance.LoreScreenOpen();
                ScreenManager.Instance.GoToScreen(ScreenState.LoreTheme);
            }
        }

        private void SetMode(GAME_MODE newGameMode) => GameMode = newGameMode;

        public void EnableTrainingModeSelection()
        {
            _trainingBackground = Root.Query<VisualElement>(modeBackgroundName);
            _trainingBackground.visible = true;
            AddModeEvents();
        }

        private void DisableTrainingModeScreen()
        {
            _trainingBackground = Root.Query<VisualElement>(modeBackgroundName);
            RemoveModeEvents();
            _trainingBackground.visible = false;
        }

        public void BoxesButton() => ScreenManager.Instance.GoToScreen(ScreenState.BoxInfo);

        public void ToyoInfoButton() => ScreenManager.Instance.GoToScreen(ScreenState.ToyoInfo);

        private void FadeTrainingModuleButtons()
        {
            foreach (var _cb in trainingModuleButtons)
                ChangeVisualElementOpacity(_cb.name, 0.4f);
        }
        
        private void RevealTrainingModuleButtons()
        {
            foreach (var _cb in trainingModuleButtons)
                ChangeVisualElementOpacity(_cb.name, 1);
        }

        private void AddComingSoonPopUp(List<CustomButton> buttonList)
        {
            if(buttonList.Count <= 0 || Root == null)
                return;
            foreach (var _cb in buttonList)
            {
                _cb.onClickEvent = new UnityEvent();
                _cb.onClickEvent.AddListener(ShowComingSoonPopUp);
            }
        }

        private void ShowFailedGetEventPopUp()
            => GenericPopUp.Instance.ShowPopUp(TrainingConfig.FailedGetEventMessage);
        
        private void ApplyBrbIfIsInTraining()
        {
            if (TrainingConfig.Instance.SelectedToyoIsInTraining())
                EnableBRB();
            else
                DisableBRB();
        }

        private void EnableBRB()
        {
            EnableVisualElement(brbName);
            ToyoManager.Instance.mainMenuToyoPivot.gameObject.SetActive(false);
        }
        
        private void DisableBRB()
        {
            ToyoManager.Instance.mainMenuToyoPivot.gameObject.SetActive(true);
            DisableVisualElement(brbName);
        }

        private void SaveButtonEventsBkp()
        {
            _trainingButtonsBkp = new List<CustomButton>();
            foreach (var _button in trainingModuleButtons)
            {
                var _customButton = new CustomButton
                {
                    name = _button.name,
                    Button = _button.Button,
                    onClickEvent = _button.onClickEvent
                };
                _trainingButtonsBkp.Add(_customButton);
            }
        }

        private void ReassignDefaultTrainingButtons()
        {
            if (_trainingButtonsBkp == null)
                return;
            for (var _i = 0; _i < _trainingButtonsBkp.Count; _i++)
            {
                trainingModuleButtons[_i] = new CustomButton
                {
                    name = _trainingButtonsBkp[_i].name,
                    Button = _trainingButtonsBkp[_i].Button,
                    onClickEvent = _trainingButtonsBkp[_i].onClickEvent
                };
            }
        }
    }
}

public enum GAME_MODE
{
    Normal = 0,
    Ranked = 1,
    Training = 2
}