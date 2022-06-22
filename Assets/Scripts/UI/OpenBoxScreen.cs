using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class OpenBoxScreen : UIController
    {
                
        public void AwesomeButton()
            => ScreenManager.Instance.GoToScreen(ScreenState.MainMenu);
        
        
        private string vitalityProgressName = "Vitality";
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
        
        protected string toyoNameField = "toyoName";
        
        protected string toyoLevelField = "numberLvl";
        protected string toyoHearthBoundField = "numberHb";

        private ToyoObject _toyoObject;

        private float maxStatValue = 200.0f;

        public string overviewTabName = "overviewDescription";
        public string toyoTabName = "toyoPartsDescription";
        

        protected ToyoObject GetSelectedToyo() => ToyoManager.GetSelectedToyo();
        
        protected override void UpdateUI()
        {
            _toyoObject = GetSelectedToyo();
            SetToyoStats();
            Root.Q<Label>(toyoNameField).text = _toyoObject.GetToyoName();
            ToyoManager.MoveToyoToCenterOpenBox();
        }

        protected void SetToyoStats(ToyoObject selectedToyo = null)
        {
            foreach (TOYO_STAT _stat in Enum.GetValues(typeof(TOYO_STAT)))
                SetStatUI(_stat, selectedToyo);
        }

        protected void SetStatUI(TOYO_STAT stat, ToyoObject selectedToyo)
        {
            selectedToyo ??= _toyoObject;
            var _progressBar = GetProgressBar(stat);
            var _statValue = selectedToyo.GetToyoStat(stat);
            var _progressLabel = GetProgressLabel(stat);
            var _toyoLevelLabel = Root.Q<Label>(toyoLevelField);
            var _toyoHbLabel = Root.Q<Label>(toyoHearthBoundField);
            
            maxStatValue = _progressBar.highValue;
            _progressBar.lowValue = _statValue <= maxStatValue ? _statValue : maxStatValue;
            if (_progressLabel != null)
                _progressLabel.text = _progressBar.lowValue.ToString();
            if (_toyoHbLabel != null)
                _toyoHbLabel.text = selectedToyo.GetToyoHearthBound().ToString();
            if (_toyoLevelLabel != null)
                _toyoLevelLabel.text = selectedToyo.GetToyoLevel().ToString();

        }

        protected ProgressBar GetProgressBar(TOYO_STAT stat) => Root.Q<ProgressBar>(GetFieldName(stat,"Progress"));
        
        protected Label GetProgressLabel(TOYO_STAT stat) => Root.Q<Label>(GetFieldName(stat,"Value")); 

        private string GetFieldName(TOYO_STAT stat, string field)
        {
            return stat switch
            {
                TOYO_STAT.VITALITY => vitalityProgressName + field,
                TOYO_STAT.RESISTANCE => resistanceProgressName + field,
                TOYO_STAT.RESILIENCE => resilienceProgressName + field,
                TOYO_STAT.PHYSICAL_STRENGTH => strengthProgressName + field,
                TOYO_STAT.CYBER_FORCE => cyberforceProgressName + field,
                TOYO_STAT.TECHNIQUE => techniqueProgressName + field,
                TOYO_STAT.ANALYSIS => analysisProgressName + field,
                TOYO_STAT.AGILITY => agilityProgressName + field,
                TOYO_STAT.SPEED => speedProgressName + field,
                TOYO_STAT.PRECISION => precisionProgressName + field,
                TOYO_STAT.STAMINA => staminaProgressName + field,
                TOYO_STAT.LUCK => luckProgressName + field,
                _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
            };
        }

        public void OverviewTab()
        {
            DisableVisualElement(toyoTabName);
            EnableVisualElement(overviewTabName);
        }
        public void ToyoTab()
        {
            DisableVisualElement(overviewTabName);
            EnableVisualElement(toyoTabName);
        }
    }
}
