
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
    }

    public ToyoObject() : this(Argument<Toyo>.First) { }
    
    public ToyoObject(Toyo toyo)
    {
        SetTotalStats(toyo.parts.ToList());
    }
}
