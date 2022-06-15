
using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using UnityEngine;
using Extensions;
using System.Linq;


public class ToyoManager : MonoBehaviour
{
    public CarouselManager carouselToyo;
    public Transform toyoListParent;
    public GameObject toyoBasePrefab;
    public List<ToyoPersonaSO> toyoPersonaPrefabs;
    
    
    private List<ToyoObject> _toyoList;
    public List<ToyoObject> ToyoList => _toyoList ??= CreateToyoObjectList();
    

    List<ToyoObject> CreateToyoObjectList()
    {
        var _databaseToyoList = DatabaseConnection.Instance.player.toyos.ToList();
        var _toyoObjectList = new List<ToyoObject>();
        
        foreach (var _databaseToyo in _databaseToyoList)
        {
            var _toyoPrefab = Instantiate(GetToyoPersonaPrefab(_databaseToyo.toyoPersona.objectId), toyoListParent);
            var _toyoObjectInstance =_toyoPrefab.AddComponent<ToyoObject, Toyo>(_databaseToyo);
            carouselToyo.allObjects.Add(_toyoPrefab.transform);
            _toyoObjectList.Add(_toyoObjectInstance);
        }

        return _toyoObjectList;
    }

    private void Start()
    {
        _ = ToyoList;
    }

    private GameObject GetToyoPersonaPrefab(string objectId)
    {
        return toyoPersonaPrefabs.FirstOrDefault(toyoPersona => objectId == toyoPersona.objectId)?.toyoPrefab;
    }
}
