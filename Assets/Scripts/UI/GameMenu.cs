using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public bool IsPaused
    {
        get
        {
            return _isPaused;
        }
    }

    private bool _isPaused;

    public void EnableMenu()
    {
        _isPaused = true;
        Cursor.lockState = CursorLockMode.Confined;
        GameObject.FindObjectOfType<PlayerController>().enabled = false;
        GameObject.FindObjectOfType<AutomaticGunScriptLPFP>().enabled = false;
        GetComponent<Image>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void DisableMenu()
    {
        _isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameObject.FindObjectOfType<PlayerController>().enabled = true;
        GameObject.FindObjectOfType<AutomaticGunScriptLPFP>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<Image>().enabled = false;
        Time.timeScale = 1f;
    }
}
