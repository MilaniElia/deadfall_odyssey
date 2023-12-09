using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour
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
        Settings.Instance.SetAudioVolume(volume);
    }

    public void SetMouseXSensitivity(float sensitivityX)
    {
        Settings.Instance.SetMouseXSensitivity(sensitivityX);
    }
    
    public void SetMouseYSensitivity(float sensitivityY)
    {
        Settings.Instance.SetMouseYSensitivity(sensitivityY);
    }
}
