
using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using UnityEngine;
using Extensions;
using System.Linq;
using UnityEngine.Serialization;


public class ToyoManager : Singleton<ToyoManager>
{
    public CarouselManager carouselToyo;
    private List<BoxConfig> _allBoxesConfigInCarousel = new();
    public Transform toyoListParent;
    public GameObject toyoBasePrefab;
    public List<ToyoPersonaSO> toyoPersonaPrefabs;
    
    public Transform openBoxToyoPivot;
    public Transform mainMenuToyoPivot;

    private GameObject _selectedBox;
    
    private List<ToyoObject> _toyoList;
    public List<ToyoObject> ToyoList => _toyoList ??= CreateToyoObjectList();

    public static ToyoObject GetSelectedToyo() => Instance.ToyoList.Find(toyoObject => toyoObject.IsToyoSelected);

    public static void SetSelectedBox(GameObject selectedBox) => Instance._selectedBox = selectedBox;
    
    public static GameObject GetSelectedBox() => Instance._selectedBox;

    public static Camera MainCamera;

    private Player _player;
    public Player Player
    {
        get => _player;
        private set => _player = value;
    }

    public static void MoveToyoToCenterOpenBox()
    {
        GetSelectedToyo().transform
            .SetPositionAndRotation(Instance.openBoxToyoPivot.position, Instance.openBoxToyoPivot.rotation);
        GetSelectedToyo().transform.SetParent(Instance.openBoxToyoPivot);
        GetSelectedToyo().transform.LookAt(MainCamera.transform);
    }
    
    public static void MoveToyoToCenterMainMenu()
    {
        GetSelectedToyo().transform
            .SetPositionAndRotation(Instance.mainMenuToyoPivot.position, Instance.mainMenuToyoPivot.rotation);
        GetSelectedToyo().transform.SetParent(Instance.mainMenuToyoPivot);
        GetSelectedToyo().transform.LookAt(MainCamera.transform);
    }

    public static void AddBoxToGlobalList(BoxConfig box) => Instance._allBoxesConfigInCarousel.Add(box);
    
    List<ToyoObject> CreateToyoObjectList()
    {
        var _databaseToyoList = Instance.Player.toyos.ToList();
        var _toyoObjectList = new List<ToyoObject>();

        var _index = 0;
        foreach (var _databaseToyo in _databaseToyoList)
        {
            var _toyoPrefab = InstantiateAndConfigureToyo(_databaseToyo, ref _toyoObjectList);
            if (_databaseToyo.isToyoSelected)
                carouselToyo.SetFirstSelectedObject(_toyoPrefab.transform, _index);
            _index++;
        }

        return _toyoObjectList;
    }

    private GameObject InstantiateAndConfigureToyo(Toyo databaseToyo, ref List<ToyoObject> toyoObjectList)
    {
        var _toyoPrefab = Instantiate(GetToyoPersonaPrefab(databaseToyo.toyoPersona.objectId), toyoListParent);
        var _toyoObjectInstance =_toyoPrefab.AddComponent<ToyoObject, Toyo>(databaseToyo);
        _toyoObjectInstance.spriteAnimator = _toyoPrefab.GetComponentInChildren<SpriteAnimator>();
        carouselToyo.allObjects.Add(_toyoPrefab.transform);
        toyoObjectList.Add(_toyoObjectInstance);
        return _toyoPrefab;
    }

    public static void InitializeInstance() => Instance.Initialize();
    
    private void Initialize()
    {
        _ = ToyoList;
        MainCamera = FindObjectOfType<Camera>();
        var _boxManager = FindObjectsOfType<CarouselManager>(true).First(manager => !manager.isToyoCarousel);
        AddBoxesToGlobalList(_boxManager.allObjects);
    }
    
    
    private void AddBoxesToGlobalList(List<Transform> allBoxes)
    {
        foreach (var _component in allBoxes.Select(box => box.GetComponent<BoxConfig>()))
            AddBoxToGlobalList(_component);
    }

    private GameObject GetToyoPersonaPrefab(string objectId)
        => toyoPersonaPrefabs.FirstOrDefault(toyoPersona => objectId == toyoPersona.objectId)?.toyoPrefab;

    public static void SetPlayerData(Player playerData)
    {
        Instance.Player = playerData;
    }


    
    public static void SetPlayerBoxes()
    {
        foreach (var _boxFromPlayer in Instance.Player.boxes)
        {
            foreach (var _boxConfigInCarousel in Instance._allBoxesConfigInCarousel
                         .Where(t => 
                            GetBoxTypeInPlayerBox(_boxFromPlayer) == t.BoxType && 
                            GetBoxRegionInPlayerBox(_boxFromPlayer) == t.BoxRegion)) 
                _boxConfigInCarousel.boxList.Add(_boxFromPlayer);
        }
    }

    private static BOX_TYPE GetBoxTypeInPlayerBox(Box box)
    {
        return box.type?.ToUpper() switch
        {
            "REGULAR" => BOX_TYPE.Regular,
            "FORTIFIED" => BOX_TYPE.Fortified,
            _ => BOX_TYPE.None
        };
    }

    private static BOX_REGION GetBoxRegionInPlayerBox(Box box)
    {
        var _boxRegion = "";
        _boxRegion = string.IsNullOrEmpty(box.region.name) ? box.toyo?.toyoPersona?.region : box.type;
        
        return _boxRegion?.ToUpper() switch
        {
            "KYTUNT" => BOX_REGION.Kytunt,
            "JAKANA" => BOX_REGION.Jakana,
            _ => BOX_REGION.None
        };
    }
}