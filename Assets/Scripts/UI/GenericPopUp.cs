using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UI
{
    public class GenericPopUp : MonoBehaviour
    {
        public static GenericPopUp Instance;
        
        public UIDocument PopUp;
        private Label MessageLabel; 
        private Button PositiveButton;
        private Button NegativeButton;
        private const string PositiveName = "PositiveButton";
        private const string NegativeName = "NegativeButton";
        private const string LabelName = "message";

        private PopUpInfo pInfo;

        private void Start()
        {
            if(Instance != null)
                Destroy(this.gameObject);
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        public void ShowPopUp(string message, Action positiveAction = null, Action negativeAction = null)
        {
            InitVariables();
            pInfo = new PopUpInfo
            {
                messageText = message,
                positiveAction = positiveAction,
                negativeAction = negativeAction,
            };
            MessageLabel.text = message;
            PositiveButton.text = negativeAction != null ? "Yes" : "Ok"; 
            PositiveButton.RegisterCallback<ClickEvent>(evt =>
            {
                pInfo.positiveAction?.Invoke();
                HidePopUp();
            });
            if (negativeAction == null)
            {
                DisableNegativeButton();
                return;
            }
            NegativeButton.text = "No";
            NegativeButton.RegisterCallback<ClickEvent>(evt =>
                {
                    pInfo.negativeAction();
                    HidePopUp();
                });
        }

        public void ShowPopUp(PopUpInfo _pInfo)
        {
            pInfo = _pInfo;
            InitVariables();
            MessageLabel.text = pInfo.messageText;
            PositiveButton.text = pInfo.positiveText;
            PositiveButton.RegisterCallback<ClickEvent>(evt =>
            {
                pInfo.positiveAction?.Invoke();
                HidePopUp();
            });
            if (pInfo.negativeAction == null)
            {
                DisableNegativeButton();
                return;
            }
            NegativeButton.text = pInfo.negativeText;
            NegativeButton.RegisterCallback<ClickEvent>(evt =>
                {
                    pInfo.negativeAction();
                    HidePopUp();
                });
        }

        private void InitVariables()
        {
            PopUp.gameObject.SetActive(true);
            var root = PopUp.rootVisualElement;
            PositiveButton = root.Q<Button>(PositiveName);
            NegativeButton = root.Q<Button>(NegativeName);
            MessageLabel = root.Q<Label>(LabelName);
            EnableNegativeButton();
        }

        private void HidePopUp()
        {
            PositiveButton.clicked -= pInfo.positiveAction;
            NegativeButton.clicked -= pInfo.negativeAction;
            PopUp.gameObject.SetActive(false);
            ResetPopUpInfo();
        }

        private void ResetPopUpInfo()
        {
            pInfo = new PopUpInfo
            {
                messageText = "",
                positiveText = "",
                negativeText = "",
                positiveAction = null,
                negativeAction = null
            };
        }
        

        private void EnableNegativeButton() =>
            NegativeButton.style.display = DisplayStyle.Flex;

        private void DisableNegativeButton() =>
            NegativeButton.style.display = DisplayStyle.None;
    }

    public struct PopUpInfo
    {
        public string messageText;
        public string positiveText;
        public string negativeText;

        public Action positiveAction;
        public Action negativeAction;

        public PopUpInfo(string message, string positiveText, string negativeText, Action positiveAction = null, Action negativeAction = null)
        {
            messageText = message;
            this.positiveText = positiveText;
            this.negativeText = negativeText;
            this.positiveAction = positiveAction;
            this.negativeAction = negativeAction;
        }
    }
}
