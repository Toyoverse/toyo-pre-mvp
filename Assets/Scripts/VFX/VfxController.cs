using System;
using UnityEngine;
using UnityEngine.Events;

public class VfxController : MonoBehaviour
{
    public RarityColorConfigSO rarityColorConfig;
    public Effect[] effecs;

    void Start()
    {
        foreach (var _effect in effecs)
        {
            _effect.particleSystem.gameObject.AddComponent<ParticleCallbacks>();
            var _main = _effect.particleSystem.main;
            _main.stopAction = ParticleSystemStopAction.Callback;
        }
    }

    public void PlayParticle(Effect effect)
    {
        effect.particleSystem.Play();
    }

    public void StopParticle(Effect effect)
    {
        effect.particleSystem.Stop();
    }
}

[Serializable]
public class Effect
{
    public ParticleSystem particleSystem;
    public float durationInSeconds;
    //public UnityEvent startEventTrigger;
    public UnityEvent endEventTrigger;
}

public class ParticleCallbacks : MonoBehaviour
{
    public Effect effect;

    /*private void Start()
    {
        effect.startEventTrigger.Invoke();
    }*/
    
    public void OnParticleSystemStopped()
    {
        effect.endEventTrigger.Invoke();
        Debug.Log("Stop particle " + gameObject.name);
    }
}