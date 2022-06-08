using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;


public class CarouselManager : MonoBehaviour
{

    public float carouselOffset = 5.0f;
    
    public List<Transform> allObjects;

    public Transform anchor;

    private Transform _currentSelectedObject;

    private int _currentSelectedIndex = 0;

    private bool IsFirstObjectSelected() => _currentSelectedIndex == 0;

    private bool IsLastObjectSelected() => _currentSelectedIndex + 1 == allObjects.Count;

    private bool IsObjectToRotate(Transform objectToRotate) => objectToRotate == GetPreviousObject() || objectToRotate == GetNextObject() ||
                                                        objectToRotate == _currentSelectedObject || objectToRotate == _objectToHide;

    private Transform GetNextObject() => IsLastObjectSelected() ? allObjects.First() : allObjects[_currentSelectedIndex + 1];
    
    private Transform GetPreviousObject() => IsFirstObjectSelected() ? allObjects.Last() : allObjects[_currentSelectedIndex - 1];

    private Transform _objectToHide;

    private void Awake()
    {
        _currentSelectedObject = allObjects[_currentSelectedIndex];
        MoveToStartingPosition();
    }

    private void MoveToStartingPosition()
    {
        
        foreach (var _object in allObjects.Where(objectToRotate => objectToRotate != _currentSelectedObject))
        {
            if (GetPreviousObject() == _object)
                RotateRight(_object);
            else if (GetNextObject() == _object)
                RotateLeft(_object);
            else 
                RotateToBack(_object);
        }
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
            yield return null;
        }
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
        

    public void SwipeRight()
    {
        _objectToHide = GetPreviousObject();
        if (_currentSelectedIndex + 1 < allObjects.Count)
        {
            _currentSelectedIndex++;
            _currentSelectedObject = allObjects[_currentSelectedIndex];
        }
        else
        {
            _currentSelectedIndex = 0;
            _currentSelectedObject = allObjects[_currentSelectedIndex];
        }
        foreach (var _object in allObjects.Where(IsObjectToRotate))
            RotateRight(_object);
    }
    
    public void SwipeLeft()
    {
        _objectToHide = GetNextObject();
        if (_currentSelectedIndex > 0)
        {
            _currentSelectedIndex--;
            _currentSelectedObject = allObjects[_currentSelectedIndex];
        }
        else
        {
            _currentSelectedIndex = allObjects.Count-1;
            _currentSelectedObject = allObjects[_currentSelectedIndex];
        }
        foreach (var _object in allObjects.Where(IsObjectToRotate))
            RotateLeft(_object);
    }
}
