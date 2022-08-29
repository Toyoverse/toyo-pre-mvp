using UnityEngine;
using UnityTemplateProjects.Audio;

namespace UI
{
    public class WelcomeScreen : UIController
    {
        private void Start()
            => AudioManager.Instance.startHomePageSfx.PlayOneShot(transform.position);

        public void OpenSeaButton()
            => Application.OpenURL("https://opensea.io/collection/toyo-first-9-jakana");

        public void BoxesButton()
            => ScreenManager.Instance.GoToScreen(ScreenState.BoxInfo);

        public void OtherButton()
            => Debug.Log("Other button has clicked!");
    }
}
