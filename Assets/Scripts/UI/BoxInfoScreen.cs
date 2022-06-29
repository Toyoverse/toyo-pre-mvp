using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class BoxInfoScreen : UIController
    {
        public CarouselManager carousel;
        public string titleBoxName = "titleDescription";
        public string infoTextName = "chanceDescription";
        public string scrollName = "dropScroll";
        public string rewardName = "reward";
        public string boxCountLabelName = "countLabel";
        public string boxCountName = "count";
        public string openBoxButtonName = "openBoxButton";
        private int _rewardsCount = 0;

        private const string ConfirmMessage = "Are you sure you want to open your box?\n \nThis action can't be " +
            "undone! \n You will receive a wallet request to transfer your closed box momentarily to us. We will " +
            "then swap it by your Toyo and opened box NFTs and make another wallet request to transfer it back to you.";

        public override void ActiveScreen()
        {
            base.ActiveScreen();
            carousel.OnEndRotation += EnableBoxCount;
        }

        public override void DisableScreen()
        {
            base.DisableScreen();
            carousel.OnEndRotation -= EnableBoxCount;
        }

        public void OpenBoxButton() 
            => GenericPopUp.Instance.ShowPopUp(ConfirmMessage, OpenSelectedBox, () => {});

        public override void BackButton() 
            => ScreenManager.Instance.GoToScreen(ScreenManager.Instance.haveToyo 
                ? ScreenState.MainMenu : ScreenState.Welcome);

        public void NextBoxButton()
        {
            DisableBoxCount();
            carousel.SwipeRight();
            UpdateUI();
        }

        public void PreviousBoxButton()
        {
            DisableBoxCount();
            carousel.SwipeLeft();
            UpdateUI();
        }

        private BoxConfig GetBoxSelected() 
            => carousel.CurrentSelectedObject.GetComponent<BoxConfig>();

        private void OpenSelectedBox()
        {
            
            ToyoManager.SetSelectedBox(carousel.CurrentSelectedObject.gameObject);
            GetBoxSelected().unboxingVfx.SetRarityColor(TOYO_RARITY.LIMITED); //TODO: Move to correct location and pass correct color
            ScreenManager.Instance.GoToScreen(ScreenState.Unboxing);
        }

        private void SetTitleText(string text)
            => SetTextInLabel(titleBoxName, text);

        private void SetDescriptionText(string text)
            => SetTextInLabel(infoTextName, text);

        private void SetBoxCount(int count) => SetTextInLabel(boxCountLabelName, count.ToString());
        private void EnableBoxCount() => EnableVisualElement(boxCountName);
        private void DisableBoxCount() => DisableVisualElement(boxCountName);

        private void CheckCountAndEnableOrDisableOpenBoxButton(int count)
        {
            if(count > 0)
                EnableVisualElement(openBoxButtonName);
            else
                DisableVisualElement(openBoxButtonName);
        }

        protected override void UpdateUI() 
        {
            if (GetBoxSelected() == null)
            {
                SetDescriptionText(GetBoxDescription(carousel.allObjects[0].GetComponent<BoxConfig>()));
                SetTitleText(GetBoxTitle(carousel.allObjects[0].GetComponent<BoxConfig>()));
                SetBoxCount(carousel.allObjects[0].GetComponent<BoxConfig>().quantity);
                return;
            }
            SetDescriptionText(GetBoxDescription(GetBoxSelected()));
            SetTitleText(GetBoxTitle(GetBoxSelected()));
            SetPossibleRewards(GetBoxSelected());
            var _boxAmount = GetBoxSelected().quantity;
            SetBoxCount(_boxAmount);
            CheckCountAndEnableOrDisableOpenBoxButton(_boxAmount);
        }

        private string GetBoxTitle(BoxConfig boxConfig)
        {
            var _name = boxConfig.BoxName.ToUpper() + "BOX";
            return _name;
        }

        private string GetBoxDescription(BoxConfig boxConfig)
        {
            var _name = boxConfig.BoxName.ToUpper() + " BOX";
            var _region = "REGION: " + boxConfig.BoxRegion.ToString().ToUpper();
            var _type = "TYPE: " + boxConfig.BoxType.ToString().ToUpper();

            var _ratesList = new List<string>();
            foreach (var _dropRate in boxConfig.DropRate)
            {
                var _rarity = _dropRate.Key.ToString();
                var _percent = _dropRate.Value.ToString();
                _ratesList.Add(_rarity + ": " + _percent + "%");
            }

            var _rateString = "";
            foreach (var _s in _ratesList) 
                _rateString = _rateString + (_s + "\n");

            return _region + "\n" + _type + "\n \n" + _rateString;
        }

        private void SetPossibleRewards(BoxConfig boxConfig)
        {
            var _scrollView = Root.Q<ScrollView>(scrollName);
            _scrollView.Clear();
            for (var _i = 0; _i < boxConfig.PossibleRewards.Count; _i++)
            {
                var _reward = boxConfig.PossibleRewards[_i];
                var _visualE = new VisualElement
                {
                    name = rewardName + _i,
                    style =
                    {
                        width = 130,
                        height = 130,
                        marginBottom = 17,
                        marginTop = 50,
                        marginLeft = 17,
                        marginRight = 17,
                        backgroundImage = new StyleBackground(_reward.sprite)
                    }
                };
                _scrollView.Add(_visualE);
                _rewardsCount++;
            }
        }
    }
}
