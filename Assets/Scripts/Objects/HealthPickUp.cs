using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthPickUp : Interactable
{
    [SerializeField]
    private int HealthFillAmount;

    protected override void PickUp()
    {
        if(FindObjectOfType<PlayerController>().currentHealth + HealthFillAmount < 100)
        {
            Debug.Log("Picking up item.");
            FindObjectOfType<PlayerController>().AddHealth(HealthFillAmount);
            gameObject.GetComponent<Animator>().enabled = true;
            gameObject.GetComponent<KeyButtonTrigger>().enabled = false;
            _interactable = false;
        }   
    }
}
