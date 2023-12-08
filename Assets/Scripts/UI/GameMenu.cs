using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public void EnableMenu()
    {
        GameObject.FindObjectOfType<PlayerController>().enabled = false;
        GetComponent<Image>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void DisableMenu()
    {
        GameObject.FindObjectOfType<PlayerController>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<Image>().enabled = false;
        Time.timeScale = 1f;
    }
}
