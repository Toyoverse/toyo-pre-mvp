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
        private SpriteAnimator _carouselToyoAnimator;

        public void NextToyoButton()
        {
            _carouselToyoAnimator.StopAnimation();
            carousel.SwipeRight();
            UpdateUI();
        }
        public void PreviousToyoButton()
        {
            _carouselToyoAnimator.StopAnimation();
            carousel.SwipeLeft();
            UpdateUI();
        }

        protected override void UpdateUI()
        {
            UpdateSelectedToyo();
            _carouselToyoAnimator.PlayAnimation();
            SetToyoStats(_carouselToyoObject);
            SetToyoRarity(_carouselToyoObject.GetToyoRarity());
            Root.Q<Label>(toyoNameField).text = _carouselToyoObject.GetToyoName();
        }

        private void UpdateSelectedToyo()
        {
            _carouselToyoObject = GetSelectedToyoFromCarousel();
            _carouselToyoAnimator = _carouselToyoObject.GetComponentInChildren<SpriteAnimator>();
        }
        
        private ToyoObject GetSelectedToyoFromCarousel() 
            => carousel.CurrentSelectedObject.GetComponent<ToyoObject>();
        

    }
}


