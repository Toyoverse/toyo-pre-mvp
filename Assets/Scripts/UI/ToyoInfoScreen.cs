using UnityEngine;

namespace UI
{
    public class ToyoInfoScreen : UIController
    {
        public void NextToyoButton()
        {
            Debug.Log("ChangeToyo Next");
        }
        public void PreviousToyoButton()
        {
            Debug.Log("ChangeToyo Previous");
        }

        public void OverviewButton()
        {
            Debug.Log("Overview button clicked!");
        }

        public void ToyoPartsButton()
        {
            Debug.Log("ToyoParts button clicked!");
        }
    }
}