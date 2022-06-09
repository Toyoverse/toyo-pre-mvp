using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoxSO", menuName = "ScriptableObject/BoxSO")]
public class BoxConfigSO : ScriptableObject
{
    public string boxName;
    public BOX_REGION boxRegion;
    public BOX_TYPE boxType;
    public List<BoxRewardSO> possibleRewards;
    [SerializeField] private List<DropRate> dropRate;
    private Dictionary<TOYO_RARITY, float> _dropRateDict;
    public Dictionary<TOYO_RARITY, float> DropRate
    {
        get
        {
            if (_dropRateDict == null)
            {
                _dropRateDict = new Dictionary<TOYO_RARITY, float>();
                foreach (var _dpr in dropRate)
                {
                    _dropRateDict.Add(_dpr.Rarity, _dpr.DropChance);
                }
            }

            return _dropRateDict;
        }
    }
}

public enum BOX_TYPE
{
    None = 0,
    Regular = 1,
    Fortified = 2,
}

public enum BOX_REGION
{
    None = 0,
    Kytunt = 1,
    Jakana = 2
}

[Serializable]
public class DropRate
{
    public TOYO_RARITY Rarity;
    [Range(0, 100)] 
    public float DropChance;
}