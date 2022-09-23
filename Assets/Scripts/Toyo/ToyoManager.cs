
using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using UnityEngine;
using Extensions;
using System.Linq;
using Tools;
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

    public RarityColorConfigSO rarityColorsConfig;
    private Dictionary<string, bool> _isStakeBackup = new ();

    public static ToyoObject GetSelectedToyo()
    {
        if (Instance.ToyoList.Count == 0)
        {
            Print.Log("No toyo to select. Toyo List is empty!");
            return null;
        }
        if (Instance.ToyoList.Find(toyoObject => toyoObject.IsToyoSelected) == null)
            SetFirstToyoToSelected();
        var _selectedToyo = Instance.ToyoList.Find(toyoObject => toyoObject.IsToyoSelected);
        return _selectedToyo;
    }

    private static void SetFirstToyoToSelected()
    {
        Instance.ToyoList[0].IsToyoSelected = true;
        Instance.carouselToyo.SetFirstSelectedObject();
    }

    public void AddToyoToToyoObjectList(Toyo toyo)
    {
        if (GetSelectedToyo() != null)
        {
            GetSelectedToyo().transform.SetParent(carouselToyo.StartingPosition);
            GetSelectedToyo().IsToyoSelected = false;
        }

        toyo.isToyoSelected = true;
        var toyoPrefab = InstantiateAndConfigureToyo(toyo, ref _toyoList);
        var databaseToyoList = Instance.Player.toyos.ToList();
        carouselToyo.SetFirstSelectedObject	(toyoPrefab.transform, databaseToyoList.Count - 1);
    }

    public static void SetSelectedBox(GameObject selectedBox) => Instance._selectedBox = selectedBox;
    
    public static GameObject GetSelectedBox() => Instance._selectedBox;

    public static Camera MainCamera;

    private Player _player;
    
    public ToyoPersonaSO toyoPersonaSOTestPreventsNull;
    
    public Player Player
    {
        get => _player;
        set => _player = value;
    }

    public static void MoveToyoToCenterOpenBox()
    {
        var _toyoTransform = GetSelectedToyo().transform;
        _toyoTransform.SetPositionAndRotation(Instance.openBoxToyoPivot.position, Instance.openBoxToyoPivot.rotation);
        _toyoTransform.transform.SetParent(Instance.openBoxToyoPivot);
        _toyoTransform.transform.LookAt(MainCamera.transform);
    }
    
    public static void MoveToyoToCenterMainMenu()
    {
        var _toyoTransform = GetSelectedToyo().transform;
        _toyoTransform.SetPositionAndRotation(Instance.mainMenuToyoPivot.position, Instance.mainMenuToyoPivot.rotation);
        _toyoTransform.transform.SetParent(Instance.mainMenuToyoPivot);
        _toyoTransform.transform.LookAt(MainCamera.transform);
        CentralizePlatformInMainMenu();
    }

    private static void CentralizePlatformInMainMenu()
    {
        var _platform = GetSelectedToyo().transform.GetChild(1);
        var _rotation = _platform.rotation;
        _rotation.z = 0;
        _platform.localRotation = _rotation;
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
        var _toyoPrefab = Instantiate(GetToyoPersonaPrefab(databaseToyo.toyoPersonaOrigin), toyoListParent);
        var _toyoObjectInstance = _toyoPrefab.AddComponent<ToyoObject, Toyo>(databaseToyo);
        _toyoObjectInstance.tokenId = databaseToyo.tokenId;
        _toyoObjectInstance.spriteAnimator = _toyoPrefab.GetComponentInChildren<SpriteAnimator>();
        _toyoObjectInstance.isStaked = GetIsStakedById(_toyoObjectInstance.tokenId);
        _toyoObjectInstance.isAutomata = databaseToyo.toyoPersonaOrigin.isAutomata;
        carouselToyo.allObjects.Add(_toyoPrefab.transform);
        toyoObjectList.Add(_toyoObjectInstance);
        return _toyoPrefab;
    }

    public static void InitializeBoxes() => Instance.InitializeBoxesFromDatabase();
    
    private void InitializeBoxesFromDatabase()
    {
        MainCamera = Camera.main;
        var _boxManager = FindObjectsOfType<CarouselManager>(true).First(manager => !manager.isToyoCarousel);
        AddBoxesToGlobalList(_boxManager.allObjects);
    }

    public static void InitializeToyos() => Instance.InitializeToyosFromDatabase();

    public static void StartGame()
    {
        InitializeToyos();
        Loading.EndLoading?.Invoke();
    }
    
    private void InitializeToyosFromDatabase() => _ = ToyoList;
    
    private void AddBoxesToGlobalList(List<Transform> allBoxes)
    {
        foreach (var _component in allBoxes.Select(box => box.GetComponent<BoxConfig>()))
            AddBoxToGlobalList(_component);
    }

    private GameObject GetToyoPersonaPrefab(ToyoPersona toyoPersona)
    {
        ToyoPersonaSO _first = null;
        foreach (var _toyoPersonaPrefab in toyoPersonaPrefabs)
        {
            if (string.Equals(_toyoPersonaPrefab.toyoName, toyoPersona.name, StringComparison.CurrentCultureIgnoreCase))
            {
                _first = _toyoPersonaPrefab;
                break;
            }
        }

        if (_first == null) //TODO: Remove after fix null
            return toyoPersonaSOTestPreventsNull.toyoPrefab;
        return _first?.toyoPrefab;
    }

    public static void SetPlayerData(Player playerData)
    {
        Instance.Player = playerData;
    }

    public static void SetPlayerBoxes()
    {
        foreach (var _boxFromPlayer in Instance.Player.boxes)
            foreach (var _boxConfigInCarousel in Instance._allBoxesConfigInCarousel)
                CompareBoxModifiers(_boxFromPlayer, _boxConfigInCarousel);
    }

    private static void CompareBoxModifiers(Box boxFromPlayer, BoxConfig boxConfigInCarousel)
    {
        if (GetBoxTypeInPlayerBox(boxFromPlayer) == boxConfigInCarousel.BoxType
            && GetBoxRegionInPlayerBox(boxFromPlayer) == boxConfigInCarousel.BoxRegion
            && !boxFromPlayer.isOpen)
            boxConfigInCarousel.boxList.Add(boxFromPlayer);
    }

    private static BOX_TYPE GetBoxTypeInPlayerBox(Box box)
    {
        return box.type?.ToUpper() switch
        {
            "SIMPLE" => BOX_TYPE.Regular,
            "FORTIFIED" => BOX_TYPE.Fortified,
            _ => BOX_TYPE.None
        };
    }

    private static BOX_REGION GetBoxRegionInPlayerBox(Box box)
    {
        var _boxRegion = "";
        _boxRegion = string.IsNullOrEmpty(box.region) ? box.toyo?.toyoPersonaOrigin?.region : box.region;
        return _boxRegion?.ToUpper() switch
        {
            "KYTUNT" => BOX_REGION.Kytunt,
            "JAKANA" => BOX_REGION.Jakana,
            "XEON" => BOX_REGION.Xeon,
            _ => BOX_REGION.None
        };
    }
    
    public static void SaveIsStakeBackup(Toyo[] toyos)
    {
        Instance._isStakeBackup = new();
        foreach (var _toyo in toyos)
            Instance._isStakeBackup.Add(_toyo.tokenId, _toyo.isStaked);
    }

    public static bool GetIsStakedById(string tokenID)
    {
        foreach (var _stake in Instance._isStakeBackup)
            if (tokenID == _stake.Key)
                return _stake.Value;
        Print.Log("isStaked not found in dictionary backup;");
        return false;
    }
}