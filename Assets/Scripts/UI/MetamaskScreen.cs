using System;
using Database;
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
            DatabaseConnection.Instance.blockchainIntegration.StartLoginMetamask();
        }
    }
}
