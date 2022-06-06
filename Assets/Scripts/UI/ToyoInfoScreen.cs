using UnityEngine;

namespace UI
{
    public class ToyoInfoScreen : UIController
    {
        public void ChangeToyoButton(int i)
        {
            var test = 0;
            test = i > 0 ? 1 : -1;
            //Add or remove index to TOYO list
            Debug.Log("ChangeToyo " + test);
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