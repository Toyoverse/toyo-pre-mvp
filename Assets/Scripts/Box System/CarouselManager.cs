using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine.Serialization;
using UnityTemplateProjects.Audio;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;


public class CarouselManager : MonoBehaviour
{
    private enum CAROUSEL_UNDER_4_POS
    {
        LEFT,
        CENTRE,
        RIGHT
    }
    
    public float carouselOffset = 5.0f;

    [SerializeField]
    public bool is2DCarousel;
    
    public bool isToyoCarousel;

    public Transform StartingPosition;
    
    public List<Transform> allObjects;

    public Transform anchor;

    public Transform CurrentSelectedObject { get; private set; }

    [SerializeField] private Vector3 _selectedPlatformRotation;
    [SerializeField] private Vector3 _leftPlatformRotation;
    [SerializeField] private Vector3 _backPlatformRotation;

    public event Action OnEndRotation;

    private CAROUSEL_UNDER_4_POS _carouselUnder4Pos;
    
    public void SetFirstSelectedObject(Transform objectToSelect = null, int currentSelectedIndex = 0)
    { 
        CurrentSelectedObject = objectToSelect != null ? objectToSelect : allObjects[_currentSelectedIndex];
        _currentSelectedIndex = currentSelectedIndex;
    } 

    private int _currentSelectedIndex = 0;

    private bool IsFirstObjectSelected() => _currentSelectedIndex == 0;

    private bool IsLastObjectSelected() => _currentSelectedIndex + 1 == allObjects.Count;

    private bool IsObjectToRotate(Transform objectToRotate) => objectToRotate == GetPreviousObject() || objectToRotate == GetNextObject() ||
                                                        objectToRotate == CurrentSelectedObject || objectToRotate == _objectToHide;

    private Transform GetNextObject() => IsLastObjectSelected() ? allObjects.First() : allObjects[_currentSelectedIndex + 1];
    
    private Transform GetPreviousObject() => IsFirstObjectSelected() ? allObjects.Last() : allObjects[_currentSelectedIndex - 1];

    private Transform _objectToHide;

    private void UpdateCurrentSelectedToyo(Transform toyoTransform, int index)
    {
        CurrentSelectedObject = toyoTransform;
        _currentSelectedIndex = index;
    }

    private void OnEnable()
    {
        if (!isToyoCarousel)
            CurrentSelectedObject = allObjects[_currentSelectedIndex];

        if (is2DCarousel)
        {
            if (allObjects.Count == 2)
                _carouselUnder4Pos = CAROUSEL_UNDER_4_POS.RIGHT;
            else if(allObjects.Count == 3)
                _carouselUnder4Pos = CAROUSEL_UNDER_4_POS.CENTRE;
            
            int index = 0;
            if(isToyoCarousel)
                foreach (var item in allObjects)
                {
                    if (item == ToyoManager.GetSelectedToyo().transform)
                    { 
                        UpdateCurrentSelectedToyo(item, index);
                    }
                    index++;
                }
            Set2DStartingPosition();
        }

        MoveToStartingPosition();
    }

    public void Set2DStartingPosition()
    {
        foreach (var _object in allObjects)
        {
            if (isToyoCarousel)
                _object.gameObject.GetComponent<ToyoObject>().spriteAnimator.disableAnimationOnStart = true;
            _object.SetPositionAndRotation(StartingPosition.position, StartingPosition.rotation);
            _object.SetParent(StartingPosition);
            _object.LookAt(ToyoManager.MainCamera.transform);
        }
    }

    public void MoveToStartingPosition()
    {
        foreach (var _object in allObjects.Where(objectToRotate => objectToRotate != CurrentSelectedObject))
        {
            if (GetPreviousObject() == _object)
                RotateRight(_object);
            else if (GetNextObject() == _object)
                RotateLeft(_object);
            else 
                RotateToBack(_object);
        }

        if (isToyoCarousel)
            CorrectSelectedPlatformRotation(CurrentSelectedObject, 1);
    }

    void RotateLeft(Transform objectToRotate)
    {
        StartCoroutine(SmoothRotateAround(objectToRotate, -90f, 1f));
    }
    
    void RotateRight(Transform objectToRotate)
    {
        StartCoroutine(SmoothRotateAround(objectToRotate, 90f, 1f));
    }
    
    void RotateToBack(Transform objectToRotate)
    {
        StartCoroutine(SmoothRotateAround(objectToRotate, 180f, 1f));
    }
    
    IEnumerator SmoothRotateAround(Transform objectToRotate, float angle, float duration)
    {
        var _currentTime = 0.0f;
        var _angleDelta = angle/duration;
        while (_currentTime< duration)
        {
            _currentTime+=Time.deltaTime;
            var _ourTimeDelta= Time.deltaTime;
            if (_currentTime > duration)
                _ourTimeDelta-= (_currentTime-duration);
            objectToRotate.RotateAround(anchor.position, Vector3.up, _angleDelta*_ourTimeDelta);

            if (is2DCarousel)
                objectToRotate.LookAt(ToyoManager.MainCamera.transform);

            CorrectToyoPlatformsRotation(objectToRotate, _ourTimeDelta);

            yield return null;
        }
        
        OnEndRotation?.Invoke();
    }
    
