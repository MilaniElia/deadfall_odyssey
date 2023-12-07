using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{

    public GameObject pauseUI;
    public AudioMixer audioMixer;

    public void Pause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
    }

    
    public void Resume()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
    }

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
        audioMixer.SetFloat("mainVolume", volume);
    }
}
