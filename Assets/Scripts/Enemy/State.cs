using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected EnemyAi enemy;
    public string stateName;

    public abstract void Action();
    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }
    public virtual void GetState() { Debug.Log(stateName); }

    public State(EnemyAi enemy)
    {
        this.enemy = enemy;
    }
}
