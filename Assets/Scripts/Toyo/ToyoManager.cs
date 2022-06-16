﻿
using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using UnityEngine;
using Extensions;
using System.Linq;
using UnityEngine.Serialization;


public class ToyoManager : MonoBehaviour
{
    public CarouselManager carouselToyo;
    public Transform toyoListParent;
    public GameObject toyoBasePrefab;
    public List<ToyoPersonaSO> toyoPersonaPrefabs;
    
    public Transform openBoxToyoPivot;
    
    
    private List<ToyoObject> _toyoList;
    public List<ToyoObject> ToyoList => _toyoList ??= CreateToyoObjectList();

    private static ToyoManager _instance;

    public static ToyoObject GetSelectedToyo() => _instance.ToyoList.Find(toyoObject => toyoObject.IsToyoSelected);
    
    public static void MoveToyoToCenter() => GetSelectedToyo().transform.SetPositionAndRotation(_instance.openBoxToyoPivot.position, _instance.openBoxToyoPivot.rotation);
    

    List<ToyoObject> CreateToyoObjectList()
    {
        var _databaseToyoList = DatabaseConnection.Instance.player.toyos.ToList();
        var _toyoObjectList = new List<ToyoObject>();

        var _index = 0;
        foreach (var _databaseToyo in _databaseToyoList)
        {
            var _toyoPrefab = Instantiate(GetToyoPersonaPrefab(_databaseToyo.toyoPersona.objectId), toyoListParent);
            var _toyoObjectInstance =_toyoPrefab.AddComponent<ToyoObject, Toyo>(_databaseToyo);
            carouselToyo.allObjects.Add(_toyoPrefab.transform);
            _toyoObjectList.Add(_toyoObjectInstance);
            if (_databaseToyo.isToyoSelected)
                carouselToyo.SetFirstSelectedObject(_toyoPrefab.transform, _index);
                
            _index++;
        }

        return _toyoObjectList;
    }

    private void Start()
    {
        _ = ToyoList;
        _instance = this;
    }

    private GameObject GetToyoPersonaPrefab(string objectId)
    {
        return toyoPersonaPrefabs.FirstOrDefault(toyoPersona => objectId == toyoPersona.objectId)?.toyoPrefab;
    }
}
