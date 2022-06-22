using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class ToyoInfoScreen : OpenBoxScreen
    {
        public CarouselManager carousel;

        private ToyoObject _carouselToyoObject;

        public void NextToyoButton()
        {
            carousel.SwipeRight();
            UpdateUI();
        }
        public void PreviousToyoButton()
        {
            carousel.SwipeLeft();
            UpdateUI();
        }

        protected override void UpdateUI()
        {
            _carouselToyoObject = GetSelectedToyoFromCarousel();
            SetToyoStats(_carouselToyoObject);
            Root.Q<Label>(toyoNameField).text = _carouselToyoObject.GetToyoName();
        }

        private ToyoObject GetSelectedToyoFromCarousel() 
            => carousel.CurrentSelectedObject.GetComponent<ToyoObject>();
        

    }
}


