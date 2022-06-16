using UnityEngine;

[CreateAssetMenu(fileName = "TrainingActionSO", menuName = "ScriptableObject/TrainingActionSO")]
public class TrainingActionSO : ScriptableObject
{
    public string name;
    public TrainingActionType type;
    public Sprite sprite;
    public int id;
}
