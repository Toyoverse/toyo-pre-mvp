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
    
    [Header("Texts in UI")] //TODO: Get real strings (Maybe json or txt)
    public string titleText = "WEEKLY LORE THEME";
    public string descriptionText = @"Lore
When a Toyo makes much choices by its own, it slowly rises the ability to become rogue which isn't bad by definition. 
A rogue second generation Toyo is nothing more than a Toyo that don't depend on the Mentor's Heart link anymore. 
The link between them may keep immutable if their Heart Bound was pure but may be completely dismantled if it was corrupted.";
    public string timer = "03D 18H 35M";

    private float _count;

    private void Update()
    {
        _count += Time.deltaTime;
        if (_count > 60)
            UpdateTimerText();
    }

    private string GetTimerString()
    {
        //TODO: Return the remaining time of this quest
        return "TIME REMAINING: " + timer;
    }

    private void UpdateTimerText() => SetTextInLabel(timerName, GetTimerString());

    protected override void UpdateUI()
    {
        SetTextInLabel(titleName, titleText);
        SetTextInLabel(descriptionName, descriptionText);
        UpdateTimerText();
    }

    public void StartButton() => ScreenManager.Instance.GoToScreen(ScreenState.TrainingModule);
}
