using UnityEngine;

public class Ammunition : Interactable
{
    [SerializeField]
    private int AmmunitionFillAmount;

    [SerializeField]
    private GameObject AmmunitionObject;

    [SerializeField]
    private Animator Animator;


    protected override void PickUp()
    {
        if (GameObject.FindObjectOfType<AutomaticGunScriptLPFP>().totalAmmo < 250)
        {
            Debug.Log("Picking up item.");
            Animator.enabled = true;
            if (GameObject.FindObjectOfType<AutomaticGunScriptLPFP>().totalAmmo + AmmunitionFillAmount < 250)
            {
                GameObject.FindObjectOfType<AutomaticGunScriptLPFP>().AddAmmo(AmmunitionFillAmount);
                var animator = gameObject.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetBool("finished", true);
                }
                gameObject.GetComponent<KeyButtonTrigger>().enabled = false;
                _interactable = false;
            }
    }
    }

}
