using UnityEngine;

namespace UI
{
    public class WelcomeScreen : UIController
    {
        public void OpenSeaButton()
        {
            ScreenManager.Instance.haveToyo = true;
            Debug.Log("OpenSea button has clicked!");
        }

        public void BoxesButton()
            => ScreenManager.Instance.GoToScreen(SCREEN_STATE.BOX_INFO);

        public void OtherButton()
            => Debug.Log("Other button has clicked!");
    }
}
