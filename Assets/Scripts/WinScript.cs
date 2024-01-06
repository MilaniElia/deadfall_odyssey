using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WinScript : MonoBehaviour
{
    [Header("Enemy List"), SerializeField]
    private List<EnemyAI> Enemies;

    [Header("Win Canvas")]
    public GameObject gameOverCanvas;

    public void Start()
    {
        Enemies = GameObject.FindObjectsOfType<EnemyAI>().ToList();
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
        foreach(EnemyAI enemy in Enemies)
        {
            if(enemy.currentHealth > 0)
            {
                return false;
            }
        }
        return true;
    }
}
