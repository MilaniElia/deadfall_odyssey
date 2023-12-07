using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToNextLevelDoor : Interactable
{
    [SerializeField]
    private GameObject enemy;

    public void Update()
    {
        if (!enemy)
        {
            gameObject.GetComponent<KeyButtonTrigger>().enabled = true;
        }
    }


    protected override void PickUp()
    {
        SceneManager.LoadScene(2);
    }
}