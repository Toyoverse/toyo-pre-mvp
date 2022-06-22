using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBox : MonoBehaviour
{
    public float rotateSpeed = 2f;
    
    void Update()
    {
        transform.parent.RotateAround(transform.position,Vector3.up, rotateSpeed * Time.deltaTime * 100f);
    }
}
