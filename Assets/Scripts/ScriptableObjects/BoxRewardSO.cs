using UnityEngine;

[CreateAssetMenu(fileName = "BoxRewardSO", menuName = "ScriptableObject/BoxRewardSO")]
public class BoxRewardSO : ScriptableObject
{
    public string rewardName;
    public Sprite sprite;
    public TOYO_RARITY toyoRarity;

    /*public ITEM_TYPE ItemType;
    [Header("If ToyoPart")]
    public TOYO_PIECE ToyoPiece;
    public TOYO_RARITY PieceRarity;
    
    [Header("If Card")]
    public CARD_TYPE CardType;*/
}
/*
public enum ITEM_TYPE
{
    NONE = 0,
    CARD = 1,
    TOYO_PART = 2,
    FULL_TOYO = 3
}*/