namespace UI
{
    public class OpenBoxScreen : UIController
    {
        public void AwesomeButton()
            => ScreenManager.Instance.GoToScreen(SCREEN_STATE.MAIN_MENU);
    }
}
