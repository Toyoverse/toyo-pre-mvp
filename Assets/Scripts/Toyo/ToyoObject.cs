
using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Extensions;
using UnityEngine;
using UnityEngine.UIElements;

public class ToyoObject : MonoBehaviour
{
    public string tokenId;
    public GameObject model;
    public SpriteAnimator spriteAnimator;
    
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

    public bool IsToyoSelected;
    private string _toyoName;
    private int _toyoTotalPartsLevel;
    private int _toyoTotalPartsHearthbound;
    private int _numberOfParts;
    private string _rarity;
    private float _minimunToyoStat = 9.0f; //Minimun value so it doesn't break the progress bar.
    
    private int _highLevelInToyoParts;
    private int _fixedHeartBound = 20;

    public float GetToyoStat(TOYO_STAT stat) => _toyoStats[stat] > _minimunToyoStat ? _toyoStats[stat] : _minimunToyoStat;
    public float GetToyoStatAverageAcrossAllParts(TOYO_STAT stat)
    {
        var _averageValue = _numberOfParts != 0 ? _toyoStats[stat] / _numberOfParts : _toyoStats[stat];
        return _averageValue > _minimunToyoStat ? _averageValue : _minimunToyoStat;
    }

    public string GetToyoName() => _toyoName;
    
    public TOYO_RARITY GetToyoRarity() => ConvertStringToRarity(_rarity);

    public int GetToyoLevel() => _highLevelInToyoParts; /*_numberOfParts != 0 ? _toyoTotalPartsLevel / _numberOfParts : _toyoTotalPartsLevel;*/

    public int GetToyoHearthBound() => _fixedHeartBound; /*_numberOfParts != 0 ? _toyoTotalPartsHearthbound / _numberOfParts : _toyoTotalPartsHearthbound;*/
    
    void SetTotalStats(List<ToyoPart> parts)
    {
        foreach (var _part in parts)
        {
            foreach (TOYO_STAT _stat in Enum.GetValues(typeof(TOYO_STAT)))
            {
                _toyoStats[_stat] += GetStatValue(_stat, _part);
                _bonusStats[_stat] += GetStatValue(_stat, _part, true);
            }

            _toyoTotalPartsLevel += _part.level;
            _toyoTotalPartsHearthbound += _part.hearthbound;
        }
            

        foreach (TOYO_STAT _stat in Enum.GetValues(typeof(TOYO_STAT)))
            _toyoStats[_stat] *= _bonusStats[_stat] > 0 ? _bonusStats[_stat] : 1;
    }

    private int GetHighLevelInToyoParts(List<ToyoPart> parts)
    {
        var _result = 0;
        foreach (var _part in parts)
        {
            if (_part.level > _result)
                _result = _part.level;
        }
        return _result;
    }
    
    private float GetStatValue(TOYO_STAT stat, ToyoPart part, bool isBonusStat = false)
    {
        return stat switch
        {
            TOYO_STAT.VITALITY => isBonusStat ? part.bonusStats.vitality : part.stats.vitality ,
            TOYO_STAT.RESISTANCE => isBonusStat ? part.bonusStats.resistance : part.stats.resistance,
            TOYO_STAT.RESILIENCE => isBonusStat ? part.bonusStats.resilience : part.stats.resilience,
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
        if (toyo.parts != null)
        {
            SetTotalStats(toyo.parts.ToList());
            _numberOfParts = toyo.parts.Length;
        }
        else
            Debug.LogError(toyo.name + ".parts is null.");

        if (toyo.parts != null) 
            _highLevelInToyoParts = GetHighLevelInToyoParts(toyo.parts.ToList());

        _rarity = toyo.toyoPersonaOrigin.rarity;
        
        //Todo Uncoment this when we use toyo rename system
        //_toyoName = !string.IsNullOrEmpty(toyo.name) ? toyo.name : toyo.parts[0].toyoPersona.name;
        _toyoName = toyo.toyoPersonaOrigin.name;
        IsToyoSelected = toyo.isToyoSelected;
    }

    private void SetToyoModel()
    {
        
    }

    private TOYO_RARITY ConvertStringToRarity(string rarityString)
    {
        return rarityString switch
        {
            "COMMON" => TOYO_RARITY.COMMON,
            "UNCOMMON" => TOYO_RARITY.UNCOMMON,
            "RARE" => TOYO_RARITY.RARE,
            "LIMITED" => TOYO_RARITY.LIMITED,
            "COLLECTOR" => TOYO_RARITY.COLLECTOR,
            "PROTOTYPE" => TOYO_RARITY.PROTOTYPE,
            _ => TOYO_RARITY.COMMON
        };
    }
}
