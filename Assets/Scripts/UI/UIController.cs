using System;
using System.Collections.Generic;
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

        protected VisualElement Root => uiDoc.rootVisualElement;
        public List<CustomButton> buttons;

        public virtual void OnDestroy() => DisableScreen();

        public virtual void ActiveScreen()
        {
            uiDoc.gameObject.SetActive(true);
            EnableButtonEvents();
        }

        public virtual void DisableScreen()
        {
            DisableButtonEvents();
            uiDoc.gameObject.SetActive(false);
        }

        private void EnableButtonEvents()
        {
            if(buttons.Count <= 0 || Root == null)
                return;
            foreach (var _cb in buttons)
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
                {
                    Debug.Log(_cb.name + " not found in " + uiDoc.name);
                }
            }

            UpdateUI();
        }

        private void DisableButtonEvents()
        {
            if (buttons.Count <= 0 || Root == null)
                return;

            foreach (var _cb in buttons)
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
                {
                    Debug.Log(_cb.name + " not found in " + uiDoc.name);
                }
            }
        }

        private void PlayClickSound()
        {
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.buttonClickSfx, transform.position);
        }

        public virtual void BackButton() => ScreenManager.Instance.BackToOldScreen();
        
        protected virtual void UpdateUI() {}
        
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

        protected Label CreateNewLabel(string labelName, string labelText, FontAsset fontAsset, int fontSize, Color fontColor, Color backgroundColor)
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
                    color = fontColor
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

        
        private readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        protected long GetTimeStampFromDate(DateTime date)
        {
            TimeSpan _elapsedTime = date - _epoch;
            return (long) _elapsedTime.TotalSeconds;
        }

        /*protected DateTime GetDateFromTimeStamp(long timeStamp)
        {
            
        }*/

        protected long GetActualTimeStamp()
            => (long)System.DateTime.UtcNow.Subtract(new System.DateTime(
                1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }

    [Serializable]
    public class CustomButton
    {
        public string name;
        public Button Button;
        public UnityEvent onClickEvent;
    }
}
