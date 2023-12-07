using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Interactable : MonoBehaviour
{
    public bool IsInteractable
    {
        get
        {
            return _interactable;
        }
    }

    public float Radius
    {
        get
        {
            return _radius;
        }
    }

    protected bool _interactable = true;
    [SerializeField, Range(0.1f, 1.5f)]
    private float _radius = 1;

    public void Interact()
    {
        if(_interactable)
        { 
            PickUp();
        }
    }
    protected abstract void PickUp();
}
