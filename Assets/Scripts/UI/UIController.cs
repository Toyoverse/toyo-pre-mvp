using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
                    _cb.Button.clickable.clicked += PlayClickSound;
                    _cb.Button.clickable.clicked += _cb.onClickEvent.Invoke;
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
                    _cb.Button.clickable.clicked -= PlayClickSound;
                    _cb.Button.clickable.clicked -= _cb.onClickEvent.Invoke;
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
    }

    [Serializable]
    public class CustomButton
    {
        public string name;
        public Button Button;
        public UnityEvent onClickEvent;
    }
}
