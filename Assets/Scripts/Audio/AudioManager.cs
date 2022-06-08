using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace UnityTemplateProjects.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        
        [Header("UI Sounds")]
        public EventReference buttonClickSfx;
        
        public List<EventInstance> EventInstances = new();
        
        public void StopAll()
        {
            foreach (var _eventInstance in EventInstances.Where(instance => instance.isValid()))
            {
                _eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }

            EventInstances = new List<EventInstance>();
        }

        public void ClearInactiveInstances()
        {
            for (var _index = 0; _index <= EventInstances.Count - 1; _index++)
            {
                EventInstances[_index].getPlaybackState(out var _playbackState);
                if (_playbackState == PLAYBACK_STATE.STOPPED)
                    EventInstances.RemoveAt(_index);
            }
        }
        
        public EventInstance PlayOneShot(EventInstance eventInstance, Vector3 position)
        {
            try
            {
                ClearInactiveInstances();
                eventInstance.set3DAttributes(position.To3DAttributes());
                eventInstance.start();
                eventInstance.release();
                return eventInstance;
            }
            catch(Exception _err)
            {
                Debug.Log("AUDIO RUNTIME ERROR: " + _err.Message);
                return eventInstance;
            }
        }

        public void PlayOneShotWithParameter(EventInstance eventInstance, Vector3 position, string parameterName, int parameterValue)
        {
            try
            {
                ClearInactiveInstances();
                eventInstance.setParameterByName(parameterName, parameterValue);
                eventInstance.set3DAttributes(position.To3DAttributes());
                eventInstance.start();
                eventInstance.release();
                EventInstances.Add(eventInstance);
            }
            catch (Exception _err)
            {
                Debug.Log("AUDIO RUNTIME ERROR: " + _err.Message);
            }
        }

    }
}