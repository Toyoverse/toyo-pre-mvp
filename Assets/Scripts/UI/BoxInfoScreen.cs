using UnityEngine;

namespace UI
{
    public class BoxInfoScreen : UIController
    {
        public void OpenBoxButton() //TODO: Call confirm popup before go to open_box
            => ScreenManager.Instance.GoToScreen(SCREEN_STATE.OPEN_BOX);

        public override void BackButton() 
            => ScreenManager.Instance.GoToScreen(ScreenManager.Instance.haveToyo 
                ? SCREEN_STATE.MAIN_MENU : SCREEN_STATE.WELCOME);

        public void NextBoxButton()
        {
            Debug.Log("Next Box Button clicked!");
        }

        public void PreviousBoxButton()
        {
            Debug.Log("Previous Box Button clicked!");
        }
    }
}
