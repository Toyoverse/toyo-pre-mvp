using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using UnityTemplateProjects.Audio;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [Header("UI Document")]
        public UIDocument uiDoc;

        protected bool carouselButtonsAreInteractable = true;
        private string[] carouselButtons = { "previousToyoButton", "nextToyoButton", "selectToyoButton"};

        protected VisualElement Root => uiDoc.rootVisualElement;
        public List<CustomButton> buttons;

        public virtual void OnDestroy() => DisableScreen();

        public virtual void ActiveScreen()
        {
            uiDoc.gameObject.SetActive(true);
            EnableButtonEvents(buttons);
        }

        public virtual void DisableScreen()
        {
            DisableButtonEvents(buttons);
            uiDoc.gameObject.SetActive(false);
        }

        protected void EnableButtonEvents(List<CustomButton> buttonList)
        {
            if(buttonList.Count <= 0 || Root == null)
                return;
            foreach (var _cb in buttonList)
            {
                if (Root?.Q<Button>(_cb.name) != null)
                {
                    _cb.Button = Root.Q<Button>(_cb.name);
                    _cb.Button.RegisterCallback<MouseUpEvent>
                        (_ =>
                        {
                            PlayClickSound();
                            _cb.onClickEvent.Invoke();
                        });
                }
                else
                    Print.Log(_cb.name + " not found in " + uiDoc.name);
            }

            UpdateUI();
        }

        protected void DisableButtonEvents(List<CustomButton> buttonList)
        {
            if (buttonList.Count <= 0 || Root == null)
                return;

            foreach (var _cb in buttonList)
            {
                if (Root?.Q<Button>(_cb.name) != null)
                {
                    _cb.Button = Root.Q<Button>(_cb.name);
                    _cb.Button.UnregisterCallback<MouseUpEvent>
                        (_ =>
                        {
                            PlayClickSound();
                            _cb.onClickEvent.Invoke();
                        });
                }
                else
                    Print.LogWarning(_cb.name + " not found in " + uiDoc.name);
            }
        }

        protected void PlayClickSound()
        {
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.buttonClickSfx, transform.position);
        }

        public virtual void BackButton() => ScreenManager.Instance.BackToOldScreen();
        
        protected virtual void UpdateUI() {}
        public void CallUpdateUI() => UpdateUI();
        
        protected void SetVisualElementSprite(string visualElementName, Sprite newSprite)
        {
            var _visualE = Root.Q<VisualElement>(visualElementName);
            if(_visualE != null) 
                _visualE.style.backgroundImage = new StyleBackground(newSprite);
        }

        protected void SetTextInLabel(string labelName, string value)
        {
            var _label = Root?.Q<Label>(labelName);
            if (_label != null)
                _label.text = value;
        }

        protected void SetLabelColor(string labelName, Color color)
        {
            var _label = Root?.Q<Label>(labelName);
            if (_label != null)
                _label.style.color = new StyleColor(color);
        }
        
        protected void SetTextInButton(string labelName, string value)
        {
            var _label = Root?.Q<Button>(labelName);
            if (_label != null)
                _label.text = value;
        }

        protected void EnableVisualElement(string elementName)
        {
            var _visualE = Root?.Q<VisualElement>(elementName);
            if (_visualE != null)
                _visualE.style.display = DisplayStyle.Flex;
        }
        
        protected void DisableVisualElement(string elementName)
        {
            var _visualE = Root?.Q<VisualElement>(elementName);
            if (_visualE != null)
                _visualE.style.display = DisplayStyle.None;
        }

        protected void ChangeVisualElementOpacity(string elementName, float opacity)
        {
            var _visualE = Root?.Q<VisualElement>(elementName);
            if (_visualE != null)
                _visualE.style.opacity = opacity;
            //Print.Log(_visualE.name + "OPACITY: " + _visualE.style.opacity);
        }

        protected Label CreateNewLabel(string labelName, string labelText, FontAsset fontAsset, int fontSize, 
            Color fontColor, Color backgroundColor, Texture2D backgroundTexture)
        {
            return new Label()
            {
                name = labelName,
                text = labelText,
                style =
                {
                    fontSize = fontSize,
                    unityFontDefinition = new StyleFontDefinition(fontAsset),
                    backgroundColor = backgroundColor,
                    color = fontColor, 
                    backgroundImage = new StyleBackground(backgroundTexture),
                    paddingLeft = Length.Percent(4)
                }
            };
        }

        protected string ConvertMinutesToString(int durationInMinutes)
        {
            var _minutes = durationInMinutes;
            var _hours = 0;
            var _days = 0;
            while (_minutes >= 60)
            {
                _hours += 1;
                _minutes -= 60;
            }
            while (_hours >= 24)
            {
                _days += 1;
                _hours -= 24;
            }

            var _result = _days > 0 ? _days + "d " : "";
            _result += _hours > 0 ? _hours + "h " : "";
            _result += _minutes + "m";
            return _result;
        }

        protected string ConvertMinutesToString(float durationInMinutes)
        {
            var _minutes = durationInMinutes;
            var _hours = 0;
            var _days = 0;
            while (_minutes >= 60)
            {
                _hours += 1;
                _minutes -= 60;
            }
            while (_hours >= 24)
            {
                _days += 1;
                _hours -= 24;
            }

            var _result = _days > 0 ? _days + "d " : "";
            _result += _hours > 0 ? _hours + "h " : "";
            _result += _minutes + "m";
            return _result;
        }
        
        protected void DisableCarouselButtons()
        {
            if (!carouselButtonsAreInteractable) return;

            carouselButtonsAreInteractable = false;
            foreach (var button in carouselButtons)
            {
                var _label = Root?.Q<Button>(button);
                _label.SetEnabled(false);
            }
        }

        protected void EnableCarouselButtons()
        {
            if (carouselButtonsAreInteractable) return;
            
            carouselButtonsAreInteractable = true;
            foreach (var button in carouselButtons)
            {
                var _label = Root?.Q<Button>(button);
                _label.SetEnabled(true);
            }
        }
        
        protected void ShowComingSoonPopUp() => GenericPopUp.Instance.ShowPopUp("Coming soon...");
    }

    [Serializable]
    public class CustomButton
    {
        public string name;
        public Button Button;
        public UnityEvent onClickEvent;
    }
}
