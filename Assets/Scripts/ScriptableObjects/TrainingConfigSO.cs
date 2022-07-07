using Database;
using UnityEngine;

[CreateAssetMenu(fileName = "TrainingConfigSO", menuName = "ScriptableObject/TrainingConfigSO")]
public class TrainingConfigSO : ScriptableObject
{
    public TrainingActionSO[] possibleActions;
    public TrainingActionSO[] correctCombination;
    public Card cardReward;
}
