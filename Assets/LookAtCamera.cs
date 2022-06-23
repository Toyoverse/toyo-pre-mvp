using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(ToyoManager.MainCamera.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
