using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "CardSO", menuName = "ScriptableObject/CardSO")]
public class CardRewardSO : ScriptableObject
{
    private Guid _guid;
    [SerializeField] private string guidString;
    public int id;
    public string cardName;
    public string description;
    public string memory;
    public CARD_TYPE cardType;
    public Sprite cardImage;
    public ToyoPersonaSO toyoPersona;
    public TrainingActionSO[] correctCombination;
    public string imageURL;
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        var path = AssetDatabase.GetAssetPath(this);
        _guid = new Guid(AssetDatabase.AssetPathToGUID(path));
        guidString = _guid.ToString();
    }
    #endif
}
