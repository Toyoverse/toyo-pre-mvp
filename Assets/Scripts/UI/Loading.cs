using System;
using UnityEngine;

public class Loading : MonoBehaviour
{
    public GameObject loadingCanvas;
    public Transform transformToRotate;
    public float rotateSpeed;
    
    public static bool InLoading { get; private set; }
    public static Action StartLoading;
    public static Action EndLoading;

    private void Awake()
    {
        StartLoading += EnableLoading;
        EndLoading += DisableLoading;
    }

    private void OnDestroy()
    {
        StartLoading -= EnableLoading;
        EndLoading -= DisableLoading;
    }

    private void Update()
    {
        if(InLoading)
            transformToRotate.Rotate(0, 0, - rotateSpeed * Time.deltaTime);
    }

    private void EnableLoading()
    {
        SetLoading(true);
        loadingCanvas.SetActive(true);
        Debug.Log("Enable Loading");
    }

    private void DisableLoading()
    {
        SetLoading(false);
        loadingCanvas.SetActive(false);
        Debug.Log("Disable Loading");
    }

    private void SetLoading(bool on) => InLoading = on;
}
