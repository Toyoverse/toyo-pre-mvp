using UnityEngine;

[CreateAssetMenu(fileName = "CardSO", menuName = "ScriptableObject/CardSO")]
public class CardRewardSO : ScriptableObject
{
    public int id;
    public string cardName;
    public string description;
    public string memory;
    public CARD_TYPE cardType;
    public Sprite cardImage;
    public ToyoPersonaSO toyoPersona;
    public TrainingActionSO[] correctCombination;
    public string imageURL;
}
