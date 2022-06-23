using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    void Start()
    {
        transform.DOMoveY(1.35f, 0.5f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo).Play();
    }

}
