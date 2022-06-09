using System;
using System.Collections;
using UnityEngine;
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
                    welcomeScreen.gameObject.SetActive(true);
                    _welcomeScript.EnableButtonEvents();
                    break;
                case ScreenState.MainMenu:
                    mainMenuScreen.gameObject.SetActive(true);
                    _mainMenuScript.EnableButtonEvents();
                    break;
                case ScreenState.ToyoInfo:
                    toyoInfoScreen.gameObject.SetActive(true);
                    _toyoInfoScript.EnableButtonEvents();
                    break;
                case ScreenState.BoxInfo:
                    boxInfoScreen.gameObject.SetActive(true);
                    _boxInfoScript.EnableButtonEvents();
                    _boxInfoScript.UpdateDescriptionText();
                    break;
                case ScreenState.OpenBox:
                    openBoxScreen.gameObject.SetActive(true);
                    _openBoxScript.EnableButtonEvents();
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
                    _welcomeScript.DisableButtonEvents();
                    welcomeScreen.gameObject.SetActive(false);
                    break;
                case ScreenState.MainMenu:
                    _mainMenuScript.DisableButtonEvents();
                    mainMenuScreen.gameObject.SetActive(false);
                    break;
                case ScreenState.ToyoInfo:
                    _toyoInfoScript.DisableButtonEvents();
                    toyoInfoScreen.gameObject.SetActive(false);
                    break;
                case ScreenState.BoxInfo:
                    _boxInfoScript.DisableButtonEvents();
                    boxInfoScreen.gameObject.SetActive(false);
                    break;
                case ScreenState.OpenBox:
                    _openBoxScript.DisableButtonEvents();
                    openBoxScreen.gameObject.SetActive(false);
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
        }
    }

    public enum ScreenState
    {
        None = 0,
        Welcome = 1,
        MainMenu = 2,
        ToyoInfo = 3,
        BoxInfo = 4,
        OpenBox = 5
    }
}
