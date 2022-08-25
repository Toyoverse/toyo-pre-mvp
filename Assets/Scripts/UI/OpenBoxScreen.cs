using System;
using System.Collections;
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
        protected string rarityNameField = "rarity_name";
        protected string tokenIdNumber = "token_id_number";
        
        protected string toyoLevelField = "numberLvl";
        protected string toyoHearthBoundField = "numberHb";

        private ToyoObject _toyoObject;

        private float maxStatValue = 200.0f;

        public string overviewTabName = "overviewDescription";
        public string toyoTabName = "toyoPartsDescription";
        

        protected ToyoObject GetSelectedToyo() => ToyoManager.GetSelectedToyo();

        public override void ActiveScreen()
        {
            base.ActiveScreen();
            UpdateUI();
        }
        
        protected override void UpdateUI()
        {
            _toyoObject = GetSelectedToyo();
            SetToyoStats();
            SetToyoRarity(_toyoObject.GetToyoRarity());
            SetToyoId(_toyoObject.tokenId);
            Root.Q<Label>(toyoNameField).text = _toyoObject.GetToyoName();
            ToyoManager.MoveToyoToCenterOpenBox();
            if(FadeController.InFade)
                StartCoroutine(EndFade());
        }

        IEnumerator EndFade()
        {
            yield return new WaitForSeconds(1.5f);
            FadeController.Out();
        }

        protected void SetToyoRarity(TOYO_RARITY rarity)
        {
            SetTextInLabel(rarityNameField, rarity.ToString());
            SetLabelColor(rarityNameField, GetColorInRarity(rarity));
        }

        private Color GetColorInRarity(TOYO_RARITY rarity)
        {
            foreach (var _rarityColor in ToyoManager.Instance.rarityColorsConfig.rarityColors)
            {
                if(_rarityColor.rarity != rarity)
                    continue;
                return _rarityColor.color;
            }
            Debug.Log("RarityColor not found.");
            return Color.white;
        }

        protected void SetToyoId(string id) => SetTextInLabel(tokenIdNumber, id);

        protected void SetToyoStats(ToyoObject selectedToyo = null)
        {
            /*if (selectedToyo == null)
                return;*/
            foreach (TOYO_STAT _stat in Enum.GetValues(typeof(TOYO_STAT)))
                SetStatUI(_stat, selectedToyo);
        }

        protected void SetStatUI(TOYO_STAT stat, ToyoObject selectedToyo)
        {
            selectedToyo ??= _toyoObject;
            var _progressBar = GetProgressBar(stat);
            var _statValue = selectedToyo.GetToyoStat(stat);
            //var _statValue = selectedToyo.GetToyoStatAverageAcrossAllParts(stat);

            maxStatValue = _progressBar.highValue;
            _progressBar.lowValue = _statValue <= maxStatValue ? _statValue : maxStatValue;

            var _progressFieldName = GetFieldName(stat, "Value");
            SetTextInLabel(_progressFieldName, /*_progressBar.lowValue.ToString()*/ _statValue.ToString());
            //Debug.Log(_progressFieldName + _statValue);
            SetTextInLabel(toyoHearthBoundField, selectedToyo.GetToyoHearthBound().ToString());
            SetTextInLabel(toyoLevelField, selectedToyo.GetToyoLevel().ToString());
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
