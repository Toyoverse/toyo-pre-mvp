using UI;
using UnityEngine;
using UnityEngine.UIElements;

public class TrainingModuleScreen : UIController
{
    public string titleName = "title";
    public string trainingTimeName = "time";
    public string[] combinationPoolNames;
    public string toyoImageName = "toyoImage";
    public string investName = "invest";
    public string receiveName = "receive";
    public string durationName = "duration";

    protected override void UpdateUI()
    {
        //TODO: Set title and training time
        //TODO: Set Toyo and combination pool images
    }
    
    public void SendToyoToQuestButton()
    {
        Debug.Log("Send Toyo to Quest button clicked!");
    }

    public void PunchesButton()
    {
        Debug.Log("Punches button clicked!");
    }
    
    public void KicksButton()
    {
        Debug.Log("Kicks button clicked!");
    }
    
    public void MovesButton()
    {
        Debug.Log("Moves button clicked!");
    }

    public void TrainingInfoButton() => ScreenManager.Instance.GoToScreen(ScreenState.LoreTheme);

    public override void BackButton() => ScreenManager.Instance.GoToScreen(ScreenState.MainMenu);

    public void SetInvestUI(float value) => SetTextInLabel(investName, "INVEST: $" + value);
    
    public void SetDurationUI(float value) => SetTextInLabel(durationName, "DURATION: " + value + "H");
    
    public void SetReceiveUI(float value) => SetTextInLabel(receiveName, "RECEIVE: $" + value);

    public void SetPoolImage(Sprite sprite, int position)
        => SetVisualElementSprite(combinationPoolNames[position], sprite);

    private void SetToyoImage(Sprite sprite) => SetVisualElementSprite(toyoImageName, sprite);

    public void StartButton()
    {
        Debug.Log("Send Toyo to Quest button clicked!");
    }
}
