using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

public class TrainingModuleRewardScreen : UIController
{   
    public string eventTitleName = "eventTitle";
    public string eventTimeName = "eventTime";
    public string rewardTitleName = "rewardTitle";
    public string rewardValueName = "rewardValue";
    public string rewardImageName = "cardImage";
    public string rewardDescriptionName = "rewardDescription";
    public string[] combinationPoolNames;

    //TODO: Get correct variables
    public string eventTitle = "NEW EVENT NAME!";
    public int eventTime = 4736;

    protected override void UpdateUI()
    {
        ApplySelectedActions();
        SetTextInLabel(eventTitleName, eventTitle);
        SetTextInLabel(eventTimeName, GetDurationConvertedToString(eventTime));
        ShowRewards();
    }
    
    public void ClaimButton()
    {
        Debug.Log("Claim Rewards");
        ScreenManager.Instance.GoToScreen(ScreenState.MainMenu);
    }

    private void ApplySelectedActions()
    {
        for (var _i = 0; _i < ScreenManager.Instance.trainingModuleScript.selectedActions.Count; _i++)
        {
            if (_i > 2)
                Root.Q<VisualElement>(combinationPoolNames[_i]).style.display = DisplayStyle.Flex;
            SetVisualElementSprite(combinationPoolNames[_i], 
                ScreenManager.Instance.trainingModuleScript.selectedActions[_i].sprite);
        }
    }

    private void ShowRewards()
    {
        SetTextInLabel(rewardTitleName, "Reward Title");
        SetTextInLabel(rewardValueName, 350 + " $TOYO");
        SetTextInLabel(rewardDescriptionName, "Try again to get card.");
        SetVisualElementSprite(rewardImageName, null);
    }
}
