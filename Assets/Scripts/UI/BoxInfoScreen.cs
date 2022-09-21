using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Database;
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

        private const string UnavailableMessage = "Opening this box is not currently available.";

        public override void ActiveScreen()
        {
            base.ActiveScreen();
            carousel.OnEndRotation += EnableBoxCount;
        }

        public override void DisableScreen()
        {
            carousel.OnEndRotation -= EnableBoxCount;
            base.DisableScreen();
        }

        public void OpenBoxButton()
        {
            if (GetBoxSelected().BoxRegion == BOX_REGION.Xeon)
            {
                GenericPopUp.Instance.ShowPopUp(UnavailableMessage);
                return;
            }
            GenericPopUp.Instance.ShowPopUp(ConfirmMessage, OpenSelectedBox, () => { });
        }

        public override void BackButton() 
            => ScreenManager.Instance.GoToScreen(ToyoManager.Instance.ToyoList.Count > 0 
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

        public BoxConfig GetBoxSelected() 
            => carousel.CurrentSelectedObject.GetComponent<BoxConfig>();

        private void OpenSelectedBox()
        {
            Loading.StartLoading?.Invoke();
            ToyoManager.SetSelectedBox(carousel.CurrentSelectedObject.gameObject);
            DatabaseConnection.Instance.GetOpenBox(DatabaseConnection.Instance.blockchainIntegration.CallOpenBox, GetBoxSelected().GetFirstUnopenedBoxId());
        }

        public void TestOpenBox()
        {
            var json = "{     \"objectId\": \"ZWliRHUxbEFsVQ==\",             \"name\": \"Don Barko\",             \"hasTenParts\": true,             \"isToyoSelected\": false,             \"createdAt\": \"2022-07-07T15:34:16.518Z\",             \"updateAt\": \"2022-07-07T15:34:16.518Z\",             \"tokenId\": \"5230\",             \"toyoPersonaOrigin\": {                 \"objectId\": \"R8xYTwz4xl\",                 \"name\": \"Don Barko\",                 \"rarityId\": 2,                 \"rarity\": \"UNCOMMON\",                 \"thumbnail\": \"https://toyoverse.com/nft_thumbs/toyos/don-barko.png\",                 \"video\": \"https://toyoverse.com/nft_thumbs/toyos/don-barko.mp4\",                 \"bodyType\": 3,                 \"createdAt\": \"2022-07-01T01:04:12.482Z\",                 \"updateAt\": \"2022-07-01T01:04:12.482Z\"             }          }";
            
            var _myBox = JsonUtility.FromJson<Toyo>(json);

            TOYO_RARITY myToyoRarity = ParseEnums.StringToEnum<TOYO_RARITY>(_myBox.toyoPersonaOrigin.rarity);
            //GetBoxSelected().unboxingVfx.SetRarityColor(myToyoRarity);
            Loading.EndLoading?.Invoke();
            ToyoManager.Instance.AddToyoToToyoObjectList(_myBox);

            carousel.SetFirstSelectedObject();
            SetDescriptionText(GetBoxDescription(carousel.allObjects[0].GetComponent<BoxConfig>()));
            SetTitleText(GetBoxTitle(carousel.allObjects[0].GetComponent<BoxConfig>()));
            SetBoxCount(carousel.allObjects[0].GetComponent<BoxConfig>().GetQuantity());

            ToyoManager.SetSelectedBox(carousel.allObjects[0].gameObject);
            
            ScreenManager.Instance.GoToScreen(ScreenState.Unboxing);
            
        }

        public void CallOpenBoxAnimation(string json)
        {
            var _myBox = JsonUtility.FromJson<BoxParent>(json);

            TOYO_RARITY myToyoRarity = ParseEnums.StringToEnum<TOYO_RARITY>(_myBox.box.toyo.toyoPersonaOrigin.rarity);
            GetBoxSelected().unboxingVfx.SetRarityColor(myToyoRarity);
            GetBoxSelected().GetFirstUnopenedBox().isOpen = true;
            Loading.EndLoading?.Invoke();
            ToyoManager.Instance.AddToyoToToyoObjectList(_myBox.box.toyo);
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
                SetBoxCount(carousel.allObjects[0].GetComponent<BoxConfig>().GetQuantity());
                return;
            }
            SetDescriptionText(GetBoxDescription(GetBoxSelected()));
            SetTitleText(GetBoxTitle(GetBoxSelected()));
            SetPossibleRewards(GetBoxSelected());
            var _boxAmount = GetBoxSelected().GetQuantity();
            SetBoxCount(_boxAmount);
            CheckCountAndEnableOrDisableOpenBoxButton(_boxAmount);
        }

        private string GetBoxTitle(BoxConfig boxConfig)
        {
            var _name = boxConfig.BoxName.ToUpper();
            return _name;
        }

        private string GetBoxDescription(BoxConfig boxConfig)
        {
            var _name = boxConfig.BoxName.ToUpper();
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
