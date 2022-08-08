using System;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

public class LoreThemeScreen : UIController
{
    [Header("Label names")]
    public string titleName = "infoTitle";
    public string descriptionName = "descriptionText";
    public string timerName = "timingInfo";
    public string timeRemainString = "Time Remaining: ";
    
    //[Header("Texts in UI")] 
    public string titleText => TrainingConfig.Instance.eventTitle;
    public string descriptionText => TrainingConfig.Instance.lore;
    /*@"Lore
When a Toyo makes much choices by its own, it slowly rises the ability to become rogue which isn't bad by definition. 
A rogue second generation Toyo is nothing more than a Toyo that don't depend on the Mentor's Heart link anymore. 
The link between them may keep immutable if their Heart Bound was pure but may be completely dismantled if it was corrupted.";*/
    public string timer => ConvertMinutesToString(TrainingConfig.Instance.GetEventTimeRemain());

    private float _count = 0;

    private void Update()
    {
        if (ScreenManager.ScreenState != ScreenState.LoreTheme) 
            return;
        _count += Time.deltaTime;
        if (_count > 60)
        {
            UpdateTimerText();
            _count = 0;
        }
    }

    private string GetTimerString() => timeRemainString + timer;

    
    private void UpdateTimerText() => SetTextInLabel(timerName, GetTimerString());

    protected override void UpdateUI()
    {
        SetTextInLabel(titleName, titleText);
        SetTextInLabel(descriptionName, descriptionText);
        UpdateTimerText();
    }

    public void StartButton() => ScreenManager.Instance.GoToScreen(ScreenState.TrainingModule);
}
