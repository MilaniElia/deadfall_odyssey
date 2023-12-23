using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WinScript : MonoBehaviour
{
    [Header("Enemy List"), SerializeField]
    private List<EnemyAi> Enemies;

    [Header("Win Canvas")]
    public GameObject gameOverCanvas;

    public void Start()
    {
        Enemies = GameObject.FindObjectsOfType<EnemyAi>().ToList();
    }


    private void Update()
    {
        if(CheckEnemiesState())
        {
            gameOverCanvas.SetActive(true);
        }
    }


    private bool CheckEnemiesState()
    {
        foreach(EnemyAi enemy in Enemies)
        {
            if(enemy.currentHealth > 0)
            {
                return false;
            }
        }
        return true;
    }
}
