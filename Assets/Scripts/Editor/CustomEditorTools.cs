using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(CarouselManager))]
    public class CustomButton : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CarouselManager myScript = (CarouselManager)target;
            if (GUILayout.Button("Right Swipe"))
            {
                myScript.SwipeRight();
            }
            if (GUILayout.Button("Left Swipe"))
            {
                myScript.SwipeLeft();
            }
        }
    }

    [CustomEditor(typeof(TrainingConfigSO))]
    public class TrainingConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TrainingConfigSO _myScript = (TrainingConfigSO)target;
            if (GUILayout.Button("Send TrainingConfig to Server"))
                _myScript.SendToServer();
        }
    }
}