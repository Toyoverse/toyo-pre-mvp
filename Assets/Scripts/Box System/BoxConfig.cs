using System;
using System.Collections.Generic;
using Database;
using UnityEngine;

public class BoxConfig : MonoBehaviour
{
    [SerializeField] private BoxConfigSO boxConfigSo;
    public GameObject model3D;
    public GameObject model2D;
    public List<Box> boxList;

    private string _boxName;
    public string BoxName
        => _boxName ??= boxConfigSo.boxName; 

    private BOX_REGION _boxRegion;
    public BOX_REGION BoxRegion
    {
        get
        {
            if (_boxRegion == BOX_REGION.None)
                _boxRegion = boxConfigSo.boxRegion;
            return _boxRegion;
        }
    }

    private BOX_TYPE _boxType;
    public BOX_TYPE BoxType
    {
        get
        {
            if (_boxType == BOX_TYPE.None)
                _boxType = boxConfigSo.boxType;
            return _boxType;
        }
    }

    private List<BoxRewardSO> _possibleRewards;
    public List<BoxRewardSO> PossibleRewards => _possibleRewards ??= boxConfigSo.possibleRewards;

    private Dictionary<TOYO_RARITY, float> _dropRate;
    public Dictionary<TOYO_RARITY, float> DropRate => _dropRate ??= boxConfigSo.DropRate; 

    public UnboxingVfx unboxingVfx;

    public Box GetFirstUnopenedBox()
    {
        var box = new Box();
        for (var i = 0; i < boxList.Count; i++)
        {
            if(boxList[i].isOpen)
                continue;

            box = boxList[i];
            break;
        }

        return box;
    }

    public string GetFirstUnopenedBoxId()
    {
        var id = "";
        for (var i = 0; i < boxList.Count; i++)
        {
            if(boxList[i].isOpen)
                continue;

            id = boxList[i].objectId;
            break;
        }

        return id;
    }
    
    public int GetQuantity()
    {
        var quantity = 0;

        foreach (var item in boxList)
        {
            if (!item.isOpen)
                quantity++;
        }

        return quantity;
    }

    public void OnEnable()
    {
        if(unboxingVfx == null) 
            unboxingVfx = GetComponentInChildren<UnboxingVfx>();
        if (DatabaseConnection.Instance.isDebug)
            boxList.Add(new ());
    }
}
