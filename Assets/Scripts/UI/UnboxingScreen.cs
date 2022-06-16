using UI;

public class UnboxingScreen : UIController
{
    public void ConfirmButton() => ScreenManager.Instance.GoToScreen(ScreenState.OpenBox);
}
