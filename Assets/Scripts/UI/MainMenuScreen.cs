using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class MainMenuScreen : UIController
    {
        VisualElement trainingBackground;
        private const string modeBackgroundString = "TrainingBackground";
        private const string normalModeString = "NormalMode";
        private const string rankedModeString = "RankedMode"; 
        private const string trainingModeString = "TrainingMode";

        public static GAME_MODE GameMode { get; private set; }

        private void AddModeEvents()
        {
            root?.Q<VisualElement>(normalModeString).RegisterCallback<ClickEvent>
            (evt 
                    =>
                {   DisableTrainingModeScreen(); 
                    SetMode(GAME_MODE.NORMAL); }
            );
            root?.Q<VisualElement>(rankedModeString).RegisterCallback<ClickEvent>
            (evt 
                    => { DisableTrainingModeScreen(); 
                    SetMode(GAME_MODE.RANKED); }
            );
            root?.Q<VisualElement>(trainingModeString).RegisterCallback<ClickEvent>
            (evt 
                    => { DisableTrainingModeScreen();  
                    SetMode(GAME_MODE.TRAINING); }
            );
        }

        private void RemoveModeEvents()
        {
            root?.Q<VisualElement>(normalModeString).UnregisterCallback<ClickEvent>
            (evt 
                    => { DisableTrainingModeScreen(); 
                    SetMode(GAME_MODE.NORMAL); }
            );
            root?.Q<VisualElement>(rankedModeString).UnregisterCallback<ClickEvent>
            (evt 
                    => { DisableTrainingModeScreen(); 
                    SetMode(GAME_MODE.RANKED); }
            );
            root?.Q<VisualElement>(trainingModeString).UnregisterCallback<ClickEvent>
            (evt 
                    => { DisableTrainingModeScreen(); 
                    SetMode(GAME_MODE.TRAINING); }
            );
        }
        
        public void StartButton()
        {
            //TODO: Go to "gameplay"!
            Debug.Log("Start button clicked! Go to gameplay!");
        }

        public void SetMode(GAME_MODE newGameMode)
        {
            GameMode = newGameMode;
            Debug.Log(GameMode);
        }

        public void EnableTrainingModeSelection()
        {
            trainingBackground = root.Query<VisualElement>(modeBackgroundString);
            trainingBackground.visible = true;
            AddModeEvents();
        }

        public void DisableTrainingModeScreen()
        {
            trainingBackground = root.Query<VisualElement>(modeBackgroundString);
            RemoveModeEvents();
            trainingBackground.visible = false;
        }

        public void ChangeToyoButton(int i)
        {
            var test = 0;
            test = i > 0 ? 1 : -1;
            //Add or remove index to TOYO list
            Debug.Log("Change Toyo Button " + test);
        }

        public void BoxesButton() => ScreenManager.Instance.GoToScreen(SCREEN_STATE.BOX_INFO);

        public void ToyoInfoButton() => ScreenManager.Instance.GoToScreen(SCREEN_STATE.TOYO_INFO);

        public enum GAME_MODE
        {
            NORMAL = 0,
            RANKED = 1,
            TRAINING = 2
        }
    }
}