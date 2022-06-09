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
        private const string InfoTextName = "boxDescription";

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
            UpdateDescriptionText();
        }

        public void PreviousBoxButton()
        {
            carousel.SwipeLeft();
            UpdateDescriptionText();
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
            var _root = uiDoc.rootVisualElement;
            var _textLabel = _root.Q<Label>(InfoTextName);
            _textLabel.text = text;
        }

        public void UpdateDescriptionText() //TODO: RENAME!
        {
            if (GetBoxSelected() == null)
            {
                SetDescriptionText(GetBoxDescription(carousel.allObjects[0].GetComponent<BoxConfig>()));
                return;
            }
            SetDescriptionText(GetBoxDescription(GetBoxSelected()));
            SetPossibleRewards(); 
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

        private void SetPossibleRewards()
        {
            //TODO: Set possible rewards images in UI
        }
    }
}