    IEnumerator MoveToPosition(Transform objectToRotate, Vector3 newPosition, float time)
    {
        var _elapsedTime = 0f;
        var _startingPos = objectToRotate.position;
        while (_elapsedTime < time)
        {
            objectToRotate.position = Vector3.Lerp(_startingPos, newPosition, (_elapsedTime / time));
            _elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void GetHiddenObjectFromList(List<Transform> objects, bool isSwipeRight)
    {
        if (isSwipeRight)
        {
            if (objects.Count >= 4)
            {
                if (GetNextObject() == allObjects.First())
                    _objectToHide = allObjects[1];
                else if (GetNextObject() == allObjects.Last())
                    _objectToHide = allObjects.First();
                else
                    _objectToHide = allObjects[_currentSelectedIndex + 2];
            }
        }
        else
        {
            if (objects.Count >= 4)
            {
                if (GetPreviousObject() == allObjects.First())
                    _objectToHide = allObjects.Last();
                else if (GetPreviousObject() == allObjects[1])
                    _objectToHide = allObjects.First();
                else if (GetPreviousObject() == allObjects.Last())
                    _objectToHide = allObjects[allObjects.Count - 2];
                else
                    _objectToHide = allObjects[_currentSelectedIndex - 2];
            }
        }
    }
    
    public void SwipeRight()
    {
        AudioManager.Instance.rotateRightSfx.PlayOneShot(transform.position);

        if (allObjects.Count <= 1)
            return;
        if(allObjects.Count < 4 && _carouselUnder4Pos == CAROUSEL_UNDER_4_POS.RIGHT)
            return;

        if (allObjects.Count == 2)
            _carouselUnder4Pos = CAROUSEL_UNDER_4_POS.RIGHT;
        else if(allObjects.Count == 3)
            _carouselUnder4Pos++;

        GetHiddenObjectFromList(allObjects, true);
        
        foreach (var _object in allObjects.Where(IsObjectToRotate))
            RotateRight(_object);
        
        if (_currentSelectedIndex + 1 < allObjects.Count)
        {
            _currentSelectedIndex++;
            CurrentSelectedObject = allObjects[_currentSelectedIndex];
        }
        else
        {
            _currentSelectedIndex = 0;
            CurrentSelectedObject = allObjects[_currentSelectedIndex];
        }
    }
    
    public void SwipeLeft()
    {
        AudioManager.Instance.rotateLeftSfx.PlayOneShot(transform.position);
        
        if (allObjects.Count <= 1)
            return;
        if(allObjects.Count < 4 && _carouselUnder4Pos == CAROUSEL_UNDER_4_POS.LEFT)
            return;

        if (allObjects.Count == 2)
            _carouselUnder4Pos = CAROUSEL_UNDER_4_POS.LEFT;
        else if(allObjects.Count == 3)
            _carouselUnder4Pos--;
        
        GetHiddenObjectFromList(allObjects, false);

        foreach (var _object in allObjects.Where(IsObjectToRotate))
            RotateLeft(_object);
        
        if (_currentSelectedIndex > 0)
        {
            _currentSelectedIndex--;
            CurrentSelectedObject = allObjects[_currentSelectedIndex];
        }
        else
        {
            _currentSelectedIndex = allObjects.Count-1;
            CurrentSelectedObject = allObjects[_currentSelectedIndex];
        }
    }

    private void CorrectToyoPlatformsRotation(Transform objectToRotate, float ourTimeDelta)
    {
        if (!isToyoCarousel) return;
        
        CorrectNextPlatformRotation(ourTimeDelta);
        CorrectSelectedPlatformRotation(objectToRotate, ourTimeDelta);
        CorrectAntiPreviousPlatformRotation(ourTimeDelta);
    }

    private void CorrectSelectedPlatformRotation(Transform objectToRotate, float ourTimeDelta)
    {
        var _selectedPlatform = objectToRotate.GetChild(1);
        _selectedPlatform.localRotation = Quaternion.Lerp(_selectedPlatform.localRotation,
            Quaternion.Euler(_selectedPlatformRotation), ourTimeDelta);
    }

    private void CorrectNextPlatformRotation(float ourTimeDelta)
    {
        var _nextPlatform = GetNextObject().GetChild(1);
        _nextPlatform.localRotation = Quaternion.Lerp(_nextPlatform.localRotation,
            Quaternion.Euler(_leftPlatformRotation), ourTimeDelta);
    }

    private void CorrectAntiPreviousPlatformRotation(float ourTimeDelta)
    {
        var _i = _currentSelectedIndex switch
        {
            0 => allObjects.Count - 2,
            1 => allObjects.Count - 1,
            _ => _currentSelectedIndex - 2
        };
        var _antiPreviousPlatform = allObjects[_i].GetChild(1);
        _antiPreviousPlatform.localRotation = Quaternion.Lerp(_antiPreviousPlatform.localRotation,
            Quaternion.Euler(_backPlatformRotation), ourTimeDelta);
    }
}
