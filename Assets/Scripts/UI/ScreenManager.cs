using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace UI
{
    public class ScreenManager : Singleton<ScreenManager>
    {
       
        [Header("Scripts References")]
        [SerializeField] private WelcomeScreen welcomeScript;
        [SerializeField] private MainMenuScreen mainMenuScript;
        [SerializeField] private ToyoInfoScreen toyoInfoScript;
        [SerializeField] private BoxInfoScreen boxInfoScript;
        [SerializeField] private OpenBoxScreen openBoxScript;
        [SerializeField] private LoreThemeScreen loreThemeScript;
        [SerializeField] private TrainingModuleScreen trainingModuleScript;
        [SerializeField] private UnboxingScreen unboxingScreen;

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
                    welcomeScript.ActiveScreen();
                    break;
                case ScreenState.MainMenu:
                    mainMenuScript.ActiveScreen();
                    break;
                case ScreenState.ToyoInfo:
                    toyoInfoScript.ActiveScreen();
                    break;
                case ScreenState.BoxInfo:
                    boxInfoScript.ActiveScreen();
                    break;
                case ScreenState.OpenBox:
                    openBoxScript.ActiveScreen();
                    break;
                case ScreenState.LoreTheme:
                    loreThemeScript.ActiveScreen();
                    break;
                case ScreenState.TrainingModule:
                    trainingModuleScript.ActiveScreen();
                    break;
                case ScreenState.Unboxing:
                    unboxingScreen.ActiveScreen();
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
                    welcomeScript.DisableScreen();
                    break;
                case ScreenState.MainMenu:
                    mainMenuScript.DisableScreen();
                    break;
                case ScreenState.ToyoInfo:
                    toyoInfoScript.DisableScreen();
                    break;
                case ScreenState.BoxInfo:
                    boxInfoScript.DisableScreen();
                    break;
                case ScreenState.OpenBox:
                    openBoxScript.DisableScreen();
                    break;
                case ScreenState.LoreTheme:
                    loreThemeScript.DisableScreen();
                    break;
                case ScreenState.TrainingModule:
                    trainingModuleScript.DisableScreen();
                    break;
                case ScreenState.Unboxing:
                    unboxingScreen.DisableScreen();
                    break;
                default:
                    break;
            }
        }

        private void GetScreenScripts()
        {
            welcomeScript ??= GetComponent<WelcomeScreen>();
            mainMenuScript ??= GetComponent<MainMenuScreen>();
            toyoInfoScript ??= GetComponent<ToyoInfoScreen>();
            boxInfoScript ??= GetComponent<BoxInfoScreen>();
            openBoxScript ??= GetComponent<OpenBoxScreen>();
            loreThemeScript ??= GetComponent<LoreThemeScreen>();
            trainingModuleScript ??= GetComponent<TrainingModuleScreen>();
            
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
        Unboxing = 8
    }
}
