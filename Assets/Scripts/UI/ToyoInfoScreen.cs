using System.Collections.Generic;
using Database;
using UnityEngine;

namespace UI
{
    public class ToyoInfoScreen : UIController
    {
        public CarouselManager carousel;

        public List<Toyo> ToyoList
        {
            get
            {
                if (ToyoList == null)
                    SetToyoList();
                return ToyoList;
            }
            private set => ToyoList = value;
        }

        public void NextToyoButton()
        {
            carousel.SwipeRight();
        }
        public void PreviousToyoButton()
        {
            carousel.SwipeLeft();
        }

        public void OverviewButton()
        {
            Debug.Log("Overview button clicked!");
        }

        public void ToyoPartsButton()
        {
            Debug.Log("ToyoParts button clicked!");
        }

        private void SetToyoList()
        {
            ToyoList = new List<Toyo>();
            foreach (var _toyo in DatabaseConnection.Instance.player.toyos)
            {
                ToyoList.Add(_toyo);
            }
        }

        private void UpdateUI()
        {
            //TODO: Atualizar Progress bars e outras informações do Toyo
        }
    }
}