using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [Header("Score"), SerializeField]
    private TMP_Text totalScoreText;


    private void Start()
    {
        Time.timeScale = 0f;
        SetCursorVisible(true);
        totalScoreText.text = $"Score: {(int)FindObjectOfType<PlayerController>().totalScore}";
    }

    public void SetCursorVisible(bool vis)
    {
        Cursor.visible = vis;
        Cursor.lockState = CursorLockMode.Confined;
    }
}
