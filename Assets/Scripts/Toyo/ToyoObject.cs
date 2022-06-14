
using System.Collections.Generic;
using System.Linq;
using Database;
using Extensions;
using UnityEngine;

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

    public float GetToyoStat(TOYO_STAT stat) => _toyoStats[stat];
    
    void SetTotalStats(List<ToyoPart> parts)
    {
        foreach (var _part in parts)
        {
            _toyoStats[TOYO_STAT.VITALITY] += _part.stats.vitality;
            _toyoStats[TOYO_STAT.RESISTANCE] += _part.stats.resistance;
            _toyoStats[TOYO_STAT.RESILIENCE] += _part.stats.resilence;
            _toyoStats[TOYO_STAT.PHYSICAL_STRENGTH] += _part.stats.physicalStrength;
            _toyoStats[TOYO_STAT.CYBER_FORCE] += _part.stats.cyberForce;
            _toyoStats[TOYO_STAT.TECHNIQUE] += _part.stats.technique;
            _toyoStats[TOYO_STAT.ANALYSIS] += _part.stats.analysis;
            _toyoStats[TOYO_STAT.AGILITY] += _part.stats.agility;
            _toyoStats[TOYO_STAT.SPEED] += _part.stats.speed;
            _toyoStats[TOYO_STAT.PRECISION] += _part.stats.precision;
            _toyoStats[TOYO_STAT.STAMINA] += _part.stats.stamina;
            _toyoStats[TOYO_STAT.LUCK] += _part.stats.luck;
        }
        
        foreach (var _part in parts)
        {
            _toyoStats[TOYO_STAT.VITALITY] *= _part.bonusStats.vitality > 0 ? _part.bonusStats.vitality : 1;
            _toyoStats[TOYO_STAT.RESISTANCE] *= _part.bonusStats.resistance > 0 ? _part.bonusStats.resistance : 1;
            _toyoStats[TOYO_STAT.RESILIENCE] *= _part.bonusStats.resilence > 0 ? _part.bonusStats.resilence : 1;
            _toyoStats[TOYO_STAT.PHYSICAL_STRENGTH] *= _part.bonusStats.physicalStrength > 0 ? _part.bonusStats.physicalStrength : 1;
            _toyoStats[TOYO_STAT.CYBER_FORCE] *= _part.bonusStats.cyberForce > 0 ? _part.bonusStats.cyberForce : 1;
            _toyoStats[TOYO_STAT.TECHNIQUE] *= _part.bonusStats.technique > 0 ? _part.bonusStats.technique : 1;
            _toyoStats[TOYO_STAT.ANALYSIS] *= _part.bonusStats.analysis > 0 ? _part.bonusStats.analysis : 1;
            _toyoStats[TOYO_STAT.AGILITY] *= _part.bonusStats.agility > 0 ? _part.bonusStats.agility : 1;
            _toyoStats[TOYO_STAT.SPEED] *= _part.bonusStats.speed > 0 ? _part.bonusStats.speed : 1;
            _toyoStats[TOYO_STAT.PRECISION] *= _part.bonusStats.precision > 0 ? _part.bonusStats.precision : 1;
            _toyoStats[TOYO_STAT.STAMINA] *= _part.bonusStats.stamina > 0 ? _part.bonusStats.stamina : 1;
            _toyoStats[TOYO_STAT.LUCK] *= _part.bonusStats.luck > 0 ? _part.bonusStats.luck : 1;
        }
    }

    public ToyoObject() : this(Argument<Toyo>.First) { }
    
    public ToyoObject(Toyo toyo)
    {
        SetTotalStats(toyo.parts.ToList());
    }
}
