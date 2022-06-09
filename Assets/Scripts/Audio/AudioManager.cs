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
        public EventReference buttonAcceptSfx;
        public EventReference buttonDenySfx;
        public EventReference loadingSfx;
        public EventReference rotateRightSfx;
        public EventReference rotateLeftSfx;
        public EventReference slideRightSfx;
        public EventReference slideLeftSfx;
        public EventReference startHomePageSfx;
        public EventReference spawnToyoSfx;
        public EventReference unboxingSfx;
        
        public EventReference backgroundMusic;
        

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

        public EventInstance PlayOneShot(EventReference eventReference, Vector3 position)
        {
            var _eventInstance = FMODUnity.RuntimeManager.CreateInstance(eventReference);
            try
            {
                ClearInactiveInstances();
                _eventInstance.set3DAttributes(position.To3DAttributes());
                _eventInstance.start();
                _eventInstance.release();
                return _eventInstance;
            }
            catch(Exception _err)
            {
                Debug.Log("AUDIO RUNTIME ERROR: " + _err.Message);
                return _eventInstance;
            }
        }

        public void PlayOneShotWithParameter(EventReference eventReference, Vector3 position, string parameterName, int parameterValue)
        {
            var _eventInstance = FMODUnity.RuntimeManager.CreateInstance(eventReference);
            try
            {
                ClearInactiveInstances();
                _eventInstance.setParameterByName(parameterName, parameterValue);
                _eventInstance.set3DAttributes(position.To3DAttributes());
                _eventInstance.start();
                _eventInstance.release();
                EventInstances.Add(_eventInstance);
            }
            catch (Exception _err)
            {
                Debug.Log("AUDIO RUNTIME ERROR: " + _err.Message);
            }
        }
        


    }

    static class ExtensionAudioManager
    {
        public static void PlayOneShot(this EventReference reference, Vector3 position)
        {
            AudioManager.Instance.PlayOneShot(reference, position);
        }
    }
}