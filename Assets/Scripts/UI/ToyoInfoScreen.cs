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
            UpdateInfoCarouselSelectedToyo();
        }

        private void UpdateInfoCarouselSelectedToyo()
        {
            _carouselToyoObject = GetSelectedToyoFromCarousel();
            _carouselToyoAnimator = _carouselToyoObject.GetComponentInChildren<SpriteAnimator>();
            _carouselToyoAnimator.PlayAnimation();
            SetToyoStats(_carouselToyoObject);
            SetToyoRarity(_carouselToyoObject.GetToyoRarity());
            SetToyoId(GetSelectedToyoFromCarousel().tokenId);
            SetTextInLabel(toyoNameField, _carouselToyoObject.GetToyoName());
        }

        private void SelectCarouselToyo()
        {
            ToyoManager.GetSelectedToyo().IsToyoSelected = false;
            _carouselToyoObject.IsToyoSelected = true;
        }
        
        private ToyoObject GetSelectedToyoFromCarousel() 
            => carousel.CurrentSelectedObject.GetComponent<ToyoObject>();
        
        public void SelectToyoButton()
        {
            SelectCarouselToyo();
            UpdateUI();
            BackButton();
        }
    }
}


