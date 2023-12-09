using Microsoft.Win32.SafeHandles;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class Settings
{
    public static float MouseXSensitivity
    {
        get
        {
            return _mouseSensitivityX; 
        }
    }
    
    public static float MouseYSensitivity
    {
        get
        {
            return _mouseSensitivityY; 
        }
    }

    public static float Volume
    {
        get
        {
            return _volume;
        }
    }

    private static Settings _instance;

    public static Settings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Settings();
            }
            return _instance;
        }
    }

    private static float _mouseSensitivityX = 1;
    private static float _mouseSensitivityY = 1;
    private static float _volume = -10;
    private static AudioMixer _audioMixer;

    private Settings()
    {
        _audioMixer = Resources.Load<AudioMixer>("MainMixer");
    }


    public void SetAudioVolume(float newVolume)
    {
        _volume = newVolume;
        _audioMixer.SetFloat("MasterVolume", _volume);
    }

    public void SetMouseXSensitivity(float newSensitivity)
    {
        _mouseSensitivityX = newSensitivity;
    }

    public void SetMouseYSensitivity(float newSensitivity)
    {
        _mouseSensitivityY = newSensitivity;
    }
}
