
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
    private List<BoxConfig> _allBoxesConfig;
    public Transform toyoListParent;
    public GameObject toyoBasePrefab;
    public List<ToyoPersonaSO> toyoPersonaPrefabs;
    
    public Transform openBoxToyoPivot;
    public Transform mainMenuToyoPivot;

    private GameObject _selectedBox;
    
    private List<ToyoObject> _toyoList;
    public List<ToyoObject> ToyoList => _toyoList ??= CreateToyoObjectList();

    private static ToyoManager _instance;

    public static ToyoObject GetSelectedToyo() => _instance.ToyoList.Find(toyoObject => toyoObject.IsToyoSelected);

    public static void SetSelectedBox(GameObject selectedBox) => _instance._selectedBox = selectedBox;
    
    public static GameObject GetSelectedBox() => _instance._selectedBox;
    
   // public static List<BoxInformation> BoxInformationList;
    
    public static Camera MainCamera;
    
    public static void MoveToyoToCenterOpenBox()
    {
        GetSelectedToyo().transform
            .SetPositionAndRotation(_instance.openBoxToyoPivot.position, _instance.openBoxToyoPivot.rotation);
        GetSelectedToyo().transform.SetParent(_instance.openBoxToyoPivot);
        GetSelectedToyo().transform.LookAt(MainCamera.transform);
    }
    
    public static void MoveToyoToCenterMainMenu()
    {
        GetSelectedToyo().transform
            .SetPositionAndRotation(_instance.mainMenuToyoPivot.position, _instance.mainMenuToyoPivot.rotation);
        GetSelectedToyo().transform.SetParent(_instance.mainMenuToyoPivot);
        GetSelectedToyo().transform.LookAt(MainCamera.transform);
    }

    public static void AddBoxToGlobalList(BoxConfig box) => _instance._allBoxesConfig.Add(box);


    List<ToyoObject> CreateToyoObjectList()
    {
        var _databaseToyoList = DatabaseConnection.Instance.player.toyos.ToList();
        var _toyoObjectList = new List<ToyoObject>();

        var _index = 0;
        foreach (var _databaseToyo in _databaseToyoList)
        {
            var _toyoPrefab = Instantiate(GetToyoPersonaPrefab(_databaseToyo.toyoPersonaOrigin.objectId), toyoListParent);
            var _toyoObjectInstance =_toyoPrefab.AddComponent<ToyoObject, Toyo>(_databaseToyo);
            _toyoObjectInstance.spriteAnimator = _toyoPrefab.GetComponentInChildren<SpriteAnimator>();
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
        MainCamera = FindObjectOfType<Camera>();
    }

    private GameObject GetToyoPersonaPrefab(string objectId)
    {
        return toyoPersonaPrefabs.FirstOrDefault(toyoPersona => objectId == toyoPersona.objectId)?.toyoPrefab;
    }

    public static void SetPlayerBoxes(Player playerData)
    {
        //InitBoxInformationList();
        foreach (var _box in playerData.boxes)
        {
            foreach (var _t in _instance._allBoxesConfig
                         .Where(t => 
                            GetBoxTypeInPlayerBox(_box) == t.BoxType
                            && GetBoxRegionInPlayerBox(_box) == t.BoxRegion))
            {
                _t.boxList.Add(_box);
            }
        }
    }
    
    private static BOX_TYPE GetBoxTypeInPlayerBox(Box box)
    {
        return box.type switch
        {
            "REGULAR" => BOX_TYPE.Regular,
            "FORTIFIED" => BOX_TYPE.Fortified,
            _ => BOX_TYPE.None
        };
    }

    private static BOX_REGION GetBoxRegionInPlayerBox(Box box)
    {
        return box.region switch
        {
            "JAKANA" => BOX_REGION.Kytunt,
            "KYTUNT" => BOX_REGION.Jakana,
            _ => BOX_REGION.None
        };
    }
    /*
    private static void InitBoxInformationList()
    {
        BoxInformationList = new List<BoxInformation>();
        var _kytuntRegularBox = new BoxInformation(BOX_TYPE.Regular, BOX_REGION.Kytunt);
        var _kytuntFortifiedBox = new BoxInformation(BOX_TYPE.Fortified, BOX_REGION.Kytunt);
        var _jakanaRegularBox = new BoxInformation(BOX_TYPE.Regular, BOX_REGION.Jakana);
        var _jakanaFortifiedBox = new BoxInformation(BOX_TYPE.Fortified, BOX_REGION.Jakana);
        BoxInformationList.Add(_kytuntRegularBox);
        BoxInformationList.Add(_kytuntFortifiedBox);
        BoxInformationList.Add(_jakanaRegularBox);
        BoxInformationList.Add(_jakanaFortifiedBox);
    }
    */
}
/*
public class BoxInformation
{
    public BOX_TYPE type;
    public BOX_REGION Region;
    public int Quantity;

    public BoxInformation(BOX_TYPE _type, BOX_REGION _region)
    {
        type = _type;
        Region = _region;
        Quantity = 0;
    }
}
*/