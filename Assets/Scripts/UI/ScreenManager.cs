using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class ScreenManager : MonoBehaviour
    {
        public static ScreenManager Instance { get; private set; }

        [Header("Screen References")]
        public UIDocument WelcomeScreen;
        public UIDocument MainMenuScreen;
        public UIDocument ToyoInfoScreen;
        public UIDocument BoxInfoScreen;
        public UIDocument OpenBoxScreen;

        private WelcomeScreen WelcomeScript;
        private MainMenuScreen MainMenuScript;
        private ToyoInfoScreen ToyoInfoScript;
        private BoxInfoScreen BoxInfoScript;
        private OpenBoxScreen OpenBoxScript;

        public static SCREEN_STATE ScreenState { get; private set; }
        private static SCREEN_STATE OldScreenState;

        public bool haveToyo = false;

        private IEnumerator Start()
        {
            if (Instance != null)
                Destroy(this.gameObject);
            else
                Instance = this;
            
            yield return new WaitForFixedUpdate();
            //GetScreenScripts();
            GoToScreen(haveToyo ? SCREEN_STATE.MAIN_MENU : SCREEN_STATE.WELCOME);
        }

        public void GoToScreen(SCREEN_STATE newScreen)
        {
            CloseCurrentScreen();
            switch (newScreen)
            {
                case SCREEN_STATE.WELCOME:
                    WelcomeScreen.gameObject.SetActive(true);
                    WelcomeScript.EnableButtonEvents();
                    break;
                case SCREEN_STATE.MAIN_MENU:
                    MainMenuScreen.gameObject.SetActive(true);
                    MainMenuScript.EnableButtonEvents();
                    break;
                case SCREEN_STATE.TOYO_INFO:
                    ToyoInfoScreen.gameObject.SetActive(true);
                    ToyoInfoScript.EnableButtonEvents();
                    break;
                case SCREEN_STATE.BOX_INFO:
                    BoxInfoScreen.gameObject.SetActive(true);
                    BoxInfoScript.EnableButtonEvents();
                    break;
                case SCREEN_STATE.OPEN_BOX:
                    OpenBoxScreen.gameObject.SetActive(true);
                    OpenBoxScript.EnableButtonEvents();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            OldScreenState = ScreenState;
            ScreenState = newScreen;
            Debug.Log(ScreenState);
        }

        public void BackToOldScreen() => GoToScreen(OldScreenState);

        private void CloseCurrentScreen()
        {
            switch (ScreenState)
            {
                case SCREEN_STATE.WELCOME:
                    WelcomeScript.DisableButtonEvents();
                    WelcomeScreen.gameObject.SetActive(false);
                    break;
                case SCREEN_STATE.MAIN_MENU:
                    MainMenuScript.DisableButtonEvents();
                    MainMenuScreen.gameObject.SetActive(false);
                    break;
                case SCREEN_STATE.TOYO_INFO:
                    ToyoInfoScript.DisableButtonEvents();
                    ToyoInfoScreen.gameObject.SetActive(false);
                    break;
                case SCREEN_STATE.BOX_INFO:
                    BoxInfoScript.DisableButtonEvents();
                    BoxInfoScreen.gameObject.SetActive(false);
                    break;
                case SCREEN_STATE.OPEN_BOX:
                    OpenBoxScript.DisableButtonEvents();
                    OpenBoxScreen.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        private void CloseAllScreens()
        {
            WelcomeScript.DisableButtonEvents();
            MainMenuScript.DisableButtonEvents();
            ToyoInfoScript.DisableButtonEvents();
            BoxInfoScript.DisableButtonEvents();
            OpenBoxScript.DisableButtonEvents();
            
            WelcomeScreen.gameObject.SetActive(false);
            MainMenuScreen.gameObject.SetActive(false);
            ToyoInfoScreen.gameObject.SetActive(false);
            BoxInfoScreen.gameObject.SetActive(false);
            OpenBoxScreen.gameObject.SetActive(false);
        }

        private void GetScreenScripts()
        {
            WelcomeScript = this.GetComponent<WelcomeScreen>();
            MainMenuScript = this.GetComponent<MainMenuScreen>();
            ToyoInfoScript = this.GetComponent<ToyoInfoScreen>();
            BoxInfoScript = this.GetComponent<BoxInfoScreen>();
            OpenBoxScript = this.GetComponent<OpenBoxScreen>();
        }
    }

    public enum SCREEN_STATE
    {
        NONE = 0,
        WELCOME = 1,
        MAIN_MENU = 2,
        TOYO_INFO = 3,
        BOX_INFO = 4,
        OPEN_BOX = 5
    }
}
