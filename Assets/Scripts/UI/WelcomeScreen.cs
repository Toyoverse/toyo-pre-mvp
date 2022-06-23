using System;
using UnityEngine;
using UnityTemplateProjects.Audio;

namespace UI
{
    public class WelcomeScreen : UIController
    {
        
        private void Start()
        {
            AudioManager.Instance.startHomePageSfx.PlayOneShot(transform.position);
            
        }

        public void OpenSeaButton()
        {
            ScreenManager.Instance.haveToyo = true;
            Debug.Log("OpenSea button has clicked!");
        }

        public void BoxesButton()
            => ScreenManager.Instance.GoToScreen(ScreenState.BoxInfo);

        public void OtherButton()
            => Debug.Log("Other button has clicked!");
    }
}
