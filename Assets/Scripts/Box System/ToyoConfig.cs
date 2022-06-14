using System;
using System.Collections;
using System.Collections.Generic;
using Database;
using UnityEngine;

public class ToyoConfig : MonoBehaviour
{
    public GameObject model;

    public Toyo toyo;
    public Dictionary<string, float> TotalStats;

    public void OnEnable()
    {
        SetTotalStats();
    }

    public void SetThisToyo(Toyo toyo)
    {
        this.toyo = toyo;
    }

    private void SetTotalStats()
    {
        foreach (var _toyoPart in toyo.parts)
        {
            foreach (var _stat in _toyoPart.stats)
            {
                
            }
        }
    }
}
