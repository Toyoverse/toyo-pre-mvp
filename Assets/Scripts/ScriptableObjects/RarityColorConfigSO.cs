using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RarityColorConfigSO", menuName = "ScriptableObject/RarityColorConfigSO")]
public class RarityColorConfigSO : ScriptableObject
{
    public RarityColor[] rarityColors;
}

[Serializable]
public class RarityColor
{
    public Color color;
    public TOYO_RARITY rarity;
}
