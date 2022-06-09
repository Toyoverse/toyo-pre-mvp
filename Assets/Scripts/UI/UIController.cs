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

        public virtual void OnDestroy() => DisableButtonEvents();

        public void EnableButtonEvents()
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
        }

        public void DisableButtonEvents()
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
    }

    [Serializable]
    public class CustomButton
    {
        public string name;
        public Button Button;
        public UnityEvent onClickEvent;
    }
}
