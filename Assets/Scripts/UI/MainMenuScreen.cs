using Unity.VisualScripting;
using UnityEngine;
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

        public static GAME_MODE GameMode { get; private set; }

        private void AddModeEvents()
        {
            Root?.Q<VisualElement>(normalModeName).RegisterCallback<ClickEvent>
            (_ 
                    => {   DisableTrainingModeScreen(); 
                    SetMode(GAME_MODE.Normal); }
            );
            Root?.Q<VisualElement>(rankedModeName).RegisterCallback<ClickEvent>
            (_ 
                    => { DisableTrainingModeScreen(); 
                    SetMode(GAME_MODE.Ranked); }
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
                    => { DisableTrainingModeScreen(); 
                    SetMode(GAME_MODE.Normal); }
            );
            Root?.Q<VisualElement>(rankedModeName).UnregisterCallback<ClickEvent>
            (_ 
                    => { DisableTrainingModeScreen(); 
                    SetMode(GAME_MODE.Ranked); }
            );
            Root?.Q<VisualElement>(trainingModeName).UnregisterCallback<ClickEvent>
            (_ 
                    => { DisableTrainingModeScreen(); 
                    SetMode(GAME_MODE.Training); }
            );
        }

        protected override void UpdateUI()
        {
            ToyoManager.MoveToyoToCenterMainMenu();
        }

        public void StartButton()
        {
            //TODO: Send selected Toyo to Training Module Screen
            ScreenManager.Instance.GoToScreen(ScreenState.LoreTheme);
        }

        public void SetMode(GAME_MODE newGameMode)
        {
            GameMode = newGameMode;
            Debug.Log(GameMode);
        }

        public void EnableTrainingModeSelection()
        {
            _trainingBackground = Root.Query<VisualElement>(modeBackgroundName);
            _trainingBackground.visible = true;
            AddModeEvents();
        }

        public void DisableTrainingModeScreen()
        {
            _trainingBackground = Root.Query<VisualElement>(modeBackgroundName);
            RemoveModeEvents();
            _trainingBackground.visible = false;
        }

        public void BoxesButton() => ScreenManager.Instance.GoToScreen(ScreenState.BoxInfo);

        public void ToyoInfoButton() => ScreenManager.Instance.GoToScreen(ScreenState.ToyoInfo);

        public enum GAME_MODE
        {
            Normal = 0,
            Ranked = 1,
            Training = 2
        }
    }
}