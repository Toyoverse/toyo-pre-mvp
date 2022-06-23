using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBox : MonoBehaviour
{
    public float rotateSpeed = 2f;

    private bool callback = false;

    private void OnEnable()
    {
        StartCoroutine(StartFadeIn());
    }

    void Update()
    {
        transform.parent.RotateAround(transform.position,Vector3.up, rotateSpeed * Time.deltaTime * 100f);
    }

    IEnumerator StartFadeIn()
    {
        yield return new WaitForSeconds(3.2f);
        FadeController.In(FadeMode.UI, AnimationMode.Fade, FadeCallBack);
    }

    void FadeCallBack() => callback = !callback;
}
