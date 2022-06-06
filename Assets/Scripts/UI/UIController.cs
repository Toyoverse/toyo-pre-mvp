using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [Header("UI Document")]
        public UIDocument UIDoc;
        protected VisualElement root;
        public List<CustomButton> Buttons;

        private bool buttonEventsEnabled = false;

        public virtual void OnDestroy() => DisableButtonEvents();

        public virtual void EnableButtonEvents()
        {
            root = UIDoc.rootVisualElement;

            if(Buttons.Count <= 0 || root == null)
                return;

            foreach (var cb in Buttons)
            {
                if (root?.Q<Button>(cb.Name) != null)
                {
                    cb.Button ??= root.Q<Button>(cb.Name);
                    cb.Button.clickable.clicked += cb.OnClickEvent.Invoke;
                }
                else
                {
                    Debug.Log(cb.Name + " not found in " + UIDoc.name);
                }
            }
        }

        public virtual void DisableButtonEvents()
        {
            if(Buttons.Count <= 0 || root == null)
                return;
            foreach (var cb in Buttons)
            {
                if (root?.Q<Button>(cb.Name) != null)
                {
                    cb.Button ??= root.Q<Button>(cb.Name);
                    cb.Button.clickable.clicked -= cb.OnClickEvent.Invoke;
                }
                else
                {
                    Debug.Log(cb.Name + " not found in " + UIDoc.name);
                }
            }
        }

        public virtual void BackButton() => ScreenManager.Instance.BackToOldScreen();
    }

    [Serializable]
    public class CustomButton
    {
        public string Name;
        public Button Button;
        public UnityEvent OnClickEvent;
    }
}
