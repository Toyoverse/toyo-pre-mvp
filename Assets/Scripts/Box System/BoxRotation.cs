using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxRotation : MonoBehaviour
{
    private Transform _transform;
    private float _rotationSpeed = 10f;

    void Start()
    {
        _transform = gameObject.transform;
    }

    void Update()
    {
        _transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
    }
}
