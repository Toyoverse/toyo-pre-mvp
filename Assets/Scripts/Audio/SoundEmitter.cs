using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System;
using Unity.Collections;
using UnityTemplateProjects.Audio;


public class SoundEmitter : MonoBehaviour
{

    [Tooltip("Is the sound already playing?")]
    [SerializeField] [ReadOnly] private bool isPlayingSound = false;

    [Tooltip("Sound to play whenever the player is close enough.")]
    public EventReference continuousSound;

    [Tooltip("Sound to play when the object is destroyed.")]
    [SerializeField] private EventReference onDestroySound;

    [Tooltip("This is sound can change music parameters?")]
    public bool musicChangeParameters;
    
    public FMOD.Studio.EventInstance ContinuousInstance;

    private void Start()
    {
        ContinuousInstance = RuntimeManager.CreateInstance(continuousSound);
        PlayContinuousSound();
        if (musicChangeParameters)
        {
            //Signals.Get<EventChangeMusicParameters>().AddListener(ChangeMusicParameters);
        }
    }
    
    private void PlayContinuousSound()
    {
        ContinuousInstance.set3DAttributes(transform.position.To3DAttributes());
        ContinuousInstance.start();
    }

    private void StopContinuousSound()
    {
        ContinuousInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void UpdatePosition()
    {
        ContinuousInstance.set3DAttributes(transform.position.To3DAttributes());
    }

    private void OnDestroy()
    {
        if(!onDestroySound.IsNull)
            onDestroySound.PlayOneShot(transform.position);
        
        if (isPlayingSound)
            StopContinuousSound();
         
        if (musicChangeParameters)
        {
            //Signals.Get<EventChangeMusicParameters>().RemoveListener(ChangeMusicParameters);
        }
    }
    
}

//public class EventChangeMusicParameters : ASignal<int> { }
