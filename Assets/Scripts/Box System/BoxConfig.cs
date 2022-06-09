using System;
using System.Collections.Generic;
using UnityEngine;

public class BoxConfig : MonoBehaviour
{
    public BoxConfigSO boxConfig;
    public GameObject model;
    
    public string BoxName { get; private set; }
    public BOX_REGION BoxRegion { get; private set; }
    public BOX_TYPE BoxType { get; private set; }
    public List<BoxRewardSO> PossibleRewards { get; private set; }
    public Dictionary<TOYO_RARITY, float> DropRate { get; private set; }

    public void OnEnable()
    {
        BoxName = boxConfig.boxName;
        BoxRegion = boxConfig.boxRegion;
        BoxType = boxConfig.boxType;
        PossibleRewards = boxConfig.possibleRewards;
        DropRate = boxConfig.DropRate;
        if(model == null)
            model = GetComponentInChildren<GameObject>();
    }
}
