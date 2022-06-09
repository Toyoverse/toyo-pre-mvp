using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityTemplateProjects.Audio;

namespace UI
{
    public class GenericPopUp : MonoBehaviour
    {
        public static GenericPopUp Instance;
        
        public UIDocument popUp;
        private Label _messageLabel; 
        private Button _positiveButton;
        private Button _negativeButton;
        private const string PositiveName = "PositiveButton";
        private const string NegativeName = "NegativeButton";
        private const string LabelName = "message";

        private PopUpInfo _pInfo;

        private void Start()
        {
            if(Instance != null)
                Destroy(gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ShowPopUp(string message, Action positiveAction = null, Action negativeAction = null)
        {
            InitVariables();
            _pInfo = new PopUpInfo
            {
                MessageText = message,
                PositiveAction = positiveAction,
                NegativeAction = negativeAction,
            };
            _messageLabel.text = message;
            _positiveButton.text = negativeAction != null ? "Yes" : "Ok"; 
            _positiveButton.RegisterCallback<ClickEvent>(_ =>
            {
                _pInfo.PositiveAction?.Invoke();
                AudioManager.Instance.buttonAcceptSfx.PlayOneShot(transform.position);
                HidePopUp();
            });
            if (negativeAction == null)
            {
                DisableNegativeButton();
                return;
            }
            _negativeButton.text = "No";
            _negativeButton.RegisterCallback<ClickEvent>(_ =>
            {
                _pInfo.NegativeAction();
                AudioManager.Instance.buttonDenySfx.PlayOneShot(transform.position);
                HidePopUp();
            });
        }

        public void ShowPopUp(PopUpInfo pInfo)
        {
            _pInfo = pInfo;
            InitVariables();
            _messageLabel.text = _pInfo.MessageText;
            _positiveButton.text = _pInfo.PositiveText;
            _positiveButton.RegisterCallback<ClickEvent>(_ =>
            {
                _pInfo.PositiveAction?.Invoke();
                AudioManager.Instance.buttonAcceptSfx.PlayOneShot(transform.position);
                HidePopUp();
            });
            if (_pInfo.NegativeAction == null)
            {
                DisableNegativeButton();
                return;
            }
            _negativeButton.text = _pInfo.NegativeText;
            _negativeButton.RegisterCallback<ClickEvent>(_ =>
            {
                _pInfo.NegativeAction();
                AudioManager.Instance.buttonDenySfx.PlayOneShot(transform.position);
                HidePopUp();
            });
        }

        private void InitVariables()
        {
            popUp.gameObject.SetActive(true);
            var _root = popUp.rootVisualElement;
            _positiveButton = _root.Q<Button>(PositiveName);
            _negativeButton = _root.Q<Button>(NegativeName);
            _messageLabel = _root.Q<Label>(LabelName);
            EnableNegativeButton();
        }

        private void HidePopUp()
        {
            _positiveButton.clicked -= _pInfo.PositiveAction;
            _negativeButton.clicked -= _pInfo.NegativeAction;
            popUp.gameObject.SetActive(false);
            ResetPopUpInfo();
        }

        private void ResetPopUpInfo()
        {
            _pInfo = new PopUpInfo
            {
                MessageText = "",
                PositiveText = "",
                NegativeText = "",
                PositiveAction = null,
                NegativeAction = null
            };
        }
        
        private void EnableNegativeButton() =>
            _negativeButton.style.display = DisplayStyle.Flex;

        private void DisableNegativeButton() =>
            _negativeButton.style.display = DisplayStyle.None;
    }

    public struct PopUpInfo
    {
        public string MessageText;
        public string PositiveText;
        public string NegativeText;

        public Action PositiveAction;
        public Action NegativeAction;

        public PopUpInfo(string message, string positiveText, string negativeText, Action positiveAction = null, Action negativeAction = null)
        {
            MessageText = message;
            PositiveText = positiveText;
            NegativeText = negativeText;
            PositiveAction = positiveAction;
            NegativeAction = negativeAction;
        }
    }
}
