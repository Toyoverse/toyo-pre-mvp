namespace UI
{
    public class OpenBoxScreen : UIController
    {
        public void AwesomeButton()
            => ScreenManager.Instance.GoToScreen(ScreenState.MainMenu);
    }
}
