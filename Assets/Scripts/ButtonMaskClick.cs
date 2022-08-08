using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ButtonMaskClick : MonoBehaviour
{
    public float alphaThreshold = 0.1f;
    void Start() => GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThreshold;
    }
