
using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Extensions;
using UnityEngine;
using UnityEngine.UIElements;

public class ToyoObject : MonoBehaviour
{

    public GameObject model;
    
    private Dictionary<TOYO_STAT, float> _toyoStats = new(){
        {TOYO_STAT.VITALITY, 0.0f},
        {TOYO_STAT.RESISTANCE, 0.0f},
        {TOYO_STAT.RESILIENCE, 0.0f},
        {TOYO_STAT.PHYSICAL_STRENGTH, 0.0f},
        {TOYO_STAT.CYBER_FORCE, 0.0f},
        {TOYO_STAT.TECHNIQUE, 0.0f},
        {TOYO_STAT.ANALYSIS, 0.0f},
        {TOYO_STAT.AGILITY, 0.0f},
        {TOYO_STAT.SPEED, 0.0f},
        {TOYO_STAT.PRECISION, 0.0f},
        {TOYO_STAT.STAMINA, 0.0f},
        {TOYO_STAT.LUCK, 0.0f},
    };
    
    private Dictionary<TOYO_STAT, float> _bonusStats = new(){
        {TOYO_STAT.VITALITY, 0.0f},
        {TOYO_STAT.RESISTANCE, 0.0f},
        {TOYO_STAT.RESILIENCE, 0.0f},
        {TOYO_STAT.PHYSICAL_STRENGTH, 0.0f},
        {TOYO_STAT.CYBER_FORCE, 0.0f},
        {TOYO_STAT.TECHNIQUE, 0.0f},
        {TOYO_STAT.ANALYSIS, 0.0f},
        {TOYO_STAT.AGILITY, 0.0f},
        {TOYO_STAT.SPEED, 0.0f},
        {TOYO_STAT.PRECISION, 0.0f},
        {TOYO_STAT.STAMINA, 0.0f},
        {TOYO_STAT.LUCK, 0.0f},
    };

    private string _toyoName;
    private int _toyoTotalPartsLevel;
    private int _numberOfParts;

    public float GetToyoStat(TOYO_STAT stat) => _toyoStats[stat];

    public string GetToyoName() => _toyoName;

    public int GetToyoLevel() => _toyoTotalPartsLevel / _numberOfParts;
    
    void SetTotalStats(List<ToyoPart> parts)
    {
        foreach (var _part in parts)
        {
            foreach (TOYO_STAT _stat in Enum.GetValues(typeof(TOYO_STAT)))
            {
                _toyoStats[_stat] += GetStatValue(_stat, _part);
                _bonusStats[_stat] += GetStatValue(_stat, _part, true);
            }

            _toyoTotalPartsLevel += _part.toyoLevel;
        }
            

        foreach (TOYO_STAT _stat in Enum.GetValues(typeof(TOYO_STAT)))
            _toyoStats[_stat] *= _bonusStats[_stat] > 0 ? _bonusStats[_stat] : 1;
    }
    
    private float GetStatValue(TOYO_STAT stat, ToyoPart part, bool isBonusStat = false)
    {
        return stat switch
        {
            TOYO_STAT.VITALITY => isBonusStat ? part.bonusStats.vitality : part.stats.vitality ,
            TOYO_STAT.RESISTANCE => isBonusStat ? part.bonusStats.resistance : part.stats.resistance,
            TOYO_STAT.RESILIENCE => isBonusStat ? part.bonusStats.resilence : part.stats.resilence,
            TOYO_STAT.PHYSICAL_STRENGTH => isBonusStat ? part.bonusStats.physicalStrength : part.stats.physicalStrength,
            TOYO_STAT.CYBER_FORCE => isBonusStat ? part.bonusStats.cyberForce : part.stats.cyberForce,
            TOYO_STAT.TECHNIQUE => isBonusStat ? part.bonusStats.technique : part.stats.technique,
            TOYO_STAT.ANALYSIS => isBonusStat ? part.bonusStats.analysis : part.stats.analysis,
            TOYO_STAT.AGILITY => isBonusStat ? part.bonusStats.agility : part.stats.agility,
            TOYO_STAT.SPEED => isBonusStat ? part.bonusStats.speed : part.stats.speed,
            TOYO_STAT.PRECISION => isBonusStat ? part.bonusStats.precision : part.stats.precision,
            TOYO_STAT.STAMINA => isBonusStat ? part.bonusStats.stamina : part.stats.stamina,
            TOYO_STAT.LUCK => isBonusStat ? part.bonusStats.luck : part.stats.luck,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
        };
    }

    public ToyoObject() : this(Argument<Toyo>.First) { }
    
    public ToyoObject(Toyo toyo)
    {
        SetTotalStats(toyo.parts.ToList());
        
        //Todo Uncoment this when we use toyo rename system
        //_toyoName = !string.IsNullOrEmpty(toyo.name) ? toyo.name : toyo.parts[0].toyoPersona.name;
        _toyoName = toyo.toyoPersona.name;
        _numberOfParts = toyo.parts.Length;
    }

    private void SetToyoModel()
    {
        
    }

}
