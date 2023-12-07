using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScript : MonoBehaviour
{
    [Header("Enemy List"), SerializeField]
    public List<GameObject> enemiesGameObject;

    [Header("Win Canvas")]
    public Canvas gameOverCanvas;


    private void Update()
    {
        foreach(GameObject enemy in enemiesGameObject)
        {
            if (!enemy)
            {
                enemiesGameObject.Remove(enemy);
            }
        }

        if(enemiesGameObject.Count == 0)
        {
            gameOverCanvas.enabled = true;
        }
    }
}
