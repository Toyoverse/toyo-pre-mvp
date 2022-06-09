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
        public string infoTextName = "boxDescription";
        public string scrollName = "dropScroll";
        public string rewardName = "reward";
        private int _rewardsCount = 0;

        private const string ConfirmMessage = "Are you sure you want to open your box?\n \nThis action can't be " +
            "undone! \n You will receive a wallet request to transfer your closed box momentarily to us. We will " +
            "then swap it by your Toyo and opened box NFTs and make another wallet request to transfer it back to you.";

        public void OpenBoxButton() 
            => GenericPopUp.Instance.ShowPopUp(ConfirmMessage, OpenSelectedBox, () => {});

        public override void BackButton() 
            => ScreenManager.Instance.GoToScreen(ScreenManager.Instance.haveToyo 
                ? ScreenState.MainMenu : ScreenState.Welcome);

        public void NextBoxButton()
        {
            carousel.SwipeRight();
            UpdateVisualInformation();
        }

        public void PreviousBoxButton()
        {
            carousel.SwipeLeft();
            UpdateVisualInformation();
        }

        private BoxConfig GetBoxSelected() 
            => carousel.CurrentSelectedObject.GetComponent<BoxConfig>();

        private void OpenSelectedBox()
        {
            //TODO: OpenBox
            Debug.Log("OPEN BOX " + GetBoxSelected().BoxName);
            ScreenManager.Instance.GoToScreen(ScreenState.OpenBox);
        }

        private void SetDescriptionText(string text)
        {
            var _textLabel = Root.Q<Label>(infoTextName);
            _textLabel.text = text;
        }

        public void UpdateVisualInformation() 
        {
            if (GetBoxSelected() == null)
            {
                SetDescriptionText(GetBoxDescription(carousel.allObjects[0].GetComponent<BoxConfig>()));
                return;
            }
            SetDescriptionText(GetBoxDescription(GetBoxSelected()));
            SetPossibleRewards(GetBoxSelected()); 
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

            return _name + "\n \n" + _region + "\n" + _type + "\n \n" + _rateString;
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
                        width = 120,
                        height = 120,
                        marginBottom = 10,
                        marginTop = 10,
                        marginLeft = 10,
                        marginRight = 10,
                        backgroundImage = new StyleBackground(_reward.sprite)
                    }
                };
                _scrollView.Add(_visualE);
                _rewardsCount++;
            }
        }
    }
}
