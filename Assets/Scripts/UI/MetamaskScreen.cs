using System;
using UnityEngine;
using UnityTemplateProjects.Audio;

namespace UI
{
    public class MetamaskScreen : UIController
    {
        private void Start()
        {
            //AudioManager.Instance.startHomePageSfx.PlayOneShot(transform.position);
        }

        public void LoginButton()
        {
            ScreenManager.Instance.GoToScreen(ScreenManager.Instance.haveToyo
                ? ScreenState.MainMenu
                : ScreenState.Welcome);
        }
        
    }
    
}
