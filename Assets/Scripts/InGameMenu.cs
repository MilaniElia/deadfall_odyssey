using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class InGameMenu : MonoBehaviour
{
    public void LoaderScene(int SceneIndex)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneIndex);
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        Settings.SetAudioVolume(volume);
    }

    public void SetMouseXSensitivity(float sensitivityX)
    {
        Settings.SetMouseXSensitivity(sensitivityX);
    }
    
    public void SetMouseYSensitivity(float sensitivityY)
    {
        Settings.SetMouseYSensitivity(sensitivityY);
    }
}
