using System.Collections.Generic;
using System.Linq;
using Database;
using UnityEngine;

namespace UI
{
    public class ToyoInfoScreen : UIController
    {
        public CarouselManager carousel;


        
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



        private void UpdateUI()
        {
            //TODO: Atualizar Progress bars e outras informações do Toyo
        }
    }
}


