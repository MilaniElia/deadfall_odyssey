using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [Header("Score")]
    public int totalScore;
    public Text totalScoreText;


    private void Start()
    {
        totalScore = 0;
    }


    public void Update()
    {
        SetCursorVisible(true);
        totalScore = (int)FindObjectOfType<PlayerController>().totalScore;
        totalScoreText.text = totalScore.ToString();
    }

    public void SetCursorVisible(bool vis)
    {
        Cursor.visible = vis;
        Cursor.lockState = CursorLockMode.Confined;
    }
}
