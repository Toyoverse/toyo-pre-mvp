using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class ToyoInfoScreen : UIController
    {
        public CarouselManager carousel;
        private string vitalityProgressName = "VitalityProgress";
        private string resistanceProgressName = "Resistance";
        private string resilienceProgressName = "Resilience";
        private string strengthProgressName = "Strength";
        private string cyberforceProgressName = "CyberForce";
        private string techniqueProgressName = "Technique";
        private string analysisProgressName = "Analysis";
        private string agilityProgressName = "Agility";
        private string speedProgressName = "Speed";
        private string precisionProgressName = "Precision";
        private string staminaProgressName = "Stamina";
        private string luckProgressName = "Luck";
        
        private string toyoNameField = "toyoName";

        private ToyoObject _toyoObject;

        private float maxStatValue = 200.0f;

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

        public void OverviewButton()
        {
            Debug.Log("Overview button clicked!");
        }

        public void ToyoPartsButton()
        {
            Debug.Log("ToyoParts button clicked!");
        }

        protected override void UpdateUI()
        {
            _toyoObject = GetToyoSelected();
            SetToyoStats();
            Root.Q<Label>(toyoNameField).text = _toyoObject.GetToyoName();
        }
        
        private ToyoObject GetToyoSelected() 
            => carousel.CurrentSelectedObject.GetComponent<ToyoObject>();

        private void SetToyoStats()
        {
            foreach (TOYO_STAT _stat in Enum.GetValues(typeof(TOYO_STAT)))
                SetStatUI(_stat);
            
        }

        private void SetStatUI(TOYO_STAT stat)
        {
            var _progressBar = GetProgressBar(stat);
            maxStatValue = _progressBar.highValue;
            var _statValue = _toyoObject.GetToyoStat(stat);
            _progressBar.lowValue = _statValue <= maxStatValue ? _statValue : maxStatValue;
        }

        private ProgressBar GetProgressBar(TOYO_STAT stat)
        {
            var _labelName = stat switch
            {
                TOYO_STAT.VITALITY => vitalityProgressName,
                TOYO_STAT.RESISTANCE => resistanceProgressName,
                TOYO_STAT.RESILIENCE => resilienceProgressName,
                TOYO_STAT.PHYSICAL_STRENGTH => strengthProgressName,
                TOYO_STAT.CYBER_FORCE => cyberforceProgressName,
                TOYO_STAT.TECHNIQUE => techniqueProgressName,
                TOYO_STAT.ANALYSIS => analysisProgressName,
                TOYO_STAT.AGILITY => agilityProgressName,
                TOYO_STAT.SPEED => speedProgressName,
                TOYO_STAT.PRECISION => precisionProgressName,
                TOYO_STAT.STAMINA => staminaProgressName,
                TOYO_STAT.LUCK => luckProgressName,
                _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
            };
            return Root.Q<ProgressBar>(_labelName);
        }

    }
}


