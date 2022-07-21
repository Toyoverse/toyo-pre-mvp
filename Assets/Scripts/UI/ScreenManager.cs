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
        [SerializeField] public TrainingModuleScreen trainingModuleScript;
        [SerializeField] public TrainingModuleRewardScreen trainingModuleRewardScript;
        [SerializeField] private UnboxingScreen unboxingScript;
        [SerializeField] private MetamaskScreen metamaskScript;
        [SerializeField] private TrainingActionSelectScreen trainingActionSelectScript;

        public static ScreenState ScreenState { get; private set; }
        public static ScreenState OldScreenState;

        //public bool haveToyo = false; //TODO: Get server variable

        private void Start()
        {
            GetScreenScripts();
            GoToScreen(ScreenState.Metamask);
        }

        public void GoToScreen(ScreenState newScreen)
        {
            AddCloseCurrentScreenEvent();
            AddOpenNewScreenEvent(newScreen);
            OldScreenState = ScreenState;
            ScreenState = newScreen;
            TransitionControl.Instance.PlayTransition();
        }

        public void BackToOldScreen() => GoToScreen(OldScreenState);

        private void AddCloseCurrentScreenEvent()
        {
            if (ScreenState == ScreenState.None)
                return;
            TransitionControl.Instance.OnCompletelyHiddenScreen += ScreenState switch
            {
                ScreenState.Welcome => welcomeScript.DisableScreen,
                ScreenState.MainMenu => mainMenuScript.DisableScreen,
                ScreenState.ToyoInfo => toyoInfoScript.DisableScreen,
                ScreenState.BoxInfo => boxInfoScript.DisableScreen,
                ScreenState.OpenBox => openBoxScript.DisableScreen,
                ScreenState.LoreTheme => loreThemeScript.DisableScreen,
                ScreenState.TrainingModule => trainingModuleScript.DisableScreen,
                ScreenState.Unboxing => unboxingScript.DisableScreen,
                ScreenState.Metamask => metamaskScript.DisableScreen,
                ScreenState.TrainingModuleRewards => trainingModuleRewardScript.DisableScreen,
                ScreenState.TrainingActionSelect => trainingActionSelectScript.DisableScreen
            };
        }

        private void AddOpenNewScreenEvent(ScreenState newScreen)
        {
            TransitionControl.Instance.OnCompletelyHiddenScreen += newScreen switch
            {
                ScreenState.Welcome => welcomeScript.ActiveScreen,
                ScreenState.MainMenu => mainMenuScript.ActiveScreen,
                ScreenState.ToyoInfo => toyoInfoScript.ActiveScreen,
                ScreenState.BoxInfo => boxInfoScript.ActiveScreen,
                ScreenState.OpenBox => openBoxScript.ActiveScreen,
                ScreenState.LoreTheme => loreThemeScript.ActiveScreen,
                ScreenState.TrainingModule => trainingModuleScript.ActiveScreen,
                ScreenState.Unboxing => unboxingScript.ActiveScreen,
                ScreenState.Metamask => metamaskScript.ActiveScreen,
                ScreenState.TrainingModuleRewards => trainingModuleRewardScript.ActiveScreen,
                ScreenState.TrainingActionSelect => trainingActionSelectScript.ActiveScreen,
                _ => throw new ArgumentOutOfRangeException()
            };
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
            metamaskScript ??= GetComponent<MetamaskScreen>();
            unboxingScript ??= GetComponent<UnboxingScreen>();
            trainingModuleRewardScript ??= GetComponent<TrainingModuleRewardScreen>();
            trainingActionSelectScript ??= GetComponent<TrainingActionSelectScreen>();
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
        Unboxing = 8,
        Metamask = 9,
        TrainingModuleRewards = 10,
        TrainingActionSelect = 11
    }
}
