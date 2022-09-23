using Tools;
using UnityEngine;
using UnityTemplateProjects.Audio;

namespace UI
{
    public class WelcomeScreen : UIController
    {
        private const string OpenSeaURL = "https://opensea.io/collection/toyo-seed-boxes";
        
        private void Start()
            => AudioManager.Instance.startHomePageSfx.PlayOneShot(transform.position);

        public void OpenSeaButton()
            => Application.OpenURL(OpenSeaURL);

        public void BoxesButton()
            => ScreenManager.Instance.GoToScreen(ScreenState.BoxInfo);

        public void OtherButton()
            => Print.Log("Other button has clicked!");
    }
}
