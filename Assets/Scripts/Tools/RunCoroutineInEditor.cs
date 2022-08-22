using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad] 
public class RunCoroutineInEditor 
{
    private static List<IEnumerator> _coroutineInProgress = new List<IEnumerator>();

    public static void StartCoroutine(IEnumerator coroutine)
    {
        _coroutineInProgress.Add(coroutine);
        return;
    }
    
    static RunCoroutineInEditor()
    {
        EditorApplication.update += ExecuteCoroutine;
    }

    private static int _currentExecute;
    private static void ExecuteCoroutine()
    {
        if (_coroutineInProgress.Count <= 0)
        {
            //Debug.Log("Not coroutine in progress.");
            return;
        }
        
        _currentExecute = (_currentExecute + 1) % _coroutineInProgress.Count;
        var _finish = !_coroutineInProgress[_currentExecute].MoveNext();
        Debug.Log("Execute coroutine " + _currentExecute);
        
        if (_finish)
        {
            _coroutineInProgress.RemoveAt(_currentExecute);
            Debug.Log("End Coroutine " + _currentExecute);
        }
    }
}
