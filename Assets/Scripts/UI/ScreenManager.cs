using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UI
{
    public class ScreenManager : Singleton<ScreenManager>
    {
        [Header("Screen References")]
        public UIDocument welcomeScreen;
        public UIDocument mainMenuScreen;
        public UIDocument toyoInfoScreen;
        public UIDocument boxInfoScreen;
        public UIDocument openBoxScreen;

        private WelcomeScreen _welcomeScript;
        private MainMenuScreen _mainMenuScript;
        private ToyoInfoScreen _toyoInfoScript;
        private BoxInfoScreen _boxInfoScript;
        private OpenBoxScreen _openBoxScript;
        private LoreThemeScreen _loreThemeScript;
        private TrainingModuleScreen _trainingModuleScript;
        private ActionSelectionScreen _actionSelectionScript;

        public static ScreenState ScreenState { get; private set; }
        private static ScreenState _oldScreenState;

        public bool haveToyo = false; //TODO: Get server variable

        private void Start()
        {
            GetScreenScripts();
            GoToScreen(haveToyo ? ScreenState.MainMenu : ScreenState.Welcome);
        }

        public void GoToScreen(ScreenState newScreen)
        {
            //TODO: Change to events
            CloseCurrentScreen();
            switch (newScreen)
            {
                case ScreenState.Welcome:
                    _welcomeScript.ActiveScreen();
                    break;
                case ScreenState.MainMenu:
                    _mainMenuScript.ActiveScreen();
                    break;
                case ScreenState.ToyoInfo:
                    _toyoInfoScript.ActiveScreen();
                    break;
                case ScreenState.BoxInfo:
                    _boxInfoScript.ActiveScreen();
                    break;
                case ScreenState.OpenBox:
                    _openBoxScript.ActiveScreen();
                    break;
                case ScreenState.LoreTheme:
                    _loreThemeScript.ActiveScreen();
                    break;
                case ScreenState.TrainingModule:
                    _trainingModuleScript.ActiveScreen();
                    break;
                case ScreenState.ActionSelection:
                    _actionSelectionScript.ActiveScreen();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _oldScreenState = ScreenState;
            ScreenState = newScreen;
        }

        public void BackToOldScreen() => GoToScreen(_oldScreenState);

        private void CloseCurrentScreen()
        {
            //TODO: Change to events
            switch (ScreenState)
            {
                case ScreenState.Welcome:
                    _welcomeScript.DisableScreen();
                    break;
                case ScreenState.MainMenu:
                    _mainMenuScript.DisableScreen();
                    break;
                case ScreenState.ToyoInfo:
                    _toyoInfoScript.DisableScreen();
                    break;
                case ScreenState.BoxInfo:
                    _boxInfoScript.DisableScreen();
                    break;
                case ScreenState.OpenBox:
                    _openBoxScript.DisableScreen();
                    break;
                case ScreenState.LoreTheme:
                    _loreThemeScript.DisableScreen();
                    break;
                case ScreenState.TrainingModule:
                    _trainingModuleScript.DisableScreen();
                    break;
                case ScreenState.ActionSelection:
                    _actionSelectionScript.DisableScreen();
                    break;
                default:
                    break;
            }
        }

        private void GetScreenScripts()
        {
            _welcomeScript = GetComponent<WelcomeScreen>();
            _mainMenuScript = GetComponent<MainMenuScreen>();
            _toyoInfoScript = GetComponent<ToyoInfoScreen>();
            _boxInfoScript = GetComponent<BoxInfoScreen>();
            _openBoxScript = GetComponent<OpenBoxScreen>();
            _loreThemeScript = GetComponent<LoreThemeScreen>();
            _trainingModuleScript = GetComponent<TrainingModuleScreen>();
            _actionSelectionScript = GetComponent<ActionSelectionScreen>();
        }
    }

    public enum ScreenState
    {
        None = 0,
        Welcome = 1,
        MainMenu = 2,
        ToyoInfo = 3,
        BoxInfo = 4,
        OpenBox = 5,
        LoreTheme = 6,
        TrainingModule = 7,
        ActionSelection = 8
    }
}
