using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBox : MonoBehaviour
{
    public float rotateSpeed = 2f;
    public Transform rotatePivot;
    
    void Start()
    {
        if (rotatePivot == null)
            rotatePivot = transform;
    }

    void Update()
    {
        transform.RotateAround(rotatePivot.position,Vector3.up, rotateSpeed * Time.deltaTime * 100f);
    }
}
