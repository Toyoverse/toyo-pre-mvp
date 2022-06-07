using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class CarouselManager : MonoBehaviour
{

    public List<Transform> AllObjects;

    public Transform Anchor;

    private Transform CurrentSelectedObject;

    private int CurrentSelectedIndex = 0;

    private bool IsFirstObjectSelected() => CurrentSelectedIndex == 0;

    private bool IsLastObjectSelected() => CurrentSelectedIndex + 1 == AllObjects.Count;

    private Transform GetNextObject() => IsLastObjectSelected() ? AllObjects.First() : AllObjects[CurrentSelectedIndex + 1];
    
    private Transform GetPreviousObject() => IsFirstObjectSelected() ? AllObjects.Last() : AllObjects[CurrentSelectedIndex - 1];

    private void Awake()
    {
        CurrentSelectedObject = AllObjects[CurrentSelectedIndex];
        UpdateCarousel();
    }

    private void UpdateCarousel()
    {
        MoveToFront(CurrentSelectedObject);
        foreach (var _object in AllObjects.Where(_object => _object != CurrentSelectedObject))
        {
            if (GetPreviousObject() == _object)
                MoveToLeftSide(_object);
            else if (GetNextObject() == _object)
                MoveToRightSide(_object);
            else
                MoveToBack(_object);
        }
    }

    void MoveToLeftSide(Transform _object)
    {
        StartCoroutine(MoveToPosition(_object, Anchor.position + new Vector3(-5f, 0, 0), 1f));
    }
    
    void MoveToRightSide(Transform _object)
    {
        StartCoroutine(MoveToPosition(_object, Anchor.position + new Vector3(5f, 0, 0), 1f));
    }
    
    void MoveToBack(Transform _object)
    {
        StartCoroutine(MoveToPosition(_object, Anchor.position + new Vector3(0f, 0, 10), 1f));
    }
    
    void MoveToFront(Transform _object)
    {
        StartCoroutine(MoveToPosition(_object, Anchor.position + new Vector3(0f, 0, -10), 1f));
    }

    IEnumerator MoveToPosition(Transform _object, Vector3 newPosition, float time)
    {
        var elapsedTime = 0f;
        var startingPos = _object.position;
        while (elapsedTime < time)
        {
            _object.position = Vector3.Lerp(startingPos, newPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
        

    public void SwipeRight()
    {
        if (CurrentSelectedIndex + 1 < AllObjects.Count)
        {
            CurrentSelectedIndex++;
            CurrentSelectedObject = AllObjects[CurrentSelectedIndex];
        }
        else
        {
            CurrentSelectedIndex = 0;
            CurrentSelectedObject = AllObjects[CurrentSelectedIndex];
        }
        UpdateCarousel();
    }
    
    public void SwipeLeft()
    {
        if (CurrentSelectedIndex > 0)
        {
            CurrentSelectedIndex--;
            CurrentSelectedObject = AllObjects[CurrentSelectedIndex];
        }
        else
        {
            CurrentSelectedIndex = AllObjects.Count-1;
            CurrentSelectedObject = AllObjects[CurrentSelectedIndex];
        }
        UpdateCarousel();
    }
}
